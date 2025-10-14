using UnityEngine;

namespace DA_Assets.CR
{
    internal static class Extensions
    {
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

        public static int NormalizeAngleToSize(this float val, float width, float height)
        {
            if (val <= 0)
            {
                val = 0;
                return (int)val;
            }

            float resW = width / val;
            float resH = height / val;

            if (resW < 2 || resH < 2)
            {
                int min = Mathf.Min((int)(width / 2), (int)(height / 2));
                val = min;
            }

            if (val <= 0)
                val = 0;

            return (int)val;
        }
    }
}