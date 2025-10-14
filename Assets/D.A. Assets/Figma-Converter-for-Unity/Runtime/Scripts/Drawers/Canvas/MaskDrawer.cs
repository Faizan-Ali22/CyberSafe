using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class MaskDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            GameObject targetGo;

            if (fobject.IsObjectMask() && !fobject.ContainsTag(FcuTag.Frame))
            {
                monoBeh.CurrentProject.TryGetByIndex(fobject.Data.ParentIndex, out FObject target);
                targetGo = target.Data.GameObject;
            }
            else
            {
                targetGo = fobject.Data.GameObject;
            }

            int reason = 0;

            if (fobject.IsObjectMask())
            {
                if (monoBeh.IsNova())
                {
#if NOVA_UI_EXISTS
                    reason = 1;
                    targetGo.TryAddComponent(out Nova.ClipMask unityMask);
                    Sprite sprite = monoBeh.SpriteProcessor.GetSprite(fobject);
                    unityMask.Mask = sprite.texture;
#endif
                }
                else if (!monoBeh.UsingSpriteRenderer())
                {
                    reason = 2;
                    monoBeh.CanvasDrawer.ImageDrawer.Draw(fobject, targetGo);
                    targetGo.TryAddComponent(out Mask unityMask);
                    unityMask.showMaskGraphic = false;
                }
                else
                {
                    reason = 3;
                }

                fobject.Data.GameObject.Destroy();
            }
            else if (fobject.IsFrameMask() || fobject.IsClipMask())
            {
                if (monoBeh.IsNova())
                {
#if NOVA_UI_EXISTS
                    reason = 4;
                    targetGo.TryAddComponent(out Nova.ClipMask unityMask);
#endif
                }
                else if (monoBeh.UseImageLinearMaterial() || fobject.Data.FRect.absoluteAngle != 0)
                {
                    reason = 5;
                    targetGo.TryAddComponent(out Mask unityMask);
                }
                else if (!monoBeh.UsingSpriteRenderer())
                {
                    reason = 6;
                    targetGo.TryAddComponent(out RectMask2D unityMask);
                }
                else
                {
                    reason = 7;
                }
            }

            FcuLogger.Debug($"Draw | {fobject.Data.NameHierarchy} | {reason}", FcuDebugSettingsFlags.LogMask);
        }
    }
}