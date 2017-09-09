using System.IO;

namespace MMDFileParser.PMXModelParser.MorphOffset
{
    public class FlipMorphOffset : MorphOffsetBase
    {
        public int MorphIndex
        {
            get;
            private set;
        }

        public float MorphValue
        {
            get;
            private set;
        }

        internal static FlipMorphOffset getFlipMorph(FileStream fs, Header header)
        {
            return new FlipMorphOffset
            {
                type = MorphType.Flip,
                MorphIndex = ParserHelper.getIndex(fs, header.MorphIndexSize),
                MorphValue = ParserHelper.getFloat(fs)
            };
        }
    }
}
