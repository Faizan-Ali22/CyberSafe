using UnityEngine;

namespace DA_Assets.FCU
{
    /// <summary>
    /// Custom RectOffset structure similar to Unity's RectOffset.
    /// </summary>
    public struct RectOffsetCustom
    {
        /// <summary>
        /// Distance from the left.
        /// </summary>
        public int left;

        /// <summary>
        /// Distance from the right.
        /// </summary>
        public int right;

        /// <summary>
        /// Distance from the top.
        /// </summary>
        public int top;

        /// <summary>
        /// Distance from the bottom.
        /// </summary>
        public int bottom;

        /// <summary>
        /// Initializes a new instance of the RectOffsetCustom structure.
        /// </summary>
        /// <param name="left">Distance from the left.</param>
        /// <param name="right">Distance from the right.</param>
        /// <param name="top">Distance from the top.</param>
        /// <param name="bottom">Distance from the bottom.</param>
        public RectOffsetCustom(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }

        /// <summary>
        /// Converts the RectOffsetCustom instance to a Unity RectOffset.
        /// </summary>
        /// <returns>A new UnityEngine.RectOffset instance.</returns>
        public RectOffset ToRectOffset()
        {
            return new RectOffset(left, right, top, bottom);
        }

        /// <summary>
        /// Provides a string representation of the RectOffsetCustom.
        /// </summary>
        /// <returns>A string describing the RectOffsetCustom.</returns>
        public override string ToString()
        {
            return $"RectOffsetCustom(Left: {left}, Right: {right}, Top: {top}, Bottom: {bottom})";
        }
    }
}
