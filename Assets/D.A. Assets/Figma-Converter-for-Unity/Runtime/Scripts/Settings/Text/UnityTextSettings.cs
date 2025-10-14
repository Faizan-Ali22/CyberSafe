using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_unity_text_settings, FcuLocKey.tooltip_unity_text_settings)]
    public class UnityTextSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] bool bestFit = true;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_best_fit, FcuLocKey.tooltip_best_fit)]
        public bool BestFit { get => bestFit; set => SetValue(ref bestFit, value); }

        [SerializeField] float fontLineSpacing = 1.0f;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_line_spacing, FcuLocKey.tooltip_line_spacing)]
        public float FontLineSpacing { get => fontLineSpacing; set => SetValue(ref fontLineSpacing, value); }

        [SerializeField] HorizontalWrapMode horizontalWrapMode = HorizontalWrapMode.Wrap;
        [FcuInspectorProperty(ComponentType.EnumField, FcuLocKey.label_horizontal_overflow, FcuLocKey.tooltip_horizontal_overflow)]
        public HorizontalWrapMode HorizontalWrapMode { get => horizontalWrapMode; set => SetValue(ref horizontalWrapMode, value); }

        [SerializeField] VerticalWrapMode verticalWrapMode = VerticalWrapMode.Truncate;
        [FcuInspectorProperty(ComponentType.EnumField, FcuLocKey.label_vertical_overflow, FcuLocKey.tooltip_vertical_overflow)]
        public VerticalWrapMode VerticalWrapMode { get => verticalWrapMode; set => SetValue(ref verticalWrapMode, value); }
    }
}