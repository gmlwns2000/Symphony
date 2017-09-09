using System;

namespace MMDFileParser
{
    public interface IFrameData : IComparable
    {
        uint FrameNumber
        {
            get;
        }
    }
}
