using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.Grid
{
    public struct MeasureGridLayout
    {
        public static readonly InputElement[] VertexElements = new InputElement[]
        {
            new InputElement
            {
                SemanticName = "POSITION",
                Format = Format.R32G32B32_Float
            }
        };

        public Vector3 Position;

        public static int SizeInBytes
        {
            get
            {
                return System.Runtime.InteropServices.Marshal.SizeOf(typeof(MeasureGridLayout));
            }
        }
    }
}
