using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.Sprite
{
    public struct SpriteVertexLayout
    {
        public Vector3 Position;

        public Vector2 UV;

        public static InputElement[] InputElements = new InputElement[]
        {
            new InputElement
            {
                SemanticName = "POSITION",
                Format = Format.R32G32B32_Float
            },
            new InputElement
            {
                SemanticName = "UV",
                Format = Format.R32G32_Float,
                AlignedByteOffset = InputElement.AppendAligned
            }
        };

        public static int SizeInBytes
        {
            get
            {
                return System.Runtime.InteropServices.Marshal.SizeOf(typeof(SpriteVertexLayout));
            }
        }
    }
}
