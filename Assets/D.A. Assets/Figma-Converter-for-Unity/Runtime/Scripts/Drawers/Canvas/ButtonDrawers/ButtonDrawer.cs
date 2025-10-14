using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

#if DABUTTON_EXISTS
using DA_Assets.DAB;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ButtonDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] List<FObject> buttons = new List<FObject>();
        public List<FObject> Buttons => buttons;

        public void Init()
        {
            buttons.Clear();
        }

        public void Draw(FObject fobject)
        {
            fobject.Data.ButtonComponent = monoBeh.Settings.ButtonSettings.ButtonComponent;

            switch (monoBeh.Settings.ButtonSettings.ButtonComponent)
            {
#if DABUTTON_EXISTS
                case ButtonComponent.DAButton:
                    {
                        fobject.Data.GameObject.TryAddComponent(out DAButton _);
                    }
                    break;
#endif
                default:
                    {
                        fobject.Data.GameObject.TryAddComponent(out UnityEngine.UI.Button _);
                    }
                    break;
            }

            buttons.Add(fobject);
        }

        public async Task SetTargetGraphics()
        {
            foreach (FObject fobject in buttons)
            {
                switch (fobject.Data.ButtonComponent)
                {
#if DABUTTON_EXISTS
                    case ButtonComponent.DAButton:
                        {
                            this.DAButtonDrawer.SetupDAButton(fobject.Data);
                        }
                        break;
#endif
                    default:
                        {
                            this.UnityButtonDrawer.SetupUnityButton(fobject.Data);
                        }
                        break;
                }

                await Task.Yield();
            }
        }

#if DABUTTON_EXISTS
        [SerializeField] DAButtonDrawer dabDrawer;
        [SerializeProperty(nameof(dabDrawer))]
        public DAButtonDrawer DAButtonDrawer => monoBeh.Link(ref dabDrawer);
#endif
        [SerializeField] UnityButtonDrawer ubDrawer;
        [SerializeProperty(nameof(ubDrawer))]
        public UnityButtonDrawer UnityButtonDrawer => monoBeh.Link(ref ubDrawer);
    }
}