using DA_Assets.DAI;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.CR
{
    [CustomEditor(typeof(CornerRounder)), CanEditMultipleObjects]
    public class CornerRounderEditor : Editor
    {
        [SerializeField] DAInspector gui;
        [SerializeField] Texture2D assetLogo;

        /// <summary>
        /// 0 - top left, 1 - top right, 3 - bottom left, 2 - bottom right
        /// </summary>
        [SerializeField] Texture2D cornerTopLeftIcon;
        [SerializeField] Texture2D cornerTopRightIcon;
        [SerializeField] Texture2D cornerBottomLeftIcon;
        [SerializeField] Texture2D cornerBottomRightIcon;
        [SerializeField] Texture2D cornerAllIcon;

        public CornerRounder monoBeh;

        private void OnEnable()
        {
            monoBeh = (CornerRounder)target;
        }

        public override void OnInspectorGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.Background,
                Body = () =>
                {
                    monoBeh.independent = gui.CheckBox(new GUIContent("Independent"), monoBeh.independent);

                    gui.Space15();

                    if (monoBeh.independent)
                    {
                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Horizontal,
                            Body = () =>
                            {
                                DrawField(0, true);
                                DrawField(1, false);
                            }
                        });

                        gui.Space15();

                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Horizontal,
                            Body = () =>
                            {
                                DrawField(3, true);
                                DrawField(2, false);
                            }
                        });
                    }
                    else
                    {
                        gui.DrawGroup(new Group
                        {
                            GroupType = GroupType.Horizontal,
                            Body = () =>
                            {
                                Rect rect = DrawIcon(4);

                                int val = (int)monoBeh.radiiSerialized[0];
                                val = gui.IntField(null, val, 50);

                                gui.DragZoneInt(rect, ref val);

                                if (val < 0)
                                    val = 0;

                                monoBeh.radiiSerialized[0] = val;
                                monoBeh.radiiSerialized[1] = val;
                                monoBeh.radiiSerialized[2] = val;
                                monoBeh.radiiSerialized[3] = val;
                            }
                        });
                    }

                    monoBeh.Refresh();
                }
            });
        }

        private Rect DrawIcon(int index)
        {
            int iconOffset = 4;
            Rect rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(25), GUILayout.Height(25));
            rect.y -= iconOffset;

            Texture2D icon = null;
            switch (index)
            {
                case 0:
                    icon = cornerTopLeftIcon;
                    break;
                case 1:
                    icon = cornerTopRightIcon;
                    break;
                case 2:
                    icon = cornerBottomRightIcon;
                    break;
                case 3:
                    icon = cornerBottomLeftIcon;
                    break;
                case 4:
                    icon = cornerAllIcon;
                    break;
                default:
                    Debug.LogWarning("Invalid icon index.");
                    break;
            }

            EditorGUI.LabelField(rect, new GUIContent(icon));
            return rect;
        }

        private void DrawField(int index, bool drawAfterIcon)
        {
            int val = (int)monoBeh.radiiSerialized[index];

            if (!drawAfterIcon)
                val = gui.IntField(null, val, 50);

            Rect rect = DrawIcon(index);

            if (drawAfterIcon)
            {
                val = gui.IntField(null, val, 50);
                gui.FlexibleSpace();
            }

            gui.DragZoneInt(rect, ref val);

            if (val < 0)
                val = 0;

            monoBeh.radiiSerialized[index] = (float)val;
        }
    }
}