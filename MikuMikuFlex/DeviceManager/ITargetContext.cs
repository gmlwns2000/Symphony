using MMF.Matricies;
using MMF.Matricies.Camera;
using SlimDX.Direct3D11;

namespace MMF.DeviceManager
{
    public interface ITargetContext : System.IDisposable
    {
        RenderTargetView RenderTargetView
        {
            get;
        }

        DepthStencilView DepthTargetView
        {
            get;
        }

        MatrixManager MatrixManager
        {
            get;
            set;
        }

        ICameraMotionProvider CameraMotionProvider
        {
            get;
            set;
        }

        WorldSpace WorldSpace
        {
            get;
            set;
        }

        bool IsSelfShadowMode1
        {
            get;
        }

        bool IsEnabledTransparent
        {
            get;
        }

        RenderContext Context
        {
            get;
            set;
        }

        void SetViewport();
    }
}
