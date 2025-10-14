using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_pui_settings, FcuLocKey.tooltip_pui_settings)]
    public class JoshPuiSettings : BaseImageSettings
    {
        [SerializeField] float falloffDistance = 1f;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_pui_falloff_distance, FcuLocKey.tooltip_pui_falloff_distance)]
        public float FalloffDistance { get => falloffDistance; set => SetValue(ref falloffDistance, value); }
    }
}