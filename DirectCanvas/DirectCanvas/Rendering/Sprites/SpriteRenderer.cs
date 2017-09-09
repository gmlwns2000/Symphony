using System;
using System.Collections.Generic;
using System.Reflection;
using DirectCanvas.Rendering.Bindings;
using DirectCanvas.Rendering.Materials;
using DirectCanvas.Rendering.Shaders;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D10;
using Device = SlimDX.Direct3D10.Device;
using ShaderVersion = DirectCanvas.Rendering.Shaders.ShaderVersion;

namespace DirectCanvas.Rendering.Sprites
{
    internal enum FilterMode
    {
        DontSet = 0,
        Linear = 1,
        Point = 2
    }

    class SpriteRenderer : IDisposable
    {
        /// <summary>
        /// Constant for how many different texture instances we can handle.  If this
        /// is updated, the HLSL must be updated
        /// </summary>
        private const int MAX_TEXTURE_INSTANCES = 16;

        /// <summary>
        /// The resource name of the HLSL in the DLL
        /// </summary>
        private const string SHADER_RESOURCE_NAME = "DirectCanvas.Rendering.Assets.Sprite.hlsl";

        /// <summary>
        /// The maximum sprite instances we can use.  The buffers to hold this are created on the instantion.
        /// </summary>
        private int m_maxSpriteInstances;

        /// <summary>
        /// Reference to the Direct3D device
        /// </summary>
        private Device m_device;

        /// <summary>
        /// Our rasterizer state we use on the device when rendering
        /// </summary>
        private RasterizerState m_rasterizerState;

        private DepthStencilState m_dsState;

        /// <summary>
        /// The linear sampler
        /// </summary>
        private SamplerState m_linearSamplerState;

        /// <summary>
        /// The point sampler
        /// </summary>
        private SamplerState m_pointSamplerState;

        /// <summary>
        /// The vertext shader for our sprite renderer
        /// </summary>
        private VertexShader10 m_vertexShaderInstanced10;

        /// <summary>
        /// The pixel shader for our sprite renderer
        /// </summary>
        private PixelShader10 m_pixelShader10;

        /// <summary>
        /// Our sprite quad that handles all our sprite instances/buffers
        /// </summary>
        private SpriteQuad m_spriteQuad;

        /// <summary>
        /// Our binding of a texture and shader
        /// </summary>
        private readonly GeometryInputShaderBinding m_spriteQuadShaderBinding;

        /// <summary>
        /// An array of sprite renderer data. This data holds some unprocessed sprite data.  We use an array the size of 
        /// m_maxSpriteInstances.
        /// </summary>
        private readonly SpriteRenderData[] m_spriteRenderData;

        /// <summary>
        /// This array holds processed structs ready to send to the GPU
        /// </summary>
        private SpriteDrawData[] m_spriteDrawData;

        /// <summary>
        /// The filter mode to use for the sampler
        /// </summary>
        private FilterMode m_filterMode = FilterMode.Linear;

        /// <summary>
        /// The current position of the sprite data in the m_spriteRenderData array
        /// </summary>
        private int m_drawDataPosition;

        /// <summary>
        /// The texture that was sent to use on the BeginDraw that will have the sprites drawn to it
        /// </summary>
        private RenderTargetTexture m_outputTexture;

        /// <summary>
        /// Holds our premade blend state modes
        /// </summary>
        private readonly Dictionary<BlendStateMode, BlendState> m_blendStates = new Dictionary<BlendStateMode, BlendState>();

        /// <summary>
        /// The blend state mode to use when sprites are flushed
        /// </summary>
        private BlendStateMode m_blendStateMode = BlendStateMode.AlphaBlend;

        /// <summary>
        /// Creates a new instance of the SpriteRenderer
        /// </summary>
        /// <param name="device">The device to use</param>
        /// <param name="maxSpriteInstances">The maximum sprite instances that can be cached before a flush happens</param>
        public SpriteRenderer(Device device, int maxSpriteInstances = 10000)
        {
            /* Initialize our arrays to hold our sprite data */
            m_spriteRenderData = new SpriteRenderData[maxSpriteInstances];
            m_spriteDrawData = new SpriteDrawData[maxSpriteInstances];

            /* Initialize all the items in the array */
            for (int i = 0; i < maxSpriteInstances; i++)
            {
                m_spriteRenderData[i] = new SpriteRenderData();
            }

            m_maxSpriteInstances = maxSpriteInstances;
            m_device = device;

            /* Create our default blend states using our helper */
            m_blendStates = SpriteRendererBlendStateHelper.InitializeDefaultBlendStates(m_device);

            /* Create our vertex shader */
            m_vertexShaderInstanced10 = new VertexShader10(device,
                                                           SHADER_RESOURCE_NAME, Assembly.GetExecutingAssembly(),
                                                           "SpriteInstancedVS", 
                                                           ShaderVersion.Vs_4_0);

            /* Create our pixel shader */
            m_pixelShader10 = new PixelShader10(device,
                                                SHADER_RESOURCE_NAME, Assembly.GetExecutingAssembly(),
                                                "SpritePS", 
                                                ShaderVersion.Ps_4_0,ShaderFlags.Debug);
            
            /* Create a new sprite quad that holds our GPU buffers */
            m_spriteQuad = new SpriteQuad(device, maxSpriteInstances);

            m_spriteQuadShaderBinding = new GeometryInputShaderBinding(m_spriteQuad, 
                                                                       m_vertexShaderInstanced10, 
                                                                       m_pixelShader10);

            var rastDesc = new RasterizerStateDescription();
            rastDesc.IsAntialiasedLineEnabled = false;
            rastDesc.CullMode = CullMode.None;
            rastDesc.DepthBias = 0;
            rastDesc.DepthBiasClamp = 1.0f;
            rastDesc.IsDepthClipEnabled = false;
            rastDesc.FillMode = FillMode.Solid;
            rastDesc.IsFrontCounterclockwise = false;
            rastDesc.IsMultisampleEnabled = false;
            rastDesc.IsScissorEnabled = false;
            rastDesc.SlopeScaledDepthBias = 0;
            m_rasterizerState = RasterizerState.FromDescription(m_device, rastDesc);
            
            var dsDesc = new DepthStencilStateDescription();
            dsDesc.IsDepthEnabled = false;
            dsDesc.DepthWriteMask = DepthWriteMask.All;
            dsDesc.DepthComparison = Comparison.Less;
            dsDesc.IsStencilEnabled = false;
            dsDesc.StencilReadMask = 0xff;
            dsDesc.StencilWriteMask = 0xff;
            dsDesc.FrontFace = new DepthStencilOperationDescription{ DepthFailOperation = StencilOperation.Keep, FailOperation = StencilOperation.Replace, Comparison = Comparison.Always };
            dsDesc.BackFace = dsDesc.FrontFace;
            m_dsState = DepthStencilState.FromDescription(m_device, dsDesc);

            var sampDesc = new SamplerDescription();
            sampDesc.AddressU = TextureAddressMode.Wrap;
            sampDesc.AddressV = TextureAddressMode.Wrap;
            sampDesc.AddressW = TextureAddressMode.Wrap;
            sampDesc.BorderColor = new Color4(0, 0, 0, 0).InternalColor4;
            sampDesc.ComparisonFunction = Comparison.Never;
            sampDesc.Filter = Filter.MinMagMipLinear;
            sampDesc.MaximumAnisotropy = 1;
            sampDesc.MaximumLod = float.MaxValue;
            sampDesc.MinimumLod = 0;
            sampDesc.MipLodBias = 0;
            m_linearSamplerState = SamplerState.FromDescription(m_device, sampDesc);

            sampDesc.Filter = Filter.MinMagMipPoint;
            m_pointSamplerState = SamplerState.FromDescription(m_device, sampDesc);
        }
        
        /// <summary>
        /// Begins the batch process.
        /// </summary>
        /// <param name="filterMode">The filter mode to use</param>
        /// <param name="outputTexture">The texture to render to</param>
        /// <param name="blendStateMode">The blend state to use</param>
        public void BeginDraw(FilterMode filterMode, RenderTargetTexture outputTexture, BlendStateMode blendStateMode)
        {
            m_blendStateMode = blendStateMode;
            m_outputTexture = outputTexture;
            m_drawDataPosition = 0;

            m_filterMode = filterMode;
            BeginDrawSetup();
        }

        /// <summary>
        /// Ends the batch process and flushes any remaining batched commands.
        /// </summary>
        public void EndDraw()
        {
            if (m_drawDataPosition == 0)
                return;

            Flush();
            m_outputTexture = null;
        }

        /// <summary>
        /// Flushes all batched data to the GPU
        /// </summary>
        private void Flush()
        {
            if (m_drawDataPosition == 0)
                return;

            RenderAllBatch();
            m_drawDataPosition = 0;
        }

        /// <summary>
        /// Gets the current render data
        /// </summary>
        /// <returns></returns>
        public SpriteRenderData GetCurrentRenderData()
        {
            m_spriteRenderData[m_drawDataPosition].texture = null;
            return m_spriteRenderData[m_drawDataPosition];
        }

        /// <summary>
        /// Adds the draw data back, and increments the position
        /// </summary>
        /// <param name="renderData"></param>
        public void AddRenderData(SpriteRenderData renderData)
        {
            m_drawDataPosition++;

            if (m_drawDataPosition >= m_maxSpriteInstances)
            {
                Flush();
            }
        }

        private void BeginDrawSetup()
        {
            /* Set our rasterizer state */
            m_device.Rasterizer.State = m_rasterizerState;

            /* Set our configured blend state to the output merger */
            m_device.OutputMerger.BlendState = m_blendStates[m_blendStateMode];

            m_device.OutputMerger.BlendFactor = new Color4(0,0,0,0).InternalColor4;
            m_device.OutputMerger.BlendSampleMask = unchecked((int)0xFFFFFFFF);
            m_device.OutputMerger.DepthStencilState = m_dsState;

            /* Set our configured filter mode */
            if(m_filterMode == FilterMode.Linear)
                m_device.PixelShader.SetSampler(m_linearSamplerState, 0);
            else if(m_filterMode == FilterMode.Point)
                 m_device.PixelShader.SetSampler(m_pointSamplerState, 0);
        }

        /// <summary>
        /// Creates constant data for our shaders
        /// </summary>
        /// <returns></returns>
        private VertexConstantData GetConstantData()
        {
            var constantData = new VertexConstantData();

            /* Our shader needs to know the viewport size to 
             * properly render.  This is how we do that */
            var viewPort = m_device.Rasterizer.GetViewports()[0];

            constantData.ViewportSize.X = viewPort.Width;
            constantData.ViewportSize.Y = viewPort.Height;

            return constantData;
        }

        /// <summary>
        /// This is the method that does any post processing of the sprite data
        /// before sending it to be rendered by the GPU
        /// </summary>
        private void RenderAllBatch()
        {
            /* Make sure we write to our render target */
            m_outputTexture.SetRenderTarget();

            /* Set our constances to our sprite quad */
            m_spriteQuad.SetConstants(GetConstantData());

            /* This will let us keep track of how many texture instances
             * we have queued up, and to make sure we don't go passed the max */
            var textureList = new IntPtr[MAX_TEXTURE_INSTANCES];

            /* Count of how many individual textures we have queued up so far */
            int textureCount = 0;

            /* The current index of the array of sprite data we are at */
            int index = 0;

            /* The amount of items we have queued up to be sent to the GPU */
            int drawDataCount = 0;

            for (index = 0; index < m_drawDataPosition; ++index)
            {
                if (drawDataCount > m_maxSpriteInstances - 1 || textureCount == MAX_TEXTURE_INSTANCES)
                {
                    /* We write our instance data to our GPU buffers */
                    m_spriteQuad.WriteInstanceData(ref m_spriteDrawData, 0, drawDataCount);

                    /* We prepare to render, setting up the input assemblers and shaders */
                    m_spriteQuadShaderBinding.SetRenderState();

                    /* Make sure we write to our render target */
                    m_outputTexture.SetRenderTarget();

                    /* Draw all the instances!! */
                    m_spriteQuad.Draw();

                    /* Reset our counters */
                    textureCount = 0;
                    drawDataCount = 0;
                }

                /* Get the unprocessed sprite data at our index*/
                var renderData = m_spriteRenderData[index];

                /* Get reference to the d3d texture */
                var texture = renderData.texture;

                /* Get the native pointer to the texture */
                var pTexture = texture.InternalShaderResourceView.ComPointer;

                bool foundTexture = false;

                int pTextureIndex = 0;

                /* Search our textureList to see if we have already used this
                 * texture, if so remember the index. */
                for (int i = 0; i < textureCount; i++)
                {
                    if (textureList[i] == pTexture)
                    {
                        foundTexture = true;
                        pTextureIndex = i;
                        break;
                    }
                }

                /* If we have not used this texture before, then
                 * make sure to prepare it based on its index (textureCount) */
                if (!foundTexture)
                {
                    m_device.PixelShader.SetShaderResource(texture.InternalShaderResourceView, textureCount);
                    textureList[textureCount] = pTexture;
                    pTextureIndex = textureCount;
                    textureCount++;
                }

                /* Configure our render data with the correct texture index */
                renderData.drawData.ShaderTextureIndex = pTextureIndex;
                /* Configure our render data with the correct texture size,
                 * which is needed by our shader to render correctly*/
                renderData.drawData.TextureSize.X = texture.Description.Width;
                renderData.drawData.TextureSize.Y = texture.Description.Height;
                
                m_spriteDrawData[drawDataCount] = renderData.drawData;
                ++drawDataCount;
            }

            if (drawDataCount > 0)
            {
                /* We write our instance data to our GPU buffers */
                m_spriteQuad.WriteInstanceData(ref m_spriteDrawData, 0, drawDataCount);

                /* We prepare to render, setting up the input assemblers and shaders */
                m_spriteQuadShaderBinding.SetRenderState();
                /* Make sure we write to our render target */
                m_outputTexture.SetRenderTarget();

                /* Draw that ish! */
                m_spriteQuad.Draw();
            }
        }

        public void Dispose()
        {
            if(m_spriteQuad != null)
            {
                m_spriteQuad.Dispose();
                m_spriteQuad = null;
            }

            if(m_linearSamplerState != null)
            {
                m_linearSamplerState.Dispose();
                m_linearSamplerState = null;
            }

            if(m_pointSamplerState != null)
            {
                m_pointSamplerState.Dispose();
                m_pointSamplerState = null;
            }

            if(m_spriteQuadShaderBinding != null)
            {
                m_spriteQuadShaderBinding.Dispose();
                m_spriteQuadShaderBinding.Dispose();
            }

            if(m_vertexShaderInstanced10 != null)
            {
                m_vertexShaderInstanced10.Dispose();
                m_vertexShaderInstanced10 = null;
            }

            foreach (var blendState in m_blendStates)
            {
                blendState.Value.Dispose();
            }

            m_blendStates.Clear();

            if(m_pixelShader10 != null)
            {
                m_pixelShader10.Dispose();
                m_pixelShader10 = null;
            }
        }
    }
}
