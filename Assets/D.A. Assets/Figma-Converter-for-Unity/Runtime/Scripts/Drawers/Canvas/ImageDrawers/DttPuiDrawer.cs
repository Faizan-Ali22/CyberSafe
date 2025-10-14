#if PROCEDURAL_UI_ASSET_STORE_RELEASE
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DTT.UI.ProceduralUI;
using System;
using System.Reflection;
using UnityEngine;
using DA_Assets.Logging;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class DttPuiDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            target.TryAddGraphic(out RoundedImage img);

            img.sprite = sprite;
            img.type = monoBeh.Settings.DttPuiSettings.Type;
            img.raycastTarget = monoBeh.Settings.DttPuiSettings.RaycastTarget;
            img.preserveAspect = monoBeh.Settings.DttPuiSettings.PreserveAspect;
            img.DistanceFalloff = monoBeh.Settings.DttPuiSettings.FalloffDistance;
#if UNITY_2020_1_OR_NEWER
            img.raycastPadding = monoBeh.Settings.DttPuiSettings.RaycastPadding;
#endif

            SetCorners(fobject, img);
            SetColor(fobject, img);
        }

        public void SetColor(FObject fobject, RoundedImage img)
        {
            FGraphic graphic = fobject.Data.Graphic;

            FcuLogger.Debug($"SetUnityImageColor | {fobject.Data.NameHierarchy} | {fobject.Data.FcuImageType} | hasFills: {graphic.HasFill} | hasStroke: {graphic.HasStroke}", FcuDebugSettingsFlags.LogComponentDrawer);

            SetBorderThickness(0, img);

            if (fobject.IsDrawableType())
            {
                monoBeh.CanvasDrawer.ImageDrawer.SetProceduralColor(fobject, img,
                setStrokeOnlyWidth: () =>
                {
                    SetBorderThickness(fobject.StrokeWeight, img);
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

        private void SetCorners(FObject fobject, RoundedImage img)
        {
            img.RoundingUnit = RoundingUnit.WORLD;

            Type objectType = img.GetType();
            FieldInfo fieldInfo = objectType.GetField("_cornerMode", BindingFlags.NonPublic | BindingFlags.Instance);

            Vector4 radius = monoBeh.GraphicHelpers.GetCornerRadius(fobject);

            if (!radius.IsDefault())
            {
                if (fieldInfo != null)
                {
#if UNITY_EDITOR
                    fieldInfo.SetValue(img, (int)RoundingCornerMode.INDIVIDUAL);
#endif
                }

                SetByIndex(0, img, objectType, "_roundingAmount", radius[0]);
                SetByIndex(1, img, objectType, "_roundingAmount", radius[1]);
                SetByIndex(2, img, objectType, "_roundingAmount", radius[2]);
                SetByIndex(3, img, objectType, "_roundingAmount", radius[3]);
            }
        }

        public void SetByIndex(int index, object classInstance, Type type, string arrayFieldName, float value)
        {
            FieldInfo fieldInfo = type.GetField(arrayFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                DALogger.LogError($"Field '{arrayFieldName}' not found in type '{type}'.");
                return;
            }

            float[] array = fieldInfo.GetValue(classInstance) as float[];
            if (array == null)
            {
                DALogger.LogError($"Field '{arrayFieldName}' is not an array or is null.");
                return;
            }

            if (index < 0 || index >= array.Length)
            {
                DALogger.LogError($"Index '{index}' is out of bounds for array '{arrayFieldName}'.");
                return;
            }

            array[index] = value;

            fieldInfo.SetValue(classInstance, array);

            FcuLogger.Debug($"Value {value} was set at index {index} of array '{arrayFieldName}'.");
        }

        public void SetBorderThickness(float thickness, RoundedImage img)
        {
            if (thickness > 0)
            {
#if UNITY_EDITOR
                img.Mode = RoundingMode.BORDER;
#endif
            }
            else
            {
#if UNITY_EDITOR
                img.Mode = RoundingMode.FILL;
#endif
            }

            Type objectType = img.GetType();
            FieldInfo fieldInfo = objectType.GetField("_borderThickness", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(img, (float)thickness);
        }
    }
}
#endif