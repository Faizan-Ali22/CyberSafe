using DA_Assets.DAI;
using DA_Assets.Logging;
using System;
using UnityEngine;

#pragma warning disable CS0162

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class ShadowSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {

        [SerializeField] ShadowComponent shadowComponent = ShadowComponent.Figma;
        public ShadowComponent ShadowComponent
        {
            get => shadowComponent;
            set
            {
                switch (value)
                {
                    case ShadowComponent.TrueShadow:
#if TRUESHADOW_EXISTS == false
                        DALogger.LogError(FcuLocKey.log_asset_not_imported.Localize(nameof(ShadowComponent.TrueShadow)));
                        value = ShadowComponent.Figma;
#endif
                        break;
                }

                SetValue(ref shadowComponent, value);
            }
        }

    }
}