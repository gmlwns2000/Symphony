using MMF.Matricies;
using MMF.Model;
using SlimDX.Direct3D11;

namespace MMF.Light
{
    public interface IShadowmap : System.IDisposable
    {
        MatrixManager MatriciesFromLight
        {
            get;
        }

        RenderTargetView ShadowBufferRenderTarget
        {
            get;
        }

        DepthStencilView ShadowBufferDepthStencil
        {
            get;
        }

        Texture2D ShadowBufferDepthTexture
        {
            get;
        }

        ShaderResourceView DepthTextureResource
        {
            get;
        }

        ITransformer Transformer
        {
            get;
        }
    }
}
