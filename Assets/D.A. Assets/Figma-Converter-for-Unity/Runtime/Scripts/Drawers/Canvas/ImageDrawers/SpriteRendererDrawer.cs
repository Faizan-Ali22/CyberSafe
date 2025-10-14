using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.UI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class SpriteRendererDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            target.TryAddComponent(out SpriteRenderer sr);
            sr.sprite = sprite;
            sr.sortingOrder = target.transform.GetSiblingIndex();

            if (sprite == null)
            {
                sr.sprite = FcuConfig.WhiteSprite32px;
                sr.drawMode = SpriteDrawMode.Tiled;
                Vector2 size = target.GetComponent<RectTransform>().rect.size;
                sr.size = size;
                SetColor(fobject, sr);
            }
            else
            {
                sr.drawMode = SpriteDrawMode.Simple;

                if (fobject.Data.FcuImageType == FcuImageType.Generative || fobject.Data.Graphic.SpriteSingleColor.IsDefault() == false)
                {
                    SetColor(fobject, sr);
                }
                else
                {
                    sr.color = Color.white;
                }
            }

            sr.flipX = monoBeh.Settings.SpriteRendererSettings.FlipX;
            sr.flipY = monoBeh.Settings.SpriteRendererSettings.FlipY;
            sr.maskInteraction = monoBeh.Settings.SpriteRendererSettings.MaskInteraction;
            sr.spriteSortPoint = monoBeh.Settings.SpriteRendererSettings.SortPoint;
            sr.sortingLayerName = monoBeh.Settings.SpriteRendererSettings.SortingLayer;
        }

        public void SetColor(FObject fobject, SpriteRenderer img)
        {
            FGraphic graphic = fobject.Data.Graphic;

            FcuLogger.Debug($"SetUnityImageColor | {fobject.Data.NameHierarchy} | {fobject.Data.FcuImageType} | graphic.HasFills: {graphic.HasFill} | graphic.HasStrokes: {graphic.HasStroke}", FcuDebugSettingsFlags.LogComponentDrawer);

            void AddOutline(float strokeWeight, float cornerRadius, Color color)
            {
                img.gameObject.TryAddComponent(out SpriteOutline uiOutline);
                uiOutline.OutlineWidth = strokeWeight;
                uiOutline.CornerSegments = 10;
                uiOutline.CornerRadius = cornerRadius;
                uiOutline.FillCenter = false;
                uiOutline.color = color;
            }

            if (fobject.IsDrawableType())
            {
                Vector4 radius = monoBeh.GraphicHelpers.GetCornerRadius(fobject);

                bool strokeOnly = graphic.HasStroke && !graphic.HasFill;

                if (strokeOnly)
                {
                    Color tr = Color.white;
                    tr.a = 0;
                    img.color = tr;

                    AddOutline(fobject.StrokeWeight, radius.x, graphic.Stroke.SingleColor);
                }
                else
                {
                    img.color = graphic.Fill.SingleColor;

                    if (graphic.HasStroke)
                    {
                        AddOutline(fobject.StrokeWeight, radius.x, graphic.Stroke.SingleColor);
                    }
                }

                if (!graphic.HasStroke)
                {
                    fobject.Data.GameObject.TryDestroyComponent<SpriteOutline>();
                }
            }
            else if (fobject.IsGenerativeType())
            {
                if (graphic.HasFill)
                {
                    img.color = graphic.Fill.SingleColor;
                }
                else if (graphic.HasStroke)
                {
                    img.color = graphic.Stroke.SingleColor;
                }
            }
            else if (fobject.IsDownloadableType())
            {
                if (fobject.Data.Graphic.SpriteSingleColor.IsDefault() == false)
                {
                    img.color = fobject.Data.Graphic.SpriteSingleColor;
                }
                else
                {
                    img.color = Color.white;
                }
            }
        }
    }
}