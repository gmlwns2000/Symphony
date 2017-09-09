using System;
using DirectCanvas.Effects;
using DirectCanvas.Misc;
using DirectCanvas.Rendering.Bindings;
using DirectCanvas.Rendering.Materials;
using DirectCanvas.Rendering.Shaders;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.Effects
{
    /// <summary>
    /// This internal class will render a quad with a given pixel shader.
    /// This is used for the extensible effects in DirectCanvas.
    /// </summary>
    class ShaderRenderer : IDisposable
    {
        private DirectCanvasFactory m_directCanvasFactory;
        private SamplerState m_linearSamplerState;
        private SamplerState m_pointSamplerState;
        private BlendState m_alphaBlendState;

        public ShaderRenderer(DirectCanvasFactory directCanvas)
        {
            m_directCanvasFactory = directCanvas;
            Initialize();
        }

        /// <summary>
        /// Initializes device dependant resources.
        /// </summary>
        private void Initialize()
        {
            var device = m_directCanvasFactory.DeviceContext.Device;

            /* Here we create a new sampler for sampling input within
             * our pixel shader */
            var sampDesc = new SamplerDescription();
            sampDesc.AddressU = TextureAddressMode.Clamp;
            sampDesc.AddressV = TextureAddressMode.Clamp;
            sampDesc.AddressW = TextureAddressMode.Clamp;
            sampDesc.BorderColor = new Color4(0, 0, 0, 0).InternalColor4;
            sampDesc.ComparisonFunction = Comparison.Never;
            sampDesc.Filter = Filter.MinMagMipLinear;
            sampDesc.MaximumAnisotropy = 10;
            sampDesc.MaximumLod = float.MaxValue;
            sampDesc.MinimumLod = 0;
            sampDesc.MipLodBias = 0;
            m_linearSamplerState = SamplerState.FromDescription(device, sampDesc);

            sampDesc.Filter = Filter.MinMagMipPoint;
            m_pointSamplerState = SamplerState.FromDescription(device, sampDesc);

            /* Here we have a hard coded blend state.  This should be configurable in
             * the future. Like the composer has */
            var blendDesc = new BlendStateDescription();
            blendDesc.IsAlphaToCoverageEnabled = false;
            blendDesc.BlendOperation = BlendOperation.Add;
            blendDesc.AlphaBlendOperation = BlendOperation.Add;
            blendDesc.DestinationBlend = BlendOption.InverseSourceAlpha;
            blendDesc.DestinationAlphaBlend = BlendOption.One;
            blendDesc.SourceBlend = BlendOption.One;
            blendDesc.SourceAlphaBlend = BlendOption.One;

            for (uint i = 0; i < 8; i++)
            {
                blendDesc.SetWriteMask(i, ColorWriteMaskFlags.All);
                blendDesc.SetBlendEnable(i, true);
            }

            m_alphaBlendState = BlendState.FromDescription(device, blendDesc);
        }

        /// <summary>
        /// Applys a shader effect
        /// </summary>
        /// <param name="effect">The shader effect to apply</param>
        /// <param name="inTexture">The DrawingLayer to be used as input</param>
        /// <param name="outTexture">The DrawingLayer to be used as output</param>
        /// <param name="clearOutput">Clear the output before writing</param>
        public void Apply(ShaderEffect effect, RenderTargetTexture inTexture, RenderTargetTexture outTexture, bool clearOutput)
        {
            var device = m_directCanvasFactory.DeviceContext.Device;

            if(clearOutput)
                outTexture.Clear(new Color4(0, 0, 0, 0));

            outTexture.SetRenderTarget();

            device.PixelShader.SetShaderResource(inTexture.InternalShaderResourceView, 0);
            if (effect.Filter == ShaderEffectFilter.Linear)
                device.PixelShader.SetSampler(m_linearSamplerState, 0);
            else if (effect.Filter == ShaderEffectFilter.Point)
                device.PixelShader.SetSampler(m_pointSamplerState, 0);
            device.OutputMerger.BlendState = m_alphaBlendState;

            effect.Draw();
        }


        /// <summary>
        /// Applys a shader effect
        /// </summary>
        /// <param name="effect">The shader effect to apply</param>
        /// <param name="inTexture">The DrawingLayer to be used as input</param>
        /// <param name="outTexture">The DrawingLayer to be used as output</param>
        /// <param name="targetRect"></param>
        /// <param name="clearOutput">Clear the output before writing</param>
        public void Apply(ShaderEffect effect, RenderTargetTexture inTexture, RenderTargetTexture outTexture, Rectangle targetRect, bool clearOutput)
        {
            var device = m_directCanvasFactory.DeviceContext.Device;

            if (clearOutput)
                outTexture.Clear(new Color4(0, 0, 0, 0));

            outTexture.SetRenderTarget(new Viewport(targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height, 0, 1));

            device.PixelShader.SetShaderResource(inTexture.InternalShaderResourceView, 0);
            if (effect.Filter == ShaderEffectFilter.Linear)
                device.PixelShader.SetSampler(m_linearSamplerState, 0);
            else if (effect.Filter == ShaderEffectFilter.Point)
                device.PixelShader.SetSampler(m_pointSamplerState, 0);

            effect.Draw();
        }

        public void Dispose()
        {
            if(m_linearSamplerState != null)
            {
                m_linearSamplerState.Dispose();
                m_linearSamplerState = null;
            }
            if (m_pointSamplerState != null)
            {
                m_pointSamplerState.Dispose();
                m_pointSamplerState = null;
            }

            if(m_alphaBlendState != null)
            {
                m_alphaBlendState.Dispose();
                m_alphaBlendState = null;
            }
           
        }
    }
}
