using System.Runtime.InteropServices;
using SlimDX;

namespace DirectCanvas.Misc
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PointF
    {
        public PointF(float x, float y)
        {
            InternalVector2 = new Vector2(x, y);
            InternalPointF = new System.Drawing.PointF(x, y);

            X = x;
            Y = y;
        }

        public PointF(double double_x, double double_y)
        {
            float x = (float)double_x;
            float y = (float)double_y;

            InternalVector2 = new Vector2(x, y);
            InternalPointF = new System.Drawing.PointF(x, y);

            X = x;
            Y = y;
        }

        public PointF(int int_x, int int_y)
        {
            float x = int_x;
            float y = int_y;

            InternalVector2 = new Vector2(x, y);
            InternalPointF = new System.Drawing.PointF(x, y);

            X = x;
            Y = y;
        }

        public bool IsEmpty()
        {
            return (X == 0 && Y == 0);
        }

        [FieldOffset(0)]
        public float X;
        [FieldOffset(4)]
        public float Y;
        [FieldOffset(0)]
        internal System.Drawing.PointF InternalPointF;
        [FieldOffset(0)]
        internal Vector2 InternalVector2;
    }
}
