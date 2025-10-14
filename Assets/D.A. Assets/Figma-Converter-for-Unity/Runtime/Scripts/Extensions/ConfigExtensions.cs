namespace DA_Assets.FCU.Extensions
{
    public static class ConfigExtensions
    {
        public static bool IsNova(this FigmaConverterUnity fcu) => fcu.Settings.MainSettings.UIFramework == UIFramework.NOVA;
        public static bool IsUITK(this FigmaConverterUnity fcu) => fcu.Settings.MainSettings.UIFramework == UIFramework.UITK;
        public static bool IsUGUI(this FigmaConverterUnity fcu) => fcu.Settings.MainSettings.UIFramework == UIFramework.UGUI;
        public static bool IsDebug(this FigmaConverterUnity fcu) => FcuDebugSettings.Settings.HasFlag(FcuDebugSettingsFlags.DebugMode);

        public static bool UsingAnyProceduralImage(this FigmaConverterUnity fcu) =>
             fcu.UsingJoshPui() || fcu.UsingDttPui() || fcu.UsingShapes2D() || fcu.UsingMPUIKit();

        public static bool UsingUnityButton(this FigmaConverterUnity fcu) =>
            fcu.Settings.ButtonSettings.ButtonComponent == ButtonComponent.UnityButton;

        public static bool UsingDAButton(this FigmaConverterUnity fcu) =>
            fcu.Settings.ButtonSettings.ButtonComponent == ButtonComponent.DAButton;

        public static bool UsingTrueShadow(this FigmaConverterUnity fcu) =>
            fcu.Settings.ShadowSettings.ShadowComponent == ShadowComponent.TrueShadow;

        public static bool UsingUnityText(this FigmaConverterUnity fcu) =>
            fcu.Settings.TextFontsSettings.TextComponent == TextComponent.UnityText;

        public static bool UsingTextMesh(this FigmaConverterUnity fcu) =>
            fcu.Settings.TextFontsSettings.TextComponent == TextComponent.TextMeshPro || fcu.Settings.TextFontsSettings.TextComponent == TextComponent.RTLTextMeshPro;

        public static bool UsingRTLTextMeshPro(this FigmaConverterUnity fcu) =>
            fcu.Settings.TextFontsSettings.TextComponent == TextComponent.RTLTextMeshPro;

        public static bool UsingSpriteRenderer(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.SpriteRenderer;

        public static bool UsingSvgImage(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.SvgImage;

        public static bool UsingSVG(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageFormat == ImageFormat.SVG;

        public static bool UsingShapes2D(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.SubcShape;

        public static bool UsingUnityImage(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.UnityImage;

        public static bool UsingRawImage(this FigmaConverterUnity fcu) =>
             fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.RawImage;

        public static bool UsingJoshPui(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.ProceduralImage;

        public static bool UsingDttPui(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.RoundedImage;

        public static bool UsingUIBlock2D(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.UIBlock2D;

        public static bool UsingMPUIKit(this FigmaConverterUnity fcu) =>
            fcu.Settings.ImageSpritesSettings.ImageComponent == ImageComponent.MPImage;

        public static bool UseImageLinearMaterial(this FigmaConverterUnity fcu)
        {
# if UNITY_EDITOR
            if (UnityEditor.PlayerSettings.colorSpace == UnityEngine.ColorSpace.Linear)
            {
                if (fcu.Settings.ImageSpritesSettings.UseImageLinearMaterial)
                {
                    return true;
                }
            }
#endif
            return false;
        }
    }
}