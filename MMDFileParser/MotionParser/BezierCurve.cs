using SlimDX;
using System;

namespace MMDFileParser.MotionParser
{
    public class BezierCurve
    {
        private const float Epsilon = 0.001f;

        public Vector2 v1;

        public Vector2 v2;

        public float Evaluate(float Progress)
        {
            float num = CGHelper.Clamp(Progress, 0f, 1f);
            float num2;
            do
            {
                num2 = -(fx(num) - Progress) / dfx(num);
                if (float.IsNaN(num2))
                {
                    break;
                }
                num += CGHelper.Clamp(num2, -1f, 1f);
            }
            while (Math.Abs(num2) > 0.001f);
            return CGHelper.Clamp(fy(num), 0f, 1f);
        }

        private float fy(float t)
        {
            return 3f * (1f - t) * (1f - t) * t * v1.Y + 3f * (1f - t) * t * t * v2.Y + t * t * t;
        }

        private float fx(float t)
        {
            return 3f * (1f - t) * (1f - t) * t * v1.X + 3f * (1f - t) * t * t * v2.X + t * t * t;
        }

        private float dfx(float t)
        {
            return -6f * (1f - t) * t * v1.X + 3f * (1f - t) * (1f - t) * v1.X - 3f * t * t * v2.X + 6f * (1f - t) * t * v2.X + 3f * t * t;
        }
    }
}
