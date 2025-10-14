using DA_Assets.FCU.Extensions;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DA_Assets.CssNet;
using System.Threading.Tasks;

#if TextMeshPro
using TMPro;
#endif

namespace DA_Assets.FCU
{
    [Serializable]
    public class TmpDownloader : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] int samplingPointSize = 90;
        [SerializeField] int atlasPadding = 5;
        [SerializeField] UnityEngine.TextCore.LowLevel.GlyphRenderMode renderMode = UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA;
        [SerializeField] int atlasWidth = 512;
        [SerializeField] int atlasHeight = 512;
#if TextMeshPro
        [SerializeField] AtlasPopulationMode atlasPopulationMode = AtlasPopulationMode.Dynamic;
#endif
        [SerializeField] bool enableMultiAtlasSupport = true;

        [Space]

        [SerializeField] UnityFonts unityTmpFonts;

        public async Task CreateFonts(List<FontMetadata> figmaFonts)
        {
#if TextMeshPro
            unityTmpFonts = monoBeh.FontDownloader.FindUnityFonts(figmaFonts, monoBeh.FontLoader.TmpFonts);
#endif
            if (unityTmpFonts.Missing.Count == 0)
            {
                await SetFallbackFontAssets(figmaFonts);
                return;
            }

            List<FontStruct> generated = new List<FontStruct>();
            List<FontError> notGenerated = new List<FontError>();

            foreach (FontStruct missingFont in unityTmpFonts.Missing)
            {
                _ = GenerateFont(missingFont, @return =>
                {
                    generated.Add(@return.Object);

                    if (@return.Success == false)
                    {
                        notGenerated.Add(new FontError
                        {
                            FontStruct = missingFont,
                            Error = @return.Error
                        });
                    }
                });
            }

            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(generated, unityTmpFonts.Missing, FcuLocKey.log_generating_tmp_fonts, ref tempCount))
            {
                await Task.Delay(1000);
            }

            if (notGenerated.Count > 0)
            {
                monoBeh.FontDownloader.PrintFontNames(FcuLocKey.cant_generate_fonts, notGenerated);
            }

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            await monoBeh.FontLoader.AddToTmpMeshFontsList();

            await SetFallbackFontAssets(figmaFonts);
        }

        private async Task SetFallbackFontAssets(List<FontMetadata> figmaFonts)
        {
#if TextMeshPro
            unityTmpFonts = monoBeh.FontDownloader.FindUnityFonts(figmaFonts, monoBeh.FontLoader.TmpFonts);

            if (unityTmpFonts.Existing.Count == 0)
                return;

            FontStruct latinFont = unityTmpFonts.Existing.FirstOrDefault(x => x.FontSubset == FontSubset.Latin);

            if (latinFont.IsDefault())
                return;

            TMP_FontAsset latinFontAsset = latinFont.Font as TMP_FontAsset;

            if (unityTmpFonts.Existing.IsEmpty())
                return;

            foreach (FontStruct fontStruct in unityTmpFonts.Existing)
            {
                if (fontStruct.FontSubset == FontSubset.Latin)
                    continue;

                TMP_FontAsset tmpFontAsset = fontStruct.Font as TMP_FontAsset;

                if (latinFontAsset.fallbackFontAssetTable.IsEmpty())
                {
                    latinFontAsset.fallbackFontAssetTable = new List<TMP_FontAsset>();
                }

                latinFontAsset.fallbackFontAssetTable.Add(tmpFontAsset);
            }

            if (latinFontAsset.fallbackFontAssetTable.IsEmpty() == false)
                latinFontAsset.fallbackFontAssetTable = latinFontAsset.fallbackFontAssetTable.Where(x => x != null).ToList();

            if (latinFontAsset.fallbackFontAssetTable.IsEmpty() == false)
                latinFontAsset.fallbackFontAssetTable = latinFontAsset.fallbackFontAssetTable.Distinct().ToList();
#endif
            await Task.Yield();
        }

        private async Task GenerateFont(FontStruct fs, Return<FontStruct> @return)
        {
#if UNITY_EDITOR
            string baseFontName = monoBeh.FontDownloader.GetBaseFileName(fs);

            string tmpPath = Path.Combine(monoBeh.FontLoader.TmpFontsPath, $"{baseFontName}.asset");


            Font ttfFont = null;

            foreach (Font item in monoBeh.FontLoader.TtfFonts)
            {
                if (baseFontName.FormatFontName() == item.name.FormatFontName())
                {
                    ttfFont = item;
                    break;
                }
            }

            if (ttfFont == null)
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = new WebError(0, $"Can't find ttf font"),
                    Success = false
                });

                return;
            }

            DAResult<string> unicodeRangeResult = await GetUnicodeRange(fs.FontMetadata.Family, fs.FontSubset);

            string unicodeRange = null;

            if (unicodeRangeResult.Success == false)
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = unicodeRangeResult.Error,
                    Success = false
                });

                return;
            }
            else
            {
                unicodeRange = unicodeRangeResult.Object;
            }

#if TextMeshPro
            try
            {
                TMP_FontAsset tmpFontAsset = TMP_FontAsset.CreateFontAsset(
                    ttfFont,
                    samplingPointSize,
                    atlasPadding,
                    renderMode,
                    atlasWidth,
                    atlasHeight,
                    atlasPopulationMode
#if UNITY_2020_1_OR_NEWER
                    , enableMultiAtlasSupport);
#else
                    );
#endif

                UnityEditor.AssetDatabase.CreateAsset(tmpFontAsset, tmpPath.ToRelativePath());

                tmpFontAsset.material.name = $"{baseFontName} Atlas Material";
                tmpFontAsset.atlasTexture.name = $"{baseFontName} Atlas";

                UnityEditor.AssetDatabase.AddObjectToAsset(tmpFontAsset.material, tmpFontAsset);
                UnityEditor.AssetDatabase.AddObjectToAsset(tmpFontAsset.atlasTexture, tmpFontAsset);

                tmpFontAsset.SetDirtyExt();

                UnityEditor.AssetDatabase.SaveAssets();

                tmpFontAsset.TryAddCharacters(unicodeRange, out string __);

                tmpFontAsset.SetDirtyExt();
                UnityEditor.AssetDatabase.SaveAssets();

                @return.Invoke(new DAResult<FontStruct>
                {
                    Object = fs,
                    Success = true
                });

            }
            catch (Exception)
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = new WebError(0, "Can't create font asset."),
                    Success = false
                });

                return;
            }
#endif
#endif
            await Task.Yield();
        }

        private async Task<DAResult<string>> GetUnicodeRange(string family, FontSubset fontSubset)
        {
            string url = $"https://fonts.googleapis.com/css2?family=" + family;

            DARequest request = new DARequest
            {
                RequestType = RequestType.Get,
                Query = url,
                RequestHeader = new RequestHeader
                {
                    Name = "User-Agent",
                    Value = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36"
                }
            };

            string css = (await monoBeh.RequestSender.SendRequest<string>(request)).Object;

            try
            {
                CssStylesheet ss = new CssStylesheet(css);

                string unicodeRange = null;

                foreach (CssSelector item in ss.selectors)
                {
                    string formatedName = item.Name
                            .Replace("}", "")
                            .Replace("{", "")
                            .Replace(" ", "")
                            .Replace("/*", "")
                            .Replace("*/", "")
                            .Replace("@font-face", "")
                            .Replace("-", "")
                            .ToLower()
                            .Trim();

                    if (formatedName == fontSubset.ToLower())
                    {
                        unicodeRange = item.rules.First(x => x.rule == "unicode-range").objectValue
                            .ToString()
                            .Replace("U+", "")
                            .Replace(" ", "")
                            .Trim();

                        break;
                    }
                }

                if (unicodeRange.IsEmpty())
                {
                    throw new Exception("Unicode range is empty");
                }

                return new DAResult<string>
                {
                    Success = true,
                    Object = unicodeRange
                };
            }
            catch (Exception ex)
            {
                return new DAResult<string>
                {
                    Success = false,
                    Error = new WebError(0, ex.Message)
                };
            }
        }

        public int SamplingPointSize { get => samplingPointSize; set => SetValue(ref samplingPointSize, value); }
        public int AtlasPadding { get => atlasPadding; set => SetValue(ref atlasPadding, value); }
        public UnityEngine.TextCore.LowLevel.GlyphRenderMode RenderMode { get => renderMode; set => SetValue(ref renderMode, value); }
        public int AtlasWidth { get => atlasWidth; set => SetValue(ref atlasWidth, value); }
        public int AtlasHeight { get => atlasHeight; set => SetValue(ref atlasHeight, value); }
#if TextMeshPro
        public AtlasPopulationMode AtlasPopulationMode { get => atlasPopulationMode; set => SetValue(ref atlasPopulationMode, value); }
#endif
        public bool EnableMultiAtlasSupport { get => enableMultiAtlasSupport; set => SetValue(ref enableMultiAtlasSupport, value); }
    }

    [Serializable]
    public struct UnityFonts
    {
        [SerializeField] List<FontStruct> missing;
        [SerializeField] List<FontStruct> existing;

        public List<FontStruct> Existing { get => existing; set => existing = value; }
        public List<FontStruct> Missing { get => missing; set => missing = value; }
    }
}