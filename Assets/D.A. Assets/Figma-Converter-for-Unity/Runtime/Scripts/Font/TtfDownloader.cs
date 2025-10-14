using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DA_Assets.FCU
{
    [Serializable]
    public class TtfDownloader : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task Download(List<FontMetadata> figmaFonts)
        {
            UnityFonts unityTtfFonts = monoBeh.FontDownloader.FindUnityFonts(figmaFonts, monoBeh.FontLoader.TtfFonts);

            if (unityTtfFonts.Missing.Count == 0)
                return;

            await monoBeh.FontDownloader.GFontsApi.GetGoogleFontsBySubset(monoBeh.FontDownloader.GFontsApi.FontSubsets);

            List<FontStruct> downloaded = new List<FontStruct>();
            List<FontError> notDownloaded = new List<FontError>();

            foreach (FontStruct missingFont in unityTtfFonts.Missing)
            {
                _ = DownloadFont(missingFont, @return =>
                {
                    downloaded.Add(@return.Object);

                    if (@return.Success == false)
                    {
                        notDownloaded.Add(new FontError
                        {
                            FontStruct = missingFont,
                            Error = @return.Error
                        });
                    }
                });
            }

            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(downloaded, unityTtfFonts.Missing, FcuLocKey.log_downloading_fonts, ref tempCount))
            {
                await Task.Delay(1000);
            }

            if (notDownloaded.Count > 0)
            {
                monoBeh.FontDownloader.PrintFontNames(FcuLocKey.cant_download_fonts, notDownloaded);
            }

            await SaveTtfFonts(downloaded);

            await monoBeh.FontLoader.AddToTtfFontsList();
        }

        public async Task DownloadFont(FontStruct missingFont, Return<FontStruct> @return)
        {
            FontItem fontItem = monoBeh.FontDownloader.GFontsApi.GetFontItem(missingFont.FontMetadata, missingFont.FontSubset);

            if (fontItem.IsDefault())
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = new WebError(0, "Font not found in Google Fonts"),
                    Success = false
                });

                return;
            }

            string fontUrl = monoBeh.FontDownloader.GFontsApi.GetUrlByWeight(
                fontItem,
                missingFont.FontMetadata.Weight,
                missingFont.FontMetadata.FontStyle);

            if (fontUrl.IsEmpty())
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = new WebError(0, $"'{missingFont.FontMetadata.Weight.FontWeightToString()}' weight not found in Google Fonts"),
                    Success = false
                });

                return;
            }

            DARequest request = new DARequest
            {
                RequestType = RequestType.GetFile,
                Query = fontUrl
            };

            DAResult<byte[]> @return2 = await monoBeh.RequestSender.SendRequest<byte[]>(request);

            if (return2.Success)
            {
                FontStruct res = missingFont;
                res.Bytes = return2.Object;

                @return.Invoke(new DAResult<FontStruct>
                {
                    Object = res,
                    Success = true
                });
            }
            else
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = return2.Error,
                    Success = false
                });
            }
        }

        public async Task SaveTtfFonts(List<FontStruct> downloadedFonts)
        {
#if UNITY_EDITOR
            foreach (FontStruct fs in downloadedFonts)
            {
                if (fs.Bytes == null || fs.Bytes.Length < 1)
                    continue;

                try
                {
                    string baseFontName = monoBeh.FontDownloader.GetBaseFileName(fs);
                    string ttfPath = Path.Combine(monoBeh.FontLoader.TtfFontsPath, $"{baseFontName}.ttf");
                    File.WriteAllBytes(ttfPath, fs.Bytes);
                }
                catch (Exception ex)
                {
                    DALogger.LogException(ex);
                }

                await Task.Yield();
            }

            UnityEditor.AssetDatabase.Refresh();
#endif
            await Task.Yield();
        }
    }
}