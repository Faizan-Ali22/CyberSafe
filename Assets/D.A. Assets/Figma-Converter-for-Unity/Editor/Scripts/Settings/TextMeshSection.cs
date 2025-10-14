using DA_Assets.DAI;
using DA_Assets.FCU.Extensions;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class TextMeshSection : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        private string[] shaderNames = new string[] { };

        private int shaderSelectedIndex = -1;

        public override void OnLink()
        {
            SetShaderNames();
        }

        private void SetShaderNames()
        {
            shaderNames = ShaderUtil.GetAllShaderInfo().Select(info => info.name).ToArray();
        }

        public void Draw()
        {
#if TextMeshPro
            gui.SectionHeader(FcuLocKey.label_textmeshpro_settings.Localize());
            gui.Space15();

            monoBeh.Settings.TextMeshSettings.AutoSize = gui.Toggle(new GUIContent(FcuLocKey.label_auto_size.Localize(), FcuLocKey.tooltip_auto_size.Localize()),
                monoBeh.Settings.TextMeshSettings.AutoSize);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.OverrideTags = gui.Toggle(new GUIContent(FcuLocKey.label_override_tags.Localize(), FcuLocKey.tooltip_override_tags.Localize()),
                monoBeh.Settings.TextMeshSettings.OverrideTags);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.Wrapping = gui.Toggle(new GUIContent(FcuLocKey.label_wrapping.Localize(), FcuLocKey.tooltip_wrapping.Localize()),
                monoBeh.Settings.TextMeshSettings.Wrapping);

            gui.Space10();

            if (monoBeh.IsNova() || monoBeh.IsDebug())
            {
                monoBeh.Settings.TextMeshSettings.OrthographicMode = gui.Toggle(new GUIContent(FcuLocKey.label_orthographic_mode.Localize(), FcuLocKey.tooltip_orthographic_mode.Localize()),
                    monoBeh.Settings.TextMeshSettings.OrthographicMode);

                gui.Space10();
            }

            monoBeh.Settings.TextMeshSettings.RichText = gui.Toggle(new GUIContent(FcuLocKey.label_rich_text.Localize(), FcuLocKey.tooltip_rich_text.Localize()),
                monoBeh.Settings.TextMeshSettings.RichText);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.RaycastTarget = gui.Toggle(new GUIContent(FcuLocKey.label_raycast_target.Localize(), FcuLocKey.tooltip_raycast_target.Localize()),
               monoBeh.Settings.TextMeshSettings.RaycastTarget);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.ParseEscapeCharacters = gui.Toggle(new GUIContent(FcuLocKey.label_parse_escape_characters.Localize(), FcuLocKey.tooltip_parse_escape_characters.Localize()),
               monoBeh.Settings.TextMeshSettings.ParseEscapeCharacters);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.VisibleDescender = gui.Toggle(new GUIContent(FcuLocKey.label_visible_descender.Localize(), FcuLocKey.tooltip_visible_descender.Localize()),
               monoBeh.Settings.TextMeshSettings.VisibleDescender);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.Kerning = gui.Toggle(new GUIContent(FcuLocKey.label_kerning.Localize(), FcuLocKey.tooltip_kerning.Localize()),
               monoBeh.Settings.TextMeshSettings.Kerning);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.ExtraPadding = gui.Toggle(new GUIContent(FcuLocKey.label_extra_padding.Localize(), FcuLocKey.tooltip_extra_padding.Localize()),
               monoBeh.Settings.TextMeshSettings.ExtraPadding);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.Overflow = gui.EnumField(new GUIContent(FcuLocKey.label_overflow.Localize(), FcuLocKey.tooltip_overflow.Localize()),
                monoBeh.Settings.TextMeshSettings.Overflow);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.HorizontalMapping = gui.EnumField(new GUIContent(FcuLocKey.label_horizontal_mapping.Localize(), FcuLocKey.tooltip_horizontal_mapping.Localize()),
                monoBeh.Settings.TextMeshSettings.HorizontalMapping);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.VerticalMapping = gui.EnumField(new GUIContent(FcuLocKey.label_vertical_mapping.Localize(), FcuLocKey.tooltip_vertical_mapping.Localize()),
                monoBeh.Settings.TextMeshSettings.VerticalMapping);

            gui.Space10();

            monoBeh.Settings.TextMeshSettings.GeometrySorting = gui.EnumField(new GUIContent(FcuLocKey.label_geometry_sorting.Localize(), FcuLocKey.tooltip_geometry_sorting.Localize()),
                monoBeh.Settings.TextMeshSettings.GeometrySorting);

            gui.Space10();

            shaderSelectedIndex = gui.ShaderDropdown(new GUIContent(FcuLocKey.label_shader.Localize()), shaderSelectedIndex, shaderNames, (option) =>
            {
                monoBeh.Settings.TextMeshSettings.Shader = Shader.Find(shaderNames[option]);
            });

#if RTLTMP_EXISTS
            if (monoBeh.UsingRTLTextMeshPro() || monoBeh.IsDebug())
            {
                gui.Space10();

                monoBeh.Settings.TextMeshSettings.Farsi = gui.Toggle(new GUIContent(FcuLocKey.label_farsi.Localize(), FcuLocKey.tooltip_farsi.Localize()),
                    monoBeh.Settings.TextMeshSettings.Farsi);

                gui.Space10();

                monoBeh.Settings.TextMeshSettings.ForceFix = gui.Toggle(new GUIContent(FcuLocKey.label_force_fix.Localize(), FcuLocKey.tooltip_force_fix.Localize()),
                   monoBeh.Settings.TextMeshSettings.ForceFix);

                gui.Space10();

                monoBeh.Settings.TextMeshSettings.PreserveNumbers = gui.Toggle(new GUIContent(FcuLocKey.label_preserve_numbers.Localize(), FcuLocKey.tooltip_preserve_numbers.Localize()),
                   monoBeh.Settings.TextMeshSettings.PreserveNumbers);

                gui.Space10();

                monoBeh.Settings.TextMeshSettings.FixTags = gui.Toggle(new GUIContent(FcuLocKey.label_fix_tags.Localize(), FcuLocKey.tooltip_fix_tags.Localize()),
                   monoBeh.Settings.TextMeshSettings.FixTags);
            }
#endif
#endif
        }
    }
}