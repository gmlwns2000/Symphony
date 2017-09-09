using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPlayer
{
    public class nPlayerEcho : nPlayerDSP
    {
        /// <summary>
        /// Echo Length in ms
        /// </summary>
        public int EchoLength { get; private set; }
        public float _echoFactor;
        public float EchoFactor
        {
            get
            {
                return _echoFactor;
            }
            set
            {
                _echoFactor = value;
            }
        }

        private Queue<float> samples;

        public nPlayerEcho(int length = 750, float factor = 0.3f)
        {
            this.EchoLength = length+1;
            this.EchoFactor = factor;

            Name = "에코";
            Describe = "소리를 울리게하는 간단한 효과입니다.";
            Author = "Ap7 Studio";
        }

        public override float Apply(int channel, float sample, int index, int count)
        {
            float smp = (1-EchoFactor)*sample + _echoFactor * samples.Dequeue();
            samples.Enqueue(smp);
            if (on)
            {
                return smp;
            }
            else
            {
                return sample;
            }
        }

        public override void SetStatus(bool on)
        {
            this.on = on;
        }

        public override void SetOpacity(float opacity)
        {
            //throw new NotImplementedException();
        }

        public override void Init(nPlayerDSPMaster master)
        {
            samples = new Queue<float>();
            for (int i = 0; i < Math.Max(1, master.SampleRate * ((double)EchoLength/1000)); i++) { samples.Enqueue(0f); };
        }

        public override DSPCalcPoint GetCalcPoint()
        {
            return DSPCalcPoint.OnDSP;
        }

        public override float[] ArrayApply(float[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
