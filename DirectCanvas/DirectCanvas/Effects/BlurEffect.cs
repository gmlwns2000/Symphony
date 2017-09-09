using System;
using System.IO;
using System.Reflection;
using DirectCanvas.Misc;

namespace DirectCanvas.Effects
{
    public enum BlurDirection
    {
        Horizontal = 0,
        Vertical = 1
    }

    public class BlurEffect : ShaderEffect
    {
        private const string SHADER_RESOURCE_NAME = "DirectCanvas.Effects.Shaders.Blur.ps";
        private const string SHADER_ENTRY = "BlurMain";
        private const int SIGMA_REGISTER = 0;
        private const int WIDTH_REGISTER = 1;
        private const int HEIGHT_REGISTER = 2;
        private const int TINTCOLOR_REGISTER = 3;
        private const int DIRECTION_VALUE = 4;

        private Color4 m_tint;
        private SizeF m_sampleSize;
        private float m_sigma;
        private BlurDirection m_direction;

        public BlurEffect(DirectCanvasFactory directCanvas)
            : base(directCanvas)
        {
            SetShaderSource(GetResourceString(SHADER_RESOURCE_NAME, Assembly.GetExecutingAssembly()), SHADER_ENTRY);

            RegisterProperty<float>(SIGMA_REGISTER);
            RegisterProperty<float>(WIDTH_REGISTER);
            RegisterProperty<float>(HEIGHT_REGISTER);
            RegisterProperty<Color4>(TINTCOLOR_REGISTER);
            RegisterProperty<float>(DIRECTION_VALUE);
        }

        private static string GetResourceString(string embeddedResourceName, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(embeddedResourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public BlurDirection Direction
        {
            get
            {
                return m_direction;
            }
            set
            {
                m_direction = value;
                SetValue<float>(DIRECTION_VALUE, (float)m_direction);
            }
        }

        public SizeF SampleSize
        {
            get { return m_sampleSize; }
            set
            {
                m_sampleSize = value;
                SetValue<float>(WIDTH_REGISTER, m_sampleSize.Width);
                SetValue<float>(HEIGHT_REGISTER, m_sampleSize.Height);
            }
        }

        public float Sigma
        {
            get { return m_sigma; }
            set
            {
                m_sigma = value;
                SetValue(0, m_sigma);
            }
        }

        public Color4 Tint
        {
            get { return m_tint; }
            set
            {
                m_tint = value;
                SetValue(TINTCOLOR_REGISTER, m_tint);
            }
        }
    }
}