using System.Runtime.InteropServices;
using SlimDX;

namespace DirectCanvas.Rendering.Effects
{
    [StructLayout(LayoutKind.Sequential)]
    struct QuadVertex
    {
        public Vector4 Position;
        public Vector2 TexCoord;
    };
}