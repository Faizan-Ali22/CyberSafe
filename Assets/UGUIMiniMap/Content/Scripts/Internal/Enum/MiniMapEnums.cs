using System;

namespace Lovatto.MiniMap
{
    public enum ItemEffect
    {
        Pulsing,
        Fade,
        None,
    }

    [Serializable]
    public enum MiniMapFullScreenMode
    {
        /// <summary>
        /// No fullscreen mode allowed
        /// </summary>
        NoFullScreen,

        /// <summary>
        /// Resize to a defined area in the screen
        /// </summary>
        ScreenArea,

        /// <summary>
        /// Auto scale to cover the whole screen
        /// </summary>
        ScaleToCoverScreen,

        /// <summary>
        /// Auto scale to fit in the screen
        /// </summary>
        ScaleToFitScreen,
    }
}