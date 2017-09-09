using SlimDX;
using System;
using System.IO;

namespace MMDFileParser.PMXModelParser.JointParam
{
    public class ConeTwistJointParam : JointParamBase
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

        public float SwingSpan1
        {
            get;
            private set;
        }

        public float SwingSpan2
        {
            get;
            private set;
        }

        public float TwistSpan
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

        public float Damping
        {
            get;
            private set;
        }

        public float FixThresh
        {
            get;
            private set;
        }

        public bool MoterEnabled
        {
            get;
            private set;
        }

        public float MaxMotorImpluse
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
            Damping = @float.X;
            FixThresh = float2.X;
            MoterEnabled = Math.Abs(@float.Z - 1f) < 0.3;
            if (MoterEnabled)
            {
                MaxMotorImpluse = float2.Z;
            }
            Vector3 float3 = ParserHelper.getFloat3(fs);
            SwingSpan1 = float3.Z;
            SwingSpan2 = float3.Y;
            TwistSpan = float3.X;
            ParserHelper.getFloat3(fs);
            Vector3 float4 = ParserHelper.getFloat3(fs);
            SoftNess = float4.X;
            BiasFactor = float4.Y;
            RelaxationFactor = float4.Z;
            ParserHelper.getFloat3(fs);
        }
    }
}