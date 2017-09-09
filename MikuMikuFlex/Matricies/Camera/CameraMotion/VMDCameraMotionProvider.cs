using MMDFileParser.MotionParser;
using MMF.Matricies.Projection;
using MMF.Utility;
using SlimDX;
using System.Diagnostics;
using System.Linq;

namespace MMF.Matricies.Camera.CameraMotion
{
    public class VMDCameraMotionProvider : ICameraMotionProvider
    {
        private System.Collections.Generic.List<CameraFrameData> CameraFrames;

        private Stopwatch stopWatch;

        private long lastMillisecound;

        private float currentFrame = 0f;

        private bool isPlaying = false;

        private float finalFrame;

        private bool needReplay;

        public float CurrentFrame
        {
            get
            {
                return currentFrame;
            }
            set
            {
                currentFrame = value;
            }
        }

        public float FinalFrame
        {
            get
            {
                return finalFrame;
            }
        }

        public static VMDCameraMotionProvider OpenFile(string path)
        {
            return new VMDCameraMotionProvider(MotionData.getMotion(System.IO.File.OpenRead(path)));
        }

        public VMDCameraMotionProvider(MotionData cameraMotion)
        {
            CameraFrames = cameraMotion.CameraFrames.CameraFrames;
            CameraFrames.Sort(new CameraFrameData());
            stopWatch = new Stopwatch();
            if (CameraFrames.Count == 0)
            {
                finalFrame = 0f;
            }
            else
            {
                finalFrame = CameraFrames.Last<CameraFrameData>().FrameNumber;
            }
        }

        public void Start(float startFrame = 0f, bool needReplay = false)
        {
            stopWatch.Start();
            currentFrame = startFrame;
            isPlaying = true;
            this.needReplay = needReplay;
        }

        public void Stop()
        {
            stopWatch.Stop();
            isPlaying = false;
        }

        private void Leap(CameraProvider cp, IProjectionMatrixProvider projection, float frame)
        {
            if (CameraFrames.Count != 0)
            {
                for (int i = 0; i < CameraFrames.Count - 1; i++)
                {
                    if (CameraFrames[i].FrameNumber < frame && CameraFrames[i + 1].FrameNumber >= frame)
                    {
                        uint num = CameraFrames[i + 1].FrameNumber - CameraFrames[i].FrameNumber;
                        float f = (frame - CameraFrames[i].FrameNumber) / num;
                        LeapFrame(CameraFrames[i], CameraFrames[i + 1], cp, projection, f);
                        return;
                    }
                }
                LeapFrame(CameraFrames.Last<CameraFrameData>(), CameraFrames.Last<CameraFrameData>(), cp, projection, 0f);
            }
        }

        private void LeapFrame(CameraFrameData cf1, CameraFrameData cf2, CameraProvider cp, IProjectionMatrixProvider proj, float f)
        {
            float x = cf1.Curves[0].Evaluate(f);
            float y = cf1.Curves[1].Evaluate(f);
            float z = cf1.Curves[2].Evaluate(f);
            float progress = cf1.Curves[3].Evaluate(f);
            float factor = cf1.Curves[4].Evaluate(f);
            float factor2 = cf1.Curves[5].Evaluate(f);
            cp.CameraLookAt = CGHelper.ComplementTranslate(cf1, cf2, new Vector3(x, y, z));
            Quaternion rotation = CGHelper.ComplementRotateQuaternion(cf1, cf2, progress);
            float scale = CGHelper.Lerp(cf1.Distance, cf2.Distance, factor);
            float degree = CGHelper.Lerp(cf1.ViewAngle, cf2.ViewAngle, factor2);
            Vector3 vector = Vector3.TransformCoordinate(new Vector3(0f, 0f, 1f), Matrix.RotationQuaternion(rotation));
            Vector3 cameraPosition = cp.CameraLookAt + scale * vector;
            cp.CameraPosition = cameraPosition;
            proj.Fovy = CGHelper.ToRadians(degree);
        }

        public void UpdateCamera(CameraProvider cp, IProjectionMatrixProvider proj)
        {
            if (lastMillisecound == 0L)
            {
                lastMillisecound = stopWatch.ElapsedMilliseconds;
            }
            else
            {
                long elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                long num = elapsedMilliseconds - lastMillisecound;
                if (isPlaying)
                {
                    currentFrame += num / 30f;
                }
                if (needReplay && finalFrame < currentFrame)
                {
                    currentFrame = 0f;
                }
                lastMillisecound = elapsedMilliseconds;
            }
            Leap(cp, proj, currentFrame);
        }
    }
}
