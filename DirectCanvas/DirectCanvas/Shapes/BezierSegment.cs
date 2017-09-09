using System.Runtime.InteropServices;
using DirectCanvas.Misc;

namespace DirectCanvas.Shapes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct BezierSegment
    {
        [FieldOffset(0)]
        public PointF Point1;
        [FieldOffset(8)]
        public PointF Point2;
        [FieldOffset(16)]
        public PointF Point3;

        [FieldOffset(0)]
        internal SlimDX.Direct2D.BezierSegment InternalBezierSegment;
    }
}