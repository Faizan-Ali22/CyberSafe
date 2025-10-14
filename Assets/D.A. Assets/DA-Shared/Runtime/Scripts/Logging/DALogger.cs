using System;

namespace DA_Assets.Logging
{
    public static class DALogger
    {
        public static string violetColor = "#8b00ff";

        public static void LogException(Exception ex)
        {
            UnityEngine.Debug.LogException(ex);
        }

        public static void LogError(string log)
        {
            log = SubstringSafe(log, 15000);
            UnityEngine.Debug.LogError(log);
        }

        public static void LogWarning(string log)
        {
            log = SubstringSafe(log, 15000);
            UnityEngine.Debug.LogWarning(log);
        }

        public static void Log(string log)
        {
            log = SubstringSafe(log, 15000);
            UnityEngine.Debug.Log(log.TextBold());
        }

        public static void LogSuccess(string log)
        {
            UnityEngine.Debug.Log(log.TextColor(violetColor).TextBold());
        }

        public static string SubstringSafe(string value, int maxLength)
        {
            return value?.Length > maxLength ? value.Substring(0, maxLength) : value;
        }
    }

    internal static class RichTextExtensions
    {
        /// <summary>
        /// <para><see href="https://forum.unity.com/threads/easy-text-format-your-debug-logs-rich-text-format.906464/"/></para>
        /// </summary>
        public static string TextBold(this string str) => "<b>" + str + "</b>";
        public static string TextColor(this string str, string clr) => string.Format("<color={0}>{1}</color>", clr, str);
        public static string TextItalic(this string str) => "<i>" + str + "</i>";
        public static string TextSize(this string str, int size) => string.Format("<size={0}>{1}</size>", size, str);
    }
}