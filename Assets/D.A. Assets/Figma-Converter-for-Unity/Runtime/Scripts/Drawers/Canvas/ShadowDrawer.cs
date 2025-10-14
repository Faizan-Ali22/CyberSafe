using DA_Assets.FCU.Model;
using System;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

#if TRUESHADOW_EXISTS
using LeTai.TrueShadow;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ShadowDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            switch (monoBeh.Settings.ShadowSettings.ShadowComponent)
            {
                case ShadowComponent.TrueShadow:
                    DrawTrueShadow(fobject);
                    break;
            }
        }

        private void DrawTrueShadow(FObject fobject)
        {
#if TRUESHADOW_EXISTS
            TrueShadow[] oldShadows = fobject.Data.GameObject.GetComponents<TrueShadow>();

            if (fobject.IsDownloadableType())
            {
                foreach (TrueShadow item in oldShadows)
                {
                    item.enabled = false;
                }

                return;
            }

            IEnumerable<Effect> newShadows = fobject.Effects.Where(x => x.IsShadowType()).ToArray();

            int newShadowCount = newShadows.Count();
            int oldShadowCount = oldShadows.Length;

            FcuLogger.Debug($"DrawTrueShadow | {fobject.Data.NameHierarchy} | newShadowCount: {newShadowCount} | oldShadowCount: {oldShadowCount}", FcuDebugSettingsFlags.LogComponentDrawer);

            int i = 0;

            foreach (TrueShadow oldShadow in oldShadows)
            {
                if (i < newShadowCount)
                {
                    AssignShadowEffect(oldShadow, newShadows.ElementAt(i));
                    i++;
                }
                else
                {
                    oldShadow.Destroy(); // Remove unnecessary TrueShadows.
                }
            }

            // If shadows are more than oldShadows - add new TrueShadows.
            for (; i < newShadowCount; i++)
            {
                fobject.Data.GameObject.TryAddGraphic(out Image img);
                fobject.Data.GameObject.TryAddComponent(out TrueShadow trueShadow, supportMultiInstance: true);

                if (!fobject.ContainsTag(FcuTag.Image) && !fobject.ContainsTag(FcuTag.Text))
                {
                    fobject.Data.GameObject.TryGetComponentSafe(out Graphic gr);
                    gr.enabled = false;
                }


                AssignShadowEffect(trueShadow, newShadows.ElementAt(i));

            }
#endif
        }

#if TRUESHADOW_EXISTS
        void AssignShadowEffect(TrueShadow trueShadow, Effect effect)
        {
            ShadowData shadowData = GetShadowData(effect);

            trueShadow.OffsetAngle = shadowData.Angle;
            trueShadow.OffsetDistance = shadowData.Distance;
            trueShadow.Spread = shadowData.Spread;
            trueShadow.Color = shadowData.Color;
            trueShadow.Size = shadowData.Radius;

            trueShadow.BlendMode = BlendMode.Multiply;

            if (effect.Type.ToString().Contains("DROP"))
                trueShadow.Inset = false;
            else
                trueShadow.Inset = true;

            trueShadow.enabled = true;
        }
#endif

        internal ShadowData GetShadowData(Effect effect)
        {
            ShadowData shadowData = new ShadowData();
            shadowData.Offset = effect.Offset;
            shadowData.EffectType = effect.Type;

            float x = effect.Offset.x;
            float y = effect.Offset.y;

            float angle = Mathf.Atan2(y, x) * (180.0f / Mathf.PI);
            float distance = Mathf.Sqrt(x * x + y * y);

            shadowData.Angle = angle;
            shadowData.Distance = distance;
            shadowData.Spread = effect.Spread.ToFloat();

            shadowData.Color = effect.Color;
            shadowData.Radius = effect.Radius;

            return shadowData;
        }
    }

    internal struct ShadowData
    {
        public EffectType EffectType { get; set; }
        public Vector2 Offset { get; set; }
        public float Angle { get; set; }
        public float Distance { get; set; }
        public float Spread { get; set; }
        public Color Color { get; set; }
        public float Radius { get; set; }
    }
}