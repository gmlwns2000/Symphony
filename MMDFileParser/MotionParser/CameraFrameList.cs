using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MMDFileParser.MotionParser
{
    public class CameraFrameList
    {
        public uint CameraFrameCount;

        public List<CameraFrameData> CameraFrames = new List<CameraFrameData>();

        internal static CameraFrameList getCrameraFrameList(Stream fs)
        {
            CameraFrameList cameraFrameList = new CameraFrameList();
            CameraFrameList result;
            if (fs == null || fs.Position >= fs.Length)
            {
                cameraFrameList.CameraFrameCount = 0u;
                result = cameraFrameList;
            }
            else
            {
                try
                {
                    cameraFrameList.CameraFrameCount = ParserHelper.getDWORD(fs);
                    int num = 0;
                    while (num < (long)((ulong)cameraFrameList.CameraFrameCount))
                    {
                        cameraFrameList.CameraFrames.Add(CameraFrameData.getCameraFrame(fs));
                        num++;
                    }
                }
                catch (Exception ex)
                {
                    cameraFrameList.CameraFrameCount = (uint)cameraFrameList.CameraFrames.Count;
                    Debug.WriteLine(ex.StackTrace + ex.Message);
                    result = cameraFrameList;
                    return result;
                }
                result = cameraFrameList;
            }
            return result;
        }
    }
}
