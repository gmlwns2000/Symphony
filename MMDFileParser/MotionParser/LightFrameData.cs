using SlimDX;
using System.IO;

namespace MMDFileParser.MotionParser
{
    public class LightFrameData
    {
        public uint FrameNumber;

        public Vector3 LightColor;

        public Vector3 LightPosition;

        internal static LightFrameData getLightFrame(Stream fs)
        {
            return new LightFrameData
            {
                FrameNumber = ParserHelper.getDWORD(fs),
                LightColor = ParserHelper.getFloat3(fs),
                LightPosition = ParserHelper.getFloat3(fs)
            };
        }
    }
}
