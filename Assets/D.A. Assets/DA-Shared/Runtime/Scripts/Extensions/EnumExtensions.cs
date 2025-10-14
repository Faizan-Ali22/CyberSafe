using System;

namespace DA_Assets.Extensions
{
    public static class EnumExtensions
    {
        public static bool Contains(this Enum value, string str)
        {
            return value.ToString().Contains(str);
        }

        public static string ToLower(this Enum value)
        {
            return value.ToString().ToLower();
        }

        public static string ToUpper(this Enum value)
        {
            return value.ToString().ToUpper();
        }
    }
}
