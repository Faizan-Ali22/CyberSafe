#if JOSH_PUI_EXISTS
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class JoshPuiDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            target.TryAddGraphic(out ProceduralImage img);

            img.sprite = sprite;
            img.type = monoBeh.Settings.JoshPuiSettings.Type;
            img.raycastTarget = monoBeh.Settings.JoshPuiSettings.RaycastTarget;
            img.preserveAspect = monoBeh.Settings.JoshPuiSettings.PreserveAspect;
            img.FalloffDistance = monoBeh.Settings.JoshPuiSettings.FalloffDistance;
#if UNITY_2020_1_OR_NEWER
            img.raycastPadding = monoBeh.Settings.JoshPuiSettings.RaycastPadding;
#endif
            if (fobject.Type == NodeType.ELLIPSE)
            {
                target.TryAddComponent(out RoundModifier roundModifier);
            }
            else
            {
                if (fobject.CornerRadiuses != null)
                {
                    target.TryAddComponent(out FreeModifier freeModifier);
                    freeModifier.Radius = monoBeh.GraphicHelpers.GetCornerRadius(fobject);
                }
                else
                {
                    target.TryAddComponent(out UniformModifier uniformModifier);
                    uniformModifier.Radius = fobject.CornerRadius.ToFloat();
                }
            }

            SetColor(fobject, img);
        }

        public void SetColor(FObject fobject, ProceduralImage img)
        {
            FGraphic graphic = fobject.Data.Graphic;

            FcuLogger.Debug($"SetUnityImageColor | {fobject.Data.NameHierarchy} | {fobject.Data.FcuImageType} | hasFills: {graphic.HasFill} | hasStroke: {graphic.HasStroke}", FcuDebugSettingsFlags.LogComponentDrawer);

            if (fobject.IsDrawableType())
            {
                monoBeh.CanvasDrawer.ImageDrawer.SetProceduralColor(fobject, img,
                setStrokeOnlyWidth: () =>
                {
                    img.BorderWidth = fobject.StrokeWeight;
                },
                setStroke: () =>
                {
                    switch (graphic.Stroke.Align)
                    {
                        case StrokeAlign.OUTSIDE:
                            {
                                monoBeh.CanvasDrawer.ImageDrawer.AddUnityOutline(fobject);
                            }
                            break;
                        default:
                            {
                                fobject.Data.GameObject.TryDestroyComponent<UnityEngine.UI.Outline>();
                                break;
                            }
                    }
                });
            }
            else
            {
                monoBeh.CanvasDrawer.ImageDrawer.UnityImageDrawer.SetColor(fobject, img);
            }
        }
    }
}
#endif