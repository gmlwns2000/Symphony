using System;
using System.IO;
using System.Reflection;
using DirectCanvas.Misc;

namespace DirectCanvas.Effects
{
    public class RippleEffect : ShaderEffect
    {
        private const string SHADER_RESOURCE_NAME = "DirectCanvas.Effects.Shaders.Ripple.ps";
        private const string SHADER_ENTRY = "RippleMain";

        private enum RegisterTypes : int
        {
            Center = 0,
            Amplitude = 1,
            Frequency = 2,
            Phase = 3,
            LightIntensity = 4
        }

        private PointF m_center;
        private float m_amplitude;
        private float m_frequency;
        private float m_phase;
        private float m_lightIntensity;

        public RippleEffect(DirectCanvasFactory directCanvas) : base(directCanvas)
        {
            SetShaderSource(GetResourceString(SHADER_RESOURCE_NAME, Assembly.GetExecutingAssembly()), SHADER_ENTRY);

            RegisterProperty<PointF>((int)RegisterTypes.Center);
            RegisterProperty<float>((int)RegisterTypes.Amplitude);
            RegisterProperty<float>((int)RegisterTypes.Frequency);
            RegisterProperty<float>((int)RegisterTypes.Phase);
            RegisterProperty<float>((int)RegisterTypes.LightIntensity);
        }

        public PointF Center
        {
            get { return m_center; }
            set
            {
                m_center = value;
                SetValue((int)RegisterTypes.Center, m_center);
            }
        }

        public float Amplitude
        {
            get { return m_amplitude; }
            set
            {
                m_amplitude = value;
                SetValue((int)RegisterTypes.Amplitude, m_amplitude);
            }
        }

        public float Frequency
        {
            get { return m_frequency; }
            set
            {
                m_frequency = value;
                SetValue((int)RegisterTypes.Frequency, m_frequency);
            }
        }

        public float Phase
        {
            get { return m_phase; }
            set
            {
                m_phase = value;
                SetValue((int)RegisterTypes.Phase, m_phase);
            }
        }

        public float LightIntensity
        {
            get { return m_lightIntensity; }
            set
            {
                m_lightIntensity = value;
                SetValue((int)RegisterTypes.LightIntensity, m_lightIntensity);
            }
        }

        private static string GetResourceString(string embeddedResourceName, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(embeddedResourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
