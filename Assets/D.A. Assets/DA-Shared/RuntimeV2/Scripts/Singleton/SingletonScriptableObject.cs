using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable IDE1006
#pragma warning disable CS1998

namespace DA_Assets.Singleton
{
    /// <summary>
    /// An analogue of the <see href="https://docs.unity3d.com/6000.0/Documentation/ScriptReference/ScriptableSingleton_1.html">
    /// ScriptableSingleton&lt;T&gt;</see> class, but works both in Playmode and in the Editor.
    /// <para>The asset is searched in the <see cref="Resources"/> folder by sequentially traversing the subfolders
    /// specified in the <see cref="ResourcePathAttribute"/> parameters. If the attribute is defined as
    /// <see cref="ResourcePathAttribute"/>(""), the search occurs in the root of the <see cref="Resources"/> folder,
    /// and each subsequent parameter indicates the corresponding subfolder.</para>
    /// <para>Inherits from <see cref="ScriptableObject"/>.</para>
    /// </summary>
    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        /// <summary>
        /// Called upon the first access to the object instance after script recompilation and when entering Playmode.
        /// </summary>
        protected virtual void OnCreateInstance() { }

        public virtual async Task OnCreateInstanceAsync() { }

        /// <summary>
        /// Called after exiting PlayMode.
        /// </summary>
        protected virtual void OnExitPlayMode() { }

        private static SingletonScriptableObject<T> _genericInstance => _instance as SingletonScriptableObject<T>;

        /// <summary>
        /// <para>Gets the instance of the Singleton. Unity creates the Singleton instance when this property is accessed for the first time.</para>
        /// </summary>
        public static T Instance
        {
            get
            {
                CreateInstance();
                return _instance;
            }
        }

        public static async Task<T> GetInstanceAsync()
        {
            await InitializeAsync();
            return _instance;
        }

        public static void CreateInstance()
        {
            if (_instance == null)
            {
                _instance = LoadInstance();
                SetupPlayModeExitEvent();
                _genericInstance?.OnCreateInstance();
            }
        }

        public static async Task InitializeAsync()
        {
            if (_instance == null)
            {
                _instance = await LoadInstanceAsync();
                SetupPlayModeExitEvent();
                await _genericInstance?.OnCreateInstanceAsync();
            }
        }

        private static string GetAssetPath()
        {
            object[] attributes = typeof(T).GetCustomAttributes(typeof(ResourcePathAttribute), true);

            if (attributes != null && attributes.Length > 0)
            {
                ResourcePathAttribute foldersAttribute = attributes[0] as ResourcePathAttribute;
                string path = Path.Combine(foldersAttribute.Path);
                path = Path.Combine(path, typeof(T).Name);
                return path;
            }

            return typeof(T).Name;
        }

        private static T LoadInstance()
        {
            string assetPath = GetAssetPath();

            T instance = Resources.Load<T>(assetPath);

            if (instance == null)
            {
                throw new NullReferenceException(_objectNotFoundErrorStr);
            }

            return instance;
        }

        private static async Task<T> LoadInstanceAsync()
        {
            string assetPath = GetAssetPath();

            ResourceRequest request = Resources.LoadAsync<T>(assetPath);

            while (!request.isDone)
                await Task.Yield();

            T instance = request.asset as T;

            if (instance == null)
            {
                throw new NullReferenceException(_objectNotFoundErrorStr);
            }

            return instance;
        }

        private static void SetupPlayModeExitEvent()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= PlayModeExitEvent;
            UnityEditor.EditorApplication.playModeStateChanged += PlayModeExitEvent;
#endif
        }

#if UNITY_EDITOR
        private static void PlayModeExitEvent(UnityEditor.PlayModeStateChange change)
        {
            if (change == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                _genericInstance?.OnExitPlayMode();
            }
        }
#endif

        private static string _missingAttributeErrorStr => $"Missing {nameof(ResourcePathAttribute)} in {typeof(T).Name}";
        private static string _objectNotFoundErrorStr => $"{nameof(ScriptableObject)} '{typeof(T).Name}' not found in the project.";
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ResourcePathAttribute : Attribute
    {
        public string[] Path { get; private set; }

        public ResourcePathAttribute(params string[] path)
        {
            Path = path;
        }
    }
}