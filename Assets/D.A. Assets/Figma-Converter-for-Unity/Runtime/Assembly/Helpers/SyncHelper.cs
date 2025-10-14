using DA_Assets.FCU.Model;
using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable, DisallowMultipleComponent]
    public class SyncHelper : MonoBehaviour
    {
        [SerializeField] bool debug;
        public bool Debug { get => debug; set => debug = value; }   

        [SerializeField] SyncData data;
        public SyncData Data { get => data; set => data = value; }

        public int HierarchyLevel
        {
            get
            {
                int level = 0;
                Transform current = transform;

                while (current.parent != null)
                {
                    level++;
                    current = current.parent;
                }

                return level;
            }
        }

        void OnValidate()
        {
            if (data != null)
            {
                data.GameObject = gameObject;
            }
        }
    }
}