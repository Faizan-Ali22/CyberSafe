using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class AuthTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public override void OnLink()
        {
            base.OnLink();
            monoBeh.Authorizer.Init();
        }

        internal void Draw()
        {
            gui.TabHeader(FcuLocKey.label_figma_auth.Localize(), FcuLocKey.label_figma_auth.Localize());
            gui.Space15();

            if (monoBeh.Authorizer.Options.IsEmpty())
            {
                DrawButtons();
                return;
            }

            monoBeh.Authorizer.SelectedTableIndex = gui.BigDropdown(monoBeh.Authorizer.SelectedTableIndex, monoBeh.Authorizer.Options, (selected) =>
            {
                monoBeh.Authorizer.CurrentSession = monoBeh.Authorizer.RecentSessions[selected];
            }, expand: false);

            gui.Space10();

            DrawButtons();

            gui.Space10();

            if (gui.OutlineButton(new GUIContent("Delete all sessions")))
            {
                EditorPrefs.DeleteKey(FcuConfig.FIGMA_SESSIONS_PREFS_KEY);
                monoBeh.Authorizer.Init();
            }
        }

        private void DrawButtons()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (gui.OutlineButton(new GUIContent("Sign In With Web Browser")))
                    {
                        monoBeh.EventHandlers.Auth_OnClick();
                    }

                    gui.Space10();

                    if (gui.OutlineButton("Sign In With Access Token"))
                    {
                        _ = monoBeh.Authorizer.AddNew(new Model.AuthResult
                        {
                            AccessToken = monoBeh.Authorizer.Token
                        }, AuthType.Manual);
                    }

                    gui.Space10();

                    FigmaSessionItem session = monoBeh.Authorizer.CurrentSession;

                    string token = gui.BigTextField(                   
                        monoBeh.Authorizer.Token, 
                        null);

                    AuthResult authResult = session.AuthResult;
                    authResult.AccessToken = token;
                    session.AuthResult = authResult;
                    monoBeh.Authorizer.CurrentSession = session;

                    gui.FlexibleSpace();
                }
            });
        }
    }
}