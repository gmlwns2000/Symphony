using NPlayer.DSP.NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPlayer
{
    public class nPlayerEQ : nPlayerDSP
    {
        public nPlayerEQBand[] bands;
        private BiQuadFilter[,] filters;
        private nPlayerCore player;
        private int channels;
        private int bandCount;

        public float preAmp;

        private void InitClass()
        {
            Name = "이퀄라이져";
            Describe = "대역대별로 소리의 크기를 조절합니다.";
            Author = "Ap7 Studio";
        }

        public nPlayerEQ(ref nPlayerCore dsp, nPlayerEQBand[] bands, float preAmp, bool on, float opacity)
        {
            player = dsp;
            DSPmaster = dsp.DSPMaster;
            this.bands = bands;
            this.opacity = opacity;
            this.on = on;
            this.preAmp = preAmp;

            InitClass();
        }

        public nPlayerEQ(ref nPlayerCore dsp)
        {
            player = dsp;
            DSPmaster = dsp.DSPMaster;
            this.opacity = 1f;
            this.preAmp = 1f;
            this.on = true;
            #region Equlizer Bands Difine
            bands = new nPlayerEQBand[]
                    {
                        new nPlayerEQBand {Bandwidth = 0.35f, Frequency = 31, Gain = 0},     //1
                        new nPlayerEQBand {Bandwidth = 0.35f, Frequency = 62, Gain = 0},     //2
                        new nPlayerEQBand {Bandwidth = 0.35f, Frequency = 150, Gain = 0},    //3
                        new nPlayerEQBand {Bandwidth = 0.35f, Frequency = 240, Gain = 0},    //4
                        new nPlayerEQBand {Bandwidth = 0.35f, Frequency = 420, Gain = 0},    //5
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 650, Gain = 0},    //6
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 900, Gain = 0},    //7
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 1200, Gain = 0},   //8
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 1500, Gain = 0},   //9
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 2200, Gain = 0},   //10
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 3400, Gain = 0},   //11
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 4600, Gain = 0},   //12
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 6000, Gain = 0},   //13
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 7800, Gain = 0},   //14
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 9000, Gain = 0},   //15
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 12000, Gain = 0},  //16
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 16000, Gain = 0},  //17
                        new nPlayerEQBand {Bandwidth = 0.75f, Frequency = 20000, Gain = 0},  //18
                    };
            #endregion Equlizer Bands Difine
            InitClass();
        }

        public override float Apply(int ch, float sample, int index, int count)
        {
            if ((opacity != 0.0f) && on)
            {
                float sp = sample;
                for (int band = 0; band < bandCount; band++)
                {
                    if (bands[band].Gain != 0)
                    {
                        sp = filters[ch, band].Transform(sp);
                    }
                }
                return ((sp * preAmp) * opacity + sample * (1 - opacity));
            }
            else
            {
                return sample;
            }
        }

        public void UpdateEqBands()
        {
            if(DSPmaster == null)
            {
                return;
            }

            bandCount = 0;

            for(int i=0; i<bands.Length; i++)
            {
                if(bands[i].Frequency <= DSPmaster.WaveFormat.SampleRate*0.5)
                {
                    bandCount++;
                }
            }

            for (int bandIndex = 0; bandIndex < bandCount; bandIndex++)
            {
                nPlayerEQBand band = bands[bandIndex];
                for (int n = 0; n < DSPmaster.WaveFormat.Channels; n++)
                {
                    if (filters[n, bandIndex] == null)
                        filters[n, bandIndex] = BiQuadFilter.PeakingEQ(DSPmaster.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                    else
                        filters[n, bandIndex].SetPeakingEq(DSPmaster.WaveFormat.SampleRate, band.Frequency, band.Bandwidth, band.Gain);
                }
            }

            player.log.dlog("Eqaulizer is inited: ActualBandCount: " + bandCount.ToString() + ", BandCount:" + bands.Length);
        }

        public override void SetStatus(bool on)
        {
            this.on = on;
        }

        public override void SetOpacity(float opacity)
        {
            this.opacity = opacity;
        }

        public void ChangeAmp(float preAmp)
        {
            this.preAmp = preAmp;
        }

        public int GetEqLength()
        {
            return bands.Length;
        }

        public void ChangeEQ(nPlayerEQBand[] bands)
        {
            this.bands = bands;

            if (player.isPlay)
            {
                UpdateEqBands();
            }
        }

        public void ChangeEQ(int band_id, float gain)
        {
            if (bands[band_id].Gain != gain)
            {
                bands[band_id].Gain = gain;
            }
            if (player.isPlay)
            {
                UpdateEqBands();
            }
        }

        public void ChangeEQ(int band_id, float gain, float band, float freq)
        {
            if ((bands[band_id].Gain != gain) && (gain != -1))
            {
                bands[band_id].Gain = gain;
            }
            if ((bands[band_id].Bandwidth != band)&& (band != -1))
            {
                bands[band_id].Bandwidth = band;
            }
            if ((bands[band_id].Frequency != freq)&&(freq!=-1))
            {
                bands[band_id].Frequency = freq;
            }
            if (player.isPlay)
            {
                UpdateEqBands();
            }
        }

        public override void Init(nPlayerDSPMaster master)
        {
            DSPmaster = master;
            channels = DSPmaster.WaveFormat.Channels;
            bandCount = bands.Length;
            filters = new BiQuadFilter[channels, bands.Length];
            UpdateEqBands();
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
