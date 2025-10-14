using DA_Assets.FCU.Attributes;
using DA_Assets.FCU.Model;
using DA_Assets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#pragma warning disable CS0219

namespace DA_Assets.FCU.Extensions
{
    public static class GraphicExtensions
    {


        public static bool GetBoundingSize(this FObject fobject, out Vector2 size)
        {
            size = default;

            float? x = fobject.AbsoluteBoundingBox.Width;
            float? y = fobject.AbsoluteBoundingBox.Height;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value, FcuConfig.Rounding.GetBoundingSizeDigits);
            float yR = (float)Math.Round(y.Value,  FcuConfig.Rounding.GetBoundingSizeDigits);

            size = new Vector2(xR, yR);
            return true;
        }

        public static bool GetBoundingPosition(this FObject fobject, out Vector2 position)
        {
            position = default;

            float? x = fobject.AbsoluteBoundingBox.X;
            float? y = fobject.AbsoluteBoundingBox.Y;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value,  FcuConfig.Rounding.GetBoundingSizeDigits);
            float yR = (float)Math.Round(y.Value,  FcuConfig.Rounding.GetBoundingSizeDigits);

            position = new Vector2(xR, yR);
            return true;
        }

        public static bool GetRenderSize(this FObject fobject, out Vector2 size)
        {
            size = default;

            float? x = fobject.AbsoluteRenderBounds.Width;
            float? y = fobject.AbsoluteRenderBounds.Height;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value,  FcuConfig.Rounding.GetBoundingSizeDigits);
            float yR = (float)Math.Round(y.Value,  FcuConfig.Rounding.GetBoundingSizeDigits);

            size = new Vector2(xR, yR);
            return true;
        }

        public static bool GetRenderPosition(this FObject fobject, out Vector2 position)
        {
            position = default;

            float? x = fobject.AbsoluteRenderBounds.X;
            float? y = fobject.AbsoluteRenderBounds.Y;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value,  FcuConfig.Rounding.GetBoundingSizeDigits);
            float yR = (float)Math.Round(y.Value,  FcuConfig.Rounding.GetBoundingSizeDigits);

            position = new Vector2(xR, yR);
            return true;
        }

        public static bool IsZeroSize(this FObject fobject)
        {
            if (fobject.AbsoluteBoundingBox.Width <= 0 || fobject.AbsoluteBoundingBox.Height <= 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsVisible(this FObject fobject) => fobject.Visible.ToBoolNullTrue();

        public static bool IsVisible(this Paint paint) => paint.Visible.ToBoolNullTrue();

        public static bool IsVisible(this Effect effect) => effect.Visible.ToBoolNullTrue();

        public static bool IsSingleLinearGradient(this FObject fobject, out Paint paint)
        {
            paint = default;
            bool result = false;
            int reason = 0;

            if (fobject.Fills.IsEmpty())
            {
                result = false;
                reason = 1;
            }
            else
            {
                IEnumerable<Paint> enabledFills = fobject.Fills.Where(x => x.IsVisible());
                IEnumerable<Paint> linearFills = fobject.Fills.Where(x => x.IsVisible() && x.Type == PaintType.GRADIENT_LINEAR);

                if (!fobject.Strokes.IsEmpty())
                {
                    result = false;
                    reason = 2;
                }
                else if (!fobject.Effects.IsEmpty())
                {
                    result = false;
                    reason = 3;
                }
                else if (enabledFills.Count() > 1)
                {
                    result = false;
                    reason = 4;
                }
                else if (linearFills.Count() == 1)
                {
                    paint = linearFills.First();
                    result = true;
                    reason = 4;
                }
            }

            return result;
        }

        public static bool ContainsImageEmojiVideo(this FObject fobject)
        {
            if (fobject.Fills.IsEmpty())
                return false;

            foreach (Paint paint in fobject.Fills)
            {
                if (!paint.IsVisible())
                    return false;

                if (paint.ImageRef.IsEmpty() == false || paint.GifRef.IsEmpty() == false)
                    return true;

                switch (paint.Type)
                {
                    case PaintType.IMAGE:
                    case PaintType.EMOJI:
                    case PaintType.VIDEO:
                        return true;
                }
            }

            return false;
        }

        public static bool IsSingleColor(this FObject fobject, out Color color)
        {
            Dictionary<Color, float?> values = new Dictionary<Color, float?>();
            List<bool> flags = new List<bool>();

            IsSingleColorRecursive(fobject, flags, values);

            if (flags.Count > 0)
            {
                color = default;
                return false;
            }

            if (values.Count == 1)
            {
                color = values.First().Key;
                return true;
            }
            else
            {
                color = default;
                return false;
            }
        }

        private static void IsSingleColorRecursive(FObject fobject, List<bool> flags, Dictionary<Color, float?> values)
        {
            if (fobject.Fills.IsEmpty() == false)
            {
                foreach (Paint paint in fobject.Fills)
                {
                    if (!paint.IsVisible())
                        continue;

                    if (paint.ImageRef.IsEmpty() == false || paint.GifRef.IsEmpty() == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    if (paint.Type.ToString().Contains("SOLID") == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    values.TryAddValue<Color, float?>(paint.Color, paint.Opacity);
                }
            }

            if (fobject.Strokes.IsEmpty() == false)
            {
                foreach (Paint paint in fobject.Strokes)
                {
                    if (!paint.IsVisible())
                        continue;

                    if (paint.ImageRef.IsEmpty() == false || paint.GifRef.IsEmpty() == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    if (paint.Type.ToString().Contains("SOLID") == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    values.TryAddValue<Color, float?>(paint.Color, paint.Opacity);
                }
            }

            if (fobject.Effects.IsEmpty() == false)
            {
                foreach (Effect effect in fobject.Effects)
                {
                    if (!effect.IsVisible())
                        continue;

                    if (effect.Type.ToString().Contains("SOLID") == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    values.TryAddValue<Color, float?>(effect.Color, effect.Opacity);
                }
            }

            if (fobject.Children.IsEmpty())
                return;

            foreach (FObject item in fobject.Children)
            {
                if (item.Type == NodeType.TEXT)
                    continue;

                IsSingleColorRecursive(item, flags, values);
            }
        }

        public static bool ContainsRoundedCorners(this FObject fobject)
        {
            return fobject.CornerRadius > 0 || (fobject.CornerRadiuses?.Any(radius => radius > 0)).ToBoolNullFalse();
        }

        public static bool IsArcDataFilled(this FObject fobject)
        {
            if (fobject.ArcData.Equals(default(ArcData)))
            {
                return false;
            }

            return fobject.ArcData.EndingAngle < 6.28f;
        }

        public static Paint GetFirstSolidPaint(this List<Paint> paints)
        {
            if (paints == null)
                return default;

            Paint fill = default;

            foreach (Paint paint in paints)
            {
                if (!paint.IsVisible())
                    continue;

                if (paint.Type == PaintType.SOLID)
                {
                    fill = paint;
                    fill.Color = fill.Color.SetAlpha(paint.Opacity);
                    break;
                }
            }

            return fill;
        }
        public static int GetPriority(this PaintType paintType)
        {
            Type type = paintType.GetType();

            MemberInfo[] memberInfo = type.GetMember(paintType.ToString());

            if (memberInfo.Length > 0)
            {
                PaintPriorityAttribute attribute = memberInfo[0].GetCustomAttribute<PaintPriorityAttribute>();

                if (attribute != null)
                {
                    return attribute.Priority;
                }
            }

            return 0;
        }
    }
}
