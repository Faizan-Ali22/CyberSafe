using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using System;
using UnityEngine;
using Resources = UnityEngine.Resources;

#if DALOC_EXISTS
using DA_Assets.DAL;
#endif

#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class DALocalizatorDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Init()
        {

        }

        public void ConnectTable(string filePath)
        {
            if (monoBeh.Settings.LocalizationSettings.Localizator == null)
            {
                DALogger.LogError("Localizator is null.");
                return;
            }

            TextAsset localizationFile = Resources.Load<TextAsset>(filePath);

            if (localizationFile != null)
            {
#if DALOC_EXISTS
                ILocalizator localizator = monoBeh.Settings.LocalizationSettings.Localizator as ILocalizator;
                localizator.TableConfigs[0].TextAsset = localizationFile;
#endif
            }
            else
            {
                Debug.LogError($"Localization file '{filePath}' could not be loaded!");
            }
        }

        public void Draw(string locKey, FObject fobject)
        {
#if DALOC_EXISTS
            if (monoBeh.UsingTextMesh())
            {
                fobject.Data.GameObject.TryAddComponent(out TextMeshLocalizator tmpText);
                tmpText.Key = locKey;
            }
            else if (monoBeh.UsingUnityText())
            {
                fobject.Data.GameObject.TryAddComponent(out UITextLocalizator uiTextLoc);
                uiTextLoc.Key = locKey;
            }
#endif
        }
    }
}