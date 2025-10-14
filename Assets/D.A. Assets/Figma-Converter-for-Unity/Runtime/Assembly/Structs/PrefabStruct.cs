using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public struct PrefabStruct
    {
        [SerializeField] string name;
        public string Name { get => name; set => name = value; }

        [SerializeField] int current;
        public int Current { get => current; set => current = value; }

        [SerializeField] int parent;
        public int Parent { get => parent; set => parent = value; }

        [Space]

        [SerializeField] List<int> childs;
        public List<int> Childs { get => childs; set => childs = value; }

        [Space]

        [SerializeField] int siblingIndex;
        public int SiblingIndex { get => siblingIndex; set => siblingIndex = value; }

        [SerializeField] int hash;
        public int Hash { get => hash; set => hash = value; }

        [SerializeField] string id;
        public string Id { get => id; set => id = value; }

        [SerializeField] int prefabNumber;
        public int PrefabNumber { get => prefabNumber; set => prefabNumber = value; }

        [SerializeField] string text;
        public string Text { get => text; set => text = value; }


        [SerializeField] UguiTransformData uguiTransformData;
        public UguiTransformData UguiTransformData { get => uguiTransformData; set => uguiTransformData = value; }

#if NOVA_UI_EXISTS
        [SerializeField] NovaTransformData novaTransformData;
        public NovaTransformData NovaTransformData { get => novaTransformData; set => novaTransformData = value; }
#endif
    }
}