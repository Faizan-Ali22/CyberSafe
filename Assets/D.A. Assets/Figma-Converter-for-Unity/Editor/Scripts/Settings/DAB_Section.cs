#if DABUTTON_EXISTS
using DA_Assets.DAI;

namespace DA_Assets.FCU
{
    internal class DAB_Section : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        internal void Draw()
        {
            gui.TabHeader(FcuLocKey.label_dabutton_settings.Localize(), FcuLocKey.tooltip_dabutton_settings.Localize());
            gui.Space15();

            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject, 
                x => x.Settings.ButtonSettings.DAB_Settings.ScaleProperties);
            gui.Space15();
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject,
                x => x.Settings.ButtonSettings.DAB_Settings.ScaleAnimations);
            gui.Space15();
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject,
                x => x.Settings.ButtonSettings.DAB_Settings.ColorAnimations);
            gui.Space15();
            gui.SerializedPropertyField<FigmaConverterUnity>(scriptableObject.SerializedObject,
                x => x.Settings.ButtonSettings.DAB_Settings.SpriteAnimations);
        }
    }
}
#endif