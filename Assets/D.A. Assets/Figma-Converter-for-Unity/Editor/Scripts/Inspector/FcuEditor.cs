using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.Logging;
using DA_Assets.Tools;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#pragma warning disable IDE0003
#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [CustomEditor(typeof(FigmaConverterUnity)), CanEditMultipleObjects]
    internal class FcuEditor :  DAEditor<FcuEditor, FigmaConverterUnity>
    {
        [SerializeField] Texture2D _fcuLogo;
        public Texture2D FcuLogo => _fcuLogo;

        protected override void OnEnable()
        {
            base.OnEnable();

            monoBeh.DelegateHolder.SetSpriteRects = SpriteEditorUtility.SetSpriteRects;
            monoBeh.DelegateHolder.ShowDifferenceChecker = ShowDifferenceChecker;
            monoBeh.DelegateHolder.UpdateScrollContent = this.FrameList.UpdateScrollContent;
            monoBeh.DelegateHolder.UpdateScrollContent();
            monoBeh.DelegateHolder.SetGameViewSize = GameViewUtils.SetGameViewSize;

            _ = monoBeh.Authorizer.TryRestoreSession();
        }

        private void ShowDifferenceChecker(PreImportInput data, Action<PreImportOutput> callback)
        {
            this.DifferenceCheckerWindow.SetData(data, callback);
            this.DifferenceCheckerWindow.Show();
        }

        public void DrawBaseOnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Body = () => base.OnInspectorGUI()
            });
        }

        public override void OnInspectorGUI()
        {
            if (monoBeh.Settings.MainSettings.WindowMode)
            {
                DrawWindowedGUI();
            }
            else
            {
                DrawGUI(gui.ColoredStyle.Background);
            }
        }

        public void DrawWindowedGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.Background,
                Body = () =>
                {
                    gui.TopProgressBar(monoBeh.RequestSender.PbarProgress);

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            this.Header.DrawSmallHeader();

                            gui.Space15();

                            if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconOpen, FcuLocKey.tooltip_open_fcu_window.Localize())))
                            {
                                this.SettingsWindow.Show();
                            }

                            gui.Space5();

                            if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconExpandWindow, FcuLocKey.tooltip_change_window_mode.Localize())))
                            {
                                if (monoBeh.Settings.MainSettings.WindowMode)
                                {
                                    monoBeh.Settings.MainSettings.WindowMode = false;
                                }
                            }
                        }
                    });
                }
            });
        }

        public void DrawGUI(GUIStyle customStyle)
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = customStyle,
                Body = () =>
                {
                    this.Header.Draw();

                    gui.Space15();

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Horizontal,
                        Body = () =>
                        {
                            monoBeh.Settings.MainSettings.ProjectUrl = gui.BigTextField(monoBeh.Settings.MainSettings.ProjectUrl, null);

                            gui.Space5();

                            Group gr = new Group();

                            if (monoBeh.Settings.MainSettings.WindowMode)
                            {
                                gr.Style = gui.ColoredStyle.Group5Buttons;
                            }
                            else
                            {
                                gr.Style = gui.ColoredStyle.Group6Buttons;
                            }

                            gr.GroupType = GroupType.Horizontal;
                            gr.Body = () =>
                            {
                                if (gui.SquareButton30x30(new GUIContent(gui.Resources.ImgViewRecent, FcuLocKey.tooltip_recent_projects.Localize())))
                                {
                                    ShowRecentProjectsPopup_OnClick();
                                }

                                gui.Space5();
                                if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconDownload, FcuLocKey.tooltip_download_project.Localize())))
                                {
                                    if (monoBeh.Authorizer.IsAuthed() == false)
                                    {
                                        DALogger.Log(FcuLocKey.log_not_authorized.Localize());
                                    }
                                    else if (monoBeh.Settings.MainSettings.ProjectUrl.IsEmpty())
                                    {
                                        DALogger.Log(FcuLocKey.log_incorrent_project_url.Localize());
                                    }
                                    else
                                    {
                                        monoBeh.EventHandlers.DownloadProject_OnClick();
                                    }
                                }

                                gui.Space5();
                                if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconImport, FcuLocKey.tooltip_import_frames.Localize())))
                                {
                                    monoBeh.EventHandlers.ImportSelectedFrames_OnClick();
                                }

                                gui.Space5();
                                if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconStop, FcuLocKey.tooltip_stop_import.Localize())))
                                {
                                    monoBeh.EventHandlers.StopImport_OnClick();
                                }

                                if (monoBeh.Settings.MainSettings.WindowMode == false)
                                {
                                    gui.Space5();
                                    if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconSettings, FcuLocKey.tooltip_open_settings_window.Localize())))
                                    {
                                        this.SettingsWindow.Show();
                                    }
                                }

                                gui.Space5();

                                if (gui.SquareButton30x30(new GUIContent(gui.Resources.IconExpandWindow, FcuLocKey.tooltip_change_window_mode.Localize())))
                                {
                                    if (monoBeh.Settings.MainSettings.WindowMode)
                                    {
                                        monoBeh.Settings.MainSettings.WindowMode = false;
                                        this.SettingsWindow.CreateTabs();
                                    }
                                    else
                                    {
                                        monoBeh.Settings.MainSettings.WindowMode = true;
                                        this.SettingsWindow.Show();
                                    }
                                }
                            };

                            gui.DrawGroup(gr);
                        }
                    });

                    gui.Space5();

                    if (!monoBeh.InspectorDrawer.SelectableDocument.IsProjectEmpty())
                    {
                        this.FrameList.DrawDocument();
                    }

                    UpdateChecker.DrawDeveloperMessages(AssetType.fcu, FcuConfig.Instance.ProductVersion);

                    if (!monoBeh.IsJsonNetExists())
                    {
                        EditorGUILayout.HelpBox(@$"Newtonsoft.Json, which is required for the asset to function, is not installed.
Please read the PDF manual included with the asset to resolve this issue.", MessageType.Error);
                        EditorGUILayout.Space(5);
                    }

                    if (monoBeh.AssetTools.NeedShowRateMe)
                    {
                        RateMeUI();
                    }

                    gui.DrawFooter();
                }
            });
        }

        private void DrawStar()
        {
            GUILayout.Box(gui.Resources.ImgStar, gui.ColoredStyle.ImgStar);
        }

        private void Draw5Stars()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    gui.FlexibleSpace();

                    for (int i = 0; i < 5; i++)
                    {
                        DrawStar();

                        if (i != 5)
                        {
                            gui.Space5();
                        }
                    }

                    gui.FlexibleSpace();
                }
            });
        }

        private void RateMeUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Style = EditorStyles.helpBox,
                Body = () =>
                {
                    int dc = UpdateChecker.GetFirstVersionDaysCount(AssetType.fcu);
                    GUILayout.Label(new GUIContent(FcuLocKey.label_rateme_desc.Localize(dc)), GUILayout.ExpandWidth(true));

                    gui.Space5();

                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        Body = () =>
                        {
                            Draw5Stars();

                            gui.FlexibleSpace();

                            gui.Colorize(() =>
                            {
                                if (GUILayout.Button(new GUIContent("Don't show")))
                                {
                                    DontShowRateMe_OnClick();
                                }

                                gui.Space5();

                                if (GUILayout.Button(new GUIContent("Open Asset Store")))
                                {
                                    int packageId;

                                    if (monoBeh.IsUGUI())
                                    {
                                        packageId = 198134;
                                    }
                                    else
                                    {
                                        packageId = 272042;
                                    }

                                    Application.OpenURL("https://assetstore.unity.com/packages/tools/utilities/" + packageId + "#reviews");

                                    DontShowRateMe_OnClick();
                                }
                            });
                        }
                    });
                }
            });
        }

        private void DontShowRateMe_OnClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetInt(FcuConfig.RATEME_PREFS_KEY, 1);
#endif
        }

        private void DisplayDeveloperMessages(DeveloperMessage assetMessage, DeveloperMessage versionMessage)
        {
            bool hasAssetMessage = !string.IsNullOrEmpty(assetMessage.Text);
            bool hasVersionMessage = !string.IsNullOrEmpty(versionMessage.Text);

            if (hasAssetMessage || hasVersionMessage)
            {
                EditorGUILayout.Space(15);
            }

            if (hasAssetMessage)
            {
                gui.HelpBox(assetMessage.Text, assetMessage.Type);
                EditorGUILayout.Space(5);
            }

            if (hasVersionMessage)
            {
                gui.HelpBox(versionMessage.Text, versionMessage.Type);
                EditorGUILayout.Space(5);
            }
        }

        private void ShowRecentProjectsPopup_OnClick()
        {
            List<RecentProject> recentProjects = monoBeh.ProjectCacher.GetRecentProjects();

            List<GUIContent> options = new List<GUIContent>();

            if (recentProjects.IsEmpty())
            {
                options.Add(new GUIContent(FcuLocKey.label_no_recent_projects.Localize()));
            }
            else
            {
                foreach (RecentProject project in recentProjects)
                {
                    options.Add(new GUIContent(project.Name));
                }
            }

            EditorUtility.DisplayCustomMenu(new Rect(11, 150, 0, 0), options.ToArray(), -1, (userData, ops, selected) =>
            {
                RecentProject recentProject = recentProjects[selected];
                monoBeh.Settings.MainSettings.ProjectUrl = recentProject.Url;
                monoBeh.EventHandlers.DownloadProject_OnClick();
            }, null);
        }

        internal DifferenceCheckerWindow DifferenceCheckerWindow =>
            DifferenceCheckerWindow.GetInstance(this, monoBeh, new Vector2(900, 600), false, title: "Layout Update Manager (beta)");

        internal FcuSettingsWindow SettingsWindow =>
            FcuSettingsWindow.GetInstance(this, monoBeh, new Vector2(800, 600), false, title: "Settings");

        private HeaderSection _headerSection;
        internal HeaderSection Header => monoBeh.Link(ref _headerSection, this);

        private FramesSection _frameListSection;
        internal FramesSection FrameList => monoBeh.Link(ref _frameListSection, this);


    }

    public enum HamburgerMenuId
    {
        MainSettingsKey,
        UnityTextSettingsKey,
        TextMeshSettingsKey,
        PuiSettingsKey,
        MPUIKitSettingsKey,
        FrameListKey,
        AssetsConfigKey,
        AssetToolsKey,
        DebugToolsKey,
        RemoveUnusedSpritesKey,
        ImportEventsKey,
        UnityImageSettingsKey,
        Shapes2DSettingsKey,
        TMFontsConverterKey
    }
}