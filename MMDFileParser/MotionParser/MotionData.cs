using System.IO;
using System;

namespace MMDFileParser.MotionParser
{
    public class MotionData : IDisposable
    {
        public Header header;

        public BoneFrameList boneFrameList;

        public MorphFrameList morphFrameList;

        public CameraFrameList CameraFrames;

        public LightFrameList LightFrames;

        public static MotionData getMotion(Stream fs)
        {
            return new MotionData
            {
                header = Header.getHeader(fs),
                boneFrameList = BoneFrameList.getBoneFrameList(fs),
                morphFrameList = MorphFrameList.getFraceFrameList(fs),
                CameraFrames = CameraFrameList.getCrameraFrameList(fs),
                LightFrames = LightFrameList.getLightFrameList(fs)
            };
        }

        public void Dispose()
        {
            if(boneFrameList != null)
            {
                boneFrameList.Dispose();
                boneFrameList = null;
            }

            if(morphFrameList != null)
            {
                morphFrameList.Dispose();
                morphFrameList = null;
            }

            CameraFrames = null;
            LightFrames = null;
        }
    }
}
