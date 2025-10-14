using DA_Assets.DAI;
using DA_Assets.FCU.Model;
using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    internal class TextFontsTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_text_and_fonts.Localize(), FcuLocKey.tooltip_text_and_fonts.Localize());
            gui.Space15();

            monoBeh.Settings.TextFontsSettings.TextComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_text_component.Localize(), FcuLocKey.tooltip_text_component.Localize()),
                monoBeh.Settings.TextFontsSettings.TextComponent, false);

            gui.Space10();

            switch (monoBeh.Settings.TextFontsSettings.TextComponent)
            {
                case TextComponent.UnityText:
                    DrawDefaultTextSettings();
                    break;
                case TextComponent.TextMeshPro:
                case TextComponent.RTLTextMeshPro:
                    this.TextMeshSettingsSection.Draw();
                    break;
            }

            gui.Space15();
            gui.SectionHeader(FcuLocKey.label_font_settings.Localize());
            gui.Space15();

            DrawPathSettings();

            gui.Space15();

            DrawGoogleFontsSettings();

#if TextMeshPro
            gui.Space15();
            DrawFontGenerationSettings();
#endif

            gui.Space30();
        }

        public void DrawDefaultTextSettings()
        {
            gui.DrawObjectFields(monoBeh.Settings.UnityTextSettings);

            if (monoBeh.Settings.UnityTextSettings.VerticalWrapMode == VerticalWrapMode.Overflow)
            {
                monoBeh.Settings.UnityTextSettings.BestFit = false;
            }
        }

        private void DrawFontGenerationSettings()
        {
            gui.SectionHeader(FcuLocKey.label_asset_creator_settings.Localize(), FcuLocKey.tooltip_asset_creator_settings.Localize());
            gui.Space15();

            monoBeh.FontDownloader.TmpDownloader.SamplingPointSize = gui.IntField(new GUIContent(FcuLocKey.label_sampling_point_size.Localize(), FcuLocKey.tooltip_sampling_point_size.Localize()),
                monoBeh.FontDownloader.TmpDownloader.SamplingPointSize);

            gui.Space10();

            monoBeh.FontDownloader.TmpDownloader.AtlasPadding = gui.IntField(new GUIContent(FcuLocKey.label_atlas_padding.Localize(), FcuLocKey.tooltip_atlas_padding.Localize()),
                monoBeh.FontDownloader.TmpDownloader.AtlasPadding);

            gui.Space10();

            monoBeh.FontDownloader.TmpDownloader.RenderMode = gui.EnumField(new GUIContent(FcuLocKey.label_render_mode.Localize(), FcuLocKey.tooltip_render_mode.Localize()),
                monoBeh.FontDownloader.TmpDownloader.RenderMode);

            gui.Space10();

            Vector2Int atlasResolution = new Vector2Int(monoBeh.FontDownloader.TmpDownloader.AtlasWidth, monoBeh.FontDownloader.TmpDownloader.AtlasHeight);
            atlasResolution = gui.Vector2IntField(new GUIContent(FcuLocKey.label_atlas_resolution.Localize(), FcuLocKey.tooltip_atlas_resolution.Localize()), atlasResolution);
            monoBeh.FontDownloader.TmpDownloader.AtlasWidth = atlasResolution.x;
            monoBeh.FontDownloader.TmpDownloader.AtlasHeight = atlasResolution.y;

            gui.Space10();

#if TextMeshPro
            monoBeh.FontDownloader.TmpDownloader.AtlasPopulationMode = gui.EnumField(new GUIContent(FcuLocKey.label_atlas_population_mode.Localize(), FcuLocKey.tooltip_atlas_population_mode.Localize()),
                monoBeh.FontDownloader.TmpDownloader.AtlasPopulationMode);
#endif
            gui.Space10();

            monoBeh.FontDownloader.TmpDownloader.EnableMultiAtlasSupport = gui.Toggle(new GUIContent(FcuLocKey.label_enable_multi_atlas_support.Localize(), FcuLocKey.tooltip_enable_multi_atlas_support.Localize()),
               monoBeh.FontDownloader.TmpDownloader.EnableMultiAtlasSupport);

            gui.Space15();

            if (gui.OutlineButton(FcuLocKey.label_download_fonts_from_project.Localize(monoBeh.Settings.TextFontsSettings.TextComponent)))
            {
                _ = monoBeh.FontDownloader.DownloadAllProjectFonts();
            }
        }

        private void DrawGoogleFontsSettings()
        {
            gui.SectionHeader(FcuLocKey.label_google_fonts_settings.Localize());
            gui.Space15();

            FcuConfig.GoogleFontsApiKey = gui.TextField(
                new GUIContent(FcuLocKey.label_google_fonts_api_key.Localize(), FcuLocKey.tooltip_google_fonts_api_key.Localize(FcuLocKey.label_google_fonts_api_key.Localize())),
                FcuConfig.GoogleFontsApiKey,
                new GUIContent(FcuLocKey.label_get_google_api_key.Localize()), () =>
                {
                    Application.OpenURL("https://developers.google.com/fonts/docs/developer_api#identifying_your_application_to_google");
                });

            gui.Space10();

            monoBeh.FontDownloader.GFontsApi.FontSubsets |= FontSubset.Latin;

            gui.SerializedPropertyField<FigmaConverterUnity>(
                scriptableObject.SerializedObject, x => x.FontDownloader.GFontsApi.FontSubsets);
        }

        private void DrawPathSettings()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    monoBeh.FontLoader.TtfFontsPath = gui.FolderField(
                       new GUIContent(FcuLocKey.label_ttf_path.Localize(), ""),
                       monoBeh.FontLoader.TtfFontsPath,
                       new GUIContent(FcuLocKey.label_change.Localize()),
                       FcuLocKey.label_select_fonts_folder.Localize());

                    gui.Space10();

                    if (gui.SubButtonText(FcuLocKey.label_add_ttf_fonts_from_folder.Localize(), FcuLocKey.tooltip_add_fonts_from_folder.Localize()))
                    {
                        _ = monoBeh.FontLoader.AddToTtfFontsList();
                    }
                }
            });


            gui.Space5();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.FontLoader.TtfFonts);

#if TextMeshPro
            gui.Space15();

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    monoBeh.FontLoader.TmpFontsPath = gui.FolderField(
                        new GUIContent(FcuLocKey.label_tmp_path.Localize(), ""),
                        monoBeh.FontLoader.TmpFontsPath,
                        new GUIContent(FcuLocKey.label_change.Localize()),
                        FcuLocKey.label_select_fonts_folder.Localize());

                    gui.Space5();

                    if (gui.SubButtonText(FcuLocKey.label_add_tmp_fonts_from_folder.Localize(), FcuLocKey.tooltip_add_fonts_from_folder.Localize()))
                    {
                        _ = monoBeh.FontLoader.AddToTmpMeshFontsList();
                    }
                }
            });

            gui.Space5();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.FontLoader.TmpFonts);
#endif
        }

        private TextMeshSection textMeshSettingsSection;
        internal TextMeshSection TextMeshSettingsSection => monoBeh.Link(ref textMeshSettingsSection, scriptableObject);
    }
}
