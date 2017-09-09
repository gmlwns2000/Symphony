using System.Runtime.InteropServices;

namespace DirectCanvas
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Color4
    {
        public Color4(float a, float r, float g, float b)
        {
            InternalColor4 = new SlimDX.Color4();

            A = a;
            R = r;
            G = g;
            B = b;
        }

        public Color4(byte a, byte r, byte g, byte b)
        {
            InternalColor4 = new SlimDX.Color4();

            R = (float)r / byte.MaxValue;
            G = (float)g / byte.MaxValue;
            B = (float)b / byte.MaxValue;
            A = (float)a / byte.MaxValue;
        }

        public Color4(System.Windows.Media.Color c) : this(c.A, c.R, c.G, c.B)
        {
            
        }

        [FieldOffset(0)]
        public float R;
        [FieldOffset(4)]
        public float G;
        [FieldOffset(8)]
        public float B;
        [FieldOffset(12)]
        public float A;
        
        [FieldOffset(0)]
        internal SlimDX.Color4 InternalColor4;
    }
}
