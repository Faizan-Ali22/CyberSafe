using DA_Assets.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.DAI
{
    public class LinkedEditorWindow<T1, T2, T3> : EditorWindow, IDAEditor
        where T1 : LinkedEditorWindow<T1, T2, T3>
        where T2 : Editor
        where T3 : MonoBehaviour
    {
        private static Dictionary<int, T1> _instances = new Dictionary<int, T1>();

        public static T1 GetInstance(T2 inspector, T3 monoBeh, Vector2 windowSize, bool fixedSize, string title)
        {
            T1 result;

            _instances.TryGetValue(monoBeh.GetInstanceID(), out result);

            if (result.IsDefault())
            {
                result = ScriptableObject.CreateInstance<T1>();
                _instances[monoBeh.GetInstanceID()] = result;
                result.titleContent = new GUIContent(title);
            }

            result.Inspector = inspector;
            result.MonoBeh = monoBeh;

            if (result.SerializedObject == null)
            {
                result.SerializedObject = new SerializedObject(monoBeh);
            }

            result.WindowSize = windowSize;
            result.FixedSize = fixedSize;

            return result;
        }

        [SerializeField] DAInspector _gui;
        public DAInspector gui { get => _gui; set => _gui = value; }

        protected T2 inspector;
        public T2 Inspector { get => inspector; set => inspector = value; }

        protected T3 monoBeh;
        public T3 MonoBeh { get => monoBeh; set => monoBeh = value; }

        protected SerializedObject serializedObject;
        public SerializedObject SerializedObject { get => serializedObject; set => serializedObject = value; }

        private Vector2 windowSize = new Vector2(800, 600);
        public Vector2 WindowSize { get => windowSize; set => windowSize = value; }

        private bool fixedSize = false;
        public bool FixedSize { get => fixedSize; set => fixedSize = value; }

        private bool onEnable;
        private bool init;

        public virtual void OnShow() { }
        public virtual void DrawGUI() { }

        public void OnEnable()
        {
            onEnable = true;
            init = false;

            DARunner.update += Repaint;
        }

        public void OnDisable()
        {
            DARunner.update -= Repaint;
        }

        public new void Show()
        {
            Show(immediateDisplay: false);

            this.position = new Rect(
                (Screen.currentResolution.width - windowSize.x * 2) / 2,
                (Screen.currentResolution.height - windowSize.y * 2) / 2,
                windowSize.x,
                windowSize.y);

            if (fixedSize)
            {
                this.minSize = windowSize;
                this.maxSize = windowSize;
            }

            OnShow();
        }

        void OnGUI()
        {
            if (onEnable)
            {
                if (monoBeh == null)
                {               
                    this.Close();
                    return;
                }
                else
                {
                    if (serializedObject == null)
                    {
                        serializedObject = new SerializedObject(monoBeh);
                    }

                    if (!init)
                    {
                        init = true;
                        OnShow();
                    }
                }
            }

            _gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = _gui.ColoredStyle.WindowRootBg,
                Body = () =>
                {
                    DrawGUI();
                }
            });
        }
    }
}
