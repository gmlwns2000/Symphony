using System.Runtime.InteropServices;

namespace DirectCanvas.Brushes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GradientStop
    {
        public GradientStop(Color4 color, float position)
        {
            InternalGradientStop = new SlimDX.Direct2D.GradientStop();
            Color = color;
            Position = position;
        }

        [FieldOffset(0)]
        public float Position;

        [FieldOffset(4)]
        public Color4 Color;

        [FieldOffset(0)]
        internal SlimDX.Direct2D.GradientStop InternalGradientStop;
    }
}