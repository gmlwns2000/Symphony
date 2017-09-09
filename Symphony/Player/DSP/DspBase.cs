using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symphony.Player.DSP
{
    public abstract class DSPBase
    {
        private static int IDIndex = 0;

        public DSPBase()
        {
            SID = IDIndex;
            IDIndex++;
        }

        public int SID;

        public bool on = true;
        public float opacity = 1;

        public string Name = "";
        public string Author = "";
        public string Describe = "";

        public DSPMaster DSPmaster;

        public abstract void SetStatus(bool on);

        public abstract void SetOpacity(float opacity);

        public abstract DSPCalcPoint GetCalcPoint();

        public abstract float[] ArrayApply(float[] buffer, int offset, int count);

        public abstract float Apply(int channel, float sample, int index, int count);

        public abstract void Init(DSPMaster master);
    }
}
