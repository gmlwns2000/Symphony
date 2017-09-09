using SlimDX.Direct3D11;

namespace MMF.Model
{
    public interface IToonTextureManager : System.IDisposable
    {
        ShaderResourceView[] ResourceViews
        {
            get;
        }

        void Initialize(RenderContext context, ISubresourceLoader subresourceManager);

        int LoadToon(string path);
    }
}
