using SlimDX;
using System;
using System.IO;

namespace MMDFileParser.PMXModelParser.JointParam
{
    public class SliderJointParam : JointParamBase
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

        public float LowerLinLimit
        {
            get;
            private set;
        }

        public float UpperLinLimit
        {
            get;
            private set;
        }

        public float LowerAngLimit
        {
            get;
            private set;
        }

        public float UpperAngLimit
        {
            get;
            private set;
        }

        public bool IsPoweredLinMoter
        {
            get;
            private set;
        }

        public float TargetLinMotorVelocity
        {
            get;
            private set;
        }

        public float MaxLinMotorForce
        {
            get;
            private set;
        }

        public bool IsPoweredAngMotor
        {
            get;
            private set;
        }

        public float TargetAngMotorVelocity
        {
            get;
            private set;
        }

        public float MaxAngMotorForce
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
            LowerLinLimit = @float.X;
            UpperLinLimit = float2.X;
            LowerAngLimit = float3.X;
            UpperAngLimit = float4.X;
            IsPoweredLinMoter = (Math.Abs(float5.X - 1f) < 0.3f);
            if (IsPoweredLinMoter)
            {
                TargetLinMotorVelocity = float5.Y;
                MaxLinMotorForce = float5.Z;
            }
            IsPoweredAngMotor = (Math.Abs(float6.X - 1f) < 0.3f);
            if (IsPoweredAngMotor)
            {
                TargetAngMotorVelocity = float6.Y;
                MaxAngMotorForce = float6.Z;
            }
        }
    }
}
