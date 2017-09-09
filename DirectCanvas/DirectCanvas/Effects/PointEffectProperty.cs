using System;
using DirectCanvas.Misc;
using DirectCanvas.Rendering.StreamBuffers;
using SlimDX.Direct3D10;

namespace DirectCanvas.Effects
{
    class PointEffectProperty : ShaderEffectProperty
    {
        private readonly ConstantStreamBuffer<PointF> m_buffer;

        internal PointEffectProperty(ShaderEffect effect, int register)
            : base(effect, register)
        {
            m_buffer = new ConstantStreamBuffer<PointF>(Effect.DirectCanvas.DeviceContext.Device,
                                                        16,
                                                        register,
                                                        ConstantStreamBufferType.PixelShader,
                                                        ResourceUsage.Dynamic,
                                                        CpuAccessFlags.Write,
                                                        false);
        }

        protected override void ValueChanged(object oldValue, object newValue)
        {
            if (newValue is PointF == false)
            {
                throw new Exception("Set value type must be a float.");
            }

            m_buffer.Write((PointF)newValue);
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