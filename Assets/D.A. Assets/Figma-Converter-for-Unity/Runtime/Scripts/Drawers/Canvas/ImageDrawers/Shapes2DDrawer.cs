#if SUBC_SHAPES_EXISTS
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using Shapes2D;
using System;
using UnityEngine;
using UnityEngine.UI;
using Shape = Shapes2D.Shape;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class Shapes2DDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            if (sprite != null)
            {
                if (target.TryGetComponentSafe(out Shape oldShape))
                {
                    oldShape.Destroy();
                }
            }

            target.TryAddGraphic(out Image img);
            img.sprite = sprite;
            img.material = null;
            img.type = monoBeh.Settings.Shapes2DSettings.Type;
            img.raycastTarget = monoBeh.Settings.Shapes2DSettings.RaycastTarget;
            img.preserveAspect = monoBeh.Settings.Shapes2DSettings.PreserveAspect;
            img.raycastTarget = monoBeh.Settings.Shapes2DSettings.RaycastTarget;
#if UNITY_2020_1_OR_NEWER
            img.raycastPadding = monoBeh.Settings.Shapes2DSettings.RaycastPadding;
#endif

            if (sprite == null)
            {
                target.TryAddComponent(out Shape shape);

                SetColor(fobject, shape);
                SetCorners(fobject, shape);
                SetBlur(fobject, shape);
            }
            else
            {
                monoBeh.CanvasDrawer.ImageDrawer.UnityImageDrawer.SetColor(fobject, img);
            }
        }

        private void SetColor(FObject fobject, Shape shape)
        {
            FGraphic graphic = fobject.Data.Graphic;

            FcuLogger.Debug($"SetUnityImageColor | {fobject.Data.Hierarchy} | {fobject.Data.FcuImageType} | hasFills: {graphic.HasFill} | hasStroke: {graphic.HasStroke}", FcuDebugSettingsFlags.LogComponentDrawer);

            shape.settings.fillType = FillType.SolidColor;

            if (graphic.Fill.HasGradient)
            {
                shape.settings.blur = 0.1f;
            }

            if (fobject.IsDrawableType())
            {
                bool strokeOnly = graphic.HasStroke && !graphic.HasFill;

                if (strokeOnly)
                {
                    Color tr = Color.white;
                    tr.a = 0;
                    shape.settings.fillColor = tr;

                    if (graphic.Stroke.HasSolid)
                    {
                        shape.settings.outlineColor = graphic.Stroke.SolidPaint.Color;
                    }
                    else if (graphic.Stroke.HasGradient)
                    {
                        shape.settings.outlineColor = Color.white;
                        monoBeh.CanvasDrawer.ImageDrawer.AddGradient(fobject, graphic.Stroke.GradientPaint, strokeOnly: true);
                    }

                    shape.settings.outlineSize = fobject.StrokeWeight;
                }
                else
                {
                    if (graphic.Fill.HasSolid)
                    {
                        shape.settings.fillColor = graphic.Fill.SolidPaint.Color;
                    }
                    else if (graphic.Fill.HasGradient)
                    {
                        shape.settings.fillColor = Color.white;
                        monoBeh.CanvasDrawer.ImageDrawer.AddGradient(fobject, graphic.Fill.GradientPaint);
                    }

                    switch (graphic.Stroke.Align)
                    {
                        case StrokeAlign.INSIDE:
                            {
                                shape.settings.outlineColor = graphic.Stroke.SingleColor;
                                shape.settings.outlineSize = fobject.StrokeWeight;
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
                                shape.settings.outlineSize = 0;
                                fobject.Data.GameObject.TryDestroyComponent<UnityEngine.UI.Outline>();
                                break;
                            }
                    }
                }

                if (!graphic.HasStroke)
                {
                    shape.settings.outlineSize = 0;
                    fobject.Data.GameObject.TryDestroyComponent<UnityEngine.UI.Outline>();
                }
            }
        }

        private void SetCorners(FObject fobject, Shape shape)
        {
            if (fobject.Type == NodeType.ELLIPSE)
            {
                shape.settings.shapeType = ShapeType.Ellipse;
            }
            else
            {
                shape.settings.shapeType = ShapeType.Rectangle;

                if (fobject.CornerRadiuses != null)
                {
                    Vector4 cr = monoBeh.GraphicHelpers.GetCornerRadius(fobject);

                    shape.settings.roundnessPerCorner = true;

                    shape.settings.roundnessBottomLeft = cr.x;
                    shape.settings.roundnessBottomRight = cr.y;
                    shape.settings.roundnessTopRight = cr.z;
                    shape.settings.roundnessTopLeft = cr.w;
                }
                else if (fobject.CornerRadius.ToFloat() != 0)
                {
                    shape.settings.roundnessPerCorner = true;

                    shape.settings.roundnessBottomLeft = fobject.CornerRadius.ToFloat();
                    shape.settings.roundnessBottomRight = fobject.CornerRadius.ToFloat();
                    shape.settings.roundnessTopRight = fobject.CornerRadius.ToFloat();
                    shape.settings.roundnessTopLeft = fobject.CornerRadius.ToFloat();

                    //For new Shape versions only.
                    //shape.settings.roundnessPerCorner = false;
                    //shape.settings.roundness = source.CornerRadius;
                }
            }
        }

        private void SetBlur(FObject fobject, Shape shape)
        {
            foreach (Effect effect in fobject.Effects)
            {
                if (!effect.IsVisible())
                    continue;

                if (effect.Type != EffectType.LAYER_BLUR)
                    continue;

                shape.settings.blur = effect.Radius;
                break;
            }
        }
    }
}
#endif