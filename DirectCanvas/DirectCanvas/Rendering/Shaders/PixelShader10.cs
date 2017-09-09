using System.Reflection;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.Shaders
{
    class PixelShader10 : ShaderBase
    {
        private Device m_device;

        public PixelShader10(Device device,
                     string shaderSource,
                     string entryPoint,
                     ShaderVersion shaderVersion,
                     ShaderFlags shaderFlags = ShaderFlags.None,
                     EffectFlags effectFlags = EffectFlags.None) :
            base(shaderSource, entryPoint, shaderVersion, shaderFlags, effectFlags)
        {
            m_device = device;
            CreatePixelShader();
        }

        public PixelShader10(Device device, 
                             string embeddedSourceResourceName,
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

        public PixelShader InternalPixelShader { get; private set; }

        private void CreatePixelShader()
        {
            InternalPixelShader = new PixelShader(m_device, InternalShaderByteCode);
        }
    }
}
