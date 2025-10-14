using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class RectTransformExtensions
    {
        public static void CopyFrom(this RectTransform target, RectTransform source)
        {
            target.anchorMin = source.anchorMin;
            target.anchorMax = source.anchorMax;
            target.anchoredPosition = source.anchoredPosition;
            target.sizeDelta = source.sizeDelta;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }

        public static float GetWidth(this RectTransform rt)
        {
            var w = (rt.anchorMax.x - rt.anchorMin.x) * Screen.width + rt.sizeDelta.x;
            return w;
        }

        public static float GetHeight(this RectTransform rt)
        {
            var h = (rt.anchorMax.y - rt.anchorMin.y) * Screen.height + rt.sizeDelta.y;
            return h;
        }

        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static float GetLeft(this RectTransform rt)
        {
            return rt.offsetMin.x;
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static float GetRight(this RectTransform rt)
        {
            return -rt.offsetMax.x;
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static float GetTop(this RectTransform rt)
        {
            return -rt.offsetMax.y;
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static float GetBottom(this RectTransform rt)
        {
            return rt.offsetMin.y;
        }
    }
}