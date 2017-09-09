using System;
using System.IO;
using System.Reflection;
using SlimDX.D3DCompiler;

namespace DirectCanvas.Rendering.Shaders
{
    class ShaderBase : IDisposable
    {
        private ShaderBytecode m_shaderByteCode;

        protected ShaderBase(string shaderSource,
                             string entryPoint,
                             ShaderVersion shaderVersion,
                             ShaderFlags shaderFlags = ShaderFlags.None,
                             EffectFlags effectFlags = EffectFlags.None)
        {
            m_shaderByteCode = ShaderBytecode.Compile(shaderSource,
                                                      entryPoint,
                                                      shaderVersion.ToShaderVersionString(),
                                                      shaderFlags,
                                                      effectFlags);
        }

        protected ShaderBase(string embeddedSourceResourceName,
                             Assembly resourceAssembly,
                             string entryPoint,
                             ShaderVersion shaderVersion,
                             ShaderFlags shaderFlags = ShaderFlags.None,
                             EffectFlags effectFlags = EffectFlags.None)
        {
            string shaderSource = GetResourceString(embeddedSourceResourceName, resourceAssembly);

            m_shaderByteCode = ShaderBytecode.Compile(shaderSource,
                                                      entryPoint,
                                                      shaderVersion.ToShaderVersionString(),
                                                      shaderFlags,
                                                      effectFlags);
        }

        public ShaderBytecode InternalShaderByteCode
        {
            get { return m_shaderByteCode; }
        }

        private static string GetResourceString(string embeddedResourceName, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(embeddedResourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public virtual void Dispose()
        {
            InternalShaderByteCode.Dispose();

            if (m_shaderByteCode != null)
            {
                m_shaderByteCode.Dispose();
                m_shaderByteCode = null;
            }
        }
    }
}
