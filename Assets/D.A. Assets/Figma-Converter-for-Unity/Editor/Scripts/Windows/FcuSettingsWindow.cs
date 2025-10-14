using DA_Assets.DAI;
using DA_Assets.FCU.Extensions;
using DA_Assets.Tools;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable IDE0003
#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    internal class FcuSettingsWindow : LinkedEditorWindow<FcuSettingsWindow, FcuEditor, FigmaConverterUnity>
    {
        private List<UITab> _tabs = new List<UITab>();
        private int _selectedTab = 0;

        public override void OnShow()
        {
            CreateTabs();
        }

        public void CreateTabs()
        {
            _tabs.Clear();

            if (monoBeh.Settings.MainSettings.WindowMode)
            {
                UITab assetTab = new UITab(FcuLocKey.label_asset.Localize(), null, this.MainTab.Draw);
                _tabs.Add(assetTab);
            }

            UITab mainSettingTab = new UITab(FcuLocKey.label_main_settings.Localize(), null, this.MainSettingsTab.Draw);
            _tabs.Add(mainSettingTab);

            UITab authTab = new UITab(FcuLocKey.label_figma_auth.Localize(), FcuLocKey.tooltip_figma_auth.Localize(), this.AuthorizerTab.Draw);
            _tabs.Add(authTab);

            UITab imageSpritesTab = new UITab(FcuLocKey.label_images_and_sprites_tab.Localize(), FcuLocKey.tooltip_images_and_sprites_tab.Localize(), this.ImageSpritesTab.Draw);
            _tabs.Add(imageSpritesTab);

            UITab textFontTab = new UITab(FcuLocKey.label_text_and_fonts.Localize(), FcuLocKey.tooltip_text_and_fonts.Localize(), this.TextFontsTab.Draw);
            _tabs.Add(textFontTab);

            if (monoBeh.IsUGUI() || monoBeh.IsNova() || monoBeh.IsDebug())
            {
                UITab buttonsTab = new UITab(FcuLocKey.label_buttons_tab.Localize(), FcuLocKey.tooltip_buttons_tab.Localize(), this.ButtonsTab.Draw);
                _tabs.Add(buttonsTab);
            }

            UITab uitkTab = new UITab(FcuLocKey.label_ui_toolkit_tab.Localize(), FcuLocKey.tooltip_ui_toolkit_tab.Localize(), this.UITK_Tab.Draw);
            _tabs.Add(uitkTab);

            UITab locTab = new UITab(FcuLocKey.label_localization_settings.Localize(), null, this.LocalizationTab.Draw);
            _tabs.Add(locTab);

            if (monoBeh.IsUGUI() || monoBeh.IsDebug())
            {
                UITab shadowsTab = new UITab(FcuLocKey.label_shadows_tab.Localize(), FcuLocKey.tooltip_shadows_tab.Localize(), this.ShadowsTab.Draw);
                _tabs.Add(shadowsTab);
            }

            if (monoBeh.IsUGUI() || monoBeh.IsNova() || monoBeh.IsDebug())
            {
                UITab prefabsTab = new UITab(FcuLocKey.label_prefab_creator.Localize(), null, this.PrefabCreatorTab.Draw);
                _tabs.Add(prefabsTab);
            }

            UITab scriptGeneratorTab = new UITab(FcuLocKey.label_script_generator.Localize(), FcuLocKey.tooltip_script_generator.Localize(), this.ScriptGeneratorTab.Draw);
            _tabs.Add(scriptGeneratorTab);

            UITab importEventsTab = new UITab(FcuLocKey.label_import_events.Localize(), null, this.ImportEventsTab.Draw);
            _tabs.Add(importEventsTab);

            UITab debugTools = new UITab(FcuLocKey.label_debug.Localize(), FcuLocKey.tooltip_debug_tools.Localize(), this.DebugTab.Draw);
            _tabs.Add(debugTools);

            _tabs[_selectedTab].Selected = true;
        }

        public override void DrawGUI()
        {
            if (_tabs.Count < 1)
            {
                return;
            }

            if (monoBeh.Settings.MainSettings.WindowMode)
            {
                titleContent = new GUIContent(FcuLocKey.label_fcu.Localize());
            }
            else
            {
                titleContent = new GUIContent(FcuLocKey.label_settings.Localize());
            }

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    DrawMenu();
                    gui.VerticalSeparator();
                    DrawTabContent();
                }
            });
        }

        private void DrawMenu()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.HamburgerTabsBg,
                Options = new[] { GUILayout.Width(200) },
                Scroll = true,
                InstanceId = monoBeh.GetInstanceID(),
                Body = () =>
                {
                    for (int i = 0; i < _tabs.Count; i++)
                    {
                        if (gui.TabButton(_tabs[i]))
                        {
                            _selectedTab = i;
                            _tabs[i].Selected = true;

                            for (int j = 0; j < _tabs.Count; j++)
                            {
                                if (i != j)
                                {
                                    _tabs[j].Selected = false;
                                }
                            }
                        }
                    }

                    gui.FlexibleSpace();

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            gui.Space10();

                            if (_currentAssetVersion == null)
                            {
                                _currentAssetVersion = UpdateChecker.GetCachedAssetVersion(AssetType.fcu, FcuConfig.Instance.ProductVersion);
                            }
                            else
                            {
                                switch (_currentAssetVersion.Value.VersionType)
                                {
                                    case VersionType.stable:
                                        gui.Label10px(FcuLocKey.label_stable_version.Localize(), null);
                                        break;
                                    case VersionType.beta:
                                        gui.Label10px(FcuLocKey.label_beta_version.Localize(), null);
                                        break;
                                    case VersionType.buggy:
                                        gui.RedLinkLabel10px(new GUIContent(FcuLocKey.label_buggy_version.Localize()));
                                        break;
                                }
                            }
                        }
                    });

                    gui.Space(7);
                }
            });
        }

        private void DrawTabContent()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.TabBg1,
                Scroll = true,
                InstanceId = monoBeh.GetInstanceID(),
                Body = () =>
                {
                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        Style = gui.ColoredStyle.TabBg2,
                        Body = () =>
                        {
                            _tabs[_selectedTab].Content.Invoke();
                        }
                    });
                }
            });
        }


        private MainTab mainTab;
        internal MainTab MainTab => monoBeh.Link(ref mainTab, this);

        private LocalizationTab localizationTab;
        internal LocalizationTab LocalizationTab => monoBeh.Link(ref localizationTab, this);

        private ButtonsTab buttonsTab;
        internal ButtonsTab ButtonsTab => monoBeh.Link(ref buttonsTab, this);

        private MainSettingsTab mainSettingsTab;
        internal MainSettingsTab MainSettingsTab => monoBeh.Link(ref mainSettingsTab, this);

        private ScriptGeneratorTab scriptGeneratorTab;
        internal ScriptGeneratorTab ScriptGeneratorTab => monoBeh.Link(ref scriptGeneratorTab, this);

        private AuthTab authorizerTab;
        internal AuthTab AuthorizerTab => monoBeh.Link(ref authorizerTab, this);

        private TextFontsTab textFontsTab;
        internal TextFontsTab TextFontsTab => monoBeh.Link(ref textFontsTab, this);

        private UITK_Tab uitkTab;
        internal UITK_Tab UITK_Tab => monoBeh.Link(ref uitkTab, this);

        private ImageSpritesTab imageSpritesTab;
        internal ImageSpritesTab ImageSpritesTab => monoBeh.Link(ref imageSpritesTab, this);

        private ShadowsTab shadowsTab;
        internal ShadowsTab ShadowsTab => monoBeh.Link(ref shadowsTab, this);

        private ImportEventsTab importEventsTab;
        internal ImportEventsTab ImportEventsTab => monoBeh.Link(ref importEventsTab, this);

        private DebugTab debugTab;
        internal DebugTab DebugTab => monoBeh.Link(ref debugTab, this);

        private PrefabCreatorTab prefabCreatorTab;
        private AssetVersion? _currentAssetVersion;

        internal PrefabCreatorTab PrefabCreatorTab => monoBeh.Link(ref prefabCreatorTab, this);
    }
}