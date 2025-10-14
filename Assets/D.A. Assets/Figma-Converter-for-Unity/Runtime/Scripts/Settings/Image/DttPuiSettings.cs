using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_procedural_ui_settings, FcuLocKey.tooltip_procedural_ui_settings)]
    public class DttPuiSettings : BaseImageSettings
    {
        [SerializeField] float falloffDistance = 1f;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_pui_falloff_distance, FcuLocKey.tooltip_pui_falloff_distance)]
        public float FalloffDistance { get => falloffDistance; set => SetValue(ref falloffDistance, value); }
    }
}