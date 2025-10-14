using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public struct FontStruct
    {
        [SerializeField] FontSubset fontSubset;
        [SerializeField] FontItem fontItem;
        [SerializeField] FontMetadata fontMetadata;
        [SerializeField] byte[] bytes;
        [SerializeField] UnityEngine.Object font;

        public FontSubset FontSubset { get => fontSubset; set => fontSubset = value; }
        public FontMetadata FontMetadata { get => fontMetadata; set => fontMetadata = value; }
        public byte[] Bytes { get => bytes; set => bytes = value; }
        public UnityEngine.Object Font { get => font; set => font = value; }
    }
}