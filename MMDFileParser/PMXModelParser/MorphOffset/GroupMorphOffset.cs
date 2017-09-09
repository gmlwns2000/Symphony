using System.IO;

namespace MMDFileParser.PMXModelParser.MorphOffset
{
    public class GroupMorphOffset : MorphOffsetBase
    {
        public int MorphIndex
        {
            get;
            private set;
        }

        public float MorphRatio
        {
            get;
            private set;
        }

        internal static GroupMorphOffset getGroupMorph(FileStream fs, Header header)
        {
            return new GroupMorphOffset
            {
                type = MorphType.Group,
                MorphIndex = ParserHelper.getIndex(fs, header.MorphIndexSize),
                MorphRatio = ParserHelper.getFloat(fs)
            };
        }
    }
}
