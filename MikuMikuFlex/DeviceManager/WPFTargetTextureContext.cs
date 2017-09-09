using MMF.Matricies;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.DeviceManager
{
    public class WPFTargetTextureContext : TextureTargetContext
    {
        public WPFTargetTextureContext(RenderContext context, System.Drawing.Size size, SampleDescription sampleDesc) : base(context, size, sampleDesc)
        {
        }

        public WPFTargetTextureContext(RenderContext context, MatrixManager matrixManager, System.Drawing.Size size, SampleDescription sampleDesc) : base(context, matrixManager, size, sampleDesc)
        {
        }

        protected override Texture2DDescription getRenderTargetTexture2DDescription()
        {
            return new Texture2DDescription
            {
                BindFlags = (BindFlags.ShaderResource | BindFlags.RenderTarget),
                Format = Format.B8G8R8A8_UNorm,
                Width = Size.Width,
                Height = Size.Height,
                MipLevels = 1,
                SampleDescription = SampleManager.getSampleDesc(8, 0, base.Context, Format.B8G8R8A8_UNorm),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };
        }
    }
}