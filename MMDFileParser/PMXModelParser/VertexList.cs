using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class VertexList
    {
        public int VertexCount
        {
            get;
            private set;
        }

        public VertexData[] Vertexes
        {
            get;
            private set;
        }

        internal static VertexList getVertexList(FileStream fs, Header header)
        {
            VertexList vertexList = new VertexList();
            vertexList.VertexCount = ParserHelper.getInt(fs);
            vertexList.Vertexes = new VertexData[vertexList.VertexCount];
            for (int i = 0; i < vertexList.VertexCount; i++)
            {
                vertexList.Vertexes[i] = VertexData.getVertex(fs, header);
            }
            return vertexList;
        }
    }
}
