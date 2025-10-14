using DA_Assets.Constants;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS1998

namespace DA_Assets.FCU
{
    [Serializable]
    public class NameSetter : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private ConcurrentDictionary<string, int> _fieldNames = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> _methodNames = new ConcurrentDictionary<string, int>();
        private ConcurrentDictionary<string, int> _classNames = new ConcurrentDictionary<string, int>();

        public void SetNames(FObject fobject)
        {
            FNames nsd = new FNames();

            nsd.FigmaName = fobject.Name;
            nsd.ObjectName = GetFcuName(fobject, FcuNameType.Object);
            nsd.UitkGuid = GetFcuName(fobject, FcuNameType.UitkGuid);
            nsd.LocKey = GetFcuName(fobject, FcuNameType.LocKey);

            nsd.FieldName = GetFcuName(fobject, FcuNameType.Field);
            nsd.MethodName = GetFcuName(fobject, FcuNameType.Method);
            nsd.ClassName = GetFcuName(fobject, FcuNameType.Class);

            nsd.HumanizedTextPrefabName = GetFcuName(fobject, FcuNameType.HumanizedTextPrefabName);

            fobject.Data.Names = nsd;
        }

        private string GetSpritePath(FObject fobject)
        {
            Sprite sprite = monoBeh.SpriteProcessor.GetSprite(fobject);
            string assetPath = URIHelpersRef.MakeAssetUri(sprite);
            return assetPath;
        }

        private string GetFontPath(FObject fobject)
        {
            Font font = monoBeh.FontLoader.GetFontFromArray(fobject, monoBeh.FontLoader.TtfFonts);

            FcuLogger.Debug($"UITK_Converter | Font | {fobject.Data.NameHierarchy} | {font}", FcuDebugSettingsFlags.LogDefault);
            string assetPath = URIHelpersRef.MakeAssetUri(font);
            return assetPath;
        }

        internal async Task Set_UITK_Names(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (!fobject.Data.SpritePath.IsEmpty())
                {
                    fobject.Data.Names.UITK_SpritePath = GetSpritePath(fobject);
                }
                else if (fobject.ContainsTag(FcuTag.Text))
                {
                    fobject.Data.Names.UITK_FontPath = GetFontPath(fobject);
                }
            }
        }

        internal async Task SetNames(List<FObject> fobjects, FcuNameType nameType)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(fobjects, fobject =>
                {
                    fobject.Data.Names.Names[nameType] = GetFcuName(fobject, nameType);
                });
            }, monoBeh.GetToken(TokenType.Import));
        }

        public string GetFcuName(FObject fobject, FcuNameType nameType, string overrideName = null)
        {
            string name = overrideName == null ? fobject.Name : overrideName;

            try
            {
                if (overrideName == null)
                {
                    BaseNameProcess(ref name);
                }

                switch (nameType)
                {
                    case FcuNameType.HumanizedTextPrefabName:
                        {
                            if (fobject.Type == NodeType.TEXT)
                            {
                                return monoBeh.NameHumanizer.GetHumanizedTextPrefabName(fobject);
                            }
                            else
                            {
                                return null;
                            }
                        }

                    case FcuNameType.UitkGuid:
                        {
                            name = Guid.NewGuid().ToString();
                            name = name.Replace("-", "");
                            return name;
                        }

                    case FcuNameType.Object:
                        {
                            if (monoBeh.IsUITK())
                            {
                                name = name.RemoveNonLettersAndNonNumbers("-");

                                if (name.Length > 0)
                                {
                                    if (char.IsDigit(name[0]))
                                    {
                                        name = "_" + name;
                                    }
                                }
                            }
                        }
                        break;

                    case FcuNameType.File:
                        {
                            if (fobject.IsSprite())
                            {
                                string fn = fobject.Name;
                                BaseNameProcess(ref fn);

                                name = "";
                                name += fn;
                                name += " ";
                                name += $"{fobject.Data.Scale.ToDotString()}x";
                                name += " ";
                                name += fobject.Data.Hash;
                                name += $".{fobject.Data.ImageFormat.ToLower()}";

                                return name;
                            }
                        }
                        break;

                    case FcuNameType.Field:
                    case FcuNameType.Method:
                    case FcuNameType.Class:
                        {
                            name = GetCsSharpName(name, nameType, fobject);
                            return name;
                        }

                    case FcuNameType.UssClass:
                        {
                            name = GetUssClassName(fobject);
                        }
                        break;

                    case FcuNameType.LocKey:
                        {
                            name = GetLocKey(name, fobject);

                            if (name == null)
                            {
                                return null;
                            }
                        }
                        break;
                }

                name = name.RemoveRepeatsForNonAlphanumericCharacters();
                name = name.Trim();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            bool nameRestored = TryRestoreName(name, fobject, out string restoredName);

            if (nameRestored)
            {
                name = GetFcuName(fobject, nameType, overrideName: restoredName);
            }
            else if (overrideName == null)
            {
                SubstringName(nameType, fobject, ref name);
            }

            return name;
        }

        private string GetLocKey(string name, FObject fobject)
        {
            int reason = 0;

            if (name.IsEmpty())
            {
                reason = 1;
            }
            else if (name.TryParseFloat(out var _))
            {
                reason = 2;
            }
            else if (DateTime.TryParse(name, out var _))
            {
                reason = 3;
            }
            else if (TimeSpan.TryParse(name, out var _))
            {
                reason = 4;
            }
            else if (name.IsValidEmail())
            {
                reason = 5;
            }
            else if (name.IsSpecialNumber())
            {
                reason = 6;
            }
            else if (name.IsPhoneNumber())
            {
                reason = 7;
            }
            else if (name.StartsWith("http://") || name.StartsWith("https://"))
            {
                reason = 8;
            }

            if (reason != 0)
            {
                //Debug.Log($"{nameof(GetFcuName)} | {fobject.Name} | {reason}");
                return null;
            }

            name = name.RemoveNonLettersAndNonNumbers("_");

            if (name.IsAllUpper())
            {
                switch (monoBeh.Settings.LocalizationSettings.LocKeyCaseType)
                {
                    case LocalizationKeyCaseType.snake_case:
                        name = name.ToLower();
                        break;
                    case LocalizationKeyCaseType.UPPER_SNAKE_CASE:
                        name = name.ToUpper();
                        break;
                    case LocalizationKeyCaseType.PascalCase:
                        name = name.ToPascalCase();
                        break;
                }
            }
            else
            {
                switch (monoBeh.Settings.LocalizationSettings.LocKeyCaseType)
                {
                    case LocalizationKeyCaseType.snake_case:
                        name = name.ToSnakeCase();
                        break;
                    case LocalizationKeyCaseType.UPPER_SNAKE_CASE:
                        name = name.ToUpperSnakeCase();
                        break;
                    case LocalizationKeyCaseType.PascalCase:
                        name = name.ToPascalCase();
                        break;
                }
            }

            return name;
        }

        private void BaseNameProcess(ref string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            string additionalCharsPattern = Regex.Escape(new string(monoBeh.Settings.MainSettings.AllowedNameChars));
            string pattern = $"[^a-zA-Z0-9{additionalCharsPattern}]";

            name = Regex.Replace(name, pattern, " ");
            name = name.RemoveInvalidCharsFromFileName(' ');

            while (!string.IsNullOrEmpty(name) && !Regex.IsMatch(name[0].ToString(), @"^[a-zA-Z]"))
            {
                name = name.Remove(0, 1);
            }
        }

        private string GetUssClassName(FObject fobject)
        {
            if (GetManualUssClassName(fobject, out string ussClass))
            {
                return ussClass;
            }
            else
            {
                string objectName = GetFcuName(fobject, FcuNameType.Object);
                return $"style-{objectName}-{fobject.Data.Hash}";
            }
        }

        private void SubstringName(FcuNameType nameType, FObject fobject, ref string name)
        {
            switch (nameType)
            {
                case FcuNameType.Field:
                    {
                        name = name.SubstringSafe(monoBeh.Settings.ScriptGeneratorSettings.FieldNameMaxLenght);
                    }
                    break;
                case FcuNameType.Method:
                    {
                        name = name.SubstringSafe(monoBeh.Settings.ScriptGeneratorSettings.MethodNameMaxLenght);
                    }
                    break;
                case FcuNameType.Class:
                    {
                        name = name.SubstringSafe(monoBeh.Settings.ScriptGeneratorSettings.ClassNameMaxLenght);
                    }
                    break;

                case FcuNameType.Object:
                case FcuNameType.File:
                case FcuNameType.HumanizedTextPrefabName:
                    {
                        if (fobject.Type == NodeType.TEXT)
                        {
                            name = name.SubstringSafe(monoBeh.Settings.MainSettings.TextObjectNameMaxLenght);
                        }
                        else
                        {
                            name = name.SubstringSafe(monoBeh.Settings.MainSettings.GameObjectNameMaxLenght);
                        }
                    }
                    break;

                case FcuNameType.LocKey:
                    {
                        name = name.SubstringSafe(monoBeh.Settings.LocalizationSettings.LocKeyMaxLenght);
                    }
                    break;

                case FcuNameType.UitkGuid:
                    break;

                case FcuNameType.UssClass:
                    break;

                case FcuNameType.UxmlPath:
                    break;
            }
        }

        /// <summary>
        /// For classes, methods, fields.
        /// </summary>
        private string GetCsSharpName(string name, FcuNameType nameType, FObject fobject)
        {
            char prefix = ' ';

            switch (nameType)
            {
                case FcuNameType.Field:
                    prefix = '_';
                    break;
                case FcuNameType.Method:
                    prefix = 'M';
                    break;
                case FcuNameType.Class:
                    prefix = 'C';
                    break;
            }

            try
            {
                name = name.RemoveNonLettersAndNonNumbers("_");
                name = name.Trim();
            }
            catch
            {

            }

            if (TryRestoreName(name, fobject, out string restoredName))
            {
                name = restoredName;
            }

            name = name.ToPascalCase();

            if (char.IsDigit(name[0]) || prefix == '_' || CsSharpKeywords.Keywords.Contains(name))
            {
                if (prefix == 'C' || prefix == 'M')
                {
                    name = prefix + name;
                    name = name.MakeCharUpper(2);
                }
                else
                {
                    name = prefix + name;
                    name = name.MakeCharLower(1);
                }
            }

            SubstringName(nameType, fobject, ref name);

            int number = 0;

            switch (nameType)
            {
                case FcuNameType.Field:
                    {
                        FindNameInDict(name, ref _fieldNames, out number);
                    }
                    break;
                case FcuNameType.Method:
                    {
                        FindNameInDict(name, ref _methodNames, out number);
                    }
                    break;
                case FcuNameType.Class:
                    {
                        if (_classNames.Count < 1)
                        {
                            int maxNumber = AssetTools.GetMaxFileNumber("Assets", name, "cs");

                            //Debug.Log($"FcuNameType.Class: {name} | GetMaxFileNumber: {maxNumber}");

                            if (maxNumber >= 0)
                            {
                                //TODO:
                                bool success = _classNames.TryAdd(name, maxNumber);
                            }
                        }

                        FindNameInDict(name, ref _classNames, out number);

                        //Debug.Log($"FcuNameType.Class: {name} | NewNumber: {number}");
                    }
                    break;
            }

            if (number > 0)
            {
                name = $"{name}_{number}";
            }

            return name;
        }

        private void FindNameInDict(string name, ref ConcurrentDictionary<string, int> dict, out int number)
        {
            if (dict.TryGetValue(name, out number))
            {
                dict[name] = number + 1;
                number = dict[name];
            }
            else
            {
                if (!dict.TryAdd(name, 0))
                {
                    //TODO:
                    //Debug.LogError($"FindNameInDict.TryAdd | {name} | false");
                }
            }
        }

        public void ClearNames()
        {
            _fieldNames.Clear();
            _methodNames.Clear();
            _classNames.Clear();
        }

        private bool TryRestoreName(string name, FObject fobject, out string restoredName)
        {
            restoredName = null;

            bool containsLatinLetters = !name.IsEmpty() && Regex.Matches(name, @"[a-zA-Z]").Count > 0;

            if (name.IsEmpty() || !containsLatinLetters)
            {
                string separator = monoBeh.IsUITK() ? "-" : " ";
                restoredName = $"{nameof(GameObject)}{separator}{fobject.Id.GetNumberOnly()}";
                return true;
            }

            return false;
        }

        public bool GetManualUssClassName(FObject fobject, out string className)
        {
            className = "";

            string input = fobject.Name;

            if (string.IsNullOrWhiteSpace(input) || !input.StartsWith(".") || !input.Contains($" {FcuConfig.RealTagSeparator} "))
            {
                return false;
            }

            int startIndex = 1;
            int endIndex = input.IndexOf($" {FcuConfig.RealTagSeparator} ");
            if (endIndex == -1)
            {
                return false;
            }

            className = input.Substring(startIndex, endIndex - startIndex);
            return true;
        }
    }
}