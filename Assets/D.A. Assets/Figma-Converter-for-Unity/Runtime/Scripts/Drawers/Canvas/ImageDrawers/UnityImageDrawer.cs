using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class UnityImageDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            MaskableGraphic graphic;

            if (monoBeh.UsingRawImage())
            {
                target.TryAddGraphic(out RawImage img);
                graphic = img;

                if (sprite != null)
                {
                    img.texture = sprite.texture;
                }
            }
            else
            {
                target.TryAddGraphic(out Image img);
                graphic = img;

                img.sprite = sprite;
                img.type = monoBeh.Settings.UnityImageSettings.Type;
                img.preserveAspect = monoBeh.Settings.UnityImageSettings.PreserveAspect;
            }

            graphic.raycastTarget = monoBeh.Settings.UnityImageSettings.RaycastTarget;
            graphic.maskable = monoBeh.Settings.UnityImageSettings.Maskable;
#if UNITY_2020_1_OR_NEWER
            graphic.raycastPadding = monoBeh.Settings.UnityImageSettings.RaycastPadding;
#endif

            if (monoBeh.UseImageLinearMaterial())
            {
                graphic.material = FcuConfig.ImageLinearMaterial;
            }
            else
            {
                graphic.material = null;
            }

            SetColor(fobject, graphic);
            monoBeh.CanvasDrawer.ImageDrawer.TryAddCornerRounder(fobject, target);
        }

        public void SetColor(FObject fobject, MaskableGraphic img)
        {
            FGraphic graphic = fobject.Data.Graphic;

            FcuLogger.Debug($"SetUnityImageColor | {fobject.Data.NameHierarchy} | {fobject.Data.FcuImageType} | graphic.HasFills: {graphic.HasFill} | graphic.HasStrokes: {graphic.HasStroke}", FcuDebugSettingsFlags.LogComponentDrawer);

            if (fobject.IsDrawableType())
            {
                bool strokeOnly = graphic.HasStroke && !graphic.HasFill;

                if (strokeOnly)
                {
                    img.color = default;
                }
                else if (graphic.Fill.HasSolid)
                {
                    img.color = graphic.Fill.SolidPaint.Color;
                }
                else if (graphic.HasStroke && graphic.Fill.HasGradient)
                {
                    img.color = graphic.Fill.SingleColor;
                }
                else if (graphic.Fill.HasGradient)
                {
                    img.color = Color.white;
                    monoBeh.CanvasDrawer.ImageDrawer.AddGradient(fobject, graphic.Fill.GradientPaint);
                }

                if (graphic.HasFill)
                {
                    if (graphic.HasStroke)
                    {
                        monoBeh.CanvasDrawer.ImageDrawer.AddUnityOutline(fobject);
                    }
                }

                if (!graphic.HasStroke)
                {
                    fobject.Data.GameObject.TryDestroyComponent<UnityEngine.UI.Outline>();
                }
            }
            else if (fobject.IsGenerativeType())
            {
                if (graphic.HasFill && graphic.HasStroke)//no need colorize
                {
                    monoBeh.CanvasDrawer.ImageDrawer.AddUnityOutline(fobject);
                }
                else if (graphic.HasFill)
                {
                    if (graphic.Fill.HasSolid)
                    {
                        img.color = graphic.Fill.SolidPaint.Color;
                    }
                    else if (graphic.Fill.HasGradient)
                    {
                        img.color = Color.white;
                        monoBeh.CanvasDrawer.ImageDrawer.AddGradient(fobject, graphic.Fill.GradientPaint);
                    }
                }
                else if (graphic.HasStroke)
                {
                    if (graphic.Stroke.HasSolid)
                    {
                        img.color = graphic.Stroke.SolidPaint.Color;
                    }
                    else
                    {
                        img.color = Color.white;
                        monoBeh.CanvasDrawer.ImageDrawer.AddGradient(fobject, graphic.Stroke.GradientPaint);
                    }
                }
            }
            else if (fobject.IsDownloadableType())
            {
                if (!fobject.Data.Graphic.SpriteSingleColor.IsDefault())
                {
                    img.color = fobject.Data.Graphic.SpriteSingleColor;
                }
                else if (fobject.Data.Graphic.HasSingleGradient && !monoBeh.Settings.ImageSpritesSettings.DownloadOptions.HasFlag(SpriteDownloadOptions.SupportedGradients))
                { 
                    monoBeh.CanvasDrawer.ImageDrawer.AddGradient(fobject, fobject.Data.Graphic.SpriteSingleLinearGradient);
                }
                else
                {
                    img.color = Color.white;
                }
            }
        }
    }
}