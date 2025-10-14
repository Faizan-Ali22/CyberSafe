using System;
using System.Globalization;
using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class MathExtensions
    {
        public static bool TryParseWithDot(this string str, out float value) =>
            float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

        public static string ToDotString(this float value) =>
            value.ToString(CultureInfo.InvariantCulture);

        public static float ToFloat(this float? value)
        {
            return value.HasValue ? value.Value : 0;
        }

        public static System.Numerics.BigInteger ToBigInteger(this byte[] data)
        {
            return new System.Numerics.BigInteger(data);
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

        /// <summary>
        /// <para><see href="https://stackoverflow.com/a/2722999"/></para>
        /// </summary>
        public static bool TryParseFloat(this string input, out float result)
        {
            // try parsing with "fr-FR" first
            bool success = float.TryParse(input,
                                          NumberStyles.Float | NumberStyles.AllowThousands,
                                          CultureInfo.GetCultureInfo("fr-FR"),
                                          out result);

            if (!success)
            {
                // parsing with "fr-FR" failed so try parsing with InvariantCulture
                success = float.TryParse(input,
                                         NumberStyles.Float | NumberStyles.AllowThousands,
                                         CultureInfo.InvariantCulture,
                                         out result);
            }

            return success;
        }

        public static float CalculateIntersectionFactor(this Vector3 lineStart, Vector2 lineEnd, Vector2 origin, Vector2 direction)
        {
            // Calculate the determinant to check if the line (lineStart to lineEnd) and the direction vector are parallel
            float determinant = (lineEnd.x - lineStart.x) * direction.y - (lineEnd.y - lineStart.y) * direction.x;

            // Use Mathf.Approximately to check for equality considering floating-point imprecision
            if (Mathf.Approximately(determinant, 0f))
            {
                // Returning -1 to indicate the lines are parallel or overlapping with no distinct intersection
                return -1;
            }

            // Calculate and return the intersection factor along the line from lineStart to lineEnd
            // where an intersection with the vector originating from 'origin' in the 'direction' occurs
            float intersectionFactor = ((origin.x - lineStart.x) * direction.y - (origin.y - lineStart.y) * direction.x) / determinant;
            return intersectionFactor;
        }

        public static UIVertex Lerp(this UIVertex vertex1, UIVertex vertex2, float interpolationFactor)
        {
            UIVertex vertex = new UIVertex();

            vertex.position = Vector3.Lerp(vertex1.position, vertex2.position, interpolationFactor);
            vertex.color = Color.Lerp(vertex1.color, vertex2.color, interpolationFactor);
            vertex.uv0 = Vector2.Lerp(vertex1.uv0, vertex2.uv0, interpolationFactor);
            vertex.uv1 = Vector2.Lerp(vertex1.uv1, vertex2.uv1, interpolationFactor);
            vertex.uv2 = Vector2.Lerp(vertex1.uv2, vertex2.uv2, interpolationFactor);
            vertex.uv3 = Vector2.Lerp(vertex1.uv3, vertex2.uv3, interpolationFactor);

            return vertex;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static float NormalizeAngle360(this float angle)
        {
            if (angle < 0)
            {
                angle = (angle % 360) + 360;
            }
            else
            {
                angle = angle % 360;
            }

            return angle;
        }


        /// <summary>
        /// <para><see href="https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/"/></para>
        /// </summary>
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        /// <summary>
        /// Remap value from these interval to another interval with saving proportion
        /// <para><see href="https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/"/></para>
        /// </summary>
        /// <param name="value">Value for remapping</param>
        /// <param name="sourceMin"></param>
        /// <param name="sourceMax"></param>
        /// <param name="targetMin"></param>
        /// <param name="targetMax"></param>
        /// <returns></returns>
        public static float Remap(this float value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            return (value - sourceMin) / (sourceMax - sourceMin) * (targetMax - targetMin) + targetMin;
        }

        /// <summary>
        /// Makes random bool.
        /// <para><see href="https://forum.unity.com/threads/random-randomboolean-function.83220/#post-548687"/></para>
        /// </summary>
        public static bool RandomBool => UnityEngine.Random.value > 0.5f;

        public static Vector4 Round(this Vector4 vector4, int dp = 0)
        {
            float x = (float)System.Math.Round(vector4.x, dp);
            float y = (float)System.Math.Round(vector4.y, dp);
            float z = (float)System.Math.Round(vector4.z, dp);
            float w = (float)System.Math.Round(vector4.w, dp);

            return new Vector4(x, y, z, w);
        }

        public static Vector2 Round(this Vector2 vector2, int dp = 0)
        {
            float x = (float)System.Math.Round(vector2.x, dp);
            float y = (float)System.Math.Round(vector2.y, dp);

            return new Vector2(x, y);
        }

        public static float RoundToNearest025(this float value) => Mathf.Round(value * 4f) / 4f;

        /// <summary>
        /// Rounds the float value to the nearest multiple of 0.05.
        /// Examples: 0.13 -> 0.15, 0.22 -> 0.2
        /// </summary>
        /// <param name="value">The float value to round.</param>
        /// <returns>The rounded value.</returns>
        public static float RoundToNearest005(this float value)
        {
            return Mathf.Round(value * 20f) / 20f;
        }

        public static float Round(this float value, int digits) => (float)Math.Round((double)value, digits);

        public static Vector2Int ToVector2Int(this Vector2 vector) => new Vector2Int((int)vector.x, (int)vector.y);
    }
}