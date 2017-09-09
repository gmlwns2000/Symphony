using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPlayer
{
    public class nPlayerLimiter : nPlayerDSP
    {
        public float limit;
        public float samplerate;
        public float strength;

        private bool inited = false;
        private float preAmp = 1;
        private float Amp = 1;
        private float max = 0;
        private nPlayerDSPMaster Master;

        public nPlayerLimiter(float limit = 1f, float strength = 0.05f)
        {
            this.limit = limit;
            this.strength = strength;
            Name = "리미터";
            Author = "Ap7 Studio";
            Describe = "음악의 소리크기를 부드럽게 제한합니다.";
        }

        public override float Apply(int channel, float sample, int index, int length)
        {
            if (index == 0)
            {
                max = 0;
            }
            else
            {
                if (max < sample)
                {
                    max = sample;
                }
            }
            return sample;
        }

        public override void Init(nPlayerDSPMaster master)
        {
            Master = master;
            inited = true;
        }

        public void SetParamater(float limit = 1f, float strength = 0.5f) {
            this.limit = limit;
            this.strength = strength;
        }

        public override void SetOpacity(float opacity)
        {
            //
        }

        public override void SetStatus(bool on)
        {
            this.on = on;
        }

        public override DSPCalcPoint GetCalcPoint()
        {
            return DSPCalcPoint.Everytime;
        }

        public override float[] ArrayApply(float[] buffer, int offset, int count)
        {
            if (on)
            {
                if (inited)
                {
                    float[] buf = buffer;
                    preAmp = Amp;
                    if (max > limit)
                    {
                        Amp = limit / max;
                    }
                    else
                    {
                        Amp = 1;
                    }

                    for (int i = 0; i < (offset + count); i++)
                    {
                        if ((i <= (int)((offset + count) * strength))&&(strength !=1))
                        {
                            buf[i] = Math.Max(-1, Math.Min( 1, buf[i] * preAmp + buf[i] * (Amp - preAmp) / ((offset + count) * (1-strength)) * i));
                        }
                        else
                        {
                            buf[i] = Math.Max(-1, Math.Min(1, buf[i] * Amp));
                        }
                    }
                    return buf;
                }
                else
                {
                    return buffer;
                }
            }
            else
            {
                return buffer;
            }
        }
    }
}
