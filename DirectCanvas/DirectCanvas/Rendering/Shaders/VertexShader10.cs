using System.Reflection;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.Shaders
{
    class VertexShader10 : ShaderBase
    {
        private Device m_device;

        public VertexShader10(Device device,
                             string embeddedSourceResourceName,
                             string entryPoint,
                             ShaderVersion shaderVersion,
                             ShaderFlags shaderFlags = ShaderFlags.None,
                             EffectFlags effectFlags = EffectFlags.None) :
            base(embeddedSourceResourceName,
                 entryPoint,
                 shaderVersion,
                 shaderFlags,
                 effectFlags)
        {
            m_device = device;
            CreatePixelShader();
        }

        public VertexShader10(Device device, string embeddedSourceResourceName,
                             Assembly resourceAssembly,
                             string entryPoint,
                             ShaderVersion shaderVersion,
                             ShaderFlags shaderFlags = ShaderFlags.None,
                             EffectFlags effectFlags = EffectFlags.None) :
            base(embeddedSourceResourceName,
                 resourceAssembly,
                 entryPoint,
                 shaderVersion,
                 shaderFlags,
                 effectFlags)
        {
            m_device = device;
            CreatePixelShader();
        }

        public VertexShader InternalVertexShader { get; private set; }

        private void CreatePixelShader()
        {
            InternalVertexShader = new VertexShader(m_device, InternalShaderByteCode);
        }
    }
}
