using DA_Assets.DAI;
using DA_Assets.Extensions;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    [CustomEditor(typeof(SyncHelper)), CanEditMultipleObjects]
    internal class SyncHelperEditor : Editor
    {
        [SerializeField] DAInspector gui;
        private FigmaConverterUnity fcu;
        private SyncHelper syncHelper;

        private void OnEnable()
        {
            syncHelper = (SyncHelper)target;

            if (syncHelper.Data != null)
            {
                fcu = syncHelper.Data.FigmaConverterUnity as FigmaConverterUnity;
            }
        }

        public override void OnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.Background,
                Body = () =>
                {
                    gui.Colorize(() =>
                    {
                        syncHelper.Debug = gui.Toggle(new GUIContent("Debug"), syncHelper.Debug);

                        gui.Space10();

                        if (fcu == null)
                        {
                            gui.Label10px(FcuLocKey.label_fcu_is_null.Localize(nameof(FigmaConverterUnity), FcuConfig.CreatePrefabs, FcuConfig.SetFcuToSyncHelpers), null, GUILayout.ExpandWidth(true));
                            gui.Space10();
                        }

                        if (syncHelper.Data == null)
                            return;

                        if (!syncHelper.Data.NameHierarchy.IsEmpty())
                        {
                            GUILayout.TextArea(syncHelper.Data.NameHierarchy);
                            gui.Space10();
                        }

                        if (!syncHelper.Data.Names.FigmaName.IsEmpty())
                        {
                            GUILayout.TextArea(syncHelper.Data.Names.FigmaName);
                            gui.Space10();
                        }

                        if (!syncHelper.Data.ProjectId.IsEmpty() && !syncHelper.Data.Id.IsEmpty())
                        {
                            if (GUILayout.Button(new GUIContent("View component in Figma")))
                            {
                                string figmaUrl = $"https://www.figma.com/design/{syncHelper.Data.ProjectId}?node-id={syncHelper.Data.Id.Replace(":", "-")}";
                                Application.OpenURL(figmaUrl);
                            }

                            gui.Space10();
                        }

                        gui.Label12px(FcuLocKey.label_dont_remove_fcu_meta.Localize(), null, GUILayout.ExpandWidth(true));
                        gui.Label10px(FcuLocKey.label_more_about_layout_updating.Localize(), null, GUILayout.ExpandWidth(true));

                        if (syncHelper.Debug)
                        {
                            gui.Space10();
                            base.OnInspectorGUI();
                        }
                    });
                }
            });
        }
    }
}