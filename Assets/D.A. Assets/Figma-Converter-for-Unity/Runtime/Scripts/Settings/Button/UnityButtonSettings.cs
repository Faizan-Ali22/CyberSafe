using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_button_settings, FcuLocKey.tooltip_button_settings)]
    public class UnityButtonSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] Color normalColor = new Color32(255, 255, 255, 255);
        [FcuInspectorProperty(ComponentType.ColorField, FcuLocKey.label_normal_color, FcuLocKey.tooltip_normal_color)]
        public Color NormalColor { get => normalColor; set => SetValue(ref normalColor, value); }

        [SerializeField] Color highlightedColor = new Color32(245, 245, 245, 255);
        [FcuInspectorProperty(ComponentType.ColorField, FcuLocKey.label_highlighted_color, FcuLocKey.tooltip_highlighted_color)]
        public Color HighlightedColor { get => highlightedColor; set => SetValue(ref highlightedColor, value); }

        [SerializeField] Color pressedColor = new Color32(200, 200, 200, 255);
        [FcuInspectorProperty(ComponentType.ColorField, FcuLocKey.label_pressed_color, FcuLocKey.tooltip_pressed_color)]
        public Color PressedColor { get => pressedColor; set => SetValue(ref pressedColor, value); }

        [SerializeField] Color selectedColor = new Color32(245, 245, 245, 255);
        [FcuInspectorProperty(ComponentType.ColorField, FcuLocKey.label_selected_color, FcuLocKey.tooltip_selected_color)]
        public Color SelectedColor { get => selectedColor; set => SetValue(ref selectedColor, value); }

        [SerializeField] Color disabledColor = new Color32(200, 200, 200, 128);
        [FcuInspectorProperty(ComponentType.ColorField, FcuLocKey.label_disabled_color, FcuLocKey.tooltip_disabled_color)]
        public Color DisabledColor { get => disabledColor; set => SetValue(ref disabledColor, value); }

        [SerializeField] float colorMultiplier = 1f;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_color_multiplier, FcuLocKey.tooltip_color_multiplier)]
        public float ColorMultiplier { get => colorMultiplier; set => SetValue(ref colorMultiplier, value); }

        [SerializeField] float fadeDuration = 0.1f;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_fade_duration, FcuLocKey.tooltip_fade_duration)]
        public float FadeDuration { get => fadeDuration; set => SetValue(ref fadeDuration, value); }
    }
}