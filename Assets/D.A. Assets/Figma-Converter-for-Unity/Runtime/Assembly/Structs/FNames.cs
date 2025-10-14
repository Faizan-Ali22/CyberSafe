using DA_Assets.Tools;
using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class FNames
    {
        [SerializeField] SerializedDictionary<FcuNameType, string> names = new SerializedDictionary<FcuNameType, string>();
        public SerializedDictionary<FcuNameType, string> Names => names;

        public string this[FcuNameType key]
        {
            get => names.TryGetValue(key, out var value) ? value : string.Empty;
            set => names[key] = value;
        }

        public string UITK_SpritePath
        {
            get => this[FcuNameType.UITK_SpritePath];
            set => this[FcuNameType.UITK_SpritePath] = value;
        }

        public string UITK_FontPath
        {
            get => this[FcuNameType.UITK_FontPath];
            set => this[FcuNameType.UITK_FontPath] = value;
        }

        public string FigmaName
        {
            get => this[FcuNameType.Figma];
            set => this[FcuNameType.Figma] = value;
        }

        public string HumanizedTextPrefabName
        {
            get => this[FcuNameType.HumanizedTextPrefabName];
            set => this[FcuNameType.HumanizedTextPrefabName] = value;
        }

        public string FileName
        {
            get => this[FcuNameType.File];
            set => this[FcuNameType.File] = value;
        }

        public string ObjectName
        {
            get => this[FcuNameType.Object];
            set => this[FcuNameType.Object] = value;
        }

        public string UitkGuid
        {
            get => this[FcuNameType.UitkGuid];
            set => this[FcuNameType.UitkGuid] = value;
        }

        public string FieldName
        {
            get => this[FcuNameType.Field];
            set => this[FcuNameType.Field] = value;
        }

        public string MethodName
        {
            get => this[FcuNameType.Method];
            set => this[FcuNameType.Method] = value;
        }

        public string ClassName
        {
            get => this[FcuNameType.Class];
            set => this[FcuNameType.Class] = value;
        }

        public string UssClassName
        {
            get => this[FcuNameType.UssClass];
            set => this[FcuNameType.UssClass] = value;
        }

        public string UxmlPath
        {
            get => this[FcuNameType.UxmlPath];
            set => this[FcuNameType.UxmlPath] = value;
        }

        public string LocKey
        {
            get => this[FcuNameType.LocKey];
            set => this[FcuNameType.LocKey] = value;
        }
    }

}
