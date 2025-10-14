using DA_Assets.DAI;
using DA_Assets.FCU.Extensions;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class UnityImageSection : MonoBehaviourLinkerEditor<FcuSettingsWindow, FigmaConverterUnity>
    {
        public void Draw()
        {
            gui.SectionHeader(FcuLocKey.label_unity_image_settings.Localize());
            gui.Space15();

            if (monoBeh.UsingRawImage() == false)
            {
                monoBeh.Settings.UnityImageSettings.Type = gui.EnumField(new GUIContent(FcuLocKey.label_image_type.Localize(), ""),
                    monoBeh.Settings.UnityImageSettings.Type);
            }

            gui.Space10();

            monoBeh.Settings.UnityImageSettings.RaycastTarget = gui.Toggle(new GUIContent(FcuLocKey.label_raycast_target.Localize(), ""),
                monoBeh.Settings.UnityImageSettings.RaycastTarget);

            if (monoBeh.UsingRawImage() == false)
            {
                gui.Space10();

                monoBeh.Settings.UnityImageSettings.PreserveAspect = gui.Toggle(new GUIContent(FcuLocKey.label_preserve_aspect.Localize(), ""),
                    monoBeh.Settings.UnityImageSettings.PreserveAspect);
            }

            gui.Space10();

            monoBeh.Settings.UnityImageSettings.RaycastPadding = gui.Vector4Field(new GUIContent(FcuLocKey.label_raycast_padding.Localize(), ""),
                monoBeh.Settings.UnityImageSettings.RaycastPadding);

            gui.Space10();

            monoBeh.Settings.UnityImageSettings.Maskable = gui.Toggle(new GUIContent(FcuLocKey.label_maskable.Localize(), ""),
                monoBeh.Settings.UnityImageSettings.Maskable);
        }
    }
}