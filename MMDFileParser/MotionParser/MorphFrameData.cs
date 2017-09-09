using System;
using System.IO;

namespace MMDFileParser.MotionParser
{
    public class MorphFrameData : IFrameData, IComparable
    {
        public string Name;

        public float MorphValue;

        public uint FrameNumber
        {
            get;
            private set;
        }

        internal static MorphFrameData getMorphFrame(Stream fs)
        {
            return new MorphFrameData
            {
                Name = ParserHelper.getShift_JISString(fs, 15),
                FrameNumber = ParserHelper.getDWORD(fs),
                MorphValue = ParserHelper.getFloat(fs)
            };
        }

        public int CompareTo(object x)
        {
            return (int)(FrameNumber - ((IFrameData)x).FrameNumber);
        }
    }
}
