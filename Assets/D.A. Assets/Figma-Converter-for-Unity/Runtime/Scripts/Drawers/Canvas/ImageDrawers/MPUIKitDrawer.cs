#if MPUIKIT_EXISTS
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using MPUIKIT;
using System;
using System.Reflection;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class MPUIKitDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            target.TryAddGraphic(out MPImage img);
            SetCorners(fobject, img);

            SetColor(fobject, img);

            img.sprite = sprite;
            img.type = monoBeh.Settings.MPUIKitSettings.Type;
            img.raycastTarget = monoBeh.Settings.MPUIKitSettings.RaycastTarget;
            img.preserveAspect = monoBeh.Settings.MPUIKitSettings.PreserveAspect;
            img.FalloffDistance = monoBeh.Settings.MPUIKitSettings.FalloffDistance;

#if UNITY_2020_1_OR_NEWER
            img.raycastPadding = monoBeh.Settings.MPUIKitSettings.RaycastPadding;
#endif

            MethodInfo initMethod = typeof(MPImage).GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            initMethod.Invoke(img, null);
        }

        public void SetColor(FObject fobject, MPImage img)
        {
            FGraphic graphic = fobject.Data.Graphic;

            FcuLogger.Debug($"SetUnityImageColor | {fobject.Data.Hierarchy} | {fobject.Data.FcuImageType} | hasFills: {graphic.HasFill} | hasStroke: {graphic.HasStroke}", FcuDebugSettingsFlags.LogComponentDrawer);

            img.GradientEffect = new GradientEffect
            {
                Enabled = false,
                GradientType = MPUIKIT.GradientType.Linear,
                Gradient = null
            };

            if (fobject.IsDrawableType())
            {
                monoBeh.CanvasDrawer.ImageDrawer.SetProceduralColor(fobject, img,
                setStrokeOnlyWidth: () =>
                {
                    img.StrokeWidth = fobject.StrokeWeight;
                },
                setStroke: () =>
                {
                    switch (graphic.Stroke.Align)
                    {
                        case StrokeAlign.INSIDE:
                            {
                                img.OutlineColor = graphic.Stroke.SingleColor;
                                img.OutlineWidth = fobject.StrokeWeight;
                                fobject.Data.GameObject.TryDestroyComponent<UnityEngine.UI.Outline>();
                            }
                            break;
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

        private void SetCorners(FObject fobject, MPImage img)
        {
            if (fobject.Type == NodeType.ELLIPSE)
            {
                img.DrawShape = DrawShape.Circle;
                img.Circle = new Circle
                {
                    FitToRect = true
                };
            }
            else
            {
                img.DrawShape = DrawShape.Rectangle;

                img.Rectangle = new Rectangle
                {
                    CornerRadius = monoBeh.GraphicHelpers.GetCornerRadius(fobject)
                };
            }
        }
    }
}
#endif