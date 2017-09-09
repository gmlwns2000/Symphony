using MMDFileParser;
using OpenMMDFormat;

namespace MMF.Motion
{
    internal class MorphMotionForVME
    {
        private FrameManager frameManager = new FrameManager();

        public string MorphName
        {
            get;
            private set;
        }

        public MorphMotionForVME(string morphName, System.Collections.Generic.List<MorphFrame> morphFrames)
        {
            MorphName = MorphName;
            foreach (MorphFrame current in morphFrames)
            {
                frameManager.AddFrameData(current);
            }
            if (!frameManager.IsSorted())
            {
                throw new System.Exception("VMEデータがソートされていません");
            }
        }

        public float GetMorphValue(ulong frameNumber)
        {
            IFrameData frameData;
            IFrameData frameData2;
            frameManager.SearchKeyFrame(frameNumber, out frameData, out frameData2);
            MorphFrame morphFrame = (MorphFrame)frameData;
            MorphFrame morphFrame2 = (MorphFrame)frameData2;
            float factor = (morphFrame2.frameNumber == morphFrame.frameNumber) ? 0f : ((frameNumber - morphFrame.frameNumber) / (morphFrame2.frameNumber - morphFrame.frameNumber));
            return MMF.Utility.CGHelper.Lerp(morphFrame.value, morphFrame2.value, factor);
        }
    }
}
