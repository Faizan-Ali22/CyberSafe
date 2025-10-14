using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using System;
using UnityEngine.UI;
using static UnityEngine.UI.AspectRatioFitter;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class AspectRatioFitterDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            AspectRatioFitter arf;

            switch (monoBeh.Settings.ImageSpritesSettings.PreserveRatioMode)
            {
                case PreserveRatioMode.WidthControlsHeight:
                    fobject.Data.GameObject.TryAddComponent(out arf);
                    arf.aspectMode = ConvertMode(monoBeh.Settings.ImageSpritesSettings.PreserveRatioMode);
                    break;
                case PreserveRatioMode.HeightControlsWidth:
                    fobject.Data.GameObject.TryAddComponent(out arf);
                    arf.aspectMode = ConvertMode(monoBeh.Settings.ImageSpritesSettings.PreserveRatioMode);
                    break;
                default:
                    break;
            }
        }

        private AspectMode ConvertMode(PreserveRatioMode mode)
        {
            AspectMode newMode = AspectMode.None;

            switch (mode)
            {
                case PreserveRatioMode.WidthControlsHeight:
                    newMode = AspectMode.WidthControlsHeight;
                    break;
                case PreserveRatioMode.HeightControlsWidth:
                    newMode = AspectMode.HeightControlsWidth;
                    break;
                default:
                    break;
            }

            return newMode;
        }
    }
}