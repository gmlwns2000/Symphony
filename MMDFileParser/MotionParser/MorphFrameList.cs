using System.Collections.Generic;
using System.IO;
using System;

namespace MMDFileParser.MotionParser
{
    public class MorphFrameList : IDisposable
    {
        public uint MorphFrameCount;

        public List<MorphFrameData> morphFrameDatas = new List<MorphFrameData>();

        internal static MorphFrameList getFraceFrameList(Stream fs)
        {
            MorphFrameList morphFrameList = new MorphFrameList();
            MorphFrameList result;
            try
            {
                morphFrameList.MorphFrameCount = ParserHelper.getDWORD(fs);
            }
            catch
            {
                morphFrameList.MorphFrameCount = 0u;
                result = morphFrameList;
                return result;
            }
            int num = 0;
            while (num < (long)((ulong)morphFrameList.MorphFrameCount))
            {
                morphFrameList.morphFrameDatas.Add(MorphFrameData.getMorphFrame(fs));
                num++;
            }
            result = morphFrameList;
            return result;
        }

        public void Dispose()
        {
            if(morphFrameDatas != null)
            {
                morphFrameDatas.Clear();
                morphFrameDatas = null;
            }
        }
    }
}
