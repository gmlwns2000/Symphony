using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using SlimDX.Direct3D10;
using SlimDX.Direct3D9;
using SlimDX.DXGI;
using Format = SlimDX.Direct3D9.Format;
using Surface = SlimDX.Direct3D9.Surface;
using SwapEffect = SlimDX.Direct3D9.SwapEffect;
using Usage = SlimDX.Direct3D9.Usage;
using System.Windows.Media;
using System.Windows.Threading;
using SlimDX.Direct2D;
using DirectCanvas.Misc;
using DirectCanvas.Scenes;
using System.Threading;
using System.Diagnostics;

namespace DirectCanvas
{
    /// <summary>
    /// WPFPresenter renders DirectCanvas content to WPF airspace.
    /// </summary>
    public sealed class WPFPresenter : Presenter
    {
        public Stopwatch Stopwatch { get; set; }

        public double TargetFPS { get; set; } = 60;

        public double FPS { get; private set; } = 0;

        public double RealFPS { get; private set; } = 0;

        /// <summary>
        /// The WPF D3DImage class that is set to the WPF Image source
        /// to render our Direct3D content
        /// </summary>
        private D3DImageSlimDX d3dImage;
        public ImageSource ImageSource
        {
            get
            {
                return d3dImage;
            }
        }

        private bool _isRenderingActive = false;
        public bool IsRenderingActive
        {
            get
            {
                return _isRenderingActive;
            }
            private set
            {
                _isRenderingActive = value;
            }
        }

        private Scene _currentScene;
        public Scene CurrentScene
        {
            get
            {
                return _currentScene;
            }
            set
            {
                if (_currentScene == value)
                    return;

                if (_currentScene != null)
                {
                    _currentScene.DeactivateScene();
                }

                _currentScene = value;

                if (_currentScene != null)
                {
                    _currentScene.ActivateScene();
                }
            }
        }

        public bool VSync { get; set; } = false;

        private bool _isAllowRender = true;
        public bool IsAllowRender
        {
            get
            {
                return _isAllowRender;
            }
            set
            {
                _isAllowRender = value;
            }
        }

        private DrawingLayer _layer;
        protected override DrawingLayer Layer
        {
            get
            {
                return _layer;
            }
        }

        public event EventHandler FrameUpdated;

        public event EventHandler Rendering;

        int _width;
        public new int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        int _height;
        public new int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        Color4 white = new Color4(1, 1, 1, 1);

        Thread RenderingThread;

        #region Initialize

        private WPFPresenter(DirectCanvasFactory directCanvas, int width, int height) : base(directCanvas)
        {
            if (Application.Current == null || Application.Current.Dispatcher == null)
                return;

            var dispatcher = Application.Current.Dispatcher;

            if (!dispatcher.CheckAccess())
            {
                throw new ArgumentException("Initialize must be process on main thread");
            }

            d3dImage = new D3DImageSlimDX();
            d3dImage.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            InitD3DImage(width, height);
        }

        public WPFPresenter(DirectCanvasFactory directCanvas, double width, double height) : this(directCanvas, (int)width, (int)height)
        {

        }

        /// <summary>
        /// Creates a new WPFPresenter instance.
        /// </summary>
        /// <param name="directCanvas">The DirectCanvas factor this presenter belongs to</param>
        /// <param name="width">The pixel width of the presenter</param>
        /// <param name="height">The pixel height of the presenter</param>
        /// <param name="image">The WPF Image to render to</param>
        public WPFPresenter(DirectCanvasFactory directCanvas, double width, double height, Image image) : this(directCanvas, (int)width, (int)height)
        {
            if (!image.Dispatcher.CheckAccess())
            {
                image.Dispatcher.Invoke(delegate
                {
                    image.Source = d3dImage;
                });
            }
            else
            {
                image.Source = d3dImage;
            }
        }

        private void InternalSetSize(int width, int height, bool calledOnThread = false)
        {
            InitD3DImage(width, height, calledOnThread);

            base.SetSize(width, height);

            if (_isAllowRender && _isRenderingActive)
                Present();
        }
        
        public override void SetSize(int width, int height)
        {
            InternalSetSize(width, height);
        }

        public void SetSize(double width, double height)
        {
            InternalSetSize((int)width, (int)height);
        }

        private void InitD3DImage(int width, int height, bool calledOnThread = false)
        {
            _width = width;
            _height = height;

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("canvas width or height must be larger then 0");
            }

            if(width > 4096 || height > 4096)
            {
                throw new ArgumentException("canvas width or height is too much large");
            }
            
            if (RenderingThread != null)
            {
                if (calledOnThread)
                {
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                }
                else
                {
                    StopRendering();
                }
                IsRenderingActive = true;
            }
            ReleaseResources();

            _layer = new DrawingLayer(Factory, width, height, SlimDX.DXGI.Format.B8G8R8A8_UNorm, ResourceOptionFlags.Shared);
            
            if (IsRenderingActive)
            {
                if (calledOnThread)
                {
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                    SetBackBuffer();
                }
                else
                {
                    StartRendering();
                }
            }
        }

        private void SetBackBuffer()
        {
            if (d3dImage.Dispatcher.CheckAccess())
            {
                d3dImage.SetBackBufferSlimDX(_layer.RenderTargetTexture.InternalTexture2D, Factory.DeviceContext.Device);
                d3dImage.PushFrame(_layer.RenderTargetTexture.InternalTexture2D, Factory.DeviceContext.Device);
                d3dImage.InvalidateD3DImage();
            }
            else
            {
                d3dImage.Dispatcher.Invoke(delegate { SetBackBuffer(); });
            }
        }

        #endregion Initialize

        #region Render

        /// <summary>
        /// Presents the DirectCanvas content to WPF
        /// </summary>
        public override void Present()
        {
            frameUpdated = false;

            DoRender();
        }

        public void StartRendering()
        {
            if (d3dImage.IsFrontBufferAvailable)
            {
                SetBackBuffer();

                if (RenderingThread != null)
                    StopRendering();

                if (Stopwatch == null)
                {
                    Stopwatch = new Stopwatch();
                    Stopwatch.Start();
                }

                IsRenderingActive = true;

                RenderingThread = new Thread(new ThreadStart(RenderProc));
                RenderingThread.Name = "DirectCanvas Rendering Thread";
                RenderingThread.IsBackground = true;
                RenderingThread.Start();

                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        public void StopRendering()
        {
            IsRenderingActive = false;

            if (RenderingThread != null)
            {
                RenderingThread.Abort();
                RenderingThread = null;
            }

            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        bool updated = false;

        internal override void DrawCall()
        {
            updated = true;

            base.DrawCall();
        }

        internal override void ComposeCall()
        {
            updated = true;

            base.ComposeCall();
        }

        long preMs = 0;
        long lastCountMs = 0;
        int fpsCount = 0;
        bool frameUpdated = true;
        bool frameUpdating = false;
        private void RenderProc()
        {
            while (_isRenderingActive)
            {
                if (_isAllowRender)
                {
                    if(_layer.Width != _width || _layer.Height != _height)
                    {
                        InternalSetSize(_width, _height, true);
                    }

                    DoRender();

                    //real frame count;
                    RealFPS = 1000 / Math.Max(1, Stopwatch.ElapsedMilliseconds - preMs);

                    //sleep
                    int sleep = (int)Math.Max(1, Math.Min(1000 / TargetFPS - 1, 1000 / TargetFPS - (Stopwatch.ElapsedMilliseconds - preMs)));
                    Thread.Sleep(sleep);

                    //frameCount
                    fpsCount++;
                    if (Stopwatch.ElapsedMilliseconds - lastCountMs > 1000)
                    {
                        FPS = fpsCount;
                        lastCountMs = Stopwatch.ElapsedMilliseconds;
                        fpsCount = 0;
                    }

                    preMs = Stopwatch.ElapsedMilliseconds;
                }
                else
                {
                    Thread.Sleep(2);
                }
            }
        }

        private void DoRender()
        {
            //check available
            bool isavailable = false;

            if (d3dImage == null)
                return;

            d3dImage.Dispatcher.Invoke(new Action(() =>
            {
                if (d3dImage == null)
                    return;

                isavailable = d3dImage.IsFrontBufferAvailable;
            }));

            if (!isavailable)
                return;

            //wait for last frame push
            if (VSync)
            {
                while (frameUpdated)
                {
                    Thread.Sleep(1);
                }
            }

            //start rendering
            frameUpdating = true;
            Scene scene = CurrentScene;
            SlimDX.Direct3D10.Device device = Factory.DeviceContext.Device;

            if (scene != null)
            {
                scene.Render();
            }

            Rendering?.Invoke(this, null);

            device.Flush();

            frameUpdating = false;

            if (updated)
            {
                updated = false;

                frameUpdated = true;

                if (!VSync)
                {
                    PushFrame();
                }
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (VSync)
            {
                while (frameUpdating)
                {
                    Console.WriteLine("waitfor rendering!");
                    if (!_isAllowRender)
                    {
                        break;
                    }
                    Thread.Sleep(1);
                }

                if (frameUpdated)
                {
                    frameUpdated = false;

                    if (_isAllowRender)
                    {
                        PushFrame();
                    }
                }
            }
        }

        private void PushFrame()
        {
            if (_layer == null)
                return;

            d3dImage.PushFrame(_layer.RenderTargetTexture.InternalTexture2D, Factory.DeviceContext.Device);
            d3dImage.Dispatcher.Invoke(new Action(() =>
            {
                d3dImage.InvalidateD3DImage();
            }));
            FrameUpdated?.Invoke(this, null);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (d3dImage.IsFrontBufferAvailable)
            {
                StartRendering();
            }
            else
            {
                StopRendering();
            }
        }

        #endregion Render

        #region Dispose

        private void ReleaseResources()
        {
            if (_layer != null)
            {
                _layer.Dispose();
                _layer = null;
            }
        }

        public override void Dispose()
        {
            StopRendering();

            if (d3dImage != null)
            {
                try
                {
                    d3dImage.Dispatcher.Invoke(delegate
                    {
                        DisposeD3d();
                    });
                }
                catch 
                {
                    Console.WriteLine("D3dImage Disposing Failed...");
                }
            }
            
            ReleaseResources();

            base.Dispose();
        }

        private void DisposeD3d()
        {
            d3dImage.Lock();
            d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            d3dImage.Unlock();

            d3dImage.Dispose();

            d3dImage = null;
        }

        #endregion Dispose
    }
}