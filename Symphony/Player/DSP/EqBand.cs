using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symphony.Player.DSP
{
    public class EqBand
    {
        public float Frequency { get; set; }
        public float Gain { get; set; }
        public float Bandwidth { get; set; }
    }
}