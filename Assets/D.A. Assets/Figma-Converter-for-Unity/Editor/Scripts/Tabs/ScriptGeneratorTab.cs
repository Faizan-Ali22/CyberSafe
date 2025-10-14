using DA_Assets.DAI;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    internal class ScriptGeneratorTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_script_generator.Localize());
            gui.Space15();

            monoBeh.Settings.ScriptGeneratorSettings.SerializationMode = gui.EnumField(
                new GUIContent(FcuLocKey.label_serialization_mode.Localize()),
                monoBeh.Settings.ScriptGeneratorSettings.SerializationMode, uppercase: false);

            gui.Space10();

            monoBeh.Settings.ScriptGeneratorSettings.Namespace = gui.TextField(
                new GUIContent(FcuLocKey.label_namespace.Localize()),
                monoBeh.Settings.ScriptGeneratorSettings.Namespace);

            gui.Space10();

            monoBeh.Settings.ScriptGeneratorSettings.BaseClass = gui.TextField(
                new GUIContent(FcuLocKey.label_base_class.Localize()),
                monoBeh.Settings.ScriptGeneratorSettings.BaseClass);

            gui.Space10();

            monoBeh.Settings.ScriptGeneratorSettings.OutputPath = gui.FolderField(
                new GUIContent(FcuLocKey.label_scripts_output_path.Localize()),
                monoBeh.Settings.ScriptGeneratorSettings.OutputPath,
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_folder.Localize());

            gui.Space10();

            monoBeh.Settings.ScriptGeneratorSettings.FieldNameMaxLenght = gui.IntField(
                new GUIContent(FcuLocKey.label_field_name_max_length.Localize(), FcuLocKey.tooltip_field_name_max_length.Localize()),
                monoBeh.Settings.ScriptGeneratorSettings.FieldNameMaxLenght);

            gui.Space10();

            monoBeh.Settings.ScriptGeneratorSettings.MethodNameMaxLenght = gui.IntField(
                new GUIContent(FcuLocKey.label_method_name_max_length.Localize(), FcuLocKey.tooltip_method_name_max_length.Localize()),
                monoBeh.Settings.ScriptGeneratorSettings.MethodNameMaxLenght);

            gui.Space10();

            monoBeh.Settings.ScriptGeneratorSettings.ClassNameMaxLenght = gui.IntField(
                new GUIContent(FcuLocKey.label_class_name_max_length.Localize(), FcuLocKey.tooltip_class_name_max_length.Localize()),
                monoBeh.Settings.ScriptGeneratorSettings.ClassNameMaxLenght);

            gui.Space10();

            if (gui.OutlineButton("Generate scripts", expand: true))
            {
                monoBeh.EventHandlers.GenerateScripts_OnClick();
            }

            gui.Space10();

            if (gui.OutlineButton("Serialize objects", expand: true))
            {
                monoBeh.EventHandlers.SerializeObjects_OnClick();
            }
        }
    }
}