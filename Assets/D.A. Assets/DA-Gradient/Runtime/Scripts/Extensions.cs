using UnityEngine;

namespace DA_Assets.DAG
{
    internal static class Extensions
    {
        public static Color Difference(this Color c1, Color c2, float intensity)
        {
            float r = Mathf.Abs(c1.r - c2.r) * intensity;
            float g = Mathf.Abs(c1.g - c2.g) * intensity;
            float b = Mathf.Abs(c1.b - c2.b) * intensity;

            Color result = new Color(r, g, b, c1.a);
            return result;
        }

        public static Color Multiply(this Color c1, Color c2, float intensity)
        {
            Color result = c1.Lerp(c1 * c2, intensity);
            return result;
        }

        public static Color Overlay(this Color c1, Color c2, float intensity)
        {
            Color blendedColor = c1.Lerp(c2, intensity);
            return blendedColor;
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

        public static Color Lerp(this Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            var result = new Color(a.r + (b.r - a.r) * t, a.g + (b.g - a.g) * t, a.b + (b.b - a.b) * t, a.a + (b.a - a.a) * t);
            return result;
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
    }
}