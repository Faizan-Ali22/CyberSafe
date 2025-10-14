using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Extensions
{
    public static class AutoLayoutExtensions
    {
        public static bool TryFixSizeWithStroke(this FObject fobject, float currentY, out float newY)
        {
            newY = 0;

            if (currentY > 0)
                return false;

            if (fobject.Strokes.IsEmpty())
                return false;

            if (!fobject.Strokes.Any(x => x.Visible.ToBoolNullTrue()))
                return false;

            newY = fobject.StrokeWeight;
            return true;
        }

        public static bool IsInsideAutoLayout(this FObject parent, out HorizontalOrVerticalLayoutGroup layoutGroup)
        {
            layoutGroup = null;

            if (!parent.ContainsTag(FcuTag.AutoLayoutGroup))
                return false;

            if (parent.Data?.GameObject == null)
                return false;

            if (!parent.Data.GameObject.TryGetComponentSafe(out layoutGroup))
                return false;

            return true;
        }

        public static RectOffsetCustom AdjustPadding(this RectOffsetCustom padding, Vector2 parentSize, Vector2[] childSizes)
        {
            float totalHorizontalPadding = padding.left + padding.right;
            float totalVerticalPadding = padding.top + padding.bottom;

            float maxChildWidth = childSizes.IsEmpty() ? 0 : childSizes.Max(c => c.x);
            float maxChildHeight = childSizes.IsEmpty() ? 0 : childSizes.Max(c => c.y);

            if (maxChildWidth + totalHorizontalPadding > parentSize.x && totalHorizontalPadding != 0)
            {
                float excessWidth = (maxChildWidth + totalHorizontalPadding) - parentSize.x;

                float leftRatio = padding.left / totalHorizontalPadding;
                float rightRatio = padding.right / totalHorizontalPadding;

                padding.left -= Mathf.CeilToInt(leftRatio * excessWidth);
                padding.right -= Mathf.CeilToInt(rightRatio * excessWidth);
            }

            if (maxChildHeight + totalVerticalPadding > parentSize.y && totalVerticalPadding != 0)
            {
                float excessHeight = (maxChildHeight + totalVerticalPadding) - parentSize.y;

                float topRatio = padding.top / totalVerticalPadding;
                float bottomRatio = padding.bottom / totalVerticalPadding;

                padding.top -= Mathf.CeilToInt(topRatio * excessHeight);
                padding.bottom -= Mathf.CeilToInt(bottomRatio * excessHeight);
            }

            return padding;
        }


        public static TextAnchor GetHorLayoutAnchor(this FObject fobject)
        {
            string aligment = "";
            aligment += fobject.PrimaryAxisAlignItems;
            aligment += " ";
            aligment += fobject.CounterAxisAlignItems;

            switch (aligment)
            {
                case "NONE NONE":
                    return TextAnchor.UpperLeft;
                case "SPACE_BETWEEN NONE":
                    return TextAnchor.UpperCenter;
                case "CENTER NONE":
                    return TextAnchor.UpperCenter;
                case "MAX NONE":
                    return TextAnchor.UpperRight;
                case "NONE CENTER":
                    return TextAnchor.MiddleLeft;
                case "NONE BASELINE":
                    return TextAnchor.MiddleLeft;
                case "SPACE_BETWEEN CENTER":
                    return TextAnchor.MiddleCenter;
                case "CENTER CENTER":
                    return TextAnchor.MiddleCenter;
                case "CENTER BASELINE":
                    return TextAnchor.MiddleCenter;
                case "MAX CENTER":
                    return TextAnchor.MiddleRight;
                case "MAX BASELINE":
                    return TextAnchor.MiddleRight;
                case "NONE MAX":
                    return TextAnchor.LowerLeft;
                case "SPACE_BETWEEN MAX":
                    return TextAnchor.LowerCenter;
                case "CENTER MAX":
                    return TextAnchor.LowerCenter;
                case "MAX MAX":
                    return TextAnchor.LowerRight;
            }

            DALogger.LogError(FcuLocKey.log_unknown_aligment.Localize(aligment, fobject.Data.NameHierarchy));
            return TextAnchor.UpperLeft;
        }

        public static TextAnchor GetVertLayoutAnchor(this FObject fobject)
        {
            string aligment = "";
            aligment += fobject.PrimaryAxisAlignItems;
            aligment += " ";
            aligment += fobject.CounterAxisAlignItems;

            switch (aligment)
            {
                case "NONE NONE":
                    return TextAnchor.UpperLeft;
                case "NONE CENTER":
                    return TextAnchor.UpperCenter;
                case "NONE MAX":
                    return TextAnchor.UpperRight;
                case "CENTER NONE":
                    return TextAnchor.MiddleLeft;
                case "SPACE_BETWEEN NONE":
                    return TextAnchor.MiddleLeft;
                case "CENTER CENTER":
                    return TextAnchor.MiddleCenter;
                case "SPACE_BETWEEN CENTER":
                    return TextAnchor.MiddleCenter;
                case "CENTER MAX":
                    return TextAnchor.MiddleRight;
                case "SPACE_BETWEEN MAX":
                    return TextAnchor.MiddleRight;
                case "MAX NONE":
                    return TextAnchor.LowerLeft;
                case "MAX CENTER":
                    return TextAnchor.LowerCenter;
                case "MAX MAX":
                    return TextAnchor.LowerRight;
            }

            DALogger.LogError(FcuLocKey.log_unknown_aligment.Localize(aligment, fobject.Data.NameHierarchy));
            return TextAnchor.UpperLeft;
        }

#if UNITY_2021_3_OR_NEWER
        public static (string alignItems, string justifyContent) ToUITK(this TextAnchor textAnchor, bool isHorizontal = true)
        {
            // Base mapping for horizontal layout
            var (primary, secondary) = textAnchor switch
            {
                TextAnchor.UpperLeft => ("flex-start", "flex-start"),
                TextAnchor.UpperCenter => ("flex-start", "center"),
                TextAnchor.UpperRight => ("flex-start", "flex-end"),

                TextAnchor.MiddleLeft => ("center", "flex-start"),
                TextAnchor.MiddleCenter => ("center", "center"),
                TextAnchor.MiddleRight => ("center", "flex-end"),

                TextAnchor.LowerLeft => ("flex-end", "flex-start"),
                TextAnchor.LowerCenter => ("flex-end", "center"),
                TextAnchor.LowerRight => ("flex-end", "flex-end"),
                _ => throw new NotImplementedException(),
            };

            // Swap values for vertical layout
            return isHorizontal ? (primary, secondary) : (secondary, primary);
        }
#endif
        public static (bool childControlWidth, bool childControlHeight) GetChildControlByLayoutMode(this FObject fobject, LayoutMode layoutMode)
        {
            bool childControlWidth = false;
            bool childControlHeight = false;

            HashSet<float?> layoutGrows = new HashSet<float?>();
            HashSet<LayoutAlign> layoutAligns = new HashSet<LayoutAlign>();

            foreach (FObject child in fobject.Children)
            {
                layoutGrows.Add(child.LayoutGrow);
                layoutAligns.Add(child.LayoutAlign);
            }

            if (layoutGrows.Count == 1)
            {
                if (layoutGrows.First() == 1)
                {
                    if (layoutMode == LayoutMode.VERTICAL)
                    {
                        childControlHeight = true;
                    }
                    else if (layoutMode == LayoutMode.HORIZONTAL)
                    {
                        childControlWidth = true;
                    }
                }
            }

            if (layoutAligns.Count == 1)
            {
                if (layoutAligns.First() == LayoutAlign.STRETCH)
                {
                    if (layoutMode == LayoutMode.VERTICAL)
                    {
                        childControlWidth = true;
                    }
                    else if (layoutMode == LayoutMode.HORIZONTAL)
                    {
                        childControlHeight = true;
                    }
                }
            }

            return (childControlWidth, childControlHeight);
        }

        public static float GetHorSpacing(this FObject fobject)
        {
            float result = 0f;
            int state = 0;

            if (fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.SPACE_BETWEEN)
            {
                if (fobject.Data.ChildIndexes.IsEmpty())
                {
                    state = 1;
                    result = 0;
                }
                else if (fobject.Data.ChildIndexes.Count == 1)
                {
                    state = 2;
                    result = 0;
                }
                else
                {
                    state = 3;
                    int childCount = fobject.Data.ChildIndexes.Count;
                    int spacingCount = childCount - 1;
                    float parentWidth = fobject.Data.FRect.size.x;

                    float allChildsWidth = 0;

                    foreach (FObject child in fobject.Children)
                    {
                        allChildsWidth += child.Data.FRect.size.x;
                    }

                    float spacingWidth = (parentWidth - allChildsWidth) / spacingCount;
                    result = spacingWidth;
                }
            }
            else
            {
                state = 4;
                result = fobject.ItemSpacing.ToFloat();
            }

            FcuLogger.Debug($"{nameof(GetHorSpacing)} | {state} | {result} | {fobject.Data.NameHierarchy}");

            return result;
        }

        public static float GetVertSpacing(this FObject fobject)
        {
            if (fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.SPACE_BETWEEN)
            {
                if (fobject.Data.ChildIndexes.IsEmpty())
                {
                    return 0;
                }
                else if (fobject.Data.ChildIndexes.Count == 1)
                {
                    return 0;
                }
                else
                {
                    int childCount = fobject.Data.ChildIndexes.Count;
                    int spacingCount = childCount - 1;
                    float parentHeight = fobject.Data.FRect.size.y;

                    float allChildsHeight = 0;

                    foreach (FObject child in fobject.Children)
                    {
                        allChildsHeight += child.Data.FRect.size.y;
                    }

                    float spacingWidth = (parentHeight - allChildsHeight) / spacingCount;
                    return spacingWidth;
                }
            }
            else
            {
                return fobject.ItemSpacing.ToFloat();
            }
        }
    }
}
