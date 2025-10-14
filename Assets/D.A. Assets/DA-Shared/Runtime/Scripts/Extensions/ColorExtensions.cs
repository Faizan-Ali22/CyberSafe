using System;
using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class ColorExtensions
    {
        public static Color Lerp(this Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            var result = new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
            return result;
        }

        public static Color SetAlpha(this Color color, float? alpha)
        {
            return new Color(color.r, color.g, color.b, alpha == null ? 1 : alpha.ToFloat());
        }

        /// <summary>
        /// <para>Example: "#ff000099".ToColor() red with alpha ~50%</para>
        /// <para>Example: "ffffffff".ToColor() white with alpha 100%</para>
        /// <para>Example: "00ff00".ToColor() green with alpha 100%</para>
        /// <para>Example: "0000ff00".ToColor() blue with alpha 0%</para>
        /// <para><see href="https://github.com/smkplus/KamaliDebug"/></para>
        /// </summary>
        public static Color ToColor(this string color)
        {
            if (color.StartsWith("#", StringComparison.InvariantCulture))
            {
                color = color.Substring(1); // strip #
            }

            if (color.Length == 6)
            {
                color += "FF"; // add alpha if missing
            }

            uint hex = Convert.ToUInt32(color, 16);
            float r = ((hex & 0xff000000) >> 0x18) / 255f;
            float g = ((hex & 0xff0000) >> 0x10) / 255f;
            float b = ((hex & 0xff00) >> 8) / 255f;
            float a = (hex & 0xff) / 255f;

            return new Color(r, g, b, a);
        }
    }
}