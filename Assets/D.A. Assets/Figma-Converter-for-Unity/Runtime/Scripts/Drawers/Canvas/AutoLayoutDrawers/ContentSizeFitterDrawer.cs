using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using System;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ContentSizeFitterDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            switch (fobject.Style.TextAutoResize)
            {
                case TextAutoResize.WIDTH_AND_HEIGHT:
                    fobject.Data.GameObject.TryAddComponent(out ContentSizeFitter csfWH);

                    csfWH.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    csfWH.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
                case TextAutoResize.HEIGHT:
                    fobject.Data.GameObject.TryAddComponent(out ContentSizeFitter csfH);

                    csfH.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    csfH.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    break;
            }
        }
    }
}