using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class MorphList
    {
        public int MorphCount
        {
            get;
            private set;
        }

        public List<MorphData> Morphes
        {
            get;
            private set;
        }

        internal static MorphList getMorphList(FileStream fs, Header header)
        {
            MorphList morphList = new MorphList();
            morphList.Morphes = new List<MorphData>();
            morphList.MorphCount = ParserHelper.getInt(fs);
            for (int i = 0; i < morphList.MorphCount; i++)
            {
                morphList.Morphes.Add(MorphData.getMorph(fs, header));
            }
            return morphList;
        }
    }
}
