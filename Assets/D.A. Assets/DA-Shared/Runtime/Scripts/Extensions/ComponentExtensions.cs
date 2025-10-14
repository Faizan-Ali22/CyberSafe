using System;
using System.Reflection;
using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class ComponentExtensions
    {
        public static void CopySerializedFields(this Component source, Component destination)
        {
            FieldInfo[] fields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                {
                    field.SetValue(destination, field.GetValue(source));
                }
            }
        }

        public static bool TryGetComponentSafe<T>(this Component gameObject, out T component)
        {
            component = default;

            if (gameObject == null)
                return false;

            return gameObject.TryGetComponent(out component);
        }

        /// <summary>
        /// Destroying script of Unity GameObject, but as an extension.
        /// <para>Works in Editor and Playmode.</para>
        /// </summary>
        public static bool Destroy(this UnityEngine.Component unityComponent)
        {
            try
            {
                if (unityComponent.IsRequiredByAnotherComponents())
                    return false;

#if UNITY_EDITOR
                UnityEngine.Object.DestroyImmediate(unityComponent);
#else
                UnityEngine.Object.Destroy(unityComponent);
#endif
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes all components from the GameObject that have a 'RequireComponent' attribute pointing to the given component.
        /// </summary>
        /// <param name="component">The target component which other components might depend on.</param>
        /// <returns>True if any dependent components were removed, false otherwise.</returns>
        public static bool RemoveComponentsDependingOn(this UnityEngine.Component component)
        {
            bool removedAny = false; // Flag to indicate if we've removed any components.
            Type type = component.GetType();
            // Get all components present on the GameObject where the target component resides.
            Component[] componentsOnObject = component.gameObject.GetComponents<Component>();

            // Iterate through each component on the GameObject.
            foreach (Component comp in componentsOnObject)
            {
                // Fetch all the RequireComponent attributes associated with the component.
                object[] requireAttributes = comp.GetType().GetCustomAttributes(typeof(RequireComponent), true);

                // Iterate through each RequireComponent attribute to check if our target component type is listed.
                foreach (RequireComponent attribute in requireAttributes)
                {
                    // Check if any of the required types match the type of our target component.
                    if (attribute.m_Type0 == type ||
                        attribute.m_Type1 == type ||
                        attribute.m_Type2 == type)
                    {
                        // Remove the component that depends on the target component.
                        comp.Destroy();// UnityEngine.Object.Destroy(comp);
                        removedAny = true; // Set the flag since we've removed a component.
                        break; // Exit the inner loop since we've made a modification.
                    }
                }
            }

            return removedAny; // Return the flag indicating if any components were removed.
        }

        /// <summary>
        /// Checks if the given component is required by any other components on the same GameObject via the RequireComponent attribute.
        /// </summary>
        /// <param name="component">The component to check for.</param>
        /// <returns>True if the component is required by another component on the same GameObject, otherwise false.</returns>
        public static bool IsRequiredByAnotherComponents(this UnityEngine.Component component)
        {
            Type type = component.GetType();
            // Get all components present on the GameObject where the target component resides.
            Component[] componentsOnObject = component.gameObject.GetComponents<Component>();

            // Iterate through each component on the GameObject.
            foreach (Component comp in componentsOnObject)
            {
                // Fetch all the RequireComponent attributes associated with the component.
                object[] requireAttributes = comp.GetType().GetCustomAttributes(typeof(RequireComponent), true);

                // Iterate through each RequireComponent attribute to check if our target component type is listed.
                foreach (RequireComponent attribute in requireAttributes)
                {
                    // Check if any of the required types match the type of our target component.
                    if (attribute.m_Type0 == type ||
                        attribute.m_Type1 == type ||
                        attribute.m_Type2 == type)
                    {
                        // If a match is found, return true indicating the component is required by another.
                        return true;
                    }
                }
            }

            // If no matches were found, return false.
            return false;
        }

    }
}
