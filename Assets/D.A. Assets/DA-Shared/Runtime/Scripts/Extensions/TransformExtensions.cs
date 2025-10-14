using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Removes all childs from Transform.
        /// <para><see href="https://www.noveltech.dev/unity-delete-children/"/></para>
        /// </summary>
        public static int ClearChilds(this Transform transform)
        {
            int childCount = transform.childCount;

            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject go = transform.GetChild(i).gameObject;
                go.Destroy();
            }

            return childCount;
        }

        public static List<Transform> GetTopLevelChilds(this Transform parentObject)
        {
            List<Transform> childs = new List<Transform>();

            Transform current = parentObject;

            while (current.parent) // Go up until obj does not have a parent
                current = current.parent;

            foreach (Transform child in current) // iterate over children
            {
                childs.Add(child);
            }

            return childs;
        }

        public static void SetTransformProps(this TransformProps transformProps, Transform source)
        {
            transformProps.position = source.position;
            transformProps.rotation = source.rotation;
            transformProps.parent = source.parent;
        }

        public static void SetTransform(this Transform target, TransformProps transformProps)
        {
            target.transform.position = transformProps.position;
            target.transform.rotation = transformProps.rotation;
            target.transform.parent = transformProps.parent;
        }
    }
    public struct TransformProps
    {
        public Vector3 position;
        public Quaternion rotation;
        public Transform parent;
    }
}
