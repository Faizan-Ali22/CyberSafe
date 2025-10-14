using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class NovaSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] Texture inputTexture;
        [SerializeProperty(nameof(inputTexture))]
        public Texture InputTexture { get => inputTexture; set => SetValue(ref inputTexture, value); }
    }
}