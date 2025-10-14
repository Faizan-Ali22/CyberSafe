using System;
using UnityEngine;

namespace DA_Assets.DAI
{
    [System.Serializable]
    public struct CachedTextureEntry
    {
        public string Key;
        public Texture2D Texture;
    }

    [Serializable]
    public struct ColorScheme
    {
        public Color BackgroundColor;
        //public bool UseGradient;
        //public float GradientAngle;
        //public Gradient BackgroundGradient;
        public Color OutlineColor;
        public Color UnityGuiColor;
        public Color CheckBoxColor;
        public Color SelectionColor;
        public Color TextColor;

    }
}
