using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser.JointParam
{
    public class P2PJointParam : JointParamBase
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

        internal override void getJointParam(Stream fs, Header header)
        {
            RigidBodyAIndex = ParserHelper.getIndex(fs, header.RigidBodyIndexSize);
            RigidBodyBIndex = ParserHelper.getIndex(fs, header.RigidBodyIndexSize);
            Position = ParserHelper.getFloat3(fs);
            Rotation = ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
            ParserHelper.getFloat3(fs);
        }
    }
}
