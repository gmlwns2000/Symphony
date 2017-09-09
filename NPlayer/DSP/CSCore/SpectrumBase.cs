using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using CSCore;
using CSCore.DSP;
using NPlayer.Common;

namespace NPlayer.DSP.CSCore
{ 
    public class SpectrumBase : INotifyPropertyChanged
    {
        public const int ScaleFactorLinear = 9;
        public const int ScaleFactorSqr = 2;
        public const double MinDbValue = -72;
        public const double MaxDbValue = 0;
        public const double DbScale = (MaxDbValue - MinDbValue);

        private int _fftSize;
        private bool _isXLogScale;
        private int _maxFftIndex;
        private int _maximumFrequency = 20000;
        private int _maximumFrequencyIndex;
        private int _minimumFrequency = 20; //Default spectrum from 20Hz to 20kHz
        private int _minimumFrequencyIndex;
        private ScalingStrategy _scalingStrategy;
        private double[] _spectrumIndexMax;
        private double[] _spectrumLogScaleIndexMax;
        private ISpectrumProvider _spectrumProvider;

        protected int SpectrumResolution;
        private bool _useAverage;

        public int MaximumFrequency
        {
            get { return _maximumFrequency; }
            set
            {
                if (value <= MinimumFrequency)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "Value must not be less or equal the MinimumFrequency.");
                }
                _maximumFrequency = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("MaximumFrequency");
            }
        }

        public int MinimumFrequency
        {
            get { return _minimumFrequency; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                _minimumFrequency = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("MinimumFrequency");
            }
        }

        [Browsable(false)]
        public ISpectrumProvider SpectrumProvider
        {
            get { return _spectrumProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _spectrumProvider = value;

                RaisePropertyChanged("SpectrumProvider");
            }
        }

        public bool IsXLogScale
        {
            get { return _isXLogScale; }
            set
            {
                _isXLogScale = value;
                UpdateFrequencyMapping();
                RaisePropertyChanged("IsXLogScale");
            }
        }

        public ScalingStrategy ScalingStrategy
        {
            get { return _scalingStrategy; }
            set
            {
                _scalingStrategy = value;
                RaisePropertyChanged("ScalingStrategy");
            }
        }

        public bool UseAverage
        { 
            get { return _useAverage; }
            set
            {
                _useAverage = value;
                RaisePropertyChanged("UseAverage");
            }
        }
        
        public FftSize FftSize
        {
            get { return (FftSize) _fftSize; }
            protected set
            {
                if ((int) Math.Log((int) value, 2) % 1 != 0)
                    throw new ArgumentOutOfRangeException("value");

                _fftSize = (int) value;
                _maxFftIndex = _fftSize / 2 - 1;

                RaisePropertyChanged("FFTSize");
            }
        }

        private bool _useResampling = true;
        public bool UseResampling
        {
            get { return _useResampling; }
            set { _useResampling = value; }
        }

        private ResamplingMode _resampleMode = ResamplingMode.Linear;
        public ResamplingMode ResamplingMode
        {
            get
            {
                return _resampleMode;
            }
            set
            {
                _resampleMode = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void UpdateFrequencyMapping()
        {
            _maximumFrequencyIndex = Math.Min(_spectrumProvider.GetFftBandIndex(MaximumFrequency) + 1, _maxFftIndex);
            _minimumFrequencyIndex = Math.Min(_spectrumProvider.GetFftBandIndex(MinimumFrequency), _maxFftIndex);

            int actualResolution = SpectrumResolution;

            int indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
            double linearIndexBucketSize = Math.Round(indexCount / (double) actualResolution, 3);

            _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, true);
            _spectrumLogScaleIndexMax = _spectrumLogScaleIndexMax.CheckBuffer(actualResolution, true);

            double maxLog = Math.Log(actualResolution, actualResolution);
            for (int i = 1; i < actualResolution; i++)
            {
                double logIndex = ((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) + _minimumFrequencyIndex;

                _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (i * linearIndexBucketSize);
                _spectrumLogScaleIndexMax[i - 1] = logIndex;
            }

            if (actualResolution > 0)
            {
                _spectrumIndexMax[_spectrumIndexMax.Length - 1] = _maximumFrequencyIndex;
                _spectrumLogScaleIndexMax[_spectrumLogScaleIndexMax.Length - 1] = _maximumFrequencyIndex;
            }
        }

        protected virtual SpectrumPointData[] CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
        {
            var dataPoints = new List<SpectrumPointData>();
            bool _resample = UseResampling;
            
            //get index
            if (IsXLogScale)
            {
                for (int i = 0; i < _spectrumLogScaleIndexMax.Length; i++)
                {
                    dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = _spectrumLogScaleIndexMax[i], Value = 0});
                }
            }
            else
            {
                for (int i = 0; i < _spectrumIndexMax.Length; i++)
                {
                    dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = _spectrumIndexMax[i], Value = 0 });
                }
            }

            //calc value
            double value0 = 0, value = 0;
            double actualMaxValue = maxValue;

            double[] calcValues = new double[_maximumFrequencyIndex - _minimumFrequencyIndex + 1];

            for (int i_fft = _minimumFrequencyIndex; i_fft <= _maximumFrequencyIndex; i_fft++)
            {
                switch (ScalingStrategy)
                {
                    case ScalingStrategy.Decibel:
                        value0 = (((20 * Math.Log10(fftBuffer[i_fft])) - MinDbValue) / DbScale) * actualMaxValue;
                        break;
                    case ScalingStrategy.Linear:
                        value0 = (fftBuffer[i_fft] * ScaleFactorLinear) * actualMaxValue;
                        break;
                    case ScalingStrategy.Sqrt:
                        value0 = ((Math.Sqrt(fftBuffer[i_fft])) * ScaleFactorSqr) * actualMaxValue;
                        break;
                }

                value = Math.Max(0, Math.Max(value0, value));

                calcValues[i_fft - _minimumFrequencyIndex] = value;

                value = 0;
            }

            //set Value, and resampling
            for(int i=0; i<dataPoints.Count; i++)
            {
                SpectrumPointData pt = dataPoints[i];
                if (_resample && calcValues.Length > (int)pt.SpectrumPointIndex - _minimumFrequencyIndex + 1)
                {
                    pt.Value = Resample(_resampleMode, pt.SpectrumPointIndex % 1, calcValues[(int)pt.SpectrumPointIndex - _minimumFrequencyIndex], calcValues[(int)pt.SpectrumPointIndex - _minimumFrequencyIndex + 1]);
                }
                else
                {
                    pt.Value = calcValues[(int)pt.SpectrumPointIndex - _minimumFrequencyIndex];
                }
                dataPoints[i] = pt;
            }

            return dataPoints.ToArray();
        }
        
        private double Resample(ResamplingMode mode, double offset, double now, double next)
        {
            switch (mode)
            {
                default:
                case ResamplingMode.Linear:
                    return now + (next - now) * offset;
                case ResamplingMode.HalfSine:
                    if (now > next)
                    {
                        return now + (next - now) * (Math.Sin((offset + 3) * Math.PI * 0.5) + 1);
                    }
                    else
                    {
                        return now + (next - now) * Math.Sin(offset * Math.PI * 0.5);
                    }
                case ResamplingMode.FullSine:
                    return now + (next - now) * (Math.Sin(offset * Math.PI - 0.5 * Math.PI) * 0.5 + 0.5);
            }

        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DebuggerDisplay("{Value}")]
        public struct SpectrumPointData
        {
            public double SpectrumPointIndex;
            public double Value;
        }
    }
}