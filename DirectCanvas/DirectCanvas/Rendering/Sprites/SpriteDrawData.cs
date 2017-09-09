using System.Runtime.InteropServices;
using SlimDX;

namespace DirectCanvas.Rendering.Sprites
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SpriteDrawData
    {
        public SlimDX.Color4 Color;
        public Vector4 DrawRect;
        public Vector2 TextureSize;
        public int ShaderTextureIndex;
        public Vector2 Translation;
        public Vector2 Scale;
        public Vector3 Rotate;
        public Vector2 RotationCenter;
    }
}