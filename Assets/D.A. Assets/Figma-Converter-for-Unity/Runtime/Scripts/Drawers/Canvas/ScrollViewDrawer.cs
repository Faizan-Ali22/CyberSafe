using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ScrollViewDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private List<FObject> scrollViews;
        public List<FObject> ScrollViews => scrollViews;

        public void Init()
        {
            scrollViews = new List<FObject>();
        }

        public void Draw(FObject fobject)
        {
            fobject.Data.GameObject.TryAddComponent(out ScrollRect scrollRect);
            scrollViews.Add(fobject);
        }

        public async Task SetTargetGraphics()
        {
            await SetTargetGraphicsScrollViews();
            scrollViews.Clear();
        }

        private async Task SetTargetGraphicsScrollViews()
        {
            foreach (FObject scrollViewFObject in scrollViews)
            {
                if (!scrollViewFObject.Data.GameObject.TryGetComponentSafe(out ScrollRect scrollRect))
                    continue;

                ScrollViewModel svm = GetGraphics(scrollViewFObject.Data);

                if (svm.Viewport.TryGetComponentSafe(out RectTransform viewport))
                {
                    viewport.gameObject.TryAddComponent(out RectMask2D viewportMask2D);
                    viewport.SetSmartAnchor(AnchorType.StretchAll);
                    viewport.SetSmartPivot(PivotType.TopLeft);

                    viewport.TryGetComponent(out SyncHelper viewportSyncHelper);
                    monoBeh.CurrentProject.TryGetById(viewportSyncHelper.Data.Id, out FObject viewportFObject);

                    scrollRect.viewport = viewport;

                    if (viewport != null)
                    {
                        switch (viewportFObject.OverflowDirection)
                        {
                            case OverflowDirection.HORIZONTAL_SCROLLING:
                                scrollRect.horizontal = true;
                                scrollRect.vertical = false;
                                break;
                            case OverflowDirection.VERTICAL_SCROLLING:
                                scrollRect.horizontal = false;
                                scrollRect.vertical = true;
                                break;
                            case OverflowDirection.HORIZONTAL_AND_VERTICAL_SCROLLING:
                                scrollRect.horizontal = true;
                                scrollRect.vertical = true;
                                break;
                            default:
                                scrollRect.horizontal = false;
                                scrollRect.vertical = false;
                                break;
                        }
                    }
                }

                if (svm.Content.TryGetComponentSafe(out RectTransform content))
                {
                    content.gameObject.TryDestroyComponent<RectMask2D>();
                    content.SetSmartAnchor(AnchorType.HorStretchTop);
                    content.SetSmartPivot(PivotType.TopLeft);

                    scrollRect.content = content;
                }

                scrollRect.enabled = false;
                await Task.Delay(10);
                scrollRect.enabled = true;
            }
        }

        private ScrollViewModel GetGraphics(SyncData syncData)
        {
            SyncHelper[] syncHelpers = syncData.GameObject.GetChilds<SyncHelper>();

            ScrollViewModel scroll = new ScrollViewModel();

            foreach (SyncHelper item in syncHelpers)
            {
                if (scroll.Viewport == null && item.name.IsScrollViewport())
                {
                    scroll.Viewport = item.gameObject;
                }
                else if (scroll.Content == null && item.name.IsScrollContent())
                {
                    scroll.Content = item.gameObject;
                }
            }

            return scroll;
        }
    }

    internal struct ScrollViewModel
    {
        public GameObject Viewport { get; set; }
        public GameObject Content { get; set; }
    }
}