#if NOVA_UI_EXISTS
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using Nova;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable IDE0003
#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers
{
    [Serializable]
    public class NovaDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task DrawToScene(List<FObject> fobjects)
        {
            monoBeh.AssetTools.SelectFcu();

            monoBeh.CanvasDrawer.TextDrawer.Init();
            await monoBeh.CanvasDrawer.DrawComponents(fobjects, DrawByTag);
        }

        private async Task DrawByTag(FObject fobject, FcuTag tag, Action onDraw)
        {
            try
            {
                if (fobject.Data.GameObject == null)
                {
                    return;
                }

                switch (tag)
                {
                    case FcuTag.Blur:
                        this.NovaBlurDrawer.Draw(fobject);
                        break;

                    case FcuTag.Shadow:
                        this.NovaShadowDrawer.Draw(fobject);
                        break;

                    case FcuTag.AutoLayoutGroup:
                        this.NovaAutoLayoutDrawer.Draw(fobject);
                        break;

                    case FcuTag.ContentSizeFitter:

                        break;

                    case FcuTag.AspectRatioFitter:

                        break;

                    case FcuTag.InputField:

                        break;

                    case FcuTag.Button:
                        this.NovaButtonDrawer.Draw(fobject);
                        break;

                    case FcuTag.Mask:
                        monoBeh.CanvasDrawer.MaskDrawer.Draw(fobject);
                        break;

                    case FcuTag.CanvasGroup:

                        break;

                    case FcuTag.Placeholder:
                    case FcuTag.Text:
                        monoBeh.CanvasDrawer.TextDrawer.Draw(fobject);
                        monoBeh.CanvasDrawer.LocalizationDrawer.Draw(fobject);
                        break;

                    case FcuTag.Image:
                        monoBeh.CanvasDrawer.ImageDrawer.Draw(fobject);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            onDraw.Invoke();
            await Task.Yield();
        }

        internal void SetupSpace()
        {
#if false
            if (monoBeh.TryGetComponentSafe(out Canvas canvas))
            {
                canvas.RemoveComponentsDependingOn();
                canvas.Destroy();
            }

            if (monoBeh.TagSetter.TagsCounter.TryGetValue(FcuTag.Blur, out int blurCount))
            {
                if (blurCount > 0)
                {
                    Camera mc = CameraTools.GetOrCreateMainCamera();
                    Camera bgbc = CameraTools.GetOrCreateBackgroundBlurCamera();

                    monoBeh.gameObject.TryAddComponent(out BackgroundBlurGroup blurGroup);
                    blurGroup.PropertyMatchCamera = mc;
                    blurGroup.BackgroundCamera = bgbc;

                    blurGroup.BlurEffects = blurGroup.BlurEffects.Where(x => x != null).ToList();

                    monoBeh.gameObject.TryAddComponent(out ScreenSpace screenSpace);
                    screenSpace.TargetCamera = mc;
                    screenSpace.enabled = false;
                    screenSpace.AddAdditionalCamera(bgbc);
                }
            }
#endif
        }

        internal void EnableScreenSpaceComponent()
        {
            if (monoBeh.gameObject.TryGetComponentSafe(out ScreenSpace screenSpace))
            {
                screenSpace.enabled = true;
            }
        }

        [SerializeField] NovaButtonDrawer novaButtonDrawer;
        [SerializeProperty(nameof(novaButtonDrawer))]
        public NovaButtonDrawer NovaButtonDrawer => monoBeh.Link(ref novaButtonDrawer);

        [SerializeField] NovaShadowDrawer novaShadowDrawer;
        [SerializeProperty(nameof(novaShadowDrawer))]
        public NovaShadowDrawer NovaShadowDrawer => monoBeh.Link(ref novaShadowDrawer);

        [SerializeField] NovaBlurDrawer novaBlurDrawer;
        [SerializeProperty(nameof(novaBlurDrawer))]
        public NovaBlurDrawer NovaBlurDrawer => monoBeh.Link(ref novaBlurDrawer);

        [SerializeField] NovaAutoLayoutDrawer novaAutoLayoutDrawer;
        [SerializeProperty(nameof(novaAutoLayoutDrawer))]
        public NovaAutoLayoutDrawer NovaAutoLayoutDrawer => monoBeh.Link(ref novaAutoLayoutDrawer);
    }
}
#endif