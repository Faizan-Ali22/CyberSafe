using UnityEngine;
using UnityEngine.SceneManagement;

namespace DA_Assets.Extensions
{
    public static class ReflectedExtensions 
    {
        /// <summary>
        /// <para><see href="https://forum.unity.com/threads/how-to-collapse-hierarchy-scene-nodes-via-script.605245/#post-6551890"/></para>
        /// </summary>
        public static void SetExpanded(this Scene scene, bool expand)
        {
#if UNITY_EDITOR
            foreach (var window in Resources.FindObjectsOfTypeAll<UnityEditor.SearchableEditorWindow>())
            {
                if (window.GetType().Name != "SceneHierarchyWindow")
                    continue;

                var method = window.GetType().GetMethod("SetExpandedRecursive",
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance, null,
                    new[] { typeof(int), typeof(bool) }, null);

                if (method == null)
                {
                    Debug.LogError(
                        "Could not find method 'UnityEditor.SceneHierarchyWindow.SetExpandedRecursive(int, bool)'.");
                    return;
                }

                var field = scene.GetType().GetField("m_Handle",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field == null)
                {
                    Debug.LogError("Could not find field 'int UnityEngine.SceneManagement.Scene.m_Handle'.");
                    return;
                }

                var sceneHandle = field.GetValue(scene);
                method.Invoke(window, new[] { sceneHandle, expand });
            }
#endif
        }
    }
}
