using DA_Assets.CR;
using DA_Assets.DAG;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ImageDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, GameObject customGameObject = null)
        {
            GameObject target = customGameObject == null ? fobject.Data.GameObject : customGameObject;

            if (fobject.Data.GameObject.IsPartOfAnyPrefab() == false)
            {
                if (target.TryGetComponentSafe(out Graphic oldGraphic))
                {
                    Type curType = monoBeh.GetCurrentImageType();

                    if (oldGraphic.GetType().Equals(curType) == false)
                    {
                        oldGraphic.RemoveComponentsDependingOn();
                        oldGraphic.Destroy();
                    }
                }
            }

            Sprite sprite = monoBeh.SpriteProcessor.GetSprite(fobject);

            if (sprite == null)
            {
                if (fobject.IsSingleImageOrVideoOrEmojiType() || fobject.IsSprite())
                {
                    sprite = FcuConfig.MissingImageTexture128px;
                }
            }

            if (monoBeh.IsNova())
            {
#if NOVA_UI_EXISTS
                this.NovaImageDrawer.Draw(fobject, sprite, target);
#endif
            }
            else if (monoBeh.UsingUnityImage() || monoBeh.UsingRawImage() || fobject.IsObjectMask() || fobject.CanUseUnityImage(monoBeh))
            {
                this.UnityImageDrawer.Draw(fobject, sprite, target);
            }
            else if (monoBeh.UsingSvgImage())
            {
#if VECTOR_GRAPHICS_EXISTS
                this.SvgImageDrawer.Draw(fobject, sprite, target);
#endif
            }
            else if (monoBeh.UsingSpriteRenderer())
            {
                this.SpriteRendererDrawer.Draw(fobject, sprite, target);
            }
            else if (monoBeh.UsingShapes2D())
            {
#if SUBC_SHAPES_EXISTS
                this.Shapes2DDrawer.Draw(fobject, sprite, target);
#endif
            }
            else if (monoBeh.UsingJoshPui())
            {
#if JOSH_PUI_EXISTS
                this.JoshPuiDrawer.Draw(fobject, sprite, target);
#endif
            }
            else if (monoBeh.UsingDttPui())
            {
#if PROCEDURAL_UI_ASSET_STORE_RELEASE
                this.DttPuiDrawer.Draw(fobject, sprite, target);
#endif
            }
            else if (monoBeh.UsingMPUIKit())
            {
#if MPUIKIT_EXISTS
                this.MPUIKitDrawer.Draw(fobject, sprite, target);
#endif
            }

            if (fobject.ContainsTag(FcuTag.Slice9))
            {
                if (!monoBeh.IsNova())
                {
                    if (target.TryGetComponent(out Image img))
                    {
                        img.type = Image.Type.Sliced;
                    }
                }
            }
        }

        public void AddUnityOutline(FObject fobject)
        {
            FGraphic graphic = fobject.Data.Graphic;

            fobject.Data.GameObject.TryAddComponent(out UnityEngine.UI.Outline outline);
            outline.useGraphicAlpha = false;
            outline.effectDistance = new Vector2(fobject.StrokeWeight, -fobject.StrokeWeight);

            if (graphic.Stroke.HasSolid)
            {
                outline.effectColor = graphic.Stroke.SolidPaint.Color;
            }
            else if (graphic.Stroke.HasGradient)
            {
                outline.effectColor = graphic.Stroke.SingleColor;
            }
            else
            {
                outline.effectColor = default;
            }
        }

        public void AddGradient(FObject fobject, Paint gradientColor, bool strokeOnly = false)
        {
            GameObject gameObject = fobject.Data.GameObject;
            List<GradientColorKey> gradientColorKeys = gradientColor.ToGradientColorKeys();
            List<GradientAlphaKey> gradientAlphaKeys = gradientColor.ToGradientAlphaKeys();

            float angle;

            switch (gradientColor.Type)
            {
                case PaintType.GRADIENT_RADIAL:
                    angle = monoBeh.GraphicHelpers.ToRadialAngle(fobject, gradientColor.GradientHandlePositions);
                    break;
                default:
                    angle = monoBeh.GraphicHelpers.ToLinearAngle(fobject, gradientColor.GradientHandlePositions);
                    break;
            }

            if (monoBeh.UsingShapes2D() && !strokeOnly)
            {
                AddDAGradient();
            }
            else if (monoBeh.UsingMPUIKit())
            {
#if MPUIKIT_EXISTS
                if (gameObject.TryGetComponentSafe(out MPUIKIT.MPImage img))
                {
                    Gradient gradient = new Gradient
                    {
                        mode = GradientMode.Blend,
                    };

                    MPUIKIT.GradientEffect ge = new MPUIKIT.GradientEffect();
                    ge.Enabled = true;

                    switch (gradientColor.Type)
                    {
                        case PaintType.GRADIENT_RADIAL:
                            ge.GradientType = MPUIKIT.GradientType.Radial;
                            break;
                        default:
                            ge.GradientType = MPUIKIT.GradientType.Linear;
                            break;
                    }

                    ge.Gradient = gradient;
                    ge.Rotation = angle;
                    img.GradientEffect = ge;

                    gradient.colorKeys = gradientColorKeys.ToArray();
                    gradient.alphaKeys = gradientAlphaKeys.ToArray();
                }
                else
                {
                    AddDAGradient();
                }
#endif
            }
            else if (monoBeh.UsingDttPui())
            {
#if PROCEDURAL_UI_ASSET_STORE_RELEASE
                if (gameObject.TryGetComponentSafe(out DTT.UI.ProceduralUI.RoundedImage roundedImage))
                {
                    gameObject.TryAddComponent(out DTT.UI.ProceduralUI.GradientEffect gradient);

                    Gradient newGradient = new Gradient();
                    newGradient.colorKeys = gradientColorKeys.ToArray();
                    newGradient.alphaKeys = gradientAlphaKeys.ToArray();

                    Type objectType = gradient.GetType();
                    System.Reflection.FieldInfo fieldInfo = objectType.GetField("_gradient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        fieldInfo.SetValue(gradient, newGradient);
                    }

                    System.Reflection.FieldInfo fieldInfo1 = objectType.GetField("_type", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (fieldInfo1 != null)
                    {
                        DTT.UI.ProceduralUI.GradientEffect.GradientType gt;

                        /// At the time of writing this code in the <see cref="DTT.UI.ProceduralUI"/> asset, 
                        /// the enums <see cref="DTT.UI.ProceduralUI.GradientEffect.GradientType.RADIAL"/> 
                        /// and <see cref="DTT.UI.ProceduralUI.GradientEffect.GradientType.ANGULAR"/> were swapped.
                        switch (gradientColor.Type)
                        {
                            case PaintType.GRADIENT_RADIAL:
                                gt = DTT.UI.ProceduralUI.GradientEffect.GradientType.ANGULAR;
                                break;
                            case PaintType.GRADIENT_ANGULAR:
                                gt = DTT.UI.ProceduralUI.GradientEffect.GradientType.RADIAL;
                                break;
                            default:
                                gt = DTT.UI.ProceduralUI.GradientEffect.GradientType.LINEAR;
                                break;
                        }

                        fieldInfo1.SetValue(gradient, gt);
                    }

                    System.Reflection.FieldInfo fieldInfo3 = objectType.GetField("_rotation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (fieldInfo3 != null)
                    {
                        fieldInfo3.SetValue(gradient, angle);
                    }
                }
                else
                {
                    AddDAGradient();
                }
#endif
            }
            else
            {
                AddDAGradient();
            }

            void AddDAGradient()
            {
                gameObject.TryAddComponent(out DAGradient gradient);

                gradient.Angle = angle;
                gradient.BlendMode = DAColorBlendMode.Multiply;

                gradient.Gradient.colorKeys = gradientColorKeys.ToArray();
                gradient.Gradient.alphaKeys = gradientAlphaKeys.ToArray();
            }
        }

        public bool TryAddCornerRounder(FObject fobject, GameObject target)
        {
            if (fobject.IsSprite())
            {
                return false;
            }

            if (fobject.ContainsRoundedCorners())
            {
                target.TryAddComponent(out CornerRounder cornerRounder);
                Vector4 cr = monoBeh.GraphicHelpers.GetCornerRadius(fobject);
                cornerRounder.SetRadii(cr);
            }

            return false;
        }

        public void SetProceduralColor(FObject fobject, Image img, Action setStrokeOnlyWidth, Action setStroke)
        {
            FGraphic graphic = fobject.Data.Graphic;

            FcuLogger.Debug($"SetUnityImageColor | {fobject.Data.NameHierarchy} | {fobject.Data.FcuImageType} | hasFills: {graphic.HasFill} | hasStroke: {graphic.HasStroke}", FcuDebugSettingsFlags.LogComponentDrawer);

            bool strokeOnly = graphic.HasStroke && !graphic.HasFill;

            if (strokeOnly)
            {
                setStrokeOnlyWidth();

                if (graphic.Stroke.HasSolid)
                {
                    img.color = graphic.Stroke.SolidPaint.Color;
                }
                else if (graphic.Stroke.HasGradient)
                {
                    img.color = Color.white;
                    monoBeh.CanvasDrawer.ImageDrawer.AddGradient(fobject, graphic.Stroke.GradientPaint);
                }
            }
            else
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

                if (graphic.HasStroke)
                {
                    setStroke();
                }
            }

            if (!graphic.HasStroke)
            {
                fobject.Data.GameObject.TryDestroyComponent<UnityEngine.UI.Outline>();
            }
        }

        [SerializeField] UnityImageDrawer unityImageDrawer;
        [SerializeProperty(nameof(unityImageDrawer))]
        public UnityImageDrawer UnityImageDrawer => monoBeh.Link(ref unityImageDrawer);

        [SerializeField] SpriteRendererDrawer spriteRendererDrawer;
        [SerializeProperty(nameof(spriteRendererDrawer))]
        public SpriteRendererDrawer SpriteRendererDrawer => monoBeh.Link(ref spriteRendererDrawer);

#if SUBC_SHAPES_EXISTS
        [SerializeField] Shapes2DDrawer shapes2DDrawer;
        [SerializeProperty(nameof(shapes2DDrawer))]
        public Shapes2DDrawer Shapes2DDrawer => monoBeh.Link(ref shapes2DDrawer);
#endif

#if JOSH_PUI_EXISTS
        [SerializeField] JoshPuiDrawer joshPuiDrawer;
        [SerializeProperty(nameof(joshPuiDrawer))]
        public JoshPuiDrawer JoshPuiDrawer => monoBeh.Link(ref joshPuiDrawer);
#endif

#if PROCEDURAL_UI_ASSET_STORE_RELEASE
        [SerializeField] DttPuiDrawer dttPuiDrawer;
        [SerializeProperty(nameof(dttPuiDrawer))]
        public DttPuiDrawer DttPuiDrawer => monoBeh.Link(ref dttPuiDrawer);
#endif

#if MPUIKIT_EXISTS
        [SerializeField] MPUIKitDrawer mpuikitDrawer;
        [SerializeProperty(nameof(mpuikitDrawer))]
        public MPUIKitDrawer MPUIKitDrawer => monoBeh.Link(ref mpuikitDrawer);
#endif

#if VECTOR_GRAPHICS_EXISTS
        [SerializeField] SvgImageDrawer svgImageDrawer;
        [SerializeProperty(nameof(svgImageDrawer))]
        public SvgImageDrawer SvgImageDrawer => monoBeh.Link(ref svgImageDrawer);
#endif

#if NOVA_UI_EXISTS
        [SerializeField] NovaImageDrawer novaImageDrawer;
        [SerializeProperty(nameof(novaImageDrawer))]
        public NovaImageDrawer NovaImageDrawer => monoBeh.Link(ref novaImageDrawer);
#endif
    }
}