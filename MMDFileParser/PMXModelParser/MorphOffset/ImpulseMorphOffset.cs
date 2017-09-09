using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser.MorphOffset
{
    public class ImpulseMorphOffset : MorphOffsetBase
    {
        public int RigidIndex
        {
            get;
            private set;
        }

        public byte LocalFlag
        {
            get;
            private set;
        }

        public Vector3 VelocityOfMoving
        {
            get;
            private set;
        }

        public Vector3 TorqueOfRotating
        {
            get;
            private set;
        }

        internal static ImpulseMorphOffset getImpulseMorph(FileStream fs, Header header)
        {
            return new ImpulseMorphOffset
            {
                type = MorphType.Impulse,
                RigidIndex = ParserHelper.getIndex(fs, header.RigidBodyIndexSize),
                LocalFlag = ParserHelper.getByte(fs),
                VelocityOfMoving = ParserHelper.getFloat3(fs),
                TorqueOfRotating = ParserHelper.getFloat3(fs)
            };
        }
    }
}
