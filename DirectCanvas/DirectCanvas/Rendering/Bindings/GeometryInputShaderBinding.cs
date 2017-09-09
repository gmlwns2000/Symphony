using DirectCanvas.Rendering.Shaders;
using DirectCanvas.Rendering.StreamBuffers;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.Bindings
{
    /// <summary>
    /// Binds geometry meshes to a vertex shader and pixel shader, creating
    /// and managing the input layout for the GPU
    /// </summary>
    class GeometryInputShaderBinding : IGeometryInputBinding
    {
        private readonly VertexShader10 m_vertexShader;
        private readonly PixelShader10 m_pixelShader;
        private readonly GeometryMesh m_geometryMesh;
        private readonly InputLayout m_inputLayout;

        public GeometryInputShaderBinding(GeometryMesh geometryMesh, VertexShader10 vertexShader, PixelShader10 pixelShader)
        {
            m_geometryMesh = geometryMesh;
            m_pixelShader = pixelShader;
            m_vertexShader = vertexShader;

            /* Create the D3D input layout for the GPU */
            m_inputLayout = new InputLayout(geometryMesh.InternalDevice, 
                                            vertexShader.InternalShaderByteCode, 
                                            geometryMesh.GetInputElements());
        }

        /// <summary>
        /// Prepares the binding for rendering
        /// </summary>
        public virtual void SetRenderState()
        {
            Device device = m_geometryMesh.InternalDevice;

            /* Configure the D3D device to use our input layout and shaders */
            device.InputAssembler.SetInputLayout(m_inputLayout);
            device.VertexShader.Set(m_vertexShader.InternalVertexShader);
            device.PixelShader.Set(m_pixelShader.InternalPixelShader);
            device.GeometryShader.Set(null);
            
            /* Prepare our geometry mesh for rendering */
            m_geometryMesh.SetRenderState();
        }

        public void Dispose()
        {
            m_inputLayout.Dispose();
        }
    }
}
