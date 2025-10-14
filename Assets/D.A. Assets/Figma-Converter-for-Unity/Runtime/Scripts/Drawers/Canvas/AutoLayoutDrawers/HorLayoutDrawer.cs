using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class HorLayoutDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            fobject.Data.GameObject.TryAddComponent(out HorizontalLayoutGroup layoutGroup);

            layoutGroup.childAlignment = fobject.GetHorLayoutAnchor();
            layoutGroup.padding = fobject.Data.FRect.padding.ToRectOffset();
#if UNITY_2020_1_OR_NEWER
            layoutGroup.reverseArrangement = false;
#endif
            layoutGroup.childScaleWidth = false;
            layoutGroup.childScaleHeight = false;

            var childControl = fobject.GetChildControlByLayoutMode(LayoutMode.HORIZONTAL);
            layoutGroup.childControlWidth = childControl.childControlWidth;
            layoutGroup.childControlHeight = childControl.childControlHeight;

            layoutGroup.spacing = fobject.GetHorSpacing();

            if (layoutGroup.childControlWidth)
            {
                layoutGroup.childForceExpandWidth = true;
            }
            else if (fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.SPACE_BETWEEN)
            {
                layoutGroup.childForceExpandWidth = true;
            }
            else
            {
                layoutGroup.childForceExpandWidth = false;
            }

            if (layoutGroup.childControlHeight)
            {
                layoutGroup.childForceExpandHeight = true;
            }
            else if (fobject.CounterAxisAlignItems == CounterAxisAlignItem.SPACE_BETWEEN)
            {
                layoutGroup.childForceExpandHeight = true;
            }
            else
            {
                layoutGroup.childForceExpandHeight = false;
            }

            // Add and configure ContentSizeFitter
            fobject.Data.GameObject.TryAddComponent(out ContentSizeFitter csf);

            if (fobject.PrimaryAxisSizingMode == PrimaryAxisSizingMode.AUTO || 
                fobject.PrimaryAxisSizingMode == PrimaryAxisSizingMode.NONE)
            {
                csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else
            {
                csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            }

            if (fobject.CounterAxisSizingMode == CounterAxisSizingMode.AUTO || 
                fobject.CounterAxisSizingMode == CounterAxisSizingMode.NONE)
            {
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else
            {
                csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            }
        }
    }
}