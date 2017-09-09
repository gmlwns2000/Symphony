using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class FrameElementData
    {
        public bool IsMorph
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        internal static FrameElementData GetFrameElementData(Stream fs, Header header)
        {
            FrameElementData frameElementData = new FrameElementData();
            frameElementData.IsMorph = (ParserHelper.getByte(fs) == 1);
            if (frameElementData.IsMorph)
            {
                frameElementData.Index = ParserHelper.getIndex(fs, header.MorphIndexSize);
            }
            else
            {
                frameElementData.Index = ParserHelper.getIndex(fs, header.BoneIndexSize);
            }
            return frameElementData;
        }
    }
}
