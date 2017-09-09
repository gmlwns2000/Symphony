using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.Grid
{
    public struct AxisGridLayout
    {
        public static readonly InputElement[] VertexElements = new InputElement[]
        {
            new InputElement
            {
                SemanticName = "POSITION",
                Format = Format.R32G32B32_Float
            },
            new InputElement
            {
                SemanticName = "COLOR",
                Format = Format.R32G32B32A32_Float,
                AlignedByteOffset = InputElement.AppendAligned
            }
        };

        public Vector4 Color;

        public Vector3 Position;

        public static int SizeInBytes
        {
            get
            {
                return System.Runtime.InteropServices.Marshal.SizeOf(typeof(AxisGridLayout));
            }
        }
    }
}
