using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class ShowFrameData
    {
        public string FrameName
        {
            get;
            private set;
        }

        public string FrameName_En
        {
            get;
            private set;
        }

        public bool IsSpecialFrame
        {
            get;
            private set;
        }

        public int ElementCount
        {
            get;
            private set;
        }

        public List<FrameElementData> FrameElements
        {
            get;
            private set;
        }

        internal static ShowFrameData getShowFrameData(Stream fs, Header header)
        {
            ShowFrameData showFrameData = new ShowFrameData();
            showFrameData.FrameName = ParserHelper.getTextBuf(fs, header.Encode);
            showFrameData.FrameName_En = ParserHelper.getTextBuf(fs, header.Encode);
            showFrameData.IsSpecialFrame = (ParserHelper.getByte(fs) == 1);
            showFrameData.ElementCount = ParserHelper.getInt(fs);
            showFrameData.FrameElements = new List<FrameElementData>();
            for (int i = 0; i < showFrameData.ElementCount; i++)
            {
                showFrameData.FrameElements.Add(FrameElementData.GetFrameElementData(fs, header));
            }
            return showFrameData;
        }
    }
}
