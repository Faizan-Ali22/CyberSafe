using DA_Assets.DAI;
using System;
using System.IO;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class PrefabSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] string prefabsPath = Path.Combine("Assets", "Prefabs");
        public string PrefabsPath { get => prefabsPath; set => SetValue(ref prefabsPath, value); }

        [SerializeField] TextPrefabNameType textPrefabNameType = TextPrefabNameType.HumanizedColorString;
        public TextPrefabNameType TextPrefabNameType { get => textPrefabNameType; set => SetValue(ref textPrefabNameType, value); }
    }

    public enum TextPrefabNameType
    {
        HumanizedColorString,
        HumanizedColorHEX,
        Figma,
    }
}