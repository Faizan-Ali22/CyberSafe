using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class CanvasGroupDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            if (fobject.Data.FcuImageType == FcuImageType.Downloadable)
                return;

            fobject.Data.GameObject.TryAddComponent(out CanvasGroup canvasGroup);
            canvasGroup.alpha = (float)fobject.Opacity;
        }
    }
}