using DA_Assets.DAI;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class PrefabCreatorTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_prefab_creator.Localize());
            gui.Space15();

            monoBeh.Settings.PrefabSettings.PrefabsPath = gui.FolderField(
                new GUIContent(FcuLocKey.label_prefabs_path.Localize(), FcuLocKey.tooltip_prefabs_path.Localize()),
                monoBeh.Settings.PrefabSettings.PrefabsPath,
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_prefabs_folder.Localize());

            gui.Space10();

            monoBeh.Settings.PrefabSettings.TextPrefabNameType = gui.EnumField(
                new GUIContent(FcuLocKey.label_text_prefab_naming_mode.Localize(), ""),
                monoBeh.Settings.PrefabSettings.TextPrefabNameType,
                false,
                new string[]
                {
                    FcuLocKey.label_humanized_color.Localize(),
                    FcuLocKey.label_hex_color.Localize(),
                    FcuLocKey.label_figma_color.Localize()
                });

            gui.Space10();

            if (gui.OutlineButton("Create", expand: true))
            {
                monoBeh.EventHandlers.CreatePrefabs_OnClick();
            }
        }
    }
}