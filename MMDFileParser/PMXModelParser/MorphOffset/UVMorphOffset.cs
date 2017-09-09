using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser.MorphOffset
{
    public class UVMorphOffset : MorphOffsetBase
    {
        public uint VertexIndex
        {
            get;
            private set;
        }

        public Vector4 UVOffset
        {
            get;
            private set;
        }

        internal static UVMorphOffset getUVMorph(FileStream fs, Header header, MorphType type)
        {
            return new UVMorphOffset
            {
                VertexIndex = ParserHelper.getVertexIndex(fs, header.VertexIndexSize),
                UVOffset = ParserHelper.getFloat4(fs),
                type = type
            };
        }
    }
}
