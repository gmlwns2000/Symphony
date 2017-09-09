using System;
using System.Runtime.InteropServices;
using DirectCanvas.Rendering.StreamBuffers;
using SlimDX.Direct3D10;

namespace DirectCanvas.Effects
{
    class ColorShaderEffectProperty : ShaderEffectProperty
    {
        private readonly ConstantStreamBuffer<Color4> m_buffer;

        internal ColorShaderEffectProperty(ShaderEffect effect, int register)
            : base(effect, register)
        {
            m_buffer = new ConstantStreamBuffer<Color4>(Effect.DirectCanvas.DeviceContext.Device, 
                                                        Marshal.SizeOf(typeof(Color4)),
                                                        register, 
                                                        ConstantStreamBufferType.PixelShader, 
                                                        ResourceUsage.Dynamic, 
                                                        CpuAccessFlags.Write, 
                                                        false);
        }

        protected override void ValueChanged(object oldValue, object newValue)
        {
            if (newValue is Color4 == false)
            {
                throw new Exception("Set value type must be a Color4.");
            }

            m_buffer.Write((Color4)newValue);
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