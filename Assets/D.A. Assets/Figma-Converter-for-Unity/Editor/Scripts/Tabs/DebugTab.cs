using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.Tools;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace DA_Assets.FCU
{
    internal class DebugTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        private Editor fcuConfigEditor;

        public override void OnLink()
        {
            fcuConfigEditor = Editor.CreateEditor(FcuConfig.Instance);
        }

        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_debug_tools.Localize());
            gui.Space15();

            FcuDebugSettingsFlags settings = FcuDebugSettings.Settings;
            EditorGUI.BeginChangeCheck();
            FcuDebugSettingsFlags newFlags = (FcuDebugSettingsFlags)EditorGUILayout.EnumFlagsField("Debug Settings", settings);
            if (EditorGUI.EndChangeCheck())
            {
                FcuDebugSettings.Settings = newFlags;
            }

            gui.Space10();

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (gui.OutlineButton("Open logs folder"))
                    {
                        FcuConfig.LogPath.OpenFolderInOS();
                    }

                    gui.Space10();

                    if (gui.OutlineButton("Open cache folder"))
                    {
                        FcuConfig.CachePath.OpenFolderInOS();
                    }

                    gui.Space10();

                    if (gui.OutlineButton("Open backup folder"))
                    {
                        SceneBackuper.GetBackupsPath().OpenFolderInOS();
                    }

                    gui.Space10();

                    if (gui.OutlineButton("Test Button"))
                    {
                        TestButton_OnClick();
                    }

                    gui.FlexibleSpace();
                }
            });

            if (monoBeh.IsDebug())
            {
                gui.Space30();

                gui.Colorize(() =>
                {
                    fcuConfigEditor.OnInspectorGUI();
                });

                if (scriptableObject.Inspector != null)
                {
                    gui.Space30();
                    scriptableObject.Inspector.DrawBaseOnInspectorGUI();
                }
            }
        }

        private void TestButton_OnClick()
        {
            //Debug.Log(Color.white);
            //UnityWebRequest request = UnityWebRequest.Get("http://google.com/generate_204");
        }
    }
}