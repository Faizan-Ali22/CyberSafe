using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    public class FontDownloader : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task DownloadFonts(List<FObject> fobjects)
        {
            if (FcuConfig.GoogleFontsApiKey.IsEmpty())
            {
                DALogger.Log(FcuLocKey.log_no_google_fonts_api_key.Localize());
                return;
            }

            List<FontMetadata> figmaFonts = GetFigmaProjectFonts(fobjects);

            monoBeh.FontLoader.RemoveDubAndMissingFonts();

            await monoBeh.FontLoader.AddToTmpMeshFontsList();
            await monoBeh.FontLoader.AddToTtfFontsList();

            await this.TtfDownloader.Download(figmaFonts);

            if (monoBeh.UsingTextMesh())
            {
                await this.TmpDownloader.CreateFonts(figmaFonts);
            }

            monoBeh.FontLoader.RemoveDubAndMissingFonts();
        }

        public UnityFonts FindUnityFonts<T>(List<FontMetadata> figmaFonts, List<T> fontArray) where T : UnityEngine.Object
        {
            UnityFonts uf = new UnityFonts
            {
                Existing = new List<FontStruct>(),
                Missing = new List<FontStruct>(),
            };

            foreach (FontMetadata figmaFont in figmaFonts)
            {
                foreach (FontSubset fontSubset in monoBeh.FontDownloader.GFontsApi.SelectedFontAssets)
                {
                    T _fontItem = null;

                    foreach (T fontItem in fontArray)
                    {
                        if (fontSubset == FontSubset.Latin)
                        {
                            if (figmaFont.FontNameToString(true, true, null, true) == fontItem.name.FormatFontName())
                            {
                                _fontItem = fontItem;
                                break;
                            }
                        }
                        else
                        {
                            if (figmaFont.FontNameToString(true, true, fontSubset, true) == fontItem.name.FormatFontName())
                            {
                                _fontItem = fontItem;
                                break;
                            }
                        }
                    }

                    if (_fontItem != null)
                    {
                        uf.Existing.Add(new FontStruct
                        {
                            FontMetadata = figmaFont,
                            FontSubset = fontSubset,
                            Font = _fontItem
                        });
                    }
                    else
                    {
                        uf.Missing.Add(new FontStruct
                        {
                            FontMetadata = figmaFont,
                            FontSubset = fontSubset,
                            Font = _fontItem
                        });
                    }
                }
            }

            return uf;
        }

        private List<FontMetadata> GetFigmaProjectFonts(List<FObject> fobjects)
        {
            HashSet<FontMetadata> fonts = new HashSet<FontMetadata>();

            foreach (FObject fobject in fobjects)
            {
                if (fobject.ContainsTag(FcuTag.Text) == false)
                    continue;

                FontMetadata fm = fobject.GetFontMetadata();
                fonts.Add(fm);
            }

            return fonts.ToList();
        }

        public async Task DownloadAllProjectFonts()
        {
            FObject virtualPage = new FObject
            {
                Id = FcuConfig.PARENT_ID,
                Children = monoBeh.CurrentProject.FigmaProject.Document.Children,
                Data = new SyncData
                {
                    Names = new FNames
                    {
                        ObjectName = FcuTag.Page.ToString(),
                    },
                    Tags = new List<FcuTag>
                    {
                        FcuTag.Page
                    }
                },
            };

            await monoBeh.TagSetter.SetTags(virtualPage);
            await monoBeh.ProjectImporter.ConvertTreeToListAsync(virtualPage, monoBeh.CurrentProject.CurrentPage);
            await monoBeh.FontDownloader.DownloadFonts(monoBeh.CurrentProject.CurrentPage);
        }

        public string GetBaseFileName(FontStruct fs)
        {
            string baseFontName;

            if (fs.FontSubset == FontSubset.Latin)
            {
                baseFontName = fs.FontMetadata.FontNameToString(true, true, null, false);
            }
            else
            {
                baseFontName = fs.FontMetadata.FontNameToString(true, true, fs.FontSubset, false);
            }

            return baseFontName;
        }

        internal void PrintFontNames(FcuLocKey locKey, List<FontError> fonts)
        {
            List<string> fontNames = new List<string>();

            foreach (var item in fonts)
            {
                string fontName;

                if (item.FontStruct.FontSubset == FontSubset.Latin)
                {
                    fontName = item.FontStruct.FontMetadata.FontNameToString(true, true, null, false);
                }
                else
                {
                    fontName = item.FontStruct.FontMetadata.FontNameToString(true, true, item.FontStruct.FontSubset, false);
                }

                string nameWithReason;

                if (item.Error.Message.IsEmpty() == false)
                {
                    nameWithReason = $"{fontName} - {item.Error.Message}";
                }
                else if (item.Error.Exception.IsDefault() == false)
                {
                    nameWithReason = $"{fontName} - {item.Error.Exception}";
                }
                else
                {
                    nameWithReason = fontName;
                }

                fontNames.Add(nameWithReason);
            }

            string joined = $"\n{string.Join("\n", fontNames)}";
            DALogger.Log(locKey.Localize(fontNames.Count, joined));
        }


        [SerializeField] TmpDownloader tmpDownloader;
        [SerializeProperty(nameof(tmpDownloader))]
        public TmpDownloader TmpDownloader => monoBeh.Link(ref tmpDownloader);

        [SerializeField] TtfDownloader ttfDownloader;
        [SerializeProperty(nameof(ttfDownloader))]
        public TtfDownloader TtfDownloader => monoBeh.Link(ref ttfDownloader);

        [SerializeField] DaGoogleFontsApi gFontsApi;
        [SerializeProperty(nameof(gFontsApi))]
        public DaGoogleFontsApi GFontsApi => monoBeh.Link(ref gFontsApi);
    }

    internal struct FontError
    {
        public FontStruct FontStruct { get; set; }
        public WebError Error { get; set; }
    }
}