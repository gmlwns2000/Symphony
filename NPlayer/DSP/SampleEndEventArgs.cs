using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPlayer
{
    public class SampleEndEventArgs : EventArgs
    {
        public float[] Buffer;
        public int Index;
        public int Count;

        public SampleEndEventArgs(float[] buffer, int index, int count)
        {
            Buffer = buffer;
            Index = index;
            Count = count;
        }
    }
}
