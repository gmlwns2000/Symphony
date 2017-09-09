using System.Diagnostics;

namespace MMF.MME
{
    internal class InvalidMMEEffectShaderException : MMEEffectException
    {
        public InvalidMMEEffectShaderException(string message) : base(message)
        {
            Debug.WriteLine(message);
        }
    }
}
