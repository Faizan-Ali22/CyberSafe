using DA_Assets.DAI;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class UITK_Tab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_ui_toolkit_tab.Localize(), FcuLocKey.tooltip_ui_toolkit_tab.Localize());
            gui.Space15();

#if UITK_LINKER_EXISTS
            monoBeh.Settings.UITK_Settings.UitkLinkingMode = gui.EnumField(
                new GUIContent(FcuLocKey.label_uitk_linking_mode.Localize(), FcuLocKey.tooltip_uitk_linking_mode.Localize()),
                monoBeh.Settings.UITK_Settings.UitkLinkingMode);
#endif

            gui.Space10();

            monoBeh.Settings.UITK_Settings.UitkOutputPath = gui.FolderField(
                new GUIContent(FcuLocKey.label_uitk_output_path.Localize(), FcuLocKey.tooltip_uitk_output_path.Localize()),
                monoBeh.Settings.UITK_Settings.UitkOutputPath,
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_folder.Localize());

            gui.Space10();
        }   
    }
}