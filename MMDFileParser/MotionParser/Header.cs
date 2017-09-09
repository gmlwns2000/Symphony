using System.IO;

namespace MMDFileParser.MotionParser
{
    public class Header
    {
        public string HeaderStr;

        public string ModelName;

        internal static Header getHeader(Stream fs)
        {
            return new Header
            {
                HeaderStr = ParserHelper.getShift_JISString(fs, 30),
                ModelName = ParserHelper.getShift_JISString(fs, 20)
            };
        }
    }
}
