#if I2LOC_EXISTS && UNITY_EDITOR
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using I2.Loc;
using System;
using System.Text;
using UnityEngine;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class I2LocalizationDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] LanguageSource languageSource;

        public void Init()
        {
            if (languageSource == null)
            {
#if UNITY_6000_0_OR_NEWER
                languageSource = MonoBehaviour.FindFirstObjectByType<LanguageSource>();
#else
                languageSource = MonoBehaviour.FindObjectOfType<LanguageSource>();
#endif

                if (languageSource == null)
                {
                    GameObject _gameObject = MonoBehExtensions.CreateEmptyGameObject();
                    _gameObject.name = FcuConfig.I2LocGameObjectName;
                    languageSource = _gameObject.AddComponent<LanguageSource>();
                }
            }
        }

        public void ConnectTable(string filePath)
        {
            ImportCSV(filePath, eSpreadsheetUpdateMode.Replace);
        }

        public void Draw(string locKey, FObject fobject)
        {
            fobject.Data.GameObject.TryAddComponent(out I2.Loc.Localize i2l);
            i2l.Source = languageSource;
            i2l.Term = locKey;
        }

        private void ImportCSV(string FileName, eSpreadsheetUpdateMode updateMode)
        {
            languageSource.mSource.Import_CSV(
                "",
                LocalizationReader.ReadCSVfile(FileName, Encoding.UTF8),
                updateMode,
                (char)monoBeh.Settings.LocalizationSettings.CsvSeparator);

            languageSource.mSource.Awake();
        }
    }
}
#endif