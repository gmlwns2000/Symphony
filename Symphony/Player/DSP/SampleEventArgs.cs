﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symphony.Player.DSP
{
    public class SampleEventArgs : EventArgs
    {
        public float Left;
        public float Right;

        public SampleEventArgs(float left, float right)
        {
            Left = left;
            Right = right;
        }
    }
}
