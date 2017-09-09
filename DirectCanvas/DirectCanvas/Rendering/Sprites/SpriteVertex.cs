using System.Runtime.InteropServices;
using SlimDX;

namespace DirectCanvas.Rendering.Sprites
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct SpriteVertex
    {
        public Vector2 Position;
        public Vector2 TexCoord;
    };
}