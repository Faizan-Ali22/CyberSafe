using DA_Assets.DAI;
using DA_Assets.Logging;
using DA_Assets.Tools;
using System;
using System.IO;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class LocalizationSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] LocalizationComponent locComponent = LocalizationComponent.None;
        public LocalizationComponent LocalizationComponent
        {
            get => locComponent;
            set
            {
                switch (value)
                {
                    case LocalizationComponent.DALocalizator:
#if DALOC_EXISTS == false
                        DALogger.LogError(FcuLocKey.log_asset_not_imported.Localize(nameof(LocalizationComponent.DALocalizator)));
                        value = LocalizationComponent.None;                   
#endif
                        break;
                    case LocalizationComponent.I2Localization:
#if I2LOC_EXISTS == false
                        DALogger.LogError(FcuLocKey.log_asset_not_imported.Localize(nameof(LocalizationComponent.I2Localization)));
                        value = LocalizationComponent.None;
#endif
                        break;
                }

                SetValue(ref locComponent, value);
            }
        }

        [SerializeField] LocalizationKeyCaseType locKeyCaseType = LocalizationKeyCaseType.snake_case;
        [SerializeProperty(nameof(locKeyCaseType))]
        public LocalizationKeyCaseType LocKeyCaseType { get => locKeyCaseType; set => SetValue(ref locKeyCaseType, value); }

        [SerializeField] string currentFigmaLayoutCulture = FcuConfig.DefaultLocalizationCulture;
        public string CurrentFigmaLayoutCulture { get => currentFigmaLayoutCulture; set => SetValue(ref currentFigmaLayoutCulture, value); }

        [SerializeField] int locKeyMaxLenght = 24;
        public int LocKeyMaxLenght { get => locKeyMaxLenght; set => SetValue(ref locKeyMaxLenght, value); }

        [SerializeField] ScriptableObject localizator;
        public ScriptableObject Localizator { get => localizator; set => SetValue(ref localizator, value); }

        [SerializeField] string locFolderPath = Path.Combine("Assets", "Resources", "Localizations");
        public string LocFolderPath { get => locFolderPath; set => SetValue(ref locFolderPath, value); }

        [SerializeField] string locFileName = "Localization.csv";
        public string LocFileName { get => locFileName; set => SetValue(ref locFileName, value); }

        [SerializeField] CsvSeparator csvSeparator = CsvSeparator.Semicolon;
        public CsvSeparator CsvSeparator { get => csvSeparator; set => SetValue(ref csvSeparator, value); }
    }
}