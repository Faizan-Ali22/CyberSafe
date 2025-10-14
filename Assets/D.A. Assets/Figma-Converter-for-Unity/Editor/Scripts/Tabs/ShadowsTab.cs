using DA_Assets.DAI;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    internal class ShadowsTab : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.TabHeader(FcuLocKey.label_shadows_tab.Localize(), FcuLocKey.tooltip_shadows_tab.Localize());
            gui.Space15();

            monoBeh.Settings.ShadowSettings.ShadowComponent = gui.EnumField(
                new GUIContent(FcuLocKey.label_shadow_type.Localize(), FcuLocKey.tooltip_shadow_type.Localize()),
                monoBeh.Settings.ShadowSettings.ShadowComponent, false);

            gui.Space30();
        }
    }
}