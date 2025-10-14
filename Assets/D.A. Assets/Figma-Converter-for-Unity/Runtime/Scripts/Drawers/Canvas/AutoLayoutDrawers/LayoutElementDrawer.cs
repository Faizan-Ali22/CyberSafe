using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using System;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class LayoutElementDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, FObject parent)
        {
            fobject.Data.GameObject.TryAddComponent(out LayoutElement layoutElement);



            //  layoutElement.preferredWidth = fobject.Size.x;
            //  layoutElement.preferredHeight = fobject.Size.y;

            if (parent.LayoutWrap == LayoutWrap.WRAP)
            {
                layoutElement.minWidth = fobject.Size.x;
                layoutElement.minHeight = fobject.Size.y;
            }

            if (fobject.LayoutPositioning == LayoutPositioning.ABSOLUTE)
            {
                layoutElement.ignoreLayout = true;
            }
            else
            {
                layoutElement.ignoreLayout = false;
            }
        }
    }
}