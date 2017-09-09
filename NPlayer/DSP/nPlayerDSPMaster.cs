using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSCore;
using CSCore.DSP;
using CSCore.Codecs;
using CSCore.SoundOut;
using NPlayer.DSP.CSCore;

namespace NPlayer
{
    public class nPlayerDSPMaster : ISampleSource
    {
        private ISampleSource sourceProvider;
        private readonly int channels;
        private readonly nPlayerCore np;

        public event EventHandler<SampleEndEventArgs> SampleBegin;
        public event EventHandler<SampleEndEventArgs> SampleEnd;

        public WaveFormat WaveFormat { get { return sourceProvider.WaveFormat; } }
        public bool UseDspProcessing = true;

        private void UpdateSRLimit()
        {
            if (UseSamplerateLimit && sourceProvider != null && SampleRate >= SamplerateLimit)
            {
                CheckSamplerateLimit = false;
            }
            else
            {
                CheckSamplerateLimit = true;
            }
        }

        private bool _useSamplrateLimit = true;
        public bool UseSamplerateLimit
        {
            get { return _useSamplrateLimit; }
            set
            {
                _useSamplrateLimit = value;

                UpdateSRLimit();
            }
        }

        private int _samplerateLimit = 96000;
        public int SamplerateLimit
        {
            get { return _samplerateLimit; }
            set
            {
                _samplerateLimit = value;

                UpdateSRLimit();
            }
        }
        
        /// <summary>
        /// Avaliable of DSP depand on Samerate of SourceProvider. 
        /// </summary>
        public bool CheckSamplerateLimit = true;

        public bool CanSeek
        {
            get
            {
                return sourceProvider.CanSeek;
            }
        }

        public long Position
        {
            get
            {
                return sourceProvider.Position;
            }

            set
            {
                sourceProvider.Position = value;
            }
        }

        public long Length
        {
            get
            {
                return sourceProvider.Length;
            }
        }

        public int Channel
        {
            get
            {
                return WaveFormat.Channels;
            }
        }

        public int SampleRate
        {
            get
            {
                return WaveFormat.SampleRate;
            }
        }

        public object DSPLocker = new object();

        public nPlayerDSP[] DSPs;
        private double ms = 0;
        private Stopwatch clock;
        private bool debug;

        public nPlayerDSPMaster(bool debug, nPlayerCore np, ISampleSource sourceProvider)
        {
            this.debug = debug;
            this.sourceProvider = sourceProvider;
            channels = sourceProvider.WaveFormat.Channels;
            this.np = np;

            DSPs = new nPlayerDSP[0];

            UpdateSRLimit();

            if (debug)
            {
                clock = new Stopwatch();
                clock.Start();
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = 0;

            if (debug)
            {
                if (clock.ElapsedMilliseconds - ms > np.DesiredLatency * 1.33)
                {
                    Console.WriteLine("Too Slow Buffer {0}" , clock.ElapsedMilliseconds - ms);
                }

                ms = clock.ElapsedMilliseconds;
            }

            samplesRead = sourceProvider.Read(buffer, offset, count);

            SampleBegin?.Invoke(this, new SampleEndEventArgs(buffer, offset, samplesRead));

            if (UseDspProcessing && CheckSamplerateLimit)
            {
                lock (DSPLocker)
                {
                    //OnDsp
                    for (int i = 0; i < DSPs.Length; i++)
                    {
                        if ((DSPs[i].GetCalcPoint() == DSPCalcPoint.OnDSP) || (DSPs[i].GetCalcPoint() == DSPCalcPoint.Everytime))
                        {
                            try
                            {
                                for (int n = 0; n < samplesRead; n++)
                                {
                                    buffer[offset + n] = DSPs[i].Apply(n % channels, buffer[offset + n], n, samplesRead);
                                }
                            }
                            catch (Exception e)
                            {
                                try
                                {
                                    np.log.derr("Exception is occured during sample processing " + DSPs[i].ToString() + ". \n DETAILS:  " + e.ToString());
                                }
                                catch
                                {
                                    np.log.derr("ERRORED! DURING ON DSP! " + e.ToString());
                                }
                            }
                        }
                    }

                    //After DSP Calc
                    for (int i = 0; i < DSPs.Length; i++)
                    {
                        if ((DSPs[i].GetCalcPoint() == DSPCalcPoint.AfterDSP) || (DSPs[i].GetCalcPoint() == DSPCalcPoint.Everytime))
                        {
                            try
                            {
                                buffer = DSPs[i].ArrayApply(buffer, offset, samplesRead);
                            }
                            catch (Exception e)
                            {
                                try
                                {
                                    np.log.derr("Exception is occured during buffer processing " + DSPs[i].ToString() + ". \n DETAILS:  " + e.ToString());
                                }
                                catch
                                {
                                    np.log.derr("ERRORED! DURING AFTER DSP! " + e.ToString());
                                }
                            }
                        }
                    }
                }
            }

            SampleEnd?.Invoke(this, new SampleEndEventArgs(buffer, offset, samplesRead));

            return samplesRead;
        }

        public void Dispose()
        {
            if (clock != null)
            {
                clock.Stop();
            }
        }
    }
}