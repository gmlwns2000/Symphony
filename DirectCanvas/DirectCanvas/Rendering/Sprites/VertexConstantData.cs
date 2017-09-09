using System.Runtime.InteropServices;
using SlimDX;

namespace DirectCanvas.Rendering.Sprites
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexConstantData
    {
        public Vector2 ViewportSize;
    };
}