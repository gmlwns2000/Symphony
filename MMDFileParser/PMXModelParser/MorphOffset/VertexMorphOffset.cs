using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser.MorphOffset
{
    public class VertexMorphOffset : MorphOffsetBase
    {
        public uint VertexIndex
        {
            get;
            private set;
        }

        public Vector3 PositionOffset
        {
            get;
            private set;
        }

        internal static VertexMorphOffset getVertexMorph(FileStream fs, Header header)
        {
            return new VertexMorphOffset
            {
                type = MorphType.Vertex,
                VertexIndex = ParserHelper.getVertexIndex(fs, header.VertexIndexSize),
                PositionOffset = ParserHelper.getFloat3(fs)
            };
        }
    }
}
