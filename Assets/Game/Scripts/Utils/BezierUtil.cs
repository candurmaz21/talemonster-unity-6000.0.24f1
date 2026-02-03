using UnityEngine;

namespace Game.Utils.Math
{
    public static class BezierUtil
    {
        public static Vector2 Quadratic(Vector2 a, Vector2 c, Vector2 b, float t)
        {
            float u = 1f - t;
            return u * u * a + 2f * u * t * c + t * t * b;
        }

        public static Vector2 GetParabolaControlPoint(Vector2 start, Vector2 end,
                        float minHeight = 120f, float maxHeight = 220f, float maxSideOffset = 80f)
        {
            Vector2 mid = (start + end) * 0.5f;

            float height = Random.Range(minHeight, maxHeight);
            float sideways = Random.Range(-maxSideOffset, maxSideOffset);

            return new Vector2(
                mid.x + sideways,
                Mathf.Max(start.y, end.y) + height
            );
        }
    }
}
