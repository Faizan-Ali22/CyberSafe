using DA_Assets.Constants;
using DA_Assets.DM;
using DA_Assets.Extensions;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.Tools
{
    public class ContextMenuItems
    {
        public const string ResetToPrefabState = "Reset to prefab state";
        public const string ResetAllComponents = "Reset all components to prefab state";
        public const string DestroyChilds = "Destroy Childs";
        public const string TryBackupActiveScene = "Try Backup Active Scene";
        public const string SimplifyHierarchy = "Simplify the hierarchy";

        [MenuItem("Tools/" + DAConstants.Publisher + "/" + nameof(DA_Assets.Tools) + ": " + "Dependency Manager", false, 89)]
        public static void ShowWindow()
        {
            DependenciesWindow window = EditorWindow.GetWindow<DependenciesWindow>("Dependency Manager");
            window.minSize = new Vector2(550, 550);
            window.maxSize = new Vector2(550, 550);
            window.Show();
        }

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + nameof(DA_Assets.Tools) + ": " + DestroyChilds, false, 1)]
        private static void DestroyChilds_OnClick()
        {
            bool backuped = SceneBackuper.TryBackupActiveScene();

            if (!backuped)
            {
                Debug.LogError("log_cant_execute_because_no_backup");
                return;
            }

            if (Selection.activeGameObject != null)
            {
                Selection.activeGameObject.DestroyChilds();
            }
            else
            {
                Debug.LogError($"Selection.activeGameObject is null.");
            }
        }

        [MenuItem("Tools/" + DAConstants.Publisher + "/" + nameof(DA_Assets.Tools) + ": " + TryBackupActiveScene, false, 2)]
        private static void TryBackupActiveScene_OnClick()
        {
            SceneBackuper.TryBackupActiveScene();
        }

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + nameof(DA_Assets.Tools) + ": " + SimplifyHierarchy, false, 92)]
        private static void SetSelectedAsParentForAllChilds_OnClick()
        {
            GameObject selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject == null)
            {
                Debug.LogError(string.Format(nameof(GameObject), "'{0}' not selected in hierarchy."));
                return;
            }

            List<Transform> childs = new List<Transform>();
            SetSelectedAsParentForAllChild(selectedGameObject);
            foreach (Transform child in childs)
            {
                child.SetParent(selectedGameObject.transform);
            }

            void SetSelectedAsParentForAllChild(GameObject @object)
            {
                if (@object == null)
                    return;

                foreach (Transform child in @object.transform)
                {
                    if (child == null)
                        continue;

                    childs.Add(child);

                    SetSelectedAsParentForAllChild(child.gameObject);
                }
            }
        }

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + nameof(DA_Assets.Tools) + ": " + ResetToPrefabState, false, 93)]
        private static void ResetToPrefabState_OnClick()
        {
            GameObject selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject == null)
            {
                Debug.LogError(string.Format(nameof(GameObject), "'{0}' not selected in hierarchy."));
                return;
            }

            PrefabUtility.RevertPrefabInstance(Selection.activeGameObject, InteractionMode.AutomatedAction);

            Debug.Log(string.Format(selectedGameObject.name, "'{0}' has been reset to a prefab state."));
        }

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + nameof(DA_Assets.Tools) + ": " + ResetAllComponents, false, 94)]
        private static void ResetAllComponents_OnClick()
        {
            GameObject selectedGameObject = Selection.activeGameObject;

            if (selectedGameObject == null)
            {
                Debug.LogError(string.Format(nameof(GameObject), "'{0}' not selected in hierarchy."));
                return;
            }

            Component[] components = selectedGameObject.GetComponents<Component>();

            if (components.IsEmpty())
            {
                Debug.LogError(string.Format(selectedGameObject.name, "No components in '{0}'."));
                return;
            }

            int count = 0;

            foreach (var item in components)
            {
                SerializedObject serializedObject = new SerializedObject(item);
                SerializedProperty propertyIterator = serializedObject.GetIterator();

                while (propertyIterator.NextVisible(true))
                {
                    PrefabUtility.RevertPropertyOverride(propertyIterator, InteractionMode.AutomatedAction);
                    count++;
                }
            }

            Debug.Log(string.Format(count.ToString(), "{0} properties reset."));
        }
    }
}