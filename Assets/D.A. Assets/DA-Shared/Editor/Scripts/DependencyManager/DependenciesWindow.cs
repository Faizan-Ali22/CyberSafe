using DA_Assets.DAI;
using UnityEditor;
using UnityEngine;
using DA_Assets.Extensions;

namespace DA_Assets.DM
{
    internal class DependenciesWindow : EditorWindow
    {
        public DAInspector gui;

        public void OnGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.TabBg2,
                Scroll = true,
                Body = () =>
                {
                    gui.DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        Scroll = true,
                        Body = () =>
                        {
                            if (DependencyItems.Instance.Items.IsEmpty())
                                return;

                            for (int i = 0; i < DependencyItems.Instance.Items.Count; i++)
                            {
                                DependencyItem ac = DependencyItems.Instance.Items[i];
                                ac.Enabled = gui.Toggle(new GUIContent(ac.Name), ac.Enabled);
                                DependencyItems.Instance.Items[i] = ac;
                                gui.Space10();
                            }
                        }
                    });

                    if (!DependencyItems.Instance.Items.IsEmpty())
                    {
                        gui.Space15();

                        if (gui.OutlineButton(new GUIContent("Apply"), true))
                        {
                            DefineModifier.Apply();
                        }
                    }
                }
            });
        }
    }
}