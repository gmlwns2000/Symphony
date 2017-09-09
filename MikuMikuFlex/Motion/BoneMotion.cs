using MMDFileParser;
using MMDFileParser.MotionParser;
using MMF.Bone;
using SlimDX;

namespace MMF.Motion
{
    internal class BoneMotion
    {
        private PMXBone bone;

        private FrameManager frameManager = new FrameManager();

        public BoneMotion(PMXBone bone)
        {
            this.bone = bone;
        }

        public void AddBoneFrameData(BoneFrameData boneFrameData)
        {
            frameManager.AddFrameData(boneFrameData);
        }

        public void SortBoneFrameDatas()
        {
            frameManager.SortFrameDatas();
        }

        public uint GetFinalFrameNumber()
        {
            return frameManager.GetFinalFrameNumber();
        }

        public string GetBoneName()
        {
            return bone.BoneName;
        }

        public void ReviseBone(float frameNumber)
        {
            IFrameData frameData;
            IFrameData frameData2;
            frameManager.SearchKeyFrame(frameNumber, out frameData, out frameData2);
            BoneFrameData boneFrameData = (BoneFrameData)frameData;
            BoneFrameData boneFrameData2 = (BoneFrameData)frameData2;
            float progress = (boneFrameData2.FrameNumber == boneFrameData.FrameNumber) ? 0f : ((frameNumber - boneFrameData.FrameNumber) / (boneFrameData2.FrameNumber - boneFrameData.FrameNumber));
            float[] array = new float[4];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = boneFrameData.Curves[i].Evaluate(progress);
            }
            bone.Translation = MMF.Utility.CGHelper.ComplementTranslate(boneFrameData, boneFrameData2, new Vector3(array[0], array[1], array[2]));
            bone.Rotation = MMF.Utility.CGHelper.ComplementRotateQuaternion(boneFrameData, boneFrameData2, array[3]);
        }
    }
}
