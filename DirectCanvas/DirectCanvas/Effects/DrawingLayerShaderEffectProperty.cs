using System;
using SlimDX.Direct3D10;

namespace DirectCanvas.Effects
{
    class DrawingLayerShaderEffectProperty : ShaderEffectProperty
    {
        private readonly Device m_device;
        private DrawingLayer m_drawingLayer;

        internal DrawingLayerShaderEffectProperty(ShaderEffect effect, int register)
            : base(effect, register)
        {
            m_device = effect.DirectCanvas.DeviceContext.Device;
        }

        protected override void ValueChanged(object oldValue, object newValue)
        {
            if (newValue is DrawingLayer == false)
            {
                throw new Exception("Set type must be a DrawingLayer.");
            }

            m_drawingLayer = (DrawingLayer)newValue;
        }

        public override void SetRenderState()
        {
            m_device.PixelShader.SetShaderResource(m_drawingLayer.RenderTargetTexture.InternalShaderResourceView, Register);
        }
    }
}