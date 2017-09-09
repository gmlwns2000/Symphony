using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class SurfaceList
    {
        public List<SurfaceData> Surfaces
        {
            get;
            private set;
        }

        public int SurfaceCount
        {
            get;
            private set;
        }

        internal static SurfaceList getSurfaceList(FileStream fs, Header header)
        {
            SurfaceList surfaceList = new SurfaceList();
            surfaceList.SurfaceCount = ParserHelper.getInt(fs);
            surfaceList.Surfaces = new List<SurfaceData>();
            for (int i = 0; i < surfaceList.SurfaceCount / 3; i++)
            {
                surfaceList.Surfaces.Add(new SurfaceData(ParserHelper.getVertexIndex(fs, header.VertexIndexSize), ParserHelper.getVertexIndex(fs, header.VertexIndexSize), ParserHelper.getVertexIndex(fs, header.VertexIndexSize)));
            }
            return surfaceList;
        }
    }
}
