using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class RigidBodyData
    {
        public string RigidBodyName
        {
            get;
            private set;
        }

        public string RigidBodyName_En
        {
            get;
            private set;
        }

        public int BoneIndex
        {
            get;
            private set;
        }

        public byte RigidBodyGroup
        {
            get;
            private set;
        }

        public ushort UnCollisionGroupFlag
        {
            get;
            private set;
        }

        public RigidBodyShape Shape
        {
            get;
            private set;
        }

        public Vector3 Size
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

        public float Mass
        {
            get;
            private set;
        }

        public float MoveAttenuation
        {
            get;
            private set;
        }

        public float RotationAttenuation
        {
            get;
            private set;
        }

        public float Repulsion
        {
            get;
            private set;
        }

        public float Friction
        {
            get;
            private set;
        }

        public PhysicsCalcType PhysicsCalcType
        {
            get;
            private set;
        }

        internal static RigidBodyData GetRigidBodyData(Stream fs, Header header)
        {
            return new RigidBodyData
            {
                RigidBodyName = ParserHelper.getTextBuf(fs, header.Encode),
                RigidBodyName_En = ParserHelper.getTextBuf(fs, header.Encode),
                BoneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                RigidBodyGroup = ParserHelper.getByte(fs),
                UnCollisionGroupFlag = ParserHelper.getUShort(fs),
                Shape = (RigidBodyShape)ParserHelper.getByte(fs),
                Size = ParserHelper.getFloat3(fs),
                Position = ParserHelper.getFloat3(fs),
                Rotation = ParserHelper.getFloat3(fs),
                Mass = ParserHelper.getFloat(fs),
                MoveAttenuation = ParserHelper.getFloat(fs),
                RotationAttenuation = ParserHelper.getFloat(fs),
                Repulsion = ParserHelper.getFloat(fs),
                Friction = ParserHelper.getFloat(fs),
                PhysicsCalcType = (PhysicsCalcType)ParserHelper.getByte(fs)
            };
        }
    }
}
