using System.Collections.Generic;
using System.Linq;

namespace MMDFileParser
{
    public class FrameManager
    {
        private List<IFrameData> frameDatas = new List<IFrameData>();

        private int beforePastFrameIndex = 0;

        public void AddFrameData(IFrameData frameData)
        {
            frameDatas.Add(frameData);
        }

        public void SortFrameDatas()
        {
            frameDatas.Sort();
        }

        public bool IsSorted()
        {
            uint frameNumber = frameDatas[0].FrameNumber;
            bool result;
            foreach (IFrameData current in frameDatas)
            {
                if (frameNumber > current.FrameNumber)
                {
                    result = false;
                    return result;
                }
                frameNumber = current.FrameNumber;
            }
            result = true;
            return result;
        }

        public uint GetFinalFrameNumber()
        {
            return frameDatas.Last<IFrameData>().FrameNumber;
        }

        public void SearchKeyFrame(float frameNumber, out IFrameData pastFrame, out IFrameData futureFrame)
        {
            if (frameNumber < frameDatas.First<IFrameData>().FrameNumber)
            {
                IFrameData frameData;
                futureFrame = (frameData = frameDatas.First<IFrameData>());
                pastFrame = frameData;
            }
            else if (frameNumber >= frameDatas.Last<IFrameData>().FrameNumber)
            {
                IFrameData frameData;
                futureFrame = (frameData = frameDatas.Last<IFrameData>());
                pastFrame = frameData;
            }
            else
            {
                int num;
                if (frameDatas[beforePastFrameIndex].FrameNumber < frameNumber)
                {
                    num = frameDatas.FindIndex(beforePastFrameIndex, (IFrameData b) => b.FrameNumber > frameNumber);
                }
                else
                {
                    num = frameDatas.FindIndex((IFrameData b) => b.FrameNumber > frameNumber);
                }
                pastFrame = frameDatas[num - 1];
                futureFrame = frameDatas[num];
                beforePastFrameIndex = num - 1;
            }
        }
    }
}
