using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class FileExtensions
    {
        public static void OpenFolderInOS(this string path)
        {
            Application.OpenURL("file://" + path);
        }

        public static string GetPathRelativeToProjectDirectory(this string path)
        {
            char slash = path.Contains('/') ? '/' : '\\';

            string result = string.Join(slash.ToString(), path.Split(new char[1] { slash })
                .SkipWhile((string s) => s.ToLower() != "assets")
                .ToArray());

            return result;
        }

        /// <summary>
        /// Replaces text in a file.
        /// <para><see href="https://stackoverflow.com/a/58377834/8265642"/></para>
        /// </summary>
        /// <param name="filePath">Path of the text file.</param>
        /// <param name="searchText">Text to search for.</param>
        /// <param name="replaceText">Text to replace the search text.</param>
        static public void ReplaceInFile(string filePath, string searchText, string replaceText)
        {
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();

            content = Regex.Replace(content, searchText, replaceText);

            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        public static void CreateFolderIfNotExists(this string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool TryReadAllText(this string path, out string text)
        {
            try
            {
                text = File.ReadAllText(path);
                return true;
            }
            catch
            {
                text = null;
                return false;
            }
        }

        public static string GetFullAssetPath(this string assetPath)
        {
            //TODO: wrong logic when no image
            if (assetPath.IsEmpty())
            {
                return "";
            }

            string p2 = assetPath.Substring("Assets".Length + 1);
            string fullPath = Path.Combine(Application.dataPath, p2);
            return fullPath;
        }

        public static bool IsPathInsideAssetsPath(this string path)
        {
            if (path.IndexOf(Application.dataPath, System.StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// http://answers.unity.com/answers/787336/view.html
        /// </summary>
        public static string ToRelativePath(this string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }

            return absolutePath;
        }

        public static string RemoveInvalidCharsFromFileName(this string fileName, char repl = '_')
        {
            if(string.IsNullOrWhiteSpace(fileName))
                return fileName;

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
                fileName = fileName.Replace(c, repl);
            return fileName;
        }
    }
}