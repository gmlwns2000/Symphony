using SlimDX;
using System;
using System.IO;

namespace MMDFileParser.PMXModelParser.JointParam
{
    public class HingeJointParam : JointParamBase
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

        public float Low
        {
            get;
            private set;
        }

        public float High
        {
            get;
            private set;
        }

        public float SoftNess
        {
            get;
            private set;
        }

        public float BiasFactor
        {
            get;
            private set;
        }

        public float RelaxationFactor
        {
            get;
            private set;
        }

        public bool MotorEnabled
        {
            get;
            private set;
        }

        public float TargetVelocity
        {
            get;
            private set;
        }

        public float MaxMotorImpulse
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
            Vector3 @float = ParserHelper.getFloat3(fs);
            Vector3 float2 = ParserHelper.getFloat3(fs);
            Vector3 float3 = ParserHelper.getFloat3(fs);
            Vector3 float4 = ParserHelper.getFloat3(fs);
            Vector3 float5 = ParserHelper.getFloat3(fs);
            Vector3 float6 = ParserHelper.getFloat3(fs);
            Low = float3.X;
            High = float4.X;
            SoftNess = float5.X;
            BiasFactor = float5.Y;
            RelaxationFactor = float5.Z;
            MotorEnabled = (Math.Abs(float6.X - 1f) < 0.3f);
            TargetVelocity = float6.Y;
            MaxMotorImpulse = float6.Z;
        }
    }
}