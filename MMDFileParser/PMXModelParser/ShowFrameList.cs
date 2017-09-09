using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class ShowFrameList
    {
        public int ShowFrameCount
        {
            get;
            private set;
        }

        public List<ShowFrameData> ShowFrames
        {
            get;
            private set;
        }

        internal static ShowFrameList getShowFrameList(Stream fs, Header header)
        {
            ShowFrameList showFrameList = new ShowFrameList();
            showFrameList.ShowFrameCount = ParserHelper.getInt(fs);
            showFrameList.ShowFrames = new List<ShowFrameData>();
            for (int i = 0; i < showFrameList.ShowFrameCount; i++)
            {
                showFrameList.ShowFrames.Add(ShowFrameData.getShowFrameData(fs, header));
            }
            return showFrameList;
        }
    }
}
