using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser.JointParam
{
    public class Basic6DofJointParam : JointParamBase
    {
        public int RigidBodyAIndex
        {
            get;
            private set;
        }

        public int RigidBodyBIndex
        {
            get;
            private set;
        }

        public Vector3 Position
        {
            get;
            private set;
        }

        public Vector3 Rotation
        {
            get;
            private set;
        }

        public Vector3 MoveLimitationMin
        {
            get;
            private set;
        }

        public Vector3 MoveLimitationMax
        {
            get;
            private set;
        }

        public Vector3 RotationLimitationMin
        {
            get;
            private set;
        }

        public Vector3 RotationLimitationMax
        {
            get;
            private set;
        }

        internal override void getJointParam(Stream fs, Header header)
        {
            RigidBodyAIndex = ParserHelper.getIndex(fs, header.RigidBodyIndexSize);
            RigidBodyBIndex = ParserHelper.getIndex(fs, header.RigidBodyIndexSize);
            Position = ParserHelper.getFloat3(fs);
            Rotation = ParserHelper.getFloat3(fs);
            MoveLimitationMin = ParserHelper.getFloat3(fs);
            MoveLimitationMax = ParserHelper.getFloat3(fs);
            RotationLimitationMin = ParserHelper.getFloat3(fs);
            RotationLimitationMax = ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
        }
    }
}
