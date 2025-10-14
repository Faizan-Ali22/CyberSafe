using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    internal class ImageSpritesTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_images_and_sprites_tab.Localize(), FcuLocKey.tooltip_images_and_sprites_tab.Localize());
            gui.Space15();

            if (monoBeh.IsUGUI())
            {
                monoBeh.Settings.ImageSpritesSettings.ImageComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_image_component.Localize(), FcuLocKey.tooltip_image_component.Localize()),
                monoBeh.Settings.ImageSpritesSettings.ImageComponent, false);

                if (monoBeh.UsingAnyProceduralImage() || monoBeh.IsDebug())
                {
                    gui.Space10();
                    gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ImageSpritesSettings.ProceduralCondition);
                }

                if (monoBeh.UsingSvgImage() || monoBeh.IsDebug())
                {
                    gui.Space10();
                    gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ImageSpritesSettings.SvgCondition);
                }


                if (monoBeh.UsingUnityImage() || monoBeh.UsingRawImage() || monoBeh.IsDebug())
                {
                    if (UnityEditor.PlayerSettings.colorSpace == ColorSpace.Linear || monoBeh.IsDebug())
                    {
                        gui.Space10();
                        monoBeh.Settings.ImageSpritesSettings.UseImageLinearMaterial = gui.Toggle(
                            new GUIContent(FcuLocKey.label_use_image_linear_material.Localize(), FcuLocKey.tooltip_use_image_linear_material.Localize()),
                            monoBeh.Settings.ImageSpritesSettings.UseImageLinearMaterial);
                    }
                }

                gui.Space10();
            }

            monoBeh.Settings.ImageSpritesSettings.ImageFormat = gui.EnumField(
                new GUIContent(FcuLocKey.label_images_format.Localize(), FcuLocKey.tooltip_images_format.Localize()),
                monoBeh.Settings.ImageSpritesSettings.ImageFormat);

            gui.Space10();

            monoBeh.Settings.ImageSpritesSettings.ImageScale = gui.SliderField(
                new GUIContent(FcuLocKey.label_images_scale.Localize(), FcuLocKey.tooltip_images_scale.Localize()),
                monoBeh.Settings.ImageSpritesSettings.ImageScale, 0.25f, 4.0f).RoundToNearest025();

            gui.Space10();

            monoBeh.Settings.ImageSpritesSettings.MaxSpriteSize = gui.Vector2IntField(
                new GUIContent(FcuLocKey.label_max_sprite_size.Localize(), FcuLocKey.tooltip_max_sprite_size.Localize()),
                monoBeh.Settings.ImageSpritesSettings.MaxSpriteSize);

            gui.Space10();

            monoBeh.Settings.ImageSpritesSettings.PixelsPerUnit = gui.FloatField(
                new GUIContent(FcuLocKey.label_pixels_per_unit.Localize(), FcuLocKey.tooltip_pixels_per_unit.Localize()),
                monoBeh.Settings.ImageSpritesSettings.PixelsPerUnit);

            gui.Space10();

            monoBeh.Settings.ImageSpritesSettings.RedownloadSprites = gui.Toggle(
                new GUIContent(FcuLocKey.label_redownload_sprites.Localize(), FcuLocKey.tooltip_redownload_sprites.Localize()),
                monoBeh.Settings.ImageSpritesSettings.RedownloadSprites);

            gui.Space10();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.ImageSpritesSettings.DownloadOptions);

            gui.Space10();

            monoBeh.Settings.ImageSpritesSettings.PreserveRatioMode = gui.EnumField(
                new GUIContent(FcuLocKey.label_preserve_ratio_mode.Localize(), FcuLocKey.tooltip_preserve_ratio_mode.Localize()),
                monoBeh.Settings.ImageSpritesSettings.PreserveRatioMode, uppercase: false);

            gui.Space10();

            monoBeh.Settings.ImageSpritesSettings.SpritesPath = gui.FolderField(
                new GUIContent(FcuLocKey.label_sprites_path.Localize(), FcuLocKey.tooltip_sprites_path.Localize()),
                monoBeh.Settings.ImageSpritesSettings.SpritesPath,
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_folder.Localize());

            if (monoBeh.IsUGUI())
            {
                gui.Space10();

                switch (monoBeh.Settings.ImageSpritesSettings.ImageComponent)
                {
                    case ImageComponent.UnityImage:
                    case ImageComponent.RawImage:
                        this.UnityImageSettingsSection.Draw();
                        break;
                    case ImageComponent.SubcShape:
                        gui.SectionHeader(FcuLocKey.label_shapes2d_settings.Localize());
                        gui.Space15();
                        gui.DrawObjectFields(monoBeh.Settings.Shapes2DSettings);
                        break;
                    case ImageComponent.ProceduralImage:
                        gui.DrawObjectFields(monoBeh.Settings.JoshPuiSettings);
                        break;
                    case ImageComponent.RoundedImage:
                        gui.DrawObjectFields(monoBeh.Settings.DttPuiSettings);
                        break;
                    case ImageComponent.MPImage:
                        gui.DrawObjectFields(monoBeh.Settings.MPUIKitSettings);
                        break;
                    case ImageComponent.SpriteRenderer:
                        gui.DrawObjectFields(monoBeh.Settings.SpriteRendererSettings);
                        break;
                    case ImageComponent.SvgImage:
                        gui.DrawObjectFields(monoBeh.Settings.SvgImageSettings);
                        break;
                }
            }
                
            gui.Space10();

            gui.DrawObjectFields(monoBeh.Settings.TextureImporterSettings);

            if (monoBeh.IsUGUI())
            {
                if (monoBeh.UsingSvgImage() || monoBeh.IsDebug())
                {
                    gui.Space10();
                    gui.DrawObjectFields(monoBeh.Settings.SVGImporterSettings);
                }
            }           
        }

        private UnityImageSection unityImageSettingsSection;
        internal UnityImageSection UnityImageSettingsSection => monoBeh.Link(ref unityImageSettingsSection, scriptableObject);
    }
}
