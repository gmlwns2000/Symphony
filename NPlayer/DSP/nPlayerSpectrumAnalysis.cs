using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSCore;
using CSCore.DSP;
using NPlayer.DSP.CSCore;

namespace NPlayer
{
    public class nPlayerSpectrumAnalysis : SpectrumBase
    {
        BasicSpectrumProvider spectrumProvider { get; set; }
        private int _barCount;
        private double _barSpacing;
        
        private double BarSpacing
        {
            get { return _barSpacing; }
            set
            {
                if (value != _barSpacing)
                {
                    _barSpacing = value;

                    RaisePropertyChanged("BarSpacing");
                    RaisePropertyChanged("BarWidth");
                }
            }
        }

        private int BarCount
        {
            get { return _barCount; }
            set
            {
                if (value != _barCount)
                {
                    if (value <= 0)
                        throw new ArgumentOutOfRangeException("value");
                    _barCount = value;
                    SpectrumResolution = value;
                    UpdateFrequencyMapping();

                    RaisePropertyChanged("BarCount");
                    RaisePropertyChanged("BarWidth");
                }
            }
        }

        public nPlayerSpectrumAnalysis()
        {
            FftSize = FftSize.Fft1024;
        }

        public void Init(nPlayerDSPMaster master)
        {
            if (SpectrumProvider != null)
            {
                SpectrumProvider = null;
            }
            
            spectrumProvider = new BasicSpectrumProvider(master.WaveFormat.Channels, master.WaveFormat.SampleRate, this.FftSize);
            SpectrumProvider = spectrumProvider;

            ScalingStrategy = ScalingStrategy.Decibel;
            IsXLogScale = true;
            UseAverage = false;
            UseResampling = false;
            MinimumFrequency = 50;
            MaximumFrequency = 25000;
        }

        int frame = 0;
        int fpp = 1;
        float[] mbuffer;

        public float[] ArrayApply(float[] buffer, int offset, int count)
        {
            mbuffer = new float[count];

            Array.Copy(buffer, mbuffer, count);

            fpp = frame+1;
            frame = 0;
            windowIndex = 0;

            return buffer;
        }

        int windowIndex = 0;

        public SpectrumPointData[] GetSpectrum(int bar_count, double bar_dash)
        {
            if (!on || SpectrumProvider == null || mbuffer == null || (mbuffer != null && mbuffer.Length < 100))
            {
                return null;
            }

            //windowing
            SpectrumPointData[] datas = null;
            int windowPos = mbuffer.Length / Math.Max(1, fpp) * windowIndex;
            int windowWidth = mbuffer.Length / Math.Max(1, fpp);
            int windowPadding = mbuffer.Length - (windowPos + windowWidth);
            if(windowPadding < 0)
            {
                windowWidth += windowPadding;
            }

            windowIndex++;
            frame++;
            if (windowIndex >= fpp)
            {
                windowIndex = 0;
            }

            float[] temp = new float[windowWidth];
            Array.Copy(mbuffer, windowPos, temp, 0, windowWidth);
            spectrumProvider.Add(temp, windowWidth);

            //calc
            BarSpacing = bar_dash;
            BarCount = bar_count;

            float[] fftBuffer = new float[(int)FftSize];

            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                datas = CalculateSpectrumPoints(1, fftBuffer);
            }

            return datas;
        }

        public SpectrumPointData[] GetSpectrum(float[] buffer, int bar_count, double bar_dash)
        {
            if (!on || SpectrumProvider == null)
            {
                return null;
            }

            //windowing
            SpectrumPointData[] datas = null;

            spectrumProvider.Add(buffer, buffer.Length);

            //calc
            BarSpacing = bar_dash;
            BarCount = bar_count;

            float[] fftBuffer = new float[(int)FftSize];

            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                datas = CalculateSpectrumPoints(1, fftBuffer);
            }

            return datas;
        }

        bool on = true;
        public void SetStatus(bool on)
        {
            this.on = on;
        }
    }
}
