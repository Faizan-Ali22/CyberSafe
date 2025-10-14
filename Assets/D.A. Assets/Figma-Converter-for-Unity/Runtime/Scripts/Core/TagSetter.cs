using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class TagSetter : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private const int MAX_THREAD_COUNT = 10;
        private const int LOG_DELAY_MS = 1000;

        private int _outstandingTasks = 0;
        private int _currentConcurrency = 0;
        private int _processedElements = 0;
        private TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();

        public Dictionary<FcuTag, int> TagsCounter { get; set; } = new Dictionary<FcuTag, int>();

        public async Task SetTags(FObject page)
        {
            CancellationToken importToken = monoBeh.GetToken(TokenType.Import);

            DALogger.Log(FcuLocKey.log_tagging_by_parts.Localize(1));
            await SetTagsAsync(page, TagAlgorithm.Figma, importToken);

            DALogger.Log(FcuLocKey.log_tagging_by_parts.Localize(2));
            await SetTagsAsync(page, TagAlgorithm.Smart, importToken);

            DALogger.Log(FcuLocKey.log_tagging_by_parts.Localize(3));
            await SetTagsAsync(page, TagAlgorithm.Ignore, importToken);
        }


        private async Task SetTagsAsync(FObject root, TagAlgorithm tagAlgorithm, CancellationToken importToken)
        {
            _outstandingTasks = 0;
            _currentConcurrency = 0;
            _processedElements = 0;

            _tcs = new TaskCompletionSource<bool>();

            CancellationTokenSource logCTS = new CancellationTokenSource();

            _ = LogTagging(logCTS.Token, importToken);

            Interlocked.Increment(ref _outstandingTasks);
            TryProcessNode(root, tagAlgorithm, importToken);

            await _tcs.Task;

            logCTS.Cancel();
        }

        private async Task LogTagging(CancellationToken logToken, CancellationToken importToken)
        {
            DALogger.Log(FcuLocKey.log_tagging.Localize(_processedElements));

            try
            {
                while (!logToken.IsCancellationRequested && !importToken.IsCancellationRequested)
                {
                    await Task.Delay(LOG_DELAY_MS, logToken);
                    DALogger.Log(FcuLocKey.log_tagging.Localize(_processedElements));
                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                DALogger.Log(FcuLocKey.log_tagging.Localize(_processedElements));
            }
        }


        private void TryProcessNode(FObject node, TagAlgorithm tagAlgorithm, CancellationToken importToken)
        {
            if (TryIncrementConcurrency())
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    try
                    {
                        ProcessNode(node, tagAlgorithm, importToken);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _currentConcurrency);

                        if (Interlocked.Decrement(ref _outstandingTasks) == 0)
                        {
                            _tcs.TrySetResult(true);
                        }
                    }
                });
            }
            else
            {
                Task.Run(() =>
                {
                    try
                    {
                        ProcessNode(node, tagAlgorithm, importToken);
                    }
                    finally
                    {
                        if (Interlocked.Decrement(ref _outstandingTasks) == 0)
                        {
                            _tcs.TrySetResult(true);
                        }
                    }
                }, importToken);
            }
        }

        private void ProcessNode(FObject parent, TagAlgorithm tagAlgorithm, CancellationToken importToken)
        {
            if (importToken.IsCancellationRequested)
                return;

            if (parent.ContainsTag(FcuTag.Frame))
            {
                parent.Data.Hierarchy = new List<FcuHierarchy>
                {
                    new FcuHierarchy
                    {
                        Index = -1,
                        Name = parent.Data.Names.ObjectName,
                        Guid = parent.Data.Names.UitkGuid
                    }
                };
            }

            Interlocked.Increment(ref _processedElements);

            if (parent.Children == null)
                return;

            if (parent.Children.Count > FcuConfig.ChildParsingLimit && tagAlgorithm != TagAlgorithm.Figma)
                return;

            for (int i = 0; i < parent.Children.Count; i++)
            {
                FObject child = parent.Children[i];
                bool shouldProcess = false;

                switch (tagAlgorithm)
                {
                    case TagAlgorithm.Figma:
                        {
                            SetTagsByFigma(ref child, ref parent, i);
                            parent.Children[i] = child;
                            shouldProcess = !child.Children.IsEmpty();
                        }
                        break;
                    case TagAlgorithm.Smart:
                        {
                            SetSmartTags(ref child, ref parent);
                            shouldProcess = child.HasVisibleProperty(x => x.Children);
                        }
                        break;
                    case TagAlgorithm.Ignore:
                        {
                            SetIgnoredObjects(ref child, ref parent);
                            shouldProcess = child.HasVisibleProperty(x => x.Children);
                        }
                        break;
                }

                if (shouldProcess)
                {
                    Interlocked.Increment(ref _outstandingTasks);
                    TryProcessNode(child, tagAlgorithm, importToken);
                }
            }
        }

        private bool TryIncrementConcurrency()
        {
            while (true)
            {
                int current = _currentConcurrency;

                if (current >= MAX_THREAD_COUNT)
                    return false;

                if (Interlocked.CompareExchange(ref _currentConcurrency, current + 1, current) == current)
                    return true;
            }
        }

        private void SetTagsByFigma(ref FObject child, ref FObject parent, int index)
        {
            child.Data = new SyncData
            {
                Id = child.Id,
                ProjectId = monoBeh.Settings.MainSettings.ProjectUrl,
                ChildIndexes = new List<int>(),
                Parent = parent,
                Graphic = monoBeh.GraphicHelpers.GetGraphic(child)
            };

            if (child.Name.IsScrollContent())
            {
                child.Data.ForceContainer = true;
            }
            else if (child.Name.IsScrollViewport())
            {
                child.Data.ForceContainer = true;
            }

            monoBeh.NameSetter.SetNames(child);

            if (TryGetManualTags(child, out List<FcuTag> manualTags))
            {
                for (int i = 0; i < manualTags.Count; i++)
                {
                    child.AddTag(manualTags[i]);
                    FcuLogger.Debug($"GetManualTag {i} | {child.Name} | {manualTags[i]}", FcuDebugSettingsFlags.LogSetTag);
                }

                if (manualTags.Contains(FcuTag.Ignore))
                {
                    child.Data.IsEmpty = true;
                }
                else if (manualTags.Contains(FcuTag.Image))
                {
                    child.Data.ForceImage = true;
                }
                else if (manualTags.Contains(FcuTag.Container))
                {
                    child.Data.ForceContainer = true;
                }
            }

            child.Data.IsEmpty = IsEmpty(child);

            if (child.ContainsTag(FcuTag.Background))
            {
                child.AddTag(FcuTag.Image);
            }

            if (parent.ContainsTag(FcuTag.Page))
            {
                child.AddTag(FcuTag.Frame);
            }

            if (child.Type == NodeType.INSTANCE)
            {
                //TODO
            }

            if (child.LayoutWrap == LayoutWrap.WRAP ||
                child.LayoutMode == LayoutMode.HORIZONTAL ||
                child.LayoutMode == LayoutMode.VERTICAL)
            {
                if (child.HasVisibleProperty(x => x.Children))
                {
                    child.AddTag(FcuTag.AutoLayoutGroup);
                }
            }

            if (child.PreserveRatio.ToBoolNullFalse())
            {
                child.AddTag(FcuTag.AspectRatioFitter);
            }

            if (child.IsAnyMask())
            {
                child.AddTag(FcuTag.Mask);
            }

            if (child.Name.ToLower() == "button")
            {
                child.AddTag(FcuTag.Button);
            }

            if (child.Type == NodeType.TEXT)
            {
                child.AddTag(FcuTag.Text);

                if (child.Style.IsDefault() == false)
                {
                    if (child.Style.TextAutoResize == TextAutoResize.WIDTH_AND_HEIGHT || child.Style.TextAutoResize == TextAutoResize.HEIGHT)
                    {
                        child.AddTag(FcuTag.ContentSizeFitter);
                    }
                }
            }
            else if (child.Type == NodeType.VECTOR)
            {
                child.AddTag(FcuTag.Image);
            }
            else if (child.HasVisibleProperty(x => x.Fills) || child.HasVisibleProperty(x => x.Strokes))
            {
                child.AddTag(FcuTag.Image);
            }

            if (child.Effects.IsEmpty() == false)
            {
                Effect[] allShadows = child.Effects.Where(x => x.IsShadowType()).ToArray();

                if (monoBeh.IsUGUI() && monoBeh.UsingTrueShadow() && !monoBeh.UsingSpriteRenderer())
                {
                    if (allShadows.Length > 0)
                    {
                        child.AddTag(FcuTag.Shadow);
                    }
                }
                else if (monoBeh.IsUITK())
                {
                    if (allShadows.Length > 0)
                    {
                        //child.AddTag(FcuTag.Shadow);
                    }
                }
                else if (monoBeh.IsNova())
                {
                    if (allShadows.Length > 0)
                    {
                        child.AddTag(FcuTag.Shadow);
                    }
                }

                if (monoBeh.IsNova())
                {
                    foreach (Effect effect in child.Effects)
                    {
                        if (effect.Type == EffectType.BACKGROUND_BLUR)
                        {
                            child.AddTag(FcuTag.Blur);
                        }
                    }
                }
            }

            child.Data.IsOverlappedByStroke = IsOverlappedByStroke(child);

            if (child.Opacity.HasValue && child.Opacity != 1)
            {
                child.AddTag(FcuTag.CanvasGroup);
            }

            child.Data.Hierarchy.AddRange(parent.Data.Hierarchy);

            int sceneIndex = GetNewIndex(parent, index);
            child.Data.Hierarchy.Add(new FcuHierarchy
            {
                Index = sceneIndex,
                Name = child.Data.Names.ObjectName,
                Guid = child.Data.Names.UitkGuid,
            });
        }

        /// <returns>True, if continue.</returns>
        private bool SetSmartTags(ref FObject child, ref FObject parent)
        {
            string methodPath = $"{nameof(SetSmartTags)}";

            if (Is9slice(child))
            {
                child.AddTag(FcuTag.Slice9);
                child.AddTag(FcuTag.Image);
                child.Data.ForceImage = true;
                return true;
            }

            bool isInputFieldTextComponents = (parent.Name.IsInputTextArea() || parent.ContainsTag(FcuTag.InputField) || parent.ContainsTag(FcuTag.PasswordField)) && child.ContainsTag(FcuTag.Text);

            //TODO: check this.
            if (child.Data.IsEmpty && !isInputFieldTextComponents)
            {
                child.Data.TagReasons.Add(nameof(child.Data.IsEmpty));
                FcuLogger.Debug($"{methodPath} | {child.Data.TagReasons} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                return true;
            }

            if (child.Data.ForceImage)
            {
                ///If a component is tagged with the 'img' tag, it will downloaded as a single image,
                ///which means there is no need to look for child components for it.
                child.Data.TagReasons.Add(nameof(child.Data.ForceImage));
                FcuLogger.Debug($"{methodPath} | {child.Data.TagReasons} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                return true;
            }

            if (child.IsRootSprite(parent))
            {
                ///If the component is a vector that is at the root of your frame,
                ///then we recognize it as a single image and do not look for child components for it,
                ///because vectors do not have it.
                child.AddTag(FcuTag.Image);
                child.Data.ForceImage = true;

                child.Data.TagReasons.Add(nameof(TagExtensions.IsRootSprite));
                FcuLogger.Debug($"{methodPath} | {child.Data.TagReasons} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                return true;
            }

            if (monoBeh.Settings.MainSettings.RawImport == false)
            {
                bool hasButtonTags = child.ContainsCustomButtonTags();

                if (hasButtonTags)
                {
                    if (child.ContainsTag(FcuTag.Image))
                    {
                        if ((child.IsDownloadableType() || child.IsGenerativeType()) == false)
                        {
                            if (monoBeh.Settings.ButtonSettings.TransitionType == ButtonTransitionType.SpriteSwapForAll)
                            {
                                child.Data.ForceImage = true;
                                child.Data.TagReasons.Add(nameof(ButtonTransitionType.SpriteSwapForAll));
                                return true;
                            }
                        }
                    }
                }

                bool hasIcon = ContainsIcon(child);
                bool singleImage = CanBeSingleImage(child);
                FcuLogger.Debug($"{methodPath} | singleImage: {singleImage} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                if (hasIcon)
                {
                    child.Data.ForceContainer = true;
                    child.AddTag(FcuTag.Container);

                    child.Data.TagReasons.Add(nameof(ContainsIcon));
                    FcuLogger.Debug($"{methodPath} | {child.Data.TagReasons} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                }
                else if (singleImage && hasButtonTags)
                {
                    child.Data.ForceImage = true;
                    child.AddTag(FcuTag.Image);
                    child.RemoveNotDownloadableTags();

                    child.Data.TagReasons.Add(nameof(TagExtensions.ContainsCustomButtonTags));
                    FcuLogger.Debug($"{methodPath} | {child.Data.TagReasons} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                    return true;
                }
                else if (singleImage)
                {
                    ///If the component tree contains only vectors and/or components whose tags
                    ///have flag 'CanBeInsideSingleImage == false', recognize that component as a single image.
                    child.Data.ForceImage = true;
                    child.AddTag(FcuTag.Image);
                    child.RemoveNotDownloadableTags();

                    child.Data.TagReasons.Add(nameof(singleImage));
                    FcuLogger.Debug($"{methodPath} | {child.Data.TagReasons} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                    return true;
                }
                else if (child.Type == NodeType.BOOLEAN_OPERATION)
                {
                    child.Data.ForceImage = true;
                    child.AddTag(FcuTag.Image);

                    child.Data.TagReasons.Add(nameof(NodeType.BOOLEAN_OPERATION));
                    return true;
                }
                else
                {
                    FcuLogger.Debug($"{methodPath} | else | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                }
            }

            if (child.HasVisibleProperty(x => x.Children))
            {
                child.Data.TagReasons.Add("children not empty");
                FcuLogger.Debug($"{methodPath} | {child.Data.TagReasons} | {child.Data.NameHierarchy}", FcuDebugSettingsFlags.LogSetTag);
                child.AddTag(FcuTag.Container);
            }

            return false;
        }

        /// <summary>
        /// If the stroke is too thick relative to the height or width of the object, it overlaps the fill.
        /// In such a case, we do not download the image for this component, and use the stroke color as the fill.
        /// </summary>
        private bool IsOverlappedByStroke(FObject fobject)
        {
            bool blockedByStroke = false;

            if (fobject.HasVisibleProperty(x => x.Fills) && fobject.HasVisibleProperty(x => x.Strokes) && !fobject.ContainsTag(FcuTag.Shadow))
            {
                if (fobject.IndividualStrokeWeights.IsDefault())
                {
                    float twoSides = fobject.StrokeWeight * 2;

                    if (twoSides >= fobject.Size.y)
                    {
                        blockedByStroke = true;
                    }
                    else if (twoSides >= fobject.Size.x)
                    {
                        blockedByStroke = true;
                    }
                }
                else
                {
                    float topBottomStrokes = fobject.IndividualStrokeWeights.Top + fobject.IndividualStrokeWeights.Bottom;
                    float leftRightStrokes = fobject.IndividualStrokeWeights.Left + fobject.IndividualStrokeWeights.Right;

                    if (topBottomStrokes >= fobject.Size.y)
                    {
                        blockedByStroke = true;
                    }
                    else if (leftRightStrokes >= fobject.Size.x)
                    {
                        blockedByStroke = true;
                    }
                }
            }

            return blockedByStroke;
        }

        /// <summary>
        /// Retrieving the index of an element in the hierarchy, considering the <see cref="SyncData.IsEmpty"/> flag.
        /// </summary>
        private int GetNewIndex(FObject parent, int figmaIndex)
        {
            int count = 0;

            for (int i = 0; i < figmaIndex; i++)
            {
                FObject child = parent.Children[i];

                if (child.Data == null)
                {
                    break;
                }

                if (!child.Data.IsEmpty)
                {
                    count++;
                }
            }

            return count;
        }

        private bool Is9slice(FObject fobject)
        {
            if (fobject.Children.IsEmpty())
                return false;

            if (fobject.Children.Count != 9)
                return false;

            AnchorType child0 = fobject.Children[0].GetFigmaAnchor();
            AnchorType child1 = fobject.Children[1].GetFigmaAnchor();
            AnchorType child2 = fobject.Children[2].GetFigmaAnchor();
            AnchorType child3 = fobject.Children[3].GetFigmaAnchor();
            AnchorType child4 = fobject.Children[4].GetFigmaAnchor();
            AnchorType child5 = fobject.Children[5].GetFigmaAnchor();
            AnchorType child6 = fobject.Children[6].GetFigmaAnchor();
            AnchorType child7 = fobject.Children[7].GetFigmaAnchor();
            AnchorType child8 = fobject.Children[8].GetFigmaAnchor();

            if (child0 == AnchorType.TopLeft &&
                child1 == AnchorType.HorStretchTop &&
                child2 == AnchorType.TopRight &&
                child3 == AnchorType.VertStretchLeft &&
                child4 == AnchorType.StretchAll &&
                child5 == AnchorType.VertStretchRight &&
                child6 == AnchorType.BottomLeft &&
                child7 == AnchorType.HorStretchBottom &&
                child8 == AnchorType.BottomRight)
            {
                return true;
            }

            return false;
        }

        private void SetIgnoredObjects(ref FObject child, ref FObject parent)
        {
            if (child.Data.IsEmpty)
            {
                child.SetFlagToAllChilds(x => x.Data.IsEmpty = true);
                return;
            }

            if (child.Data.ForceImage)
            {
                child.SetFlagToAllChilds(x => x.Data.IsEmpty = true);
                return;
            }
        }

        /// <summary>
        /// Повертає true, якщо хоч один тег знайдено.
        /// </summary>
        internal bool TryGetManualTags(FObject fobject, out List<FcuTag> tags)
        {
            tags = new List<FcuTag>();

            if (fobject.Name.Contains(FcuConfig.RealTagSeparator) == false)
            {
                return false;
            }

            IEnumerable<FcuTag> fcuTags = Enum.GetValues(typeof(FcuTag))
               .Cast<FcuTag>()
               .Where(x => x != FcuTag.None);

            foreach (FcuTag fcuTag in fcuTags)
            {
                string[] tagParts = GetTagParts(fobject.Name);

                foreach (var tagPart in tagParts)
                {
                    if (FindManualTag(tagPart, fcuTag))
                    {
                        tags.Add(fcuTag);
                    }
                }
            }

            return tags.Count > 0;
        }

        private string[] GetTagParts(string name)
        {
            string tempName = name.ToLower().Replace(" ", "");

            string[] nameParts = tempName.Split(FcuConfig.RealTagSeparator);

            if (nameParts.Length > 0)
            {
                string tagPart = nameParts[0];
                string[] tagParts = tagPart.Split(",");
                return tagParts;
            }

            return new string[] { };
        }

        private bool FindManualTag(string tagPart, FcuTag fcuTag)
        {
            string figmaTag = fcuTag.GetTagConfig().FigmaTag.ToLower();

            if (figmaTag.IsEmpty())
                return false;

            string cleaned = Regex.Replace(tagPart, "[^a-z]", "");

            if (cleaned == figmaTag)
            {
                FcuLogger.Debug($"{nameof(FindManualTag)} | tagPart: {cleaned} | tag: {figmaTag}", FcuDebugSettingsFlags.LogSetTag);
                return true;
            }

            return false;
        }

        private bool ContainsIcon(FObject fobject)
        {
            if (fobject.Children.IsEmpty())
                return false;

            foreach (FObject item in fobject.Children)
            {
                if (item.Name.ToLower().Contains("icon"))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanBeSingleImage(FObject fobject)
        {
            if (fobject.Children.IsEmpty())
            {
                FcuLogger.Debug($"Reason: 1 - Object has no children. | {fobject.Data.NameHierarchy}");
                return false;
            }

            int count = 0;
            CanBeSingleImageRecursive(fobject, fobject, ref count);
            return count == 0;
        }

        private void CanBeSingleImageRecursive(FObject fobject, FObject parent, ref int count)
        {
            if (CanBeInsideSingleImage(fobject, parent) == false)
            {
                count++;
                return;
            }

            if (fobject.Children.IsEmpty())
                return;

            foreach (FObject child in fobject.Children)
                CanBeSingleImageRecursive(child, parent, ref count);
        }

        private bool CanBeInsideSingleImage(FObject fobject, FObject parent)
        {
            int reason = 0;

            if (fobject.Data.ForceContainer)
            {
                reason = 2;
                FcuLogger.Debug($"Reason: {reason} - ForceContainer is true. | Parent: {parent.Data.NameHierarchy}, Current: {fobject.Data.NameHierarchy}");
                return false;
            }

            if (fobject.Data.ForceImage)
            {
                reason = 3;
                FcuLogger.Debug($"Reason: {reason} - ForceImage is true. | Parent: {parent.Data.NameHierarchy}, Current: {fobject.Data.NameHierarchy}");
                return false;
            }

            foreach (FcuTag fcuTag in fobject.Data.Tags)
            {
                TagConfig tc = fcuTag.GetTagConfig();

                if (tc.CanBeInsideSingleImage == false)
                {
                    reason = 4;
                    FcuLogger.Debug($"Reason: {reason} - Tag {fcuTag} cannot be inside a single image. | Parent: {parent.Data.NameHierarchy}, Current: {fobject.Data.NameHierarchy}");
                    return false;
                }
            }

            return true;
        }


        private bool IsEmpty(FObject fobject)
        {
            int count = 0;
            IsEmptyRecursive(fobject, ref count);
            return count == 0;
        }

        private void IsEmptyRecursive(FObject fobject, ref int count)
        {
            if (count > 0)
                return;

            if (fobject.Opacity == 0)
                return;

            if (!fobject.IsVisible())
                return;

            if (fobject.ContainsTag(FcuTag.Ignore))
                return;

            if (fobject.IsZeroSize() && fobject.Type != NodeType.LINE)
                return;

            bool hasFills = !fobject.Fills.IsEmpty() && fobject.Fills.Any(x => x.IsVisible());
            bool hasStrokes = !fobject.Strokes.IsEmpty() && fobject.Strokes.Any(x => x.IsVisible());
            bool hasEffects = !fobject.Effects.IsEmpty() && fobject.Effects.Any(x => x.IsVisible());

            if (hasFills || hasStrokes || hasEffects || fobject.IsObjectMask())
            {
                count++;
                return;
            }

            if (!fobject.HasVisibleProperty(x => x.Children))
                return;

            foreach (FObject item in fobject.Children)
                IsEmptyRecursive(item, ref count);
        }

        public void CountTags(List<FObject> fobjects)
        {
            ConcurrentDictionary<FcuTag, ConcurrentBag<bool>> tagsCounter = new ConcurrentDictionary<FcuTag, ConcurrentBag<bool>>();

            Array fcuTags = Enum.GetValues(typeof(FcuTag));

            foreach (FcuTag tag in fcuTags)
            {
                tagsCounter.TryAdd(tag, new ConcurrentBag<bool>());
            }

            Parallel.ForEach(fobjects, fobject =>
            {
                if (fobject.Data.GameObject == null)
                {
                    return;
                }

                foreach (FcuTag tag in fobject.Data.Tags)
                {
                    tagsCounter[tag].Add(true);
                }
            });

            Dictionary<FcuTag, int> dictionary = tagsCounter.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Count
            );

            this.TagsCounter = dictionary;
        }

        enum TagAlgorithm
        {
            Figma,
            Smart,
            Ignore
        }
    }
}