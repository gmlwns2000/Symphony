using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser.MorphOffset
{
    public class BoneMorphOffset : MorphOffsetBase
    {
        public int BoneIndex
        {
            get;
            private set;
        }

        public Vector3 QuantityOfMoving
        {
            get;
            private set;
        }

        public Vector4 QuantityOfRotating
        {
            get;
            private set;
        }

        internal static BoneMorphOffset getBoneMorph(FileStream fs, Header header)
        {
            return new BoneMorphOffset
            {
                type = MorphType.Bone,
                BoneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                QuantityOfMoving = ParserHelper.getFloat3(fs),
                QuantityOfRotating = ParserHelper.getFloat4(fs)
            };
        }
    }
}
