using DA_Assets.FCU.Model;
using DA_Assets.Extensions;
using System.Linq;
using UnityEngine;

#if TextMeshPro
using TMPro;
#endif

namespace DA_Assets.FCU.Extensions
{
    public static class TextExtensions
    {
        public static FontStyle GetFontWeight(this FObject fobject)
        {
            string fontStyleRaw = fobject.Style.FontPostScriptName;

            if (fontStyleRaw.IsEmpty() == false)
            {
                if (fontStyleRaw.Contains(FontStyle.Bold.ToString()))
                {
                    if (fobject.Style.Italic.ToBoolNullFalse())
                    {
                        return FontStyle.BoldAndItalic;
                    }
                    else
                    {
                        return FontStyle.Bold;
                    }
                }
                else if (fobject.Style.Italic.ToBoolNullFalse())
                {
                    return FontStyle.Italic;
                }
            }

            return FontStyle.Normal;
        }

        public static TextAnchor GetTextAnchor(this FObject fobject)
        {
            string textAligment = fobject.Style.TextAlignVertical + " " + fobject.Style.TextAlignHorizontal;

            switch (textAligment)
            {
                case "BOTTOM CENTER":
                    return TextAnchor.LowerCenter;
                case "BOTTOM LEFT":
                    return TextAnchor.LowerLeft;
                case "BOTTOM RIGHT":
                    return TextAnchor.LowerRight;
                case "CENTER CENTER":
                    return TextAnchor.MiddleCenter;
                case "CENTER LEFT":
                    return TextAnchor.MiddleLeft;
                case "CENTER RIGHT":
                    return TextAnchor.MiddleRight;
                case "TOP CENTER":
                    return TextAnchor.UpperCenter;
                case "TOP LEFT":
                    return TextAnchor.UpperLeft;
                case "TOP RIGHT":
                    return TextAnchor.UpperRight;
                default:
                    return TextAnchor.MiddleCenter;
            }
        }
#if TextMeshPro
        public static TextAlignmentOptions ToTextMeshAnchor(this TextAnchor textAnchor)
        {
            switch (textAnchor)
            {
                case TextAnchor.LowerCenter:
                    return TextAlignmentOptions.Bottom;
                case TextAnchor.LowerLeft:
                    return TextAlignmentOptions.BottomLeft;
                case TextAnchor.LowerRight:
                    return TextAlignmentOptions.BottomRight;
                case TextAnchor.MiddleCenter:
                    return TextAlignmentOptions.Center;
                case TextAnchor.MiddleLeft:
                    return TextAlignmentOptions.Left;
                case TextAnchor.MiddleRight:
                    return TextAlignmentOptions.Right;
                case TextAnchor.UpperCenter:
                    return TextAlignmentOptions.Top;
                case TextAnchor.UpperLeft:
                    return TextAlignmentOptions.TopLeft;
                case TextAnchor.UpperRight:
                    return TextAlignmentOptions.TopRight;
                default:
                    return TextAlignmentOptions.Center;
            }
        }
#endif

        public static string ToUITKAnchor(this TextAnchor textAnchor)
        {
            switch (textAnchor)
            {
                case TextAnchor.LowerCenter:
                    return "lower-center";
                case TextAnchor.LowerLeft:
                    return "lower-left";
                case TextAnchor.LowerRight:
                    return "lower-right";
                case TextAnchor.MiddleCenter:
                    return "middle-center";
                case TextAnchor.MiddleLeft:
                    return "middle-left";
                case TextAnchor.MiddleRight:
                    return "middle-right";
                case TextAnchor.UpperCenter:
                    return "upper-center";
                case TextAnchor.UpperLeft:
                    return "upper-left";
                case TextAnchor.UpperRight:
                    return "upper-right";
                default:
                    return "middle-center";
            }
        }

        public static string GetText(this GameObject go)
        {
#if TextMeshPro
            if (go.TryGetComponent(out TMP_Text tmpText))
            {
                return tmpText.text;
            }
#endif
            if (go.TryGetComponent(out UnityEngine.UI.Text unityText))
            {
                return unityText.text;
            }

            return null;
        }

        public static void SetText(this GameObject go, string text)
        {
            bool set = SetNovaText();
            set = SetTMPText();

            if (!set)
            {
                SetUnityText();
            }

            bool SetNovaText()
            {
#if NOVA_UI_EXISTS
                if (go.TryGetComponentSafe(out Nova.TextBlock textBlock))
                {
                    textBlock.Text = text;
                    return true;
                }
#endif
                return false;
            }

            bool SetTMPText()
            {
#if TextMeshPro
                if (go.TryGetComponentSafe(out TMP_Text tmpText))
                {
                    tmpText.text = text;
                    return true;
                }
#endif
                return false;
            }

            void SetUnityText()
            {
                if (go.TryGetComponent(out UnityEngine.UI.Text unityText))
                {
                    unityText.text = text;
                }
            }
        }

        /// <summary>
        /// https://drafts.csswg.org/css-fonts/#font-weight-numeric-values
        /// </summary>
        public static string FontWeightToString(this int weight)
        {
            switch (weight)
            {
                case 100: return "Thin";
                case 200: return "ExtraLight";
                case 300: return "Light";
                case 400: return "Regular";//Normal
                case 500: return "Medium";
                case 600: return "SemiBold";
                case 700: return "Bold";
                case 800: return "ExtraBold";
                case 900: return "Black";
            }

            return weight.ToString();
        }

        public static string FontNameToString(this FontMetadata fontMetadata,
            bool includeWeight = true,
            bool includeItalic = true,
            FontSubset? fontSubset = null,
            bool format = false)
        {
            string fullName = fontMetadata.Family;

            if (includeWeight)
                fullName += $"-{fontMetadata.Weight.FontWeightToString()}";

            if (includeItalic)
                if (fontMetadata.FontStyle == FontStyle.Italic)
                    fullName += $"-{FontStyle.Italic}";

            if (fontSubset != null)
                fullName += $"-{fontSubset}";

            if (format)
            {
                fullName = fullName.FormatFontName();
            }

            return fullName;
        }

        public static FontMetadata GetFontMetadata(this FObject fobject)
        {
            string family = null;
            int fontWeight = 0;
            FontStyle italic = FontStyle.Normal;

            if (!fobject.StyleOverrideTable.IsEmpty())
            {
                if (!fobject.StyleOverrideTable.First().Value.FontFamily.IsEmpty())
                {
                    family = fobject.StyleOverrideTable.First().Value.FontFamily;
                }

                if (fobject.StyleOverrideTable.First().Value.FontWeight > 0)
                {
                    fontWeight = fobject.StyleOverrideTable.First().Value.FontWeight;
                }

                italic = fobject.StyleOverrideTable.First().Value.Italic.ToBoolNullFalse() ? FontStyle.Italic : FontStyle.Normal;
            }

            if (family == null || fontWeight == 0)
            {
                family = fobject.Style.FontFamily;
                fontWeight = fobject.Style.FontWeight;
                italic = fobject.Style.Italic.ToBoolNullFalse() ? FontStyle.Italic : FontStyle.Normal;
            }

            FontMetadata fm = new FontMetadata
            {
                Family = family,
                Weight = fontWeight,
                FontStyle = italic,
            };

            return fm;
        }

        public static string FontNameToString(this FObject fobject,
            bool includeWeight = true,
            bool includeItalic = true,
            FontSubset? fontSubset = null,
            bool format = false)
        {
            FontMetadata fm = fobject.GetFontMetadata();
            string fn = fm.FontNameToString(includeWeight, includeItalic, fontSubset, format);
            return fn;
        }
    }
}