using MMF.Matricies;
using MMF.Matricies.Camera;
using MMF.Matricies.Projection;
using MMF.Matricies.World;
using MMF.Motion;
using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.DeviceManager
{
    public class TextureTargetContext : ITargetContext, System.IDisposable
    {
        private bool disposed = false;

        private RenderContext context;

        private FPSCounter fpsCounter;

        private SampleDescription sampleDesc = new SampleDescription(1, 0);

        private System.Drawing.Size size;

        private Texture2D renderTarget;

        private RenderTargetView renderTargetView;

        private Texture2D depthTarget;

        private DepthStencilView depthTargetView;

        public RenderContext Context
        {
            get
            {
                return context;
            }
            set
            {
                context = value;
            }
        }

        public SampleDescription SampleDesc
        {
            get
            {
                return sampleDesc;
            }
            set
            {
                SlimDX.Direct3D11.Device device = context.DeviceManager.Device;
                Format format = getRenderTargetTexture2DDescription().Format;
                int num = value.Count;
                int num2;
                while (true)
                {
                    num2 = device.CheckMultisampleQualityLevels(format, num);
                    if (num2 > 0)
                    {
                        break;
                    }
                    num--;
                    if (num <= 0)
                    {
                        goto HELLOWORLD;
                    }
                }
                int quality = System.Math.Min(num2 - 1, value.Quality);
                sampleDesc = new SampleDescription(num, quality);
                HELLOWORLD:
                if (size.Width > 0 && size.Height > 0)
                {
                    ResetTargets();
                }
            }
        }

        public System.Drawing.Size Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size != value && size.Width > 0 && size.Height > 0)
                {
                    size = value;
                    ResetTargets();
                }
            }
        }

        public Texture2D RenderTarget
        {
            get
            {
                return renderTarget;
            }
        }

        public RenderTargetView RenderTargetView
        {
            get
            {
                return renderTargetView;
            }
        }

        public Texture2D DepthTarget
        {
            get
            {
                return depthTarget;
            }
        }

        public DepthStencilView DepthTargetView
        {
            get
            {
                return depthTargetView;
            }
        }

        public MatrixManager MatrixManager
        {
            get;
            set;
        }

        public ICameraMotionProvider CameraMotionProvider
        {
            get;
            set;
        }

        public WorldSpace WorldSpace
        {
            get;
            set;
        }

        public bool IsSelfShadowMode1
        {
            get;
            private set;
        }

        public bool IsEnabledTransparent
        {
            get;
            private set;
        }

        public Color4 BackgroundColor
        {
            get;
            set;
        }

        private void ResetTargets()
        {
            if (RenderTargetView != null && !RenderTargetView.Disposed)
            {
                RenderTargetView.Dispose();
            }
            if (RenderTarget != null && !RenderTarget.Disposed)
            {
                RenderTarget.Dispose();
            }
            if (DepthTargetView != null && !DepthTargetView.Disposed)
            {
                DepthTargetView.Dispose();
            }
            if (DepthTarget != null && !DepthTarget.Disposed)
            {
                DepthTarget.Dispose();
            }
            SlimDX.Direct3D11.Device device = Context.DeviceManager.Device;
            renderTarget = new Texture2D(device, getRenderTargetTexture2DDescription());
            renderTargetView = new RenderTargetView(device, RenderTarget);
            depthTarget = new Texture2D(device, getDepthBufferTexture2DDescription());
            depthTargetView = new DepthStencilView(device, DepthTarget);
            SetViewport();
        }

        public TextureTargetContext(RenderContext context, System.Drawing.Size size, SampleDescription sampleDesc) : this(context, new MatrixManager(new BasicWorldMatrixProvider(), new BasicCamera(new Vector3(0f, 20f, -200f), new Vector3(0f, 3f, 0f), new Vector3(0f, 1f, 0f)), new BasicProjectionMatrixProvider()), size, sampleDesc)
        {
            MatrixManager.ProjectionMatrixManager.InitializeProjection(0.7853982f, size.Width / (float)size.Height, 1f, 2000f);
        }

        public TextureTargetContext(RenderContext context, MatrixManager matrixManager, System.Drawing.Size size, SampleDescription sampleDesc)
        {
            this.context = context;
            context.Timer = new MotionTimer(context);
            SlimDX.Direct3D11.Device device = context.DeviceManager.Device;
            this.size = size;
            SampleDesc = sampleDesc;
            MatrixManager = matrixManager;
            WorldSpace = new WorldSpace();
            fpsCounter = new FPSCounter();
            SetViewport();
        }

        ~TextureTargetContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context = null;
                }
                if (WorldSpace != null)
                {
                    WorldSpace.Dispose();
                    WorldSpace = null;
                }
                if (renderTargetView != null && !renderTargetView.Disposed)
                {
                    renderTargetView.Dispose();
                    renderTargetView = null;
                }
                if (renderTarget != null && !renderTarget.Disposed)
                {
                    renderTarget.Dispose();
                    renderTarget = null;
                }
                if (depthTargetView != null && !depthTargetView.Disposed)
                {
                    depthTargetView.Dispose();
                    depthTargetView = null;
                }
                if (depthTarget != null && !depthTarget.Disposed)
                {
                    depthTarget.Dispose();
                    depthTarget = null;
                }
                disposed = true;
            }
        }

        protected virtual Texture2DDescription getRenderTargetTexture2DDescription()
        {
            return new Texture2DDescription
            {
                Width = Size.Width,
                Height = Size.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                SampleDescription = SampleDesc,
                Usage = ResourceUsage.Default,
                BindFlags = (BindFlags.ShaderResource | BindFlags.RenderTarget),
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };
        }

        protected virtual Texture2DDescription getDepthBufferTexture2DDescription()
        {
            return new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float,
                Width = Size.Width,
                Height = Size.Height,
                MipLevels = 1,
                SampleDescription = SampleDesc
            };
        }

        public void SetViewport()
        {
            Context.DeviceManager.Context.Rasterizer.SetViewports(getViewport());
        }

        public void MoveCameraByCameraMotionProvider()
        {
            if (CameraMotionProvider != null)
            {
                CameraMotionProvider.UpdateCamera(MatrixManager.ViewMatrixManager, MatrixManager.ProjectionMatrixManager);
            }
        }

        protected virtual Viewport getViewport()
        {
            return new Viewport
            {
                Width = Size.Width,
                Height = Size.Height,
                MaxZ = 1f
            };
        }

        public void Render()
        {
            if (WorldSpace != null && !WorldSpace.IsDisposed)
            {
                Context.SetRenderScreen(this);
                Context.ClearScreenTarget(BackgroundColor);
                context.Timer.TickUpdater();

                MoveCameraByCameraMotionProvider();

                WorldSpace.DrawAllResources();

                context.DeviceManager.Context.Flush();

                fpsCounter.CountFrame();
            }
        }
    }
}