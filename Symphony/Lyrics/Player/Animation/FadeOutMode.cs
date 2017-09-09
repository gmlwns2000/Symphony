using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Lyrics
{
    public enum FadeOutMode
    {
        Auto = -1,

        None = 0,

        FadeOut = 1,

        BlurOut = 2,

        ZoomIn = 3,

        ZoomOut = 4,

        SlideToLeft = 5,

        SlideToRight = 6,

        SlideToTop = 7,

        SlideToBottom = 8,

        RotateClock = 9,

        RotateCounterClock = 10,

        UserAnimation = 255
    }
}
