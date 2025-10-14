using DA_Assets.DAI;

namespace DA_Assets.FCU
{
    internal class MainTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            if (scriptableObject.Inspector.Header.MonoBeh == null)
            {
                scriptableObject.Close();
            }
            else
            {
                scriptableObject.Inspector.DrawGUI(null);
            }
        }
    }
}