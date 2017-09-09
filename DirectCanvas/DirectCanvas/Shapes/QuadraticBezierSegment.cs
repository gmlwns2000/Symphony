using System.Runtime.InteropServices;
using DirectCanvas.Misc;

namespace DirectCanvas.Shapes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct QuadraticBezierSegment
    {
        [FieldOffset(0)]
        public PointF Point1;
        [FieldOffset(4)]
        public PointF Point2;

        [FieldOffset(0)]
        internal SlimDX.Direct2D.QuadraticBezierSegment InternalQuadraticBezierSegment;
    }
}
