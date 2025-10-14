using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public struct FontMetadata
    {
        [SerializeField] string family;
        [SerializeField] int weight;
        [SerializeField] FontStyle fontStyle;

        public string Family { get => family; set => family = value; }
        public int Weight { get => weight; set => weight = value; }
        public FontStyle FontStyle { get => fontStyle; set => fontStyle = value; }
    }
}
