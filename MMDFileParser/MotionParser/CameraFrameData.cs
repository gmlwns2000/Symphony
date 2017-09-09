using SlimDX;
using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.MotionParser
{
    public class CameraFrameData : IComparer<CameraFrameData>
    {
        public uint FrameNumber;

        public float Distance;

        public Vector3 CameraPosition;

        public Vector3 CameraRotation;

        public byte[][] Interpolation;

        public BezierCurve[] Curves;

        public uint ViewAngle;

        public byte Perspective;

        internal static CameraFrameData getCameraFrame(Stream fs)
        {
            CameraFrameData cameraFrameData = new CameraFrameData();
            cameraFrameData.FrameNumber = ParserHelper.getDWORD(fs);
            cameraFrameData.Distance = ParserHelper.getFloat(fs);
            cameraFrameData.CameraPosition = ParserHelper.getFloat3(fs);
            cameraFrameData.CameraRotation = ParserHelper.getFloat3(fs);
            cameraFrameData.CameraRotation.X = -cameraFrameData.CameraRotation.X;
            cameraFrameData.Interpolation = new byte[6][];
            for (int i = 0; i < 6; i++)
            {
                cameraFrameData.Interpolation[i] = new byte[4];
            }
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    cameraFrameData.Interpolation[i][j] = ParserHelper.getByte(fs);
                }
            }
            cameraFrameData.ViewAngle = ParserHelper.getDWORD(fs);
            cameraFrameData.Perspective = ParserHelper.getByte(fs);
            cameraFrameData.Curves = new BezierCurve[6];
            for (int i = 0; i < 6; i++)
            {
                BezierCurve bezierCurve = new BezierCurve();
                bezierCurve.v1 = new Vector2(cameraFrameData.Interpolation[i][0] / 128f, cameraFrameData.Interpolation[i][1] / 128f);
                bezierCurve.v2 = new Vector2(cameraFrameData.Interpolation[i][2] / 128f, cameraFrameData.Interpolation[i][3] / 128f);
                cameraFrameData.Curves[i] = bezierCurve;
            }
            return cameraFrameData;
        }

        public int Compare(CameraFrameData x, CameraFrameData y)
        {
            return (int)(x.FrameNumber - y.FrameNumber);
        }
    }
}
