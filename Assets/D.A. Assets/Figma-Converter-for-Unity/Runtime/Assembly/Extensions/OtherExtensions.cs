using DA_Assets.Extensions;
using DA_Assets.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DA_Assets.FCU
{
    public static class OtherExtensions
    {
        public static bool TryParseSpriteName(this string spriteName, out float scale, out System.Numerics.BigInteger hash)
        {
            try
            {
                if (spriteName.IsEmpty())
                {
                    throw new Exception($"Sprite name is empty.");
                }

                char delimiter = ' ';

                string withoutEx = Path.GetFileNameWithoutExtension(spriteName);
                List<string> nameParts = withoutEx.Split(delimiter).ToList();

                if (nameParts.Count < 2)
                {
                    throw new Exception($"nameParts.Count < 2: {spriteName}");
                }

                string _hash = nameParts[nameParts.Count() - 1];
                string _scale = nameParts[nameParts.Count() - 2].Replace("x", "");

                bool scaleParsed = _scale.TryParseWithDot(out scale);
                bool hashParsed = System.Numerics.BigInteger.TryParse(_hash, out hash);

                if (scaleParsed == false)
                {
                    throw new Exception($"Cant parse scale from name: {spriteName}");
                }

                if (hashParsed == false)
                {
                    throw new Exception($"Cant parse hash from name: {spriteName}");
                }

                return true;
            }
            catch
            {
                scale = 1;
                hash = -1;
                return false;
            }
        }

        public static async Task WriteLog(this DARequest request, UnityHttpClient webRequest)
        {
            FileInfo[] fileInfos = new DirectoryInfo(FcuConfig.LogPath).GetFiles($"*.*");

            if (fileInfos.Length >= FcuConfig.LogFilesLimit)
            {
                foreach (FileInfo file in fileInfos)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {

                    }
                }
            }

            string logFileName = $"{DateTime.Now.ToString(FcuConfig.DateTimeFormat1)}_{FcuConfig.WebLogFileName}";
            string logFilePath = Path.Combine(FcuConfig.LogPath, logFileName);

            string result;

            string text = webRequest.downloadHandler.text;

            JFResult jfr = DAFormatter.Format<string>(text);

            if (jfr.IsValid)
            {
                result = jfr.Json;
            }
            else
            {
                result = text;
            }

            result = $"{request.Query}\n{webRequest.error}\n{result}";

            File.WriteAllText(logFilePath, result);

            await Task.Yield();
        }

        public static bool IsProjectEmpty(this SelectableFObject sf)
        {
            if (sf == null)
                return true;

            if (sf.Id.IsEmpty())
                return true;

            return false;
        }

        public static bool IsScrollContent(this string objectName)
        {
            if (objectName.IsEmpty())
                return false;

            objectName = objectName.ToLower();
            objectName = Regex.Replace(objectName, "[^a-z]", "");
            return objectName == "content";
        }

        public static bool IsScrollViewport(this string objectName)
        {
            if (objectName.IsEmpty())
                return false;

            objectName = objectName.ToLower();
            objectName = Regex.Replace(objectName, "[^a-z]", "");
            return objectName == "viewport";
        }

        public static bool IsInputTextArea(this string objectName)
        {
            if (objectName.IsEmpty())
                return false;

            objectName = objectName.ToLower();
            objectName = Regex.Replace(objectName, "[^a-z]", "");
            return objectName == "textarea";
        }

        public static bool IsCheckmark(this string objectName)
        {
            if (objectName.IsEmpty())
                return false;

            objectName = objectName.ToLower();
            objectName = Regex.Replace(objectName, "[^a-z]", "");
            return objectName == "checkmark";
        }

    }
}
