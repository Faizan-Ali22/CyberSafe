using System;
using System.IO;
using UnityEngine;

namespace DA_Assets.Tools
{
    public static class LocalPrefs
    {
        private static readonly string prefsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LocalPrefs");

        static LocalPrefs()
        {
            if (!Directory.Exists(prefsFolderPath))
            {
                Directory.CreateDirectory(prefsFolderPath);
            }
        }

        private static string GetFilePath(string key)
        {
            return Path.Combine(prefsFolderPath, key);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            string path = GetFilePath(key);

            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                if (int.TryParse(content, out int result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        public static void SetInt(string key, int value)
        {
            string path = GetFilePath(key);
            File.WriteAllText(path, value.ToString());
        }

        public static string GetString(string key, string defaultValue = "")
        {
            string path = GetFilePath(key);
            return File.Exists(path) ? File.ReadAllText(path) : defaultValue;
        }

        public static void SetString(string key, string value)
        {
            string path = GetFilePath(key);
            File.WriteAllText(path, value);
        }
    }
}
