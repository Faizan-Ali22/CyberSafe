using DA_Assets.DAI;
using DA_Assets.FCU.Extensions;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class MainSettingsTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_main_settings.Localize());
            gui.Space15();

            gui.EnumField(new GUIContent(FcuLocKey.label_ui_framework.Localize(), FcuLocKey.tooltip_ui_framework.Localize()),
                 monoBeh.Settings.MainSettings.UIFramework, onChange: (newValue) =>
                 {
                     monoBeh.Settings.MainSettings.UIFramework = newValue;
                     scriptableObject.CreateTabs();
                 });

            if (monoBeh.IsUGUI() || monoBeh.IsNova() || monoBeh.IsDebug())
            {
                gui.Space10();

                monoBeh.Settings.MainSettings.GameObjectLayer = gui.LayerField(
                    new GUIContent(FcuLocKey.label_go_layer.Localize(), FcuLocKey.tooltip_go_layer.Localize()),
                    monoBeh.Settings.MainSettings.GameObjectLayer);
            }

            if (monoBeh.IsUGUI() || monoBeh.IsDebug())
            {
                gui.Space10();

                monoBeh.Settings.MainSettings.PositioningMode = gui.EnumField(
                    new GUIContent(FcuLocKey.label_positioning_mode.Localize(), FcuLocKey.tooltip_positioning_mode.Localize()),
                    monoBeh.Settings.MainSettings.PositioningMode);

                gui.Space10();

                monoBeh.Settings.MainSettings.PivotType = gui.EnumField(
                    new GUIContent(FcuLocKey.label_pivot_type.Localize(), FcuLocKey.tooltip_pivot_type.Localize()),
                    monoBeh.Settings.MainSettings.PivotType, uppercase: false);
            }

            gui.Space10();

            monoBeh.Settings.MainSettings.GameObjectNameMaxLenght = gui.IntField(
                new GUIContent(FcuLocKey.label_go_name_max_length.Localize(), FcuLocKey.tooltip_go_name_max_length.Localize()),
                monoBeh.Settings.MainSettings.GameObjectNameMaxLenght);

            gui.Space10();

            monoBeh.Settings.MainSettings.TextObjectNameMaxLenght = gui.IntField(
                new GUIContent(FcuLocKey.label_text_name_max_length.Localize(), FcuLocKey.tooltip_text_name_max_length.Localize()),
                monoBeh.Settings.MainSettings.TextObjectNameMaxLenght);

            gui.Space10();

            monoBeh.Settings.MainSettings.RawImport = gui.Toggle(
                new GUIContent(FcuLocKey.label_raw_import.Localize(), FcuLocKey.tooltip_raw_import.Localize()),
                monoBeh.Settings.MainSettings.RawImport);

            gui.Space10();

            monoBeh.Settings.MainSettings.Https = gui.Toggle(
                new GUIContent(FcuLocKey.label_https_setting.Localize(), FcuLocKey.tooltip_https_setting.Localize()),
                monoBeh.Settings.MainSettings.Https);

            gui.Space10();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, x => x.Settings.MainSettings.AllowedNameChars);
        }
    }
}