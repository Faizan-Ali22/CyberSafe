using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

#if DALOC_EXISTS
using DA_Assets.MiniExcelLibs;
using DA_Assets.MiniExcelLibs.Csv;
#endif

#pragma warning disable CS0649
#pragma warning disable IDE0003

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class LocalizationDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private Dictionary<string, string> _localizationDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> LocalizationDictionary => _localizationDictionary;

        internal void Init()
        {
            _localizationDictionary.Clear();

            switch (monoBeh.Settings.LocalizationSettings.LocalizationComponent)
            {
                case LocalizationComponent.DALocalizator:
                    this.DALocalizatorDrawer.Init();
                    break;
                case LocalizationComponent.I2Localization:
#if I2LOC_EXISTS && UNITY_EDITOR
                    this.I2LocalizationDrawer.Init();
#endif
                    break;
            }
        }

        public void Draw(FObject fobject)
        {

            string locKey = fobject.Data.Names.LocKey;

            if (locKey.IsEmpty())
                return;

            string text = fobject.GetText();

            if (text.IsEmpty())
                return;

            _localizationDictionary.TryAddValue(locKey, text);

            switch (monoBeh.Settings.LocalizationSettings.LocalizationComponent)
            {
                case LocalizationComponent.DALocalizator:
                    this.DALocalizatorDrawer.Draw(locKey, fobject);
                    break;
                case LocalizationComponent.I2Localization:
#if I2LOC_EXISTS && UNITY_EDITOR
                    this.I2LocalizationDrawer.Draw(locKey, fobject);
#endif
                    break;
            }
        }

        public string SaveTable()
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Key", typeof(string));
            dataTable.Columns.Add(monoBeh.Settings.LocalizationSettings.CurrentFigmaLayoutCulture, typeof(string));

            foreach (var kvp in _localizationDictionary)
            {
                dataTable.Rows.Add(kvp.Key, kvp.Value);
            }

            string folderPath = monoBeh.Settings.LocalizationSettings.LocFolderPath;
            string fileNameNoExt = Path.GetFileNameWithoutExtension(monoBeh.Settings.LocalizationSettings.LocFileName);
            string fileExt = Path.GetExtension(monoBeh.Settings.LocalizationSettings.LocFileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string[] files = Directory.GetFiles(folderPath, $"{fileNameNoExt}*{fileExt}");
            int fileCount = files.Length;
            string newFileName = $"{fileNameNoExt}-{fileCount + 1}{fileExt}";
            string filePath = Path.Combine(folderPath, newFileName);

#if DALOC_EXISTS
            CsvConfiguration config = new CsvConfiguration()
            {
                Seperator = (char)monoBeh.Settings.LocalizationSettings.CsvSeparator
            };

            MiniExcel.SaveAs(filePath, dataTable, configuration: config);
#endif
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            string resourcesPath = RemoveAssetsAndResourcesFolders(filePath).RemovePathExtension();
            return resourcesPath;
        }

        static string RemoveAssetsAndResourcesFolders(string path)
        {
            string normalizedPath = path.Replace('\\', Path.DirectorySeparatorChar)
                                        .Replace('/', Path.DirectorySeparatorChar);

            string assetsResourcesPrefix = $"Assets{Path.DirectorySeparatorChar}Resources{Path.DirectorySeparatorChar}";
            string assetsPrefix = $"Assets{Path.DirectorySeparatorChar}";

            if (normalizedPath.StartsWith(assetsResourcesPrefix))
            {
                return normalizedPath.Substring(assetsResourcesPrefix.Length);
            }
            else if (normalizedPath.StartsWith(assetsPrefix))
            {
                return normalizedPath.Substring(assetsPrefix.Length);
            }
            else
            {
                return normalizedPath;
            }
        }

        public void SaveAndConnectTable()
        {
            string filePath = SaveTable();
            ConnectTable(filePath);
        }

        internal void ConnectTable(string filePath)
        {
            switch (monoBeh.Settings.LocalizationSettings.LocalizationComponent)
            {
                case LocalizationComponent.DALocalizator:
                    {
                        this.DALocalizatorDrawer.ConnectTable(filePath);
                    }
                    break;
                case LocalizationComponent.I2Localization:
                    {
#if I2LOC_EXISTS && UNITY_EDITOR
                        this.I2LocalizationDrawer.ConnectTable(filePath);
#endif
                    }
                    break;
            }
        }

#if I2LOC_EXISTS && UNITY_EDITOR
        [SerializeField] I2LocalizationDrawer i2lDrawer;
        [SerializeProperty(nameof(i2lDrawer))]
        public I2LocalizationDrawer I2LocalizationDrawer => monoBeh.Link(ref i2lDrawer);
#endif

        [SerializeField] DALocalizatorDrawer dalDrawer;
        [SerializeProperty(nameof(dalDrawer))]
        public DALocalizatorDrawer DALocalizatorDrawer => monoBeh.Link(ref dalDrawer);
    }
}