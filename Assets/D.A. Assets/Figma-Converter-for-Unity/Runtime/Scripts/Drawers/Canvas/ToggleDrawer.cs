using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ToggleDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private List<FObject> toggles;
        public List<FObject> Toggles => toggles;

        public void Init()
        {
            toggles = new List<FObject>();
        }

        public void Draw(FObject fobject)
        {
            if (fobject.ContainsTag(FcuTag.Toggle))
            {
                fobject.Data.GameObject.TryAddComponent(out Toggle toggle);
                toggles.Add(fobject);
            }
            else if (fobject.ContainsTag(FcuTag.ToggleGroup))
            {
                fobject.Data.GameObject.TryAddComponent(out ToggleGroup toggleGroup);
            }
        }

        public async Task SetTargetGraphics()
        {
            foreach (FObject fobject in toggles)
            {
                if (!fobject.Data.GameObject.TryGetComponentSafe(out Toggle toggle))
                    continue;

                ToggleModel tm = GetGraphics(fobject.Data);

                if (tm.Checkmark.TryGetComponentSafe(out Graphic checkmark))
                {
                    toggle.graphic = checkmark;
                }

                if (tm.Background.TryGetComponentSafe(out Graphic bg))
                {
                    toggle.targetGraphic = bg;
                }

                if (tm.ToggleGroup.TryGetComponentSafe(out ToggleGroup group))
                {
                    toggle.group = group;
                }

                toggle.enabled = false;
                await Task.Delay(10);
                toggle.enabled = true;
            }

            toggles.Clear();
        }

        private ToggleModel GetGraphics(SyncData syncData)
        {
            SyncHelper[] childs = syncData.GameObject.GetChilds<SyncHelper>();
            List<Transform> parents = syncData.GameObject.transform.GetAllParents();

            ToggleModel toggle = new ToggleModel();

            foreach (SyncHelper child in childs)
            {
                if (toggle.Background == null && child.ContainsTag(FcuTag.Background))
                {
                    toggle.Background = child.gameObject;
                    break;
                }
            }

            foreach (Transform item in parents)
            {
                if (!item.TryGetComponentSafe(out SyncHelper parentSyncHelper))
                    continue;

                if (parentSyncHelper.ContainsTag(FcuTag.ToggleGroup))
                {
                    toggle.ToggleGroup = item.gameObject;
                    break;
                }
            }

            foreach (SyncHelper item in childs)
            {
                if (toggle.Checkmark == null && item.name.IsCheckmark())
                {
                    toggle.Checkmark = item.gameObject;    
                }
                else if (toggle.Background == null && item.ContainsTag(FcuTag.Image))
                {
                    toggle.Background = item.gameObject;
                }
            }

            return toggle;
        }
    }

    internal struct ToggleModel
    {
        public GameObject Checkmark { get; set; }
        public GameObject Background { get; set; }
        public GameObject ToggleGroup { get; set; }
    }
}