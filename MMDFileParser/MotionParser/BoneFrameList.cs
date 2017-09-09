using System.Collections.Generic;
using System.IO;
using System;

namespace MMDFileParser.MotionParser
{
    public class BoneFrameList : IDisposable
    {
        public uint BoneFrameCount;

        public List<BoneFrameData> boneFrameDatas = new List<BoneFrameData>();

        internal static BoneFrameList getBoneFrameList(Stream fs)
        {
            BoneFrameList boneFrameList = new BoneFrameList();
            BoneFrameList result;
            try
            {
                boneFrameList.BoneFrameCount = ParserHelper.getDWORD(fs);
            }
            catch (EndOfStreamException)
            {
                boneFrameList.BoneFrameCount = 0u;
                result = boneFrameList;
                return result;
            }
            int num = 0;
            while (num < (long)((ulong)boneFrameList.BoneFrameCount))
            {
                boneFrameList.boneFrameDatas.Add(BoneFrameData.getBoneFrame(fs));
                num++;
            }
            result = boneFrameList;
            return result;
        }

        public void Dispose()
        {
            if(boneFrameDatas != null)
            {
                boneFrameDatas.Clear();
                boneFrameDatas = null;
            }
        }
    }
}
