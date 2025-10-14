using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    public class ImageTypeSetter : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] ConcurrentBag<string> downloadableIds = new ConcurrentBag<string>();
        [SerializeField] ConcurrentBag<string> generativeIds = new ConcurrentBag<string>();
        [SerializeField] ConcurrentBag<string> drawableIds = new ConcurrentBag<string>();
        [SerializeField] ConcurrentBag<string> noneIds = new ConcurrentBag<string>();

        public ConcurrentBag<string> DownloadableIds => downloadableIds;
        public ConcurrentBag<string> GenerativeIds => generativeIds;
        public ConcurrentBag<string> DrawableIds => drawableIds;
        public ConcurrentBag<string> NoneIds => noneIds;

        public void ClearAllIds()
        {
            downloadableIds = new ConcurrentBag<string>();
            generativeIds = new ConcurrentBag<string>();
            drawableIds = new ConcurrentBag<string>();
            noneIds = new ConcurrentBag<string>();
        }

        private void SetImageFormat(FObject fobject)
        {
            ImageFormat imageFormat;

            if (monoBeh.UsingSvgImage())
            {
                if (fobject.CanUseUnityImage(monoBeh))
                {
                    imageFormat = ImageFormat.PNG;
                }
                else
                {
                    imageFormat = monoBeh.Settings.ImageSpritesSettings.ImageFormat;
                }
            }
            else
            {
                imageFormat = monoBeh.Settings.ImageSpritesSettings.ImageFormat;
            }

            fobject.Data.ImageFormat = imageFormat;
        }

        public async Task SetInsideDownloadableFlag(FObject fobject)
        {
            await Task.Run(() =>
            {
                SetInsideDownloadableFlagRecursive(fobject);
            }, monoBeh.GetToken(TokenType.Import));
        }

        private void SetInsideDownloadableFlagRecursive(FObject fobject)
        {
            fobject.Data.InsideDownloadable = fobject.IsInsideDownloadable(monoBeh, out FObject downloadableFObject);

            FcuLogger.Debug($"IsInsideDownloadable | {fobject.Data.InsideDownloadable} | {fobject.Data.NameHierarchy} | {downloadableFObject.Name}", FcuDebugSettingsFlags.LogHashGenerator);

            if (fobject.Children != null)
            {
                foreach (var child in fobject.Children)
                {
                    SetInsideDownloadableFlagRecursive(child);
                }
            }
        }

        public async Task SetImageTypes(List<FObject> fobjects)
        {
            DALogger.Log("SetImageTypes");

            downloadableIds = new ConcurrentBag<string>();
            generativeIds = new ConcurrentBag<string>();
            drawableIds = new ConcurrentBag<string>();
            noneIds = new ConcurrentBag<string>();

            await Task.Run(() =>
            {
                Parallel.ForEach(fobjects, fobject =>
                {
                    if (fobject.ContainsTag(FcuTag.Image) == false)
                        return;

                    SetImageFormat(fobject);

                    bool isDownloadable = IsDownloadable(fobject);
                    bool isGenerative = IsGenerative(fobject, isDownloadable);
                    bool isDrawable = IsDrawable(fobject);

                    if (fobject.Data.ForceImage)
                    {
                        fobject.Data.FcuImageType = FcuImageType.Downloadable;
                        downloadableIds.Add(fobject.Id);
                    }
                    else if (isGenerative)
                    {
                        fobject.Data.FcuImageType = FcuImageType.Generative;
                        generativeIds.Add(fobject.Id);
                    }
                    else if (isDownloadable)
                    {
                        fobject.Data.FcuImageType = FcuImageType.Downloadable;
                        downloadableIds.Add(fobject.Id);
                    }
                    else if (isDrawable)
                    {
                        fobject.Data.FcuImageType = FcuImageType.Drawable;
                        drawableIds.Add(fobject.Id);
                    }
                    else
                    {
                        fobject.Data.FcuImageType = FcuImageType.None;
                        noneIds.Add(fobject.Id);
                    }

                    FcuLogger.Debug($"SetImageType | {fobject.Data.NameHierarchy} | {fobject.Data.FcuImageType}", FcuDebugSettingsFlags.LogIsDownloadable);
                });

                FcuLogger.Debug($"SetImageType | {downloadableIds.Count} | {generativeIds.Count} | {drawableIds.Count} | {noneIds.Count}", FcuDebugSettingsFlags.LogIsDownloadable);
            }, monoBeh.GetToken(TokenType.Import));
        }

        private bool IsDownloadable(FObject fobject)
        {
            bool? result = null;
            string reason = "unknown";

            if (fobject.Data.IsEmpty)
            {
                reason = nameof(fobject.Data.IsEmpty);
                result = false;
            }
            else if (fobject.Data.ForceImage)
            {
                reason = nameof(fobject.Data.ForceImage);
                result = true;
            }
            else if (fobject.Type == NodeType.VECTOR)
            {
                reason = nameof(NodeType.VECTOR);
                result = true;
            }
            else if (fobject.IsMask.ToBoolNullFalse())
            {
                reason = nameof(fobject.IsMask);
                result = true;
            }
            else if (fobject.HaveUndownloadableTags(out string _reason1))
            {
                reason = _reason1;
                result = false;
            }
            else if (fobject.IsArcDataFilled())
            {
                reason = nameof(GraphicExtensions.IsArcDataFilled);
                result = true;
            }

            if (result == null)
            {
                bool? res = monoBeh.GraphicHelpers.IsDownloadableByFills(fobject, out string _reason2);

                if (res != null)
                {
                    reason = _reason2;
                    result = res;
                }
            }

            if (result == null)
            {
                if (!fobject.ContainsTag(FcuTag.Shadow))
                {
                    if (fobject.Effects.IsEmpty() == false)
                    {
                        int shadowCount = fobject.Effects.Count(x => x.IsVisible() && x.IsShadowType());

                        if (shadowCount > 0)
                        {
                            reason = "contains shadows";
                            result = true;
                        }
                    }
                }
            }

            if (result == null)
            {
                if (!fobject.ContainsTag(FcuTag.Blur))
                {
                    if (fobject.Effects.IsEmpty() == false)
                    {
                        int blurCount = fobject.Effects.Count(x => x.IsVisible() && x.IsBlurType());

                        if (blurCount > 0)
                        {
                            reason = "contains blur";
                            result = true;
                        }
                    }
                }
            }

            fobject.Data.DownloadableReason = reason;

            FcuLogger.Debug($"{nameof(IsDownloadable)} | {result} | {fobject.Data.NameHierarchy} | {reason}", FcuDebugSettingsFlags.LogIsDownloadable);
            return result.ToBoolNullFalse();
        }

        private bool IsGenerative(FObject fobject, bool isDownloadable)
        {
            bool? result = null;
            string reason = "not generative";

            FGraphic graphic = fobject.Data.Graphic;

            if (monoBeh.UsingSVG())
            {
                reason = nameof(ConfigExtensions.UsingSVG);
                result = false;
            }
            else if (monoBeh.UsingAnyProceduralImage())
            {
                reason = nameof(ConfigExtensions.UsingAnyProceduralImage);
                result = false;
            }
            else if (monoBeh.IsUITK())
            {
                reason = nameof(ConfigExtensions.IsUITK);
                result = false;
            }
            else if (monoBeh.IsNova())
            {
                reason = nameof(ConfigExtensions.IsNova);
                result = false;
            }
            else if (isDownloadable)
            {
                reason = nameof(isDownloadable);
                result = false;
            }
            else if (fobject.Data.IsOverlappedByStroke)
            {
                reason = nameof(fobject.Data.IsOverlappedByStroke);
                result = false;
            }
            else if (!fobject.Size.IsSupportedRenderSize(monoBeh.Settings.ImageSpritesSettings.ImageScale, out Vector2Int spriteSize, out Vector2Int _renderSize))
            {
                reason = $"render size is big: {spriteSize} x {_renderSize}";
                result = false;
            }
            else if (!fobject.IsRectangle())
            {
                reason = $"not rectangle";
                result = false;
            }
            else if (graphic.HasFill && graphic.HasStroke)
            {
                reason = $"hasFill && hasStroke";
                result = false;
            }
            else if (graphic.HasStroke)
            {
                reason = $"can generate stroke only";
                result = true;
            }
            else if (fobject.ContainsRoundedCorners())
            {
                reason = nameof(GraphicExtensions.ContainsRoundedCorners);
                result = true;
            }

            fobject.Data.GenerativeReason = reason;

            FcuLogger.Debug($"{nameof(IsGenerative)} | {result} | {fobject.Data.NameHierarchy} | {reason}", FcuDebugSettingsFlags.LogIsDownloadable);

            return result.ToBoolNullFalse();
        }

        private bool IsDrawable(FObject fobject)
        {
            bool result = true;
            string reason = "drawable";

            FcuLogger.Debug($"{nameof(IsDrawable)} | {result} | {fobject.Data.NameHierarchy} | {reason}", FcuDebugSettingsFlags.LogIsDownloadable);

            return result;
        }
    }
}