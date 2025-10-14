using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_mpuikit_settings, FcuLocKey.tooltip_mpuikit_settings)]
    public class MPUIKitSettings : BaseImageSettings
    {
        [SerializeField] float falloffDistance = 0.5f;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_pui_falloff_distance, FcuLocKey.tooltip_pui_falloff_distance)]
        public float FalloffDistance { get => falloffDistance; set => SetValue(ref falloffDistance, value); }
    }
}