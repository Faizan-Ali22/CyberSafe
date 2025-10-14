using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using DA_Assets.FCU.Extensions;
using UnityEngine;
using DA_Assets.Logging;
using DA_Assets.Extensions;
using System.Collections.Concurrent;

#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteDownloader : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private int _maxConcurrentDownloads = 100;
        private int _maxDownloadAttempts = 3;
        private float _maxChunkSize = 24_000_000;
        private int _errorLogSplitLimit = 50;
        private int _logDelayMs = 1000;
        private int _maxSpritesCount = 100;
        private Dictionary<ImageFormatScaleKey, List<List<SpriteData>>> idFormatChunks;

        public async Task CreateImageChunks(List<FObject> fobjects)
        {
            await Task.Run(() =>
            {
                List<FObject> needDownload = fobjects
                    .Where(x => x.Data.NeedDownload)
                    .GroupBy(x => x.Data.Hash)
                    .Select(g => g.First())
                    .ToList();

                idFormatChunks = GetIdFormatChunks(needDownload);
            });
        }

        public async Task DownloadSprites(List<FObject> fobjects)
        {
            DALogger.Log($"Download Sprites");

            List<FObject> needDownload = fobjects
                .Where(x => x.Data.NeedDownload)
                .GroupBy(x => x.Data.Hash)
                .Select(g => g.First())
                .ToList();

            if (needDownload.IsEmpty())
            {
                DALogger.Log($"DownloadSprites no need");
                return;
            }

            int totalCount = needDownload.Count;
            int downloadedCount = 0;
            int lastLoggedCount = -1;

            CancellationTokenSource downloadLogTokenSource = new CancellationTokenSource();

            var missingSpriteLinks = await GetSpriteLinks(needDownload);

            SemaphoreSlim semaphore = new SemaphoreSlim(_maxConcurrentDownloads);
            List<Task> tasks = new List<Task>();

            ConcurrentBag<FObject> failedObjects = new ConcurrentBag<FObject>();

            _ = Task.Run(async () =>
            {
                while (!downloadLogTokenSource.Token.IsCancellationRequested)
                {
                    if (lastLoggedCount != downloadedCount)
                    {
                        DALogger.Log(FcuLocKey.log_downloading_images.Localize(downloadedCount, totalCount));
                        lastLoggedCount = downloadedCount;
                    }

                    await Task.Delay(_logDelayMs, downloadLogTokenSource.Token);
                }
            }, downloadLogTokenSource.Token);

            DALogger.Log(FcuLocKey.log_start_download_images.Localize());

            foreach (var formatChunks in missingSpriteLinks)
            {
                foreach (var chunk in formatChunks.Value)
                {
                    foreach (var idFormatLink in chunk)
                    {
                        await semaphore.WaitAsync();

                        Task task = Task.Run(async () =>
                        {
                            try
                            {
                                bool success = await DownloadSprite(idFormatLink, _maxDownloadAttempts);

                                if (!success)
                                {
                                    failedObjects.Add(idFormatLink.FObject);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogException(ex);
                                failedObjects.Add(idFormatLink.FObject);
                            }
                            finally
                            {
                                Interlocked.Increment(ref downloadedCount);
                                semaphore.Release();
                            }
                        });

                        tasks.Add(task);
                    }
                }
            }

            await Task.WhenAll(tasks);

            downloadLogTokenSource.Cancel();

            DALogger.Log(FcuLocKey.log_downloading_images.Localize(downloadedCount, totalCount));

            LogFailedDownloads(failedObjects);
        }

        public async Task<bool> DownloadSprite(SpriteData idFormatLink, int maxDownloadAttempts)
        {
            try
            {
                if (idFormatLink.Link.IsEmpty())
                {
                    return false;
                }

                DARequest request = new DARequest
                {
                    RequestType = RequestType.GetFile,
                    Query = idFormatLink.Link
                };

                DAResult<byte[]> result = default;
                int attempts = 0;

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                {
                    while (attempts < maxDownloadAttempts && result.Object == null)
                    {
                        attempts++;
                        result = await monoBeh.RequestSender.SendRequest<byte[]>(request);
                    }
                }

                switch (result.Error.Status)
                {
                    case 909:
                        DALogger.LogError(FcuLocKey.log_ssl_error.Localize(result.Error.Message, result.Error.Status));
                        monoBeh.Events.OnImportFail?.Invoke(monoBeh);
                        monoBeh.AssetTools.StopImport(StopImportReason.Error);
                        break;
                }

                if (result.Object == null)
                {
                    throw new NullReferenceException();
                }

                File.WriteAllBytes(idFormatLink.FObject.Data.SpritePath, result.Object);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<Dictionary<ImageFormatScaleKey, List<List<SpriteData>>>> GetSpriteLinks(List<FObject> fobjects)
        {

            var idFormatLinkChunks = new Dictionary<ImageFormatScaleKey, List<List<SpriteData>>>();

            int totalLinks = fobjects.Count;
            int obtainedLinks = 0;
            int lastLoggedLinks = -1;

            CancellationTokenSource linkLogTokenSource = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                while (!linkLogTokenSource.Token.IsCancellationRequested)
                {
                    if (lastLoggedLinks != obtainedLinks)
                    {
                        DALogger.Log(FcuLocKey.log_getting_links.Localize(obtainedLinks, totalLinks));
                        lastLoggedLinks = obtainedLinks;
                    }

                    await Task.Delay(_logDelayMs, linkLogTokenSource.Token);
                }
            }, linkLogTokenSource.Token);

            foreach (var idFormatChunk in idFormatChunks)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    break;

                foreach (List<SpriteData> chunk in idFormatChunk.Value)
                {
                    IEnumerable<string> ids = chunk.Select(x => x.FObject.Id);

                    DARequest request = RequestCreator.CreateImageLinksRequest(
                        monoBeh.Settings.MainSettings.ProjectUrl,
                        idFormatChunk.Key.ImageFormat.ToLower(),
                        idFormatChunk.Key.Scale,
                        ids,
                        monoBeh.RequestSender.GetRequestHeader(monoBeh.Authorizer.Token));

                    DAResult<FigmaImageRequest> result = await monoBeh.RequestSender.SendRequest<FigmaImageRequest>(request);

                    if (result.Success && result.Object.images.IsEmpty())
                    {
                        Debug.LogError("result.Success && result.Object.images.IsEmpty()");
                    }
                    else if (result.Success)
                    {
                        if (!idFormatLinkChunks.ContainsKey(idFormatChunk.Key))
                        {
                            idFormatLinkChunks[idFormatChunk.Key] = new List<List<SpriteData>>();
                        }

                        List<SpriteData> linkChunk = new List<SpriteData>();

                        foreach (var idFormat in chunk)
                        {
                            result.Object.images.TryGetValue(idFormat.FObject.Id, out string link);

                            if (monoBeh.Settings.MainSettings.Https == false)
                            {
                                link = link.Replace("https://", "http://");
                            }

                            linkChunk.Add(new SpriteData
                            {
                                FObject = idFormat.FObject,
                                Format = idFormat.Format,
                                Link = link ?? string.Empty
                            });

                            Interlocked.Increment(ref obtainedLinks);
                        }

                        idFormatLinkChunks[idFormatChunk.Key].Add(linkChunk);
                    }
                    else
                    {
                        Debug.LogError(result.Error.Message);
                    }
                }
            }

            linkLogTokenSource.Cancel();

            DALogger.Log(FcuLocKey.log_getting_links.Localize(obtainedLinks, totalLinks));

            return idFormatLinkChunks;
        }

        public async Task SetScalesAndMaxSpriteSizes(List<FObject> fobjects)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(fobjects.Where(x => x.IsSprite()), fobject =>
                {
                    fobject.Data.MaxSpriteSize = GetMaxSpriteSize(fobject);
                    fobject.Data.Scale = GetMaxAllowedScale(fobject.Data.MaxSpriteSize, monoBeh.Settings.ImageSpritesSettings.MaxSpriteSize);
                });
            });
        }

        public Dictionary<ImageFormatScaleKey, List<List<SpriteData>>> GetIdFormatChunks(List<FObject> fobjects)
        {
            var formatChunks = new Dictionary<ImageFormatScaleKey, List<List<SpriteData>>>();

            // Group FObjects by their ImageFormat and Scale
            var fobjectsByFormatAndScale = new Dictionary<ImageFormatScaleKey, List<FObject>>();

            foreach (FObject fobject in fobjects.Where(x => x.IsSprite()))
            {
                ImageFormatScaleKey key = new ImageFormatScaleKey
                {
                    ImageFormat = fobject.Data.ImageFormat,
                    Scale = fobject.Data.Scale
                };

                if (!fobjectsByFormatAndScale.ContainsKey(key))
                {
                    fobjectsByFormatAndScale[key] = new List<FObject>();
                }

                fobjectsByFormatAndScale[key].Add(fobject);
            }

            // For each group (ImageFormat + Scale) create chunks
            foreach (var kvp in fobjectsByFormatAndScale)
            {
                ImageFormatScaleKey key = kvp.Key;
                List<FObject> fobjectList = kvp.Value;

                List<List<SpriteData>> chunks = new List<List<SpriteData>>();
                List<SpriteData> currentChunk = new List<SpriteData>();
                float currentChunkSize = 0;

                foreach (FObject fobject in fobjectList)
                {
                    float spriteSize = fobject.Data.MaxSpriteSize.x * fobject.Data.MaxSpriteSize.y * fobject.Data.Scale;
                    //Debug.LogError($"{spriteSize} | {fobject.Data.MaxSpriteSize.x * fobject.Data.Scale} | {fobject.Data.MaxSpriteSize.y * fobject.Data.Scale}");
                    // If adding this image exceeds the maximum chunk size, create a new chunk
                    if (currentChunkSize + spriteSize > _maxChunkSize || currentChunk.Count > _maxSpritesCount)
                    {
                        if (currentChunk.Count > 0)
                        {
                            chunks.Add(currentChunk);
                        }

                        currentChunk = new List<SpriteData>();
                        currentChunkSize = 0;
                    }

                    // Add the image to the current chunk
                    currentChunk.Add(new SpriteData
                    {
                        FObject = fobject,
                        Format = key.ImageFormat.ToString(),
                        Scale = key.Scale
                    });
                    currentChunkSize += spriteSize;
                }

                // Add the last chunk if it's not empty
                if (currentChunk.Count > 0)
                {
                    chunks.Add(currentChunk);
                }

                formatChunks[key] = chunks;
            }

            return formatChunks;
        }

        private void LogFailedDownloads(ConcurrentBag<FObject> failedObjects)
        {
            if (failedObjects.Count() > 0)
            {
                List<List<string>> comps = failedObjects.Select(x => x.Data.NameHierarchy).Split(_errorLogSplitLimit);

                foreach (List<string> comp in comps)
                {
                    string hierarchies = string.Join("\n", comp);

                    DALogger.LogError(
                        FcuLocKey.log_malformed_url.Localize(comp.Count, hierarchies));
                }
            }
        }

        private Vector2 GetMaxSpriteSize(FObject fobject)
        {
            float maxX;
            float maxY;

            bool hasBoundingSize = fobject.GetBoundingSize(out Vector2 bSize);
            bool hasRenderSize = fobject.GetRenderSize(out Vector2 rSize);

            if (hasBoundingSize && hasRenderSize)
            {
                maxX = Mathf.Max(bSize.x, rSize.x);
                maxY = Mathf.Max(bSize.y, rSize.y);
            }
            else if (hasRenderSize)
            {
                maxX = rSize.x;
                maxY = rSize.y;
            }
            else if (hasBoundingSize)
            {
                maxX = bSize.x;
                maxY = bSize.y;
            }
            else
            {
                maxX = fobject.Size.x;
                maxY = fobject.Size.y;
            }

            return new Vector2(maxX, maxY);
        }

        public float GetMaxAllowedScale(
            Vector2 imageSize,
            Vector2 maxSpriteSize,
            float minScale = FcuConfig.IMAGE_SCALE_MIN,
            float maxScale = FcuConfig.IMAGE_SCALE_MAX)
        {
            if (monoBeh.UsingSVG())
            {
                return monoBeh.Settings.ImageSpritesSettings.ImageScale;
            }

            float effectiveMaxSpriteWidth = Mathf.Min(maxSpriteSize.x, monoBeh.Settings.ImageSpritesSettings.MaxSpriteSize.x);
            float effectiveMaxSpriteHeight = Mathf.Min(maxSpriteSize.y, monoBeh.Settings.ImageSpritesSettings.MaxSpriteSize.y);

            float scaleX = effectiveMaxSpriteWidth / imageSize.x;
            float scaleY = effectiveMaxSpriteHeight / imageSize.y;
            float maxScaleBySpriteSize = Mathf.Min(scaleX, scaleY);
            maxScaleBySpriteSize = Mathf.Max(1f, maxScaleBySpriteSize);

            float maxScaleAllowed = Mathf.Clamp(maxScaleBySpriteSize, minScale, maxScale);
            maxScaleAllowed = (float)Math.Round(maxScaleAllowed, FcuConfig.Rounding.GetMaxAllowedScaleDigits);

            maxScaleAllowed = Mathf.Min(maxScaleAllowed, monoBeh.Settings.ImageSpritesSettings.ImageScale);
            //Debug.Log($"imageSize: {imageSize}, scaleX: {scaleX}, scaleY: {scaleY}, maxScaleBySpriteSize: {maxScaleBySpriteSize}, maxScaleAllowed: {maxScaleAllowed}");

            return maxScaleAllowed;
        }

        public struct FigmaImageRequest
        {
#if JSONNET_EXISTS
            [JsonProperty("err")]
#endif
            public string error;
#if JSONNET_EXISTS
            [JsonProperty("images")]
#endif
            // key = id, value = link
            public Dictionary<string, string> images;
        }
        public struct ImageFormatScaleKey
        {
            public ImageFormat ImageFormat { get; set; }
            public float Scale { get; set; }

            public override bool Equals(object obj)
            {
                if (!(obj is ImageFormatScaleKey))
                    return false;

                var other = (ImageFormatScaleKey)obj;
                return ImageFormat == other.ImageFormat && Scale.Equals(other.Scale);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + ImageFormat.GetHashCode();
                    hash = hash * 23 + Scale.GetHashCode();
                    return hash;
                }
            }
        }
        public struct SpriteData
        {
            public FObject FObject { get; set; }
            public string Format { get; set; }
            public string Link { get; set; }
            public float Scale { get; set; }
        }
    }
}
