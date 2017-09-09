using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MMDFileParser.MotionParser
{
    public class LightFrameList
    {
        public uint LightCount;

        public List<LightFrameData> LightFrames = new List<LightFrameData>();

        internal static LightFrameList getLightFrameList(Stream fs)
        {
            LightFrameList lightFrameList = new LightFrameList();
            LightFrameList result;
            if (fs == null || fs.Position >= fs.Length)
            {
                lightFrameList.LightCount = 0u;
                result = lightFrameList;
            }
            else
            {
                try
                {
                    lightFrameList.LightCount = ParserHelper.getDWORD(fs);
                    int num = 0;
                    while (num < (long)((ulong)lightFrameList.LightCount))
                    {
                        lightFrameList.LightFrames.Add(LightFrameData.getLightFrame(fs));
                        num++;
                    }
                }
                catch (Exception ex)
                {
                    lightFrameList.LightCount = (uint)lightFrameList.LightFrames.Count;
                    Debug.WriteLine(ex.StackTrace + ex.Message);
                    result = lightFrameList;
                    return result;
                }
                result = lightFrameList;
            }
            return result;
        }
    }
}
