using DA_Assets.DAI;
using DA_Assets.FCU.Extensions;
using UnityEditor;
using UnityEngine;

#if DALOC_EXISTS
using DA_Assets.DAL;
#endif

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    internal class LocalizationTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        private GenericMenu _selectLangMenu;

        public override void OnLink()
        {
            base.OnLink();

#if DALOC_EXISTS
            _selectLangMenu = LanguageCollector.CreateLanguageMenu((cultureCode) =>
            {
                monoBeh.Settings.LocalizationSettings.CurrentFigmaLayoutCulture = cultureCode;
                //Debug.Log(cultureCode);
            });
#endif
        }

        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_localization_settings.Localize(), FcuLocKey.tooltip_localization_settings.Localize());
            gui.Space15();

            monoBeh.Settings.LocalizationSettings.LocalizationComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_loc_component.Localize(), FcuLocKey.tooltip_loc_component.Localize()),
                monoBeh.Settings.LocalizationSettings.LocalizationComponent, false, null);

            if (monoBeh.Settings.LocalizationSettings.LocalizationComponent == LocalizationComponent.None)
                return;

            if (monoBeh.Settings.LocalizationSettings.LocalizationComponent == LocalizationComponent.DALocalizator || monoBeh.IsDebug())
            {
                gui.Space10();

#if DALOC_EXISTS
                monoBeh.Settings.LocalizationSettings.Localizator = (ScriptableObject)gui.ObjectField(
                    new GUIContent(FcuLocKey.label_localizator.Localize()),
                    monoBeh.Settings.LocalizationSettings.Localizator,
                    typeof(LocalizatorBase),
                    false);
#endif
                if (monoBeh.Settings.LocalizationSettings.Localizator == null)
                {
                    gui.Space10();
                    string message = $"Before starting the import, you need to serialize your localizer into the '{nameof(monoBeh.Settings.LocalizationSettings.Localizator)}' field.";
                    gui.HelpBox(message, MessageType.Warning);
                }

            }


            gui.Space10();

            monoBeh.Settings.LocalizationSettings.LocKeyMaxLenght = gui.IntField(
                new GUIContent(FcuLocKey.label_loc_key_max_lenght.Localize(), FcuLocKey.tooltip_loc_key_max_lenght.Localize()),
                monoBeh.Settings.LocalizationSettings.LocKeyMaxLenght);

            gui.Space10();

            monoBeh.Settings.LocalizationSettings.LocKeyCaseType = gui.EnumField(
                new GUIContent(FcuLocKey.label_loc_case_type.Localize(), FcuLocKey.tooltip_loc_case_type.Localize()),
                monoBeh.Settings.LocalizationSettings.LocKeyCaseType, false, null);

            gui.Space10();


            gui.GenericMenu(_selectLangMenu, new GUIContent(FcuLocKey.label_figma_layout_culture.Localize()), monoBeh.Settings.LocalizationSettings.CurrentFigmaLayoutCulture);

            gui.Space10();

            monoBeh.Settings.LocalizationSettings.CsvSeparator = gui.EnumField(
                new GUIContent(FcuLocKey.label_csv_separator.Localize(), FcuLocKey.tooltip_csv_separator.Localize()),
                monoBeh.Settings.LocalizationSettings.CsvSeparator, false, null);

            gui.Space10();

            monoBeh.Settings.LocalizationSettings.LocFolderPath = gui.FolderField(
                new GUIContent(FcuLocKey.label_loc_folder_path.Localize(), FcuLocKey.tooltip_loc_folder_path.Localize()),
                monoBeh.Settings.LocalizationSettings.LocFolderPath,
                new GUIContent(FcuLocKey.label_change.Localize()),
                FcuLocKey.label_select_folder.Localize());

            gui.Space10();

            monoBeh.Settings.LocalizationSettings.LocFileName = gui.TextField(
                new GUIContent(FcuLocKey.label_loc_file_name.Localize(), FcuLocKey.tooltip_loc_file_name.Localize()),
                monoBeh.Settings.LocalizationSettings.LocFileName);
        }
    }
}
