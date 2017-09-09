using System;

namespace DirectCanvas.Rendering.Shaders
{
    enum ShaderVersion
    {
        Vs_4_0,
        Ps_4_0
    }

    static class ShaderVersionExtensions
    {
        public static string ToShaderVersionString(this ShaderVersion streamUsage)
        {
            switch (streamUsage)
            {
                case ShaderVersion.Vs_4_0:
                    return "vs_4_0";
                case ShaderVersion.Ps_4_0:
                    return "ps_4_0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("streamUsage");
            }
        }
    }
}
