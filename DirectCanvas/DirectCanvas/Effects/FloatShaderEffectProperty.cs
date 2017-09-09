using System;
using DirectCanvas.Rendering.StreamBuffers;
using SlimDX.Direct3D10;

namespace DirectCanvas.Effects
{
    class FloatShaderEffectProperty : ShaderEffectProperty
    {
        private readonly ConstantStreamBuffer<float> m_buffer;

        internal FloatShaderEffectProperty(ShaderEffect effect, int register) : base(effect, register)
        {
            m_buffer = new ConstantStreamBuffer<float>(Effect.DirectCanvas.DeviceContext.Device, 
                                                       16,
                                                       register,
                                                       ConstantStreamBufferType.PixelShader,
                                                       ResourceUsage.Dynamic,
                                                       CpuAccessFlags.Write,
                                                       false);
        }

        protected override void ValueChanged(object oldValue, object newValue)
        {
            if(newValue is float == false)
            {
                throw new Exception("Set value type must be a float.");
            }

            m_buffer.Write((float)newValue);
        }

        public override void SetRenderState()
        {
            m_buffer.SetRenderState();
        }

        public override void Dispose()
        {
            m_buffer.Dispose();
            base.Dispose();
        }
    }
}