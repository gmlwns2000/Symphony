using SlimDX.Direct3D11;

namespace MMF.Sprite
{
    public interface IDynamicTexture : System.IDisposable
    {
        Texture2D TextureResource
        {
            get;
        }

        ShaderResourceView TextureResourceView
        {
            get;
        }

        bool NeedUpdate
        {
            get;
        }

        void UpdateTexture();
    }
}
