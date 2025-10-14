using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.Tools;
using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    internal class HeaderSection : MonoBehaviourLinkerEditor<FcuEditor, FigmaConverterUnity>
    {

        public void Draw()
        {
            gui.TopProgressBar(monoBeh.RequestSender.PbarProgress);

            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    gui.Space(18);
                    GUILayout.BeginVertical(scriptableObject.FcuLogo, gui.ColoredStyle.Logo);
                    gui.Space30();
                    GUILayout.EndVertical();
                    gui.Space(18);
                }
            });

            UpdateChecker.DrawVersionLabels(AssetType.fcu, FcuConfig.Instance.ProductVersion);
#if FCU_EXISTS && FCU_UITK_EXT_EXISTS
            UpdateChecker.DrawVersionLabels(AssetType.uitk_converter, FuitkConfig.Instance.ProductVersion);
#endif
            DrawImportInfoLine();
            DrawCurrentProjectName();
        }

        public void DrawSmallHeader()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Body = () =>
                {
                    DrawImportInfoLine();
                    DrawCurrentProjectName();
                }
            });
        }

        private string userId;
        private string userName;

        private void DrawImportInfoLine()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    gui.FlexibleSpace();
                    gui.Label10px(new GUIContent($"{Mathf.Round(monoBeh.RequestSender.PbarBytes / 1024)} kB", FcuLocKey.label_kilobytes.Localize()));
                    gui.Space5();
                    gui.Label10px(new GUIContent("—"));
                    gui.Space5();

                    userId = monoBeh.Authorizer.CurrentSession.User.Id.SubstringSafe(10);
                    userName = monoBeh.Authorizer.CurrentSession.User.Name;

                    bool isUserIdEmpty = string.IsNullOrWhiteSpace(userId);
                    bool isUserNameEmpty = string.IsNullOrWhiteSpace(userName);

                    if (isUserIdEmpty && isUserNameEmpty)
                    {
                        gui.Label10px(new GUIContent("Not logged."));
                        return;
                    }

                    if (!isUserNameEmpty)
                    {
                        gui.Label10px(new GUIContent(userName, FcuLocKey.label_user_name.Localize()));
                    }
                    else if (!isUserIdEmpty)
                    {
                        gui.Label10px(new GUIContent(userId, FcuLocKey.tooltip_user_id.Localize()));
                    }
                }
            });
        }

        private string currentProjectName;

        private void DrawCurrentProjectName()
        {
            currentProjectName = monoBeh.CurrentProject.ProjectName;

            if (currentProjectName != null)
            {
                gui.DrawGroup(new Group
                {
                    GroupType = GroupType.Horizontal,
                    Body = () =>
                    {
                        gui.FlexibleSpace();
                        gui.Label10px(currentProjectName);
                    }
                });
            }
        }
    }
}