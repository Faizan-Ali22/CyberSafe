#if NOVA_UI_EXISTS
using DA_Assets.DAI;
using DA_Assets.FCU.Model;
using NovaSamples.Effects;
using System;

namespace DA_Assets.FCU.Drawers
{
    [Serializable]
    public class NovaBlurDrawer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
#if false
            fobject.Data.GameObject.TryGetComponentSafe(out UIBlock2D uIBlock2D);

            foreach (Effect effect in fobject.Effects)
            {
                if (!effect.Type.ToString().Contains("BLUR"))
                    continue;

                fobject.Data.GameObject.TryAddComponent(out BlurEffect blurEffect);

                blurEffect.BlurMode = ConvertBlurType(effect.Type);

                float radius = effect.Radius;
                blurEffect.BlurRadius = radius;
                blurEffect.InputTexture = monoBeh.Settings.NovaSettings.InputTexture;

                if (monoBeh.TryGetComponentSafe(out BackgroundBlurGroup blurGroup))
                {
                    blurGroup.BlurEffects.Add(blurEffect);
                }
            }
#endif
        }

        private BlurMode ConvertBlurType(EffectType effectType)
        {
            switch (effectType)
            {
                case EffectType.LAYER_BLUR:
                    {
                        return BlurMode.LayerBlur;
                    }
                case EffectType.BACKGROUND_BLUR:
                    {
                        return BlurMode.BackgroundBlur;
                    }
                default:
                    {
                        return BlurMode.LayerBlur;
                    }
            }
        }
    }
}
#endif