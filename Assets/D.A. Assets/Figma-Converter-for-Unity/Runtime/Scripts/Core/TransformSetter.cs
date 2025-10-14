using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

#if NOVA_UI_EXISTS
using Nova;
#endif

#pragma warning disable CS1998

namespace DA_Assets.FCU
{

    [Serializable]
    public class TransformSetter : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public float GetAbsoluteAngle(FObject fobject)
        {
            float totalAngle = 0;

            FObject current = fobject;

            while (true)
            {
                totalAngle += current.GetFigmaRotationAngle();

                if (!monoBeh.CurrentProject.TryGetParent(current, out current))
                {
                    break;
                }
            }

            return totalAngle;
        }

        public async Task SetTransformPos(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data?.RectGameObject == null)
                {
                    continue;
                }

                RectTransform rt = fobject.Data.RectGameObject.GetComponent<RectTransform>();
                rt.SetSmartAnchor(AnchorType.BottomLeft);
                rt.SetSmartPivot(PivotType.TopLeft);

                fobject.Data.FRect = GetGlobalRect(fobject);

                rt.sizeDelta = fobject.Data.FRect.size;
                rt.anchoredPosition = fobject.Data.FRect.position;

                rt.SetSmartPivot(PivotType.MiddleCenter);
                rt.SetRotation(fobject.Data.FRect.absoluteAngle);

                if (fobject.ContainsTag(FcuTag.Frame))
                {
                    rt.SetSmartAnchor(AnchorType.TopLeft);
                }
                else if (!fobject.Data.Parent.ContainsTag(FcuTag.AutoLayoutGroup))
                {
                    rt.SetSmartAnchor(fobject.GetFigmaAnchor());
                }

                rt.SetSmartPivot(monoBeh.Settings.MainSettings.PivotType);
            }
        }

        public FRect GetGlobalRect(FObject fobject)
        {
            FRect rect = new FRect();
            Vector2 position = new Vector2();
            Vector2 size = new Vector2();

            bool hasBoundingSize = fobject.GetBoundingSize(out Vector2 bSize);
            bool hasBoundingPos = fobject.GetBoundingPosition(out Vector2 bPos);

            bool hasRenderSize = fobject.GetRenderSize(out Vector2 rSize);
            bool hasRenderPos = fobject.GetRenderPosition(out Vector2 rPos);


            bool hasLocalPos = fobject.TryGetLocalPosition(out Vector2 lPos);

            rect.angle = fobject.Data.FcuImageType == FcuImageType.Downloadable ? 0 : fobject.GetFigmaRotationAngle();
            rect.absoluteAngle = fobject.Data.FcuImageType == FcuImageType.Downloadable ? 0 : GetAbsoluteAngle(fobject);

            bool uguiOrNova = monoBeh.IsUGUI() || monoBeh.IsNova();

            float scale = 1;
            bool hasScaleInName =
                !fobject.IsSvgExtension() &&
                fobject.IsDownloadableType() &&
                fobject.Data.SpritePath.TryParseSpriteName(out scale, out var _);

            int state = 0;

            if (hasScaleInName)
            {
                state = 1;

                position = rPos;
                size = new Vector2(fobject.Data.SpriteSize.x / scale, fobject.Data.SpriteSize.y / scale);
            }
            else if (rect.absoluteAngle != 0)
            {
                state = 2;

                size = fobject.Size;
                var offset = CalculateOffset(size.x, size.y, rect.absoluteAngle);
                position = new Vector2(bPos.x + offset.x, bPos.y + offset.y);
            }
            else
            {
                state = 3;

                size = bSize;
                position = bPos;
            }

            if (fobject.TryFixSizeWithStroke(size.y, out float newY))
            {
                size.y = newY;
            }

            FcuLogger.Debug($"{nameof(GetGlobalRect)} | {fobject.Data.NameHierarchy} | state: {state} | {size} | {position} | {rect.absoluteAngle}", FcuDebugSettingsFlags.LogTransform);

            rect.size = size;
            rect.position = new Vector2(position.x, (uguiOrNova ? -position.y : position.y));

            List<Vector2> childSizes = new List<Vector2>();
            foreach (int index in fobject.Data.ChildIndexes)
            {
                if (monoBeh.CurrentProject.TryGetByIndex(index, out FObject child))
                {
                    childSizes.Add(GetGlobalRect(child).size);
                }
            }

            rect.padding = GetPadding(fobject).AdjustPadding(size, childSizes.ToArray());

            return rect;
        }

        private static RectOffsetCustom GetPadding(FObject fobject)
        {
            return new RectOffsetCustom
            {
                bottom = (int)fobject.PaddingBottom.ToFloat().Round(FcuConfig.Rounding.GetPaddingDigits),
                top = (int)fobject.PaddingTop.ToFloat().Round(FcuConfig.Rounding.GetPaddingDigits),
                left = (int)fobject.PaddingLeft.ToFloat().Round(FcuConfig.Rounding.GetPaddingDigits),
                right = (int)fobject.PaddingRight.ToFloat().Round(FcuConfig.Rounding.GetPaddingDigits)
            };
        }

        public static Vector2 CalculateOffset(float width, float height, float angleDegrees)
        {
            // Convert the angle from degrees to radians
            float angleRadians = Mathf.Deg2Rad * angleDegrees;

            // Half-width and half-height of the rectangle
            float halfWidth = width / 2f;
            float halfHeight = height / 2f;

            // Calculate the new dimensions of the bounding box after rotation
            float rotatedWidth = Mathf.Abs(halfWidth * Mathf.Cos(angleRadians)) + Mathf.Abs(halfHeight * Mathf.Sin(angleRadians));
            float rotatedHeight = Mathf.Abs(halfWidth * Mathf.Sin(angleRadians)) + Mathf.Abs(halfHeight * Mathf.Cos(angleRadians));

            // Calculate the offset
            float deltaX = rotatedWidth - halfWidth;
            float deltaY = rotatedHeight - halfHeight;

            // Return the offset as a Vector2
            return new Vector2(deltaX, deltaY);
        }

        public async Task RestoreParentsRect(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data?.RectGameObject == null)
                {
                    continue;
                }

                if (!fobject.ContainsTag(FcuTag.Frame))
                {
                    if (fobject.Data.RectGameObject != null)
                    {
                        fobject.Data.RectGameObject.transform.SetParent(fobject.Data.ParentTransformRect);
                    }
                    else
                    {
                        fobject.Data.RectGameObject.transform.SetParent(monoBeh.transform);
                    }
                }
            }
        }

        public async Task RestoreParents(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data?.GameObject == null)
                {
                    continue;
                }

                if (fobject.Data.GameObject != null)
                {
                    fobject.Data.GameObject.transform.SetParent(fobject.Data.ParentTransform);
                }
                else
                {
                    fobject.Data.GameObject.transform.SetParent(monoBeh.transform);
                }
            }
        }

        internal async Task MoveUguiTransforms(List<FObject> currPage)
        {
            foreach (FObject fobject in currPage)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                if (fobject.Data.RectGameObject == null)
                    continue;

                fobject.Data.GameObject.TryAddComponent(out RectTransform goRt);
                fobject.Data.RectGameObject.TryGetComponentSafe(out RectTransform rectRt);

                goRt.CopyFrom(rectRt);
            }

            await Task.Yield();
        }

        internal void MoveNovaTransforms(List<FObject> currPage)
        {
            Transform tempParent = MonoBehExtensions.CreateEmptyGameObject(nameof(tempParent), monoBeh.transform).transform;

            foreach (FObject fobject in currPage)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                if (fobject.Data.RectGameObject == null)
                    continue;

                fobject.Data.RectGameObject.TryGetComponentSafe(out RectTransform rectRt);
                fobject.Data.UguiTransformData = UguiTransformData.Create(rectRt);

#if NOVA_UI_EXISTS
                if (fobject.ContainsTag(FcuTag.Text))
                {
                    fobject.Data.GameObject.TryAddComponent(out TextBlock textBlock);
                }
                else
                {
                    fobject.Data.GameObject.TryAddComponent(out UIBlock2D uiBlock2d);
                }

                UIBlock uiBlock = fobject.Data.GameObject.GetComponent<UIBlock>();
                uiBlock.Color = default;

                uiBlock.Layout.Size = new Length3
                {
                    X = fobject.Data.FRect.size.x,
                    Y = fobject.Data.FRect.size.y,
                };

                fobject.ExecuteWithTemporaryParent(tempParent, x => x.Data.GameObject, () =>
                {
                    SetFigmaRotation(fobject, fobject.Data.GameObject);
                });

                uiBlock.Layout.Position = new Length3
                {
                    X = fobject.Data.UguiTransformData.LocalPosition.x,
                    Y = fobject.Data.UguiTransformData.LocalPosition.y,
                };
#endif
            }

            tempParent.gameObject.Destroy();
        }

        public async Task SetNovaAnchors(List<FObject> fobjects)
        {
            int total = fobjects.Count;
            int processed = 0;

            IEnumerable<FrameGroup> fobjectsByFrame = fobjects
                .GroupBy(x => x.Data.RootFrame)
                .Select(g => new FrameGroup
                {
                    Childs = g.Select(x => x).ToList(),
                    RootFrame = g.First()
                });

            foreach (FrameGroup rootFrame in fobjectsByFrame)
            {
                if (rootFrame.RootFrame.Data.RectGameObject == null)
                    continue;

                _ = SetNovaAnchorsRoutine(rootFrame.Childs, () => processed++);
            }

            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(
                ref processed,
                ref total,
                FcuLocKey.log_set_anchors.Localize(processed, total),
                ref tempCount))
            {
                await Task.Delay(1000);
            }
        }

        private async Task SetNovaAnchorsRoutine(List<FObject> fobjects, Action onProcess)
        {
#if NOVA_UI_EXISTS
            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.GameObject == null)
                    continue;

                fobject.Data.GameObject.TryGetComponentSafe(out UIBlock uiBlock);
                await uiBlock.SetNovaAnchor(fobject.GetFigmaAnchor());

                onProcess.Invoke();
            }
#endif

            await Task.Yield();
        }

        internal async Task RestoreNovaFramePositions(List<FObject> fobjects)
        {
            IEnumerable<FrameGroup> fobjectsByFrame = fobjects
                .GroupBy(x => x.Data.RootFrame)
                .Select(g => new FrameGroup
                {
                    Childs = g.Select(x => x).ToList(),
                    RootFrame = g.First()
                });

            foreach (FrameGroup rootFrame in fobjectsByFrame)
            {
                if (rootFrame.RootFrame.Data.GameObject == null)
                    continue;

#if NOVA_UI_EXISTS
                rootFrame.RootFrame.Data.GameObject.TryGetComponentSafe(out UIBlock uiBlock);

                await uiBlock.SetNovaAnchor(AnchorType.TopLeft);
#endif
                await Task.Delay(100);

#if NOVA_UI_EXISTS
                uiBlock.Layout.Position = new Length3
                {
                    X = rootFrame.RootFrame.AbsoluteBoundingBox.X.ToFloat(),
                    Y = rootFrame.RootFrame.AbsoluteBoundingBox.Y.ToFloat(),
                };
#endif
            }
        }

        private void SetFigmaRotation(FObject fobject, GameObject target)
        {
            Transform rect = target.GetComponent<Transform>();
            rect.SetRotation(fobject.Data.FRect.absoluteAngle);
        }

        internal async Task SetSiblingIndex(List<FObject> fobjects)
        {
            foreach (var item in fobjects)
            {
                if (item.Data.GameObject == null)
                {
                    continue;
                }

                item.Data.GameObject.transform.SetSiblingIndex(item.Data.SiblingIndex);
            }
        }

        internal async Task SetStretchAllIfNeeded(List<FObject> fobjects)
        {
            if (monoBeh.Settings.MainSettings.PositioningMode == PositioningMode.GameView)
            {
                var frames = fobjects
                    .Where(x => x.ContainsTag(FcuTag.Frame));

                await Task.Yield();

                var frameSizeGroups = frames
                    .GroupBy(x => x.Size)
                    .Select(group => new
                    {
                        Size = group.Key,
                        Count = group.Count()
                    });

                await Task.Yield();

                var mostCommonSize = frameSizeGroups
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefault();

                if (mostCommonSize.Size.x > 0 && mostCommonSize.Size.y > 0)
                {
                    monoBeh.DelegateHolder.SetGameViewSize(mostCommonSize.Size);
                }

                foreach (FObject frame in frames)
                {
                    if (frame.Data.GameObject == null)
                        continue;

                    RectTransform rt = frame.Data.GameObject.GetComponent<RectTransform>();

                    rt.SetSmartAnchor(AnchorType.StretchAll);
                    rt.offsetMin = new Vector2(0, 0);
                    rt.offsetMax = new Vector2(0, 0);
                    rt.localScale = Vector3.one;
                }
            }
        }
    }
}