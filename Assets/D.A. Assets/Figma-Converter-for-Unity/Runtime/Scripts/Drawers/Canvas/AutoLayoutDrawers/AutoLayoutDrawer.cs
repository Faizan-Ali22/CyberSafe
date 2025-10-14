using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class AutoLayoutDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            if (fobject.Data.FRect.absoluteAngle != 0)
            {
                return;
            }

            foreach (int index in fobject.Data.ChildIndexes)
            {
                if (monoBeh.CurrentProject.TryGetByIndex(index, out FObject child))
                {
                    if (child.Data.FRect.absoluteAngle != 0)
                    {
                        return;
                    }

                    if (child.Data.GameObject != null)
                    {
                        this.LayoutElementDrawer.Draw(child, fobject);
                    }
                }
            }

            if (fobject.Data.GameObject.TryGetComponentSafe(out LayoutGroup oldLayoutGroup))
            {
                oldLayoutGroup.Destroy();
            }

            if (fobject.LayoutWrap == LayoutWrap.WRAP)
            {
                this.GridLayoutDrawer.Draw(fobject);
            }
            else if (fobject.LayoutMode == LayoutMode.HORIZONTAL)
            {
                this.HorLayoutDrawer.Draw(fobject);
            }
            else if (fobject.LayoutMode == LayoutMode.VERTICAL)
            {
                this.VertLayoutDrawer.Draw(fobject);
            }
        }

        [SerializeField] GridLayoutDrawer gridLayoutDrawer;
        [SerializeProperty(nameof(gridLayoutDrawer))]
        public GridLayoutDrawer GridLayoutDrawer => monoBeh.Link(ref gridLayoutDrawer);

        [SerializeField] VertLayoutDrawer vertLayoutDrawer;
        [SerializeProperty(nameof(vertLayoutDrawer))]
        public VertLayoutDrawer VertLayoutDrawer => monoBeh.Link(ref vertLayoutDrawer);

        [SerializeField] HorLayoutDrawer horLayoutDrawer;
        [SerializeProperty(nameof(horLayoutDrawer))]
        public HorLayoutDrawer HorLayoutDrawer => monoBeh.Link(ref horLayoutDrawer);

        [SerializeField] LayoutElementDrawer layoutElementDrawer;
        [SerializeProperty(nameof(layoutElementDrawer))]
        public LayoutElementDrawer LayoutElementDrawer => monoBeh.Link(ref layoutElementDrawer);
    }
}