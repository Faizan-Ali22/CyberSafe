using System;

namespace DA_Assets.FCU
{
    public enum StopImportReason
    {
        Manual,
        Error,
        TaskCanceled,
        Hidden,
        End
    }

    [Flags]
    public enum ProceduralCondition
    {
        Sprite = 1 << 0,
        RectangleNoRoundedCorners = 1 << 1,
    }

    [Flags]
    public enum SvgCondition
    {
        ImageOrVideo = 1 << 0,
        AnyEffect = 1 << 1,
    }

    public enum PreserveRatioMode
    {
        None,
        WidthControlsHeight,
        HeightControlsWidth,
    }

    public enum FcuImageType
    {
        None,
        Downloadable,
        Drawable,
        Generative,
        Mask
    }

    public enum PositioningMode
    {
        Absolute = 0,
        GameView = 1
    }

    public enum UIFramework
    {
        UGUI = 0,
        UITK = 1,
        NOVA = 2
    }

    public enum ImageFormat
    {
        PNG = 0,
        JPG = 1,
        SVG = 2
    }

    public enum ImageComponent
    {
        UnityImage = 0,
        SubcShape = 1,
        MPImage = 2,
        ProceduralImage = 3,
        RawImage = 4,
        SpriteRenderer = 5,
        RoundedImage = 6,
        UIBlock2D = 7,
        SvgImage = 8,   
    }

    public enum TextComponent
    {
        UnityText = 0,
        TextMeshPro = 1,
        RTLTextMeshPro = 2
    }

    public enum ShadowComponent
    {
        Figma = 0,
        TrueShadow = 1
    }

    public enum ButtonComponent
    {
        UnityButton = 0,
        DAButton = 1
    }

    public enum LocalizationComponent
    {
        None = 0,
        DALocalizator = 1,
        I2Localization = 2,
    }

    public enum LocalizationKeyCaseType
    {
        snake_case = 0,
        UPPER_SNAKE_CASE = 1,
        PascalCase = 2,
    }
}