using SlimDX;

namespace MMF.MME.VariableSubscriber.ConstantSubscriber.ConstantBufferManager
{
    public struct BasicMaterialConstantBufferInputLayout
    {
        public Vector4 AmbientLight;

        public Vector4 DiffuseLight;

        public Vector4 SpecularLight;

        public float SpecularPower;

        public static int SizeInBytes
        {
            get
            {
                int num = System.Runtime.InteropServices.Marshal.SizeOf(typeof(BasicMaterialConstantBufferInputLayout));
                return (num % 16 == 0) ? num : (num + 16 - num % 16);
            }
        }
    }
}
