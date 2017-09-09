using System;
using System.Collections.Generic;
using System.Reflection;
using DirectCanvas.Misc;
using DirectCanvas.Rendering.Bindings;
using DirectCanvas.Rendering.Effects;
using DirectCanvas.Rendering.Shaders;

namespace DirectCanvas.Effects
{
    /// <summary>
    /// The ShaderEffect is a base class to create custom
    /// pixel shader effects.
    /// </summary>
    public abstract class ShaderEffect : IDisposable
    {
        /// <summary>
        /// We store our HLSL as resources in the dll as a
        /// temporary measure for now
        /// </summary>
        private const string SHADER_RESOURCE_NAME = "DirectCanvas.Rendering.Effects.Shaders.ShaderQuad.hlsl";
        
        /// <summary>
        /// Reference to the DirectCanvasFactory
        /// </summary>
        private readonly DirectCanvasFactory m_directCanvasFactory;

        /// <summary>
        /// The PixelShader wrapper
        /// </summary>
        private PixelShader10 m_pixelShader;

        /// <summary>
        /// The vertex shader wrapper
        /// </summary>
        private VertexShader10 m_vertexShader;

        /// <summary>
        /// The quad that is used to render the effect to the render target
        /// </summary>
        private ShaderQuad m_quad;

        /// <summary>
        /// The helper that binds the mesh, shaders and input element and layouts
        /// for the GPU
        /// </summary>
        private GeometryInputShaderBinding m_spriteQuadShaderBinding;

        /// <summary>
        /// This is how we keep track of shader properties, values and their resources
        /// </summary>
        private Dictionary<int, ShaderEffectProperty> m_valueProperties = new Dictionary<int, ShaderEffectProperty>();

        public ShaderEffectFilter Filter { get; protected set; }

        /// <summary>
        /// This is how we keep track of shader properties, values and their resources
        /// </summary>
        private Dictionary<int, ShaderEffectProperty> m_textureProperties = new Dictionary<int, ShaderEffectProperty>();

        protected ShaderEffect(DirectCanvasFactory directCanvas)
        {
            m_directCanvasFactory = directCanvas;
            Filter = ShaderEffectFilter.Linear;
        }

        internal PixelShader10 PixelShader
        {
            get { return m_pixelShader; }
        }

        internal DirectCanvasFactory DirectCanvas
        {
            get { return m_directCanvasFactory; }
        }

        /// <summary>
        /// We store our HLSL as resources in the dll as a
        /// temporary measure for now
        /// </summary>
        /// <param name="source">The source HLSL code</param>
        /// <param name="entryPoint">The entry point for the shader</param>
        protected void SetShaderSource(string source, string entryPoint)
        {
            var device = DirectCanvas.DeviceContext.Device;

            /* Create a new pixel shader wrapper using the source passed to us */
            m_pixelShader = new PixelShader10(device, source, entryPoint, ShaderVersion.Ps_4_0);
            
            /* Create a new vertex shader using our premade script */
            m_vertexShader = new VertexShader10(device, 
                                                SHADER_RESOURCE_NAME, 
                                                Assembly.GetExecutingAssembly(),
                                                "QuadVS", 
                                                ShaderVersion.Vs_4_0);

            /* Create our quad that will be drawn */
            m_quad = new ShaderQuad(device);

            /* Create the binding helper */
            m_spriteQuadShaderBinding = new GeometryInputShaderBinding(m_quad,
                                                                       m_vertexShader,
                                                                       PixelShader);
        }

        /// <summary>
        /// Draws the effect onto the render target
        /// </summary>
        internal void Draw()
        {
            /* Set our binding to prepare to render */
            m_spriteQuadShaderBinding.SetRenderState();

            /* Loop over each effect property and set their value */
            foreach (var shaderEffectProperty in m_valueProperties)
            {
                shaderEffectProperty.Value.SetRenderState();
            }

            foreach (var shaderEffectProperty in m_textureProperties)
            {
                shaderEffectProperty.Value.SetRenderState();
            }

            /* Draw the quad, with draws to the render target */
            m_quad.Draw();
        }

        protected void RegisterProperty<T>(int register)
        {
            Dictionary<int, ShaderEffectProperty> properties = null;

            properties = typeof(T) != typeof(DrawingLayer) ? m_textureProperties : m_valueProperties;

            if (properties.ContainsKey(register))
                    throw new Exception("Register already used.");

            if (typeof(T) != typeof(float) && typeof(T) != typeof(Color4) && typeof(T) != typeof(PointF) && typeof(T) != typeof(DrawingLayer))
            {
                throw new Exception("Only floats, PointF, DrawingLayer and Color4 values are accepted");
            }

            if (typeof(T) == typeof(float))
            {
                properties.Add(register, new FloatShaderEffectProperty(this, register));
                return;
            }

            if (typeof(T) == typeof(Color4))
            {
                properties.Add(register, new ColorShaderEffectProperty(this, register));
                return;
            }

            if (typeof(T) == typeof(PointF))
            {
                properties.Add(register, new PointEffectProperty(this, register));
                return;
            }

            if (typeof(T) == typeof(DrawingLayer))
            {
                if (register == 0)
                    throw new Exception("Input registers for drawing layers must start at 1 as 0 is reserved.");

                properties.Add(register, new DrawingLayerShaderEffectProperty(this, register));
                return;
            }
        }

        protected void SetValue<T>(int register, T value)
        {
            Dictionary<int, ShaderEffectProperty> properties = null;

            properties = typeof(T) != typeof(DrawingLayer) ? m_textureProperties : m_valueProperties;

            if (!properties.ContainsKey(register))
            {
                throw new Exception(string.Format("Register {0} not configured", register));
            }

            properties[register].Value = value;
        }

        public void Dispose()
        {
           if(PixelShader != null)
           {
               PixelShader.Dispose();
               m_pixelShader = null;
           }

           if (m_valueProperties != null)
           {
               foreach (var shaderEffectProperty in m_valueProperties)
               {
                   shaderEffectProperty.Value.Dispose();
               }

               m_valueProperties = null;
           }
        }
    }
}
