using DA_Assets.Extensions;

namespace DA_Assets.FCU.Extensions
{
    public static class FontExtensions
    {
        public static string FormatFontName(this string value)
        {
            if (value.IsEmpty())
            {
                return "null";
            }

            return value
                .Replace("SDF", "")
                .ToLower()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("_", "");
        }
    }
}
