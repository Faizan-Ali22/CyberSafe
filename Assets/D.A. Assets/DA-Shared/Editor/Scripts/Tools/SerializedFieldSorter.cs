using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace DA_Assets.Tools
{
    public class SerializedFieldSorter
    {
        public static void DrawSorted(SerializedObject serializedObject, Dictionary<string, bool> foldoutStates)
        {
            serializedObject.Update();

            // Dictionary to group properties by their component type names
            Dictionary<string, List<SerializedProperty>> groupedProperties = new Dictionary<string, List<SerializedProperty>>();

            // Iterate through all serialized properties
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false; // Only iterate through properties, not their children

                // Skip the script reference property
                if (property.propertyPath == "m_Script")
                    continue;

                // Check if the property is an ObjectReference
                if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
                {
                    Type propType = property.objectReferenceValue.GetType();
                    string typeName = propType.Name;

                    // Initialize the list if the type is encountered for the first time
                    if (!groupedProperties.ContainsKey(typeName))
                    {
                        groupedProperties[typeName] = new List<SerializedProperty>();
                    }

                    groupedProperties[typeName].Add(property.Copy());
                }
            }

            // Iterate through each group and display properties
            foreach (var group in groupedProperties.OrderBy(g => g.Key))
            {
                string typeName = group.Key;
                List<SerializedProperty> properties = group.Value;

                if (properties.Count > 1)
                {
                    // If the group has multiple properties, display them under a foldout
                    if (!foldoutStates.ContainsKey(typeName))
                    {
                        foldoutStates[typeName] = false;
                    }

                    foldoutStates[typeName] = EditorGUILayout.Foldout(foldoutStates[typeName], $"{typeName}s");

                    if (foldoutStates[typeName])
                    {
                        EditorGUI.indentLevel++;
                        foreach (var prop in properties)
                        {
                            EditorGUILayout.PropertyField(prop, true);
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    // If the group has only one property, display it directly
                    SerializedProperty singleProp = properties.First();
                    EditorGUILayout.PropertyField(singleProp, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
