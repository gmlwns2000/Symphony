using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DirectCanvas.Misc;

namespace DirectCanvas.Effects
{
    public class RadialWiggleTransitionEffect : ShaderEffect
    {
        private const string SHADER_RESOURCE_NAME = "DirectCanvas.Effects.Shaders.RadialWiggleTransitionEffect.ps";
        private const string SHADER_ENTRY = "main";

        private DrawingLayer m_oldInput;
        private DrawingLayer m_cloudInput;
        private float m_progress;
        private float m_randomSeed;

        private enum RegisterTypes : int
        {
            oldInput = 1,
            cloudInput = 2,
            progress = 0,
            randomSeed = 1,
        }

        public RadialWiggleTransitionEffect(DirectCanvasFactory factory) : base(factory)
        {
            SetShaderSource(GetResourceString(SHADER_RESOURCE_NAME, Assembly.GetExecutingAssembly()), SHADER_ENTRY);

            RegisterProperty<DrawingLayer>((int)RegisterTypes.oldInput);
            RegisterProperty<DrawingLayer>((int)RegisterTypes.cloudInput);
            RegisterProperty<float>((int)RegisterTypes.progress);
            RegisterProperty<float>((int)RegisterTypes.randomSeed);
        }

        public DrawingLayer OldInput
        {
            get { return m_oldInput; }
            set
            {
                m_oldInput = value;
                SetValue((int)RegisterTypes.oldInput, m_oldInput);
            }
        }

        public DrawingLayer CloudInput
        {
            get { return m_cloudInput; }
            set
            {
                m_cloudInput = value;
                SetValue((int)RegisterTypes.cloudInput, m_cloudInput);
            }
        }

        public float Progress
        {
            get { return m_progress; }
            set
            {
                m_progress = value;
                SetValue((int)RegisterTypes.progress, m_progress);
            }
        }

        public float RandomSeed
        {
            get { return m_randomSeed; }
            set
            {
                m_randomSeed = value;
                SetValue((int)RegisterTypes.randomSeed, m_randomSeed);
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
