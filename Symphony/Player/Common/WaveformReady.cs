using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Player
{
    public class WaveformReady : EventArgs
    {
        public float[] Buffer;
        public int MaxLength;

        public WaveformReady(float[] buf, int maxLength)
        {
            Buffer = buf;
            MaxLength = maxLength;
        }
    }
}
