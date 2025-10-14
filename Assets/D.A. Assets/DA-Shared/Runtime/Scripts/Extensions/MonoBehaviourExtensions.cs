using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0162

namespace DA_Assets.Extensions
{
    public static class MonoBehExtensions
    {
        public static void DestroyChilds(this GameObject parent)
        {
            if (parent == null)
                return;

            int childCount = parent.transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject go = parent.transform.GetChild(i).gameObject;
                go.Destroy();
            }

            Debug.Log("log_current_canvas_childs_destroy " + childCount);
        }

        public static bool TryGetComponentSafe<T>(this GameObject gameObject, out T component)
        {
            component = default;

            if (gameObject == null)
                return false;

            return gameObject.TryGetComponent(out component);
        }

        public static T[] GetChilds<T>(this GameObject gameObject)
        {
            T[] childs = gameObject.GetComponentsInChildren<T>(true).Skip(1).ToArray();
            return childs;
        }

        /// <summary>
        /// Removes the RectTransform component from a GameObject by creating a new GameObject
        /// with the same components (excluding RectTransform), children, and parent.
        /// </summary>
        /// <param name="gameObject">The GameObject from which to remove the RectTransform.</param>
        public static GameObject RemoveRectTransform(this GameObject gameObject)
        {
            // Create a new GameObject
            GameObject newGameObject = CreateEmptyGameObject();
            newGameObject.name = gameObject.name;

            // Save the siblingIndex of the old GameObject
            int siblingIndex = gameObject.transform.GetSiblingIndex();

            // Assign the parent object
            newGameObject.transform.SetParent(gameObject.transform.parent);
            newGameObject.transform.localPosition = gameObject.transform.localPosition;
            newGameObject.transform.localRotation = gameObject.transform.localRotation;
            newGameObject.transform.localScale = gameObject.transform.localScale;

            // Set siblingIndex for the new GameObject
            newGameObject.transform.SetSiblingIndex(siblingIndex);

            // Transfer all child objects
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                Transform child = gameObject.transform.GetChild(i);

                // Save the siblingIndex for child objects
                int childSiblingIndex = child.GetSiblingIndex();
                child.SetParent(newGameObject.transform);
                child.SetSiblingIndex(childSiblingIndex);
            }

            // Copy all components
            Component[] components = gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (!(component is Transform))
                {
                    Component newComponent = newGameObject.AddComponent(component.GetType());
                    component.CopySerializedFields(newComponent);
                }
            }

            // Destroy the old GameObject
            gameObject.Destroy();
            return newGameObject;
        }

        public static List<T> GetComponentsInReverseOrder<T>(this GameObject parent) where T : Component
        {
            List<T> results = new List<T>();
            AddComponentsInReverseOrder(parent.transform);
            return results;

            void AddComponentsInReverseOrder(Transform current)
            {
                for (int i = current.childCount - 1; i >= 0; i--)
                {
                    AddComponentsInReverseOrder(current.GetChild(i));
                }

                T component = current.GetComponent<T>();
                if (component != null/* && !results.Contains(component)*/)
                {
                    results.Add(component);
                }
            }
        }

        /// <summary>
        /// Saves the GameObject as a prefab asset at the specified local path and tries to get the component of type T from the prefab.
        /// </summary>
        /// <typeparam name="T">The type of the MonoBehaviour to retrieve from the prefab.</typeparam>
        /// <param name="gameObject">The GameObject to be saved as a prefab.</param>
        /// <param name="localPath">The local path within the project where the prefab should be saved.</param>
        /// <param name="savedPrefab">The component of type T retrieved from the prefab, or null if the operation failed.</param>
        /// <param name="ex">Any exceptions that occurred during the process.</param>
        /// <returns>True if the prefab was saved and the component of type T was successfully retrieved, otherwise false.</returns>
        public static bool SaveAsPrefabAsset<T>(this GameObject gameObject, string localPath, out T savedPrefab, out Exception ex) where T : MonoBehaviour
        {
            // Check for null GameObject
            if (gameObject == null)
            {
                ex = new NullReferenceException("GameObject is null.");
                savedPrefab = null;
                return false;
            }

#if UNITY_EDITOR
            // Save the GameObject as a prefab asset in Editor mode.
            GameObject prefabGo = null;

            try
            {
                prefabGo = UnityEditor.PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out bool success);
            }
            catch (Exception ex1)
            {
                ex = ex1;
            }

            if (prefabGo == null)
            {
                ex = new NullReferenceException("Prefab is null.");
                savedPrefab = null;
                return false;
            }

            // Attempt to get the component of type T from the saved prefab.
            if (prefabGo.TryGetComponent<T>(out T prefabComponent))
            {
                ex = null;
                savedPrefab = prefabComponent;
                return true;
            }
            else
            {
                // Handle the case where the component of type T can't be found on the prefab.
                ex = new Exception($"Can't get Type '{typeof(T).Name}' from GameObject '{prefabGo.name}'.");
                savedPrefab = null;
                return false;
            }
#endif

            // Handle cases outside of Editor mode.
            ex = new Exception("Unsupported in not-Editor mode.");
            savedPrefab = null;
            return false;
        }

        /// <summary>
        /// Checks if the provided UnityEngine.Object is part of any prefab.
        /// </summary>
        /// <param name="gameObject">The UnityEngine.Object to check.</param>
        /// <returns>True if the object is part of a prefab, otherwise false.</returns>
        public static bool IsPartOfAnyPrefab(this UnityEngine.Object gameObject)
        {
            if (gameObject == null)
                return false;
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.IsPartOfAnyPrefab(gameObject);
#endif
            return false;
        }

        /// <summary>
        /// Checks if any instance of the provided MonoBehaviour type exists on the scene.
        /// </summary>
        /// <typeparam name="T">Type of MonoBehaviour to check for.</typeparam>
        /// <returns>True if at least one instance of T exists on the scene, otherwise false.</returns>
        public static bool IsExistsOnScene<T>() where T : MonoBehaviour
        {
            int count = MonoBehaviour.FindObjectsByType<T>(FindObjectsSortMode.None).Length;
            return count != 0;
        }

        /// <summary>
        /// Destroying Unity GameObject, but as an extension.
        /// <para>Works in Editor and Playmode.</para>
        /// </summary>
        /// <summary>
        public static bool Destroy(this UnityEngine.Object @object)
        {
            if (@object == null)
                return false;

            try
            {
                if (@object != null)
                {
                    //Debug.LogError($"Destroy | {unityObject.name}");
                }

#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(@object);
#else
                UnityEngine.Object.Destroy(@object);
#endif
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="component"></param>
        /// <returns>Returns whether a component of the input type has been added.</returns>
        public static bool TryAddComponent<T>(this GameObject gameObject, out T component, bool supportMultiInstance = false) where T : UnityEngine.Component
        {
            component = null;

            if (gameObject == null)
            {
                Debug.LogWarning("gameObject is null.");
                return false;
            }

            if (gameObject.TryGetComponent(out component) && !supportMultiInstance)
            {
                return true;
            }
            else
            {
                component = gameObject.AddComponent<T>();
                return false;
            }
        }

        public static bool TryGetComponent<T>(this GameObject gameObject, out T component) where T : UnityEngine.Component
        {
            try
            {
                component = gameObject.GetComponent<T>();
                string _ = component.name;
                return true;
            }
            catch
            {
                component = default;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="graphic">Found or added graphic component.</param>
        /// <returns>Returns whether a component of the input type has been added.</returns>
        public static bool TryAddGraphic<T>(this GameObject gameObject, out T graphic) where T : Graphic
        {
            if (gameObject.TryGetComponent(out graphic))
            {
                return false;
            }
            else if (gameObject.TryGetComponent(out Graphic _graphic))
            {
                return false;
            }
            else
            {
                graphic = gameObject.AddComponent<T>();
                return true;
            }
        }

        public static bool TryDestroyComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
        {
            if (gameObject.TryGetComponent(out T component))
            {
                component.Destroy();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Marks target object as dirty, but as an extension.
        /// </summary>
        /// <param name="object">The object to mark as dirty.</param>
        public static void SetDirtyExt(this UnityEngine.Object @object)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(@object);
#endif
        }

        public static void MakeGameObjectSelectedInHierarchy(this GameObject activeGameObject)
        {
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = activeGameObject;
#endif
        }

        public static GameObject CreateEmptyGameObject(string name = null, Transform parent = null)
        {
            GameObject tempGO = new GameObject();
            GameObject emptyGO;

            if (parent == null)
            {
                emptyGO = UnityEngine.Object.Instantiate(tempGO);
            }
            else
            {
                emptyGO = UnityEngine.Object.Instantiate(tempGO, parent);
            }

            if (name != null)
            {
                tempGO.name = name;
            }

            tempGO.Destroy();
            return emptyGO;
        }
    }
}