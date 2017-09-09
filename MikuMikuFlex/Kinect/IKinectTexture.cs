using SlimDX.Direct3D11;

namespace MMF.Kinect
{
    public interface IKinectTexture : System.IDisposable
    {
        Texture2D ColorTexture2D
        {
            get;
        }

        ShaderResourceView ColorTexture2DResourceView
        {
            get;
        }

        void UpdateTexture();
    }
}
