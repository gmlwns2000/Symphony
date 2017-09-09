using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Player.DSP
{
    /// <summary>
    /// Nplayer Default Stereo Enhancer. Factor, Preamp, Opacity, On is Supported.
    /// </summary>
    public class StereoEnhancer : DSPBase
    {
        /// <summary>
        /// Stereo Enhance Factor.
        /// </summary>
        public float Factor = 1.5f;
        /// <summary>
        /// Previous Amp
        /// </summary>
        public float PreAmp = 0.8f;

        public override float Apply(int channel, float sample, int index, int count)
        {
            throw new NotImplementedException();
        }

        int chInd = 0;
        float mono = 0;
        float delta = 0;
        float fact = 0;

        public override float[] ArrayApply(float[] buffer, int offset, int count)
        {
            if (on)
            {
                fact = (Factor - 1) * opacity + 1;

                for (int i = offset; i < offset + count; i++)
                {
                    chInd++;
                    if (chInd == 2)
                    {
                        chInd = 0;

                        mono = buffer[i - 1] * 0.5f + buffer[i] * 0.5f;
                        delta = (buffer[i] - mono) * fact;
                        buffer[i] = (mono + delta) * PreAmp;
                        buffer[i - 1] = (mono - delta) * PreAmp;
                    }
                }
            }

            return buffer;
        }

        public override DSPCalcPoint GetCalcPoint()
        {
            return DSPCalcPoint.AfterDSP;
        }

        public override void Init(DSPMaster master)
        {
            
        }

        public override void SetOpacity(float opacity)
        {
            this.opacity = opacity;
        }

        public override void SetStatus(bool on)
        {
            this.on = on;
        }
    }
}
