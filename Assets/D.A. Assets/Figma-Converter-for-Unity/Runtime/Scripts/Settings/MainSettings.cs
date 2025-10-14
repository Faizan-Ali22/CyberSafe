using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.Logging;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class MainSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] UIFramework uiFramework = UIFramework.UGUI;
        public UIFramework UIFramework
        {
            get => uiFramework;
            set
            {
                switch (value)
                {
                    case UIFramework.UITK:
#if FCU_UITK_EXT_EXISTS == false
                        DALogger.LogError(FcuLocKey.log_asset_not_imported.Localize(nameof(UIFramework.UITK)));
                        value = UIFramework.UGUI;
#endif
                        break;
                    case UIFramework.NOVA:

#if NOVA_UI_EXISTS == false
                        DALogger.LogError(FcuLocKey.log_asset_not_imported.Localize(nameof(UIFramework.NOVA)));
                        value = UIFramework.UGUI;
#endif
                        break;
                }

                SetValue(ref uiFramework, value);
            }
        }

        [SerializeField] PositioningMode positioningMode = PositioningMode.Absolute;
        public PositioningMode PositioningMode
        {
            get => positioningMode;
            set
            {
                if (value == PositioningMode.GameView && uiFramework != UIFramework.UGUI)
                {
                    Debug.LogError($"{nameof(PositioningMode)}.{value} is not supported in {uiFramework} framework.");
                    value = PositioningMode.Absolute;
                }

                SetValue(ref positioningMode, value);
            }
        }

        [SerializeField] PivotType pivotType = PivotType.MiddleCenter;
        public PivotType PivotType { get => pivotType; set => SetValue(ref pivotType, value); }

        [SerializeField] int goLayer = 5;
        public int GameObjectLayer { get => goLayer; set => SetValue(ref goLayer, value); }

        [SerializeField] bool rawImport = false;
        public bool RawImport
        {
            get => rawImport;
            set
            {
                if (value && value != rawImport)
                {
                    DALogger.LogError(FcuLocKey.log_dev_function_enabled.Localize(FcuLocKey.label_raw_import.Localize()));
                }

                SetValue(ref rawImport, value);
            }
        }

        [SerializeField] bool https = true;
        public bool Https { get => https; set => SetValue(ref https, value); }

        [SerializeField] int gameObjectNameMaxLength = 32;
        public int GameObjectNameMaxLenght { get => gameObjectNameMaxLength; set => SetValue(ref gameObjectNameMaxLength, value); }

        [SerializeField] int textObjectNameMaxLength = 16;
        public int TextObjectNameMaxLenght { get => textObjectNameMaxLength; set => SetValue(ref textObjectNameMaxLength, value); }

        [Tooltip(@"Characters, aside from Latin letters and numbers, that may appear in GameObject names.

Some characters will be ignored in certain cases, such as when a backslash is used in a sprite name.

If you add new characters to this list, the stable operation of the asset cannot be guaranteed.")]
        [SerializeField] char[] allowedNameChars = new char[] { '_', ' ', '(', ')', '=', '.', '-', '[', ']', '+' };
        [SerializeProperty(nameof(allowedNameChars))]
        public char[] AllowedNameChars { get => allowedNameChars; set => SetValue(ref allowedNameChars, value); }

        [SerializeField] bool windowMode = false;
        public bool WindowMode { get => windowMode; set => SetValue(ref windowMode, value); }

        [SerializeField] string projectUrl;

        public string ProjectUrl
        {
            get => projectUrl;
            set
            {
                string _value = value;

                try
                {
                    string fileTag = "/file/";
                    char del = '/';

                    if (_value.IsEmpty())
                    {

                    }
                    else if (_value.Contains(fileTag))
                    {
                        _value = _value.GetBetween(fileTag, del.ToString());
                    }
                    else if (_value.Contains(del.ToString()))
                    {
                        string[] splited = value.Split(del);
                        _value = splited[4];
                    }
                }
                catch
                {
                    Debug.LogError(FcuLocKey.log_incorrent_project_url.Localize());
                }

                SetValue(ref projectUrl, _value);
            }
        }
    }
}