using MMDFileParser;
using MMDFileParser.MotionParser;

namespace MMF.Motion
{
    public class MorphMotion
    {
        private FrameManager frameManager = new FrameManager();

        public string MorphName
        {
            get;
            private set;
        }

        public MorphMotion(string morphName)
        {
            MorphName = morphName;
        }

        public void AddMorphFrameData(MorphFrameData morphFrameData)
        {
            frameManager.AddFrameData(morphFrameData);
        }

        public void SortMorphFrameDatas()
        {
            frameManager.SortFrameDatas();
        }

        public float GetMorphValue(float frameNumber)
        {
            IFrameData frameData;
            IFrameData frameData2;
            frameManager.SearchKeyFrame(frameNumber, out frameData, out frameData2);
            MorphFrameData morphFrameData = (MorphFrameData)frameData;
            MorphFrameData morphFrameData2 = (MorphFrameData)frameData2;
            float factor = (morphFrameData2.FrameNumber == morphFrameData.FrameNumber) ? 0f : ((frameNumber - morphFrameData.FrameNumber) / (morphFrameData2.FrameNumber - morphFrameData.FrameNumber));
            return MMF.Utility.CGHelper.Lerp(morphFrameData.MorphValue, morphFrameData2.MorphValue, factor);
        }
    }
}
