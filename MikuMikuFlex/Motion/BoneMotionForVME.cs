using MMDFileParser;
using MMDFileParser.MotionParser;
using MMF.Bone;
using OpenMMDFormat;
using SlimDX;

namespace MMF.Motion
{
    internal class BoneMotionForVME
    {
        private PMXBone bone;

        private FrameManager frameManager = new FrameManager();

        public BoneMotionForVME(PMXBone bone, System.Collections.Generic.List<BoneFrame> boneFrames)
        {
            this.bone = bone;
            foreach (BoneFrame current in boneFrames)
            {
                frameManager.AddFrameData(current);
            }
            if (!frameManager.IsSorted())
            {
                throw new System.Exception("VMEデータがソートされていません");
            }
        }

        public uint GetFinalFrame()
        {
            return frameManager.GetFinalFrameNumber();
        }

        public void ReviseBone(ulong frameNumber)
        {
            IFrameData frameData;
            IFrameData frameData2;
            frameManager.SearchKeyFrame(frameNumber, out frameData, out frameData2);
            BoneFrame boneFrame = (BoneFrame)frameData;
            BoneFrame boneFrame2 = (BoneFrame)frameData2;
            float num = (boneFrame.frameNumber == boneFrame2.frameNumber) ? 0f : ((frameNumber - boneFrame.frameNumber) / (boneFrame2.frameNumber - boneFrame.frameNumber));
            BezInterpolParams interpolParameters = boneFrame.interpolParameters;
            float factor;
            float factor2;
            float factor3;
            float amount;
            if (interpolParameters != null)
            {
                factor = BezEvaluate(interpolParameters.X1, interpolParameters.X2, num);
                factor2 = BezEvaluate(interpolParameters.Y1, interpolParameters.Y2, num);
                factor3 = BezEvaluate(interpolParameters.Z1, interpolParameters.Z2, num);
                amount = BezEvaluate(interpolParameters.R1, interpolParameters.R2, num);
            }
            else
            {
                factor2 = (factor = (factor3 = (amount = num)));
            }
            bone.Translation = new Vector3(MMF.Utility.CGHelper.Lerp(boneFrame.position.x, boneFrame2.position.x, factor), MMF.Utility.CGHelper.Lerp(boneFrame.position.y, boneFrame2.position.y, factor2), MMF.Utility.CGHelper.Lerp(boneFrame.position.z, boneFrame2.position.z, factor3));
            bone.Rotation = Quaternion.Slerp(boneFrame.rotation.ToSlimDX(), boneFrame2.rotation.ToSlimDX(), amount);
        }

        private float BezEvaluate(bvec2 v1, bvec2 v2, float s)
        {
            return new BezierCurve
            {
                v1 = v1.ToSlimDX() / 127f,
                v2 = v2.ToSlimDX() / 127f
            }.Evaluate(s);
        }
    }
}
