using SlimDX;
using System;
using System.IO;

namespace MMDFileParser.MotionParser
{
    public class BoneFrameData : IFrameData, IComparable
    {
        public string BoneName;

        public Vector3 BonePosition;

        public Quaternion BoneRotatingQuaternion;

        public byte[][][] Interpolation;

        public BezierCurve[] Curves;

        public uint FrameNumber
        {
            get;
            private set;
        }

        internal static BoneFrameData getBoneFrame(Stream fs)
        {
            BoneFrameData boneFrameData = new BoneFrameData();
            boneFrameData.BoneName = ParserHelper.getShift_JISString(fs, 15);
            boneFrameData.FrameNumber = ParserHelper.getDWORD(fs);
            boneFrameData.BonePosition = ParserHelper.getFloat3(fs);
            boneFrameData.BoneRotatingQuaternion = ParserHelper.getQuaternion(fs);
            boneFrameData.Interpolation = new byte[4][][];
            for (int i = 0; i < 4; i++)
            {
                boneFrameData.Interpolation[i] = new byte[4][];
                for (int j = 0; j < 4; j++)
                {
                    boneFrameData.Interpolation[i][j] = new byte[4];
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        boneFrameData.Interpolation[i][j][k] = ParserHelper.getByte(fs);
                    }
                }
            }
            boneFrameData.Curves = new BezierCurve[4];
            for (int i = 0; i < boneFrameData.Curves.Length; i++)
            {
                BezierCurve bezierCurve = new BezierCurve();
                bezierCurve.v1 = new Vector2(boneFrameData.Interpolation[0][0][i] / 128f, boneFrameData.Interpolation[0][1][i] / 128f);
                bezierCurve.v2 = new Vector2(boneFrameData.Interpolation[0][2][i] / 128f, boneFrameData.Interpolation[0][3][i] / 128f);
                boneFrameData.Curves[i] = bezierCurve;
            }
            return boneFrameData;
        }

        public int CompareTo(object x)
        {
            return (int)(FrameNumber - ((IFrameData)x).FrameNumber);
        }
    }
}