using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Lyrics
{
    public enum FadeInMode
    {
        Auto = -1,

        None = 0,

        FadeIn = 1,

        BlurIn = 2,

        ZoomIn = 3,

        ZoomOut = 4,

        SlideFromLeft = 5,

        SlideFromRight = 6,

        SlideFromTop = 7,

        SlideFromBottom = 8,

        RotateClock = 9,

        RotateCounterClock = 10,

        UserAnimation = 255
    }
}
