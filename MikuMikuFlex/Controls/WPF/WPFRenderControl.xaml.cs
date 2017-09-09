using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MMF.DeviceManager;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Diagnostics;

namespace MMF.Controls.WPF
{
    /// <summary>
    ///     Interaction logic for WPFRenderControl.xaml
    /// </summary>
    public partial class WPFRenderControl : UserControl, IDisposable
    {
        public bool KeepAspectRatio = false;

        private int _fps = 60;
        public int FPS
        {
            get { return _fps; }
            set { _fps = value; }
        }

        public WPFRenderControl()
        {
            InitializeComponent();
        }

        public WPFRenderControl(RenderContext context)
        {
            RenderContext = context;
            InitializeComponent();
        }

        /// <summary>
        ///     レンダーコンテキスト
        /// </summary>
        public RenderContext RenderContext { get; private set; }

        /// <summary>
        ///     テクスチャのレンダリングコンテキスト
        /// </summary>
        public TextureTargetContext TextureContext { get; private set; }

        public event EventHandler Render=delegate{};

        /// <summary>
        ///     このコントロールのワールド空間
        /// </summary>
        public WorldSpace WorldSpace
        {
            get { return TextureContext.WorldSpace; }
            set { TextureContext.WorldSpace = value; }
        }

        /// <summary>
        ///     背景色のクリア色
        /// </summary>
        public new Color4 Background
        {
            get { return TextureContext.BackgroundColor; }
            set { TextureContext.BackgroundColor = value; }
        }

        /// <summary>
        /// </summary>
        protected D3DImageContainer ImageContainer { get; private set; }

        public void Dispose()
        {
            if (RenderContext != null)
            {
                RenderContext.UpdateRequireWorlds.Remove(WorldSpace);
            }

            if (WorldSpace != null)
            {
                WorldSpace.Dispose();
                WorldSpace = null;
            }

            if (RenderContext != null)
            {
                RenderContext.Dispose();
                RenderContext = null;
            }

            if (ImageContainer != null)
            {
                ImageContainer.Dispose();
                ImageContainer = null;
            }
            if (TextureContext != null)
            {
                TextureContext.Dispose();
                TextureContext = null;
            }

            if (TargetImg != null)
            {
                TargetImg = null;
            }

            Render = null;
        }

        protected virtual RenderContext getRenderContext()
        {
            return RenderContext.CreateContext(null);
        }

        protected virtual TextureTargetContext GetTextureTargetContext()
        {
            //最初のサイズはおかしいので、とりあえず100,100を与えて、最初にリサイズイベントでリサイズする
            return new WPFTargetTextureContext(RenderContext, new System.Drawing.Size(100, 100), SampleManager.getSampleDesc(8, 0, RenderContext, Format.B8G8R8A8_UNorm));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (RenderContext == null)
            {
                RenderContext = getRenderContext();
            }
            TextureContext = GetTextureTargetContext();
            RenderContext.UpdateRequireWorlds.Add(TextureContext.WorldSpace);
            ImageContainer = new D3DImageContainer();
            ImageContainer.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(ImageContainer_IsFrontBufferAvailableChanged);
            TargetImg.Source = ImageContainer;
            ImageContainer.SetBackBufferSlimDX(TextureContext.RenderTarget, RenderContext);
            base.Dispatcher.ShutdownStarted += new System.EventHandler(Dispatcher_ShutdownStarted);
            BeginRenderingScene();
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Dispose();
        }

        private void BeginRenderingScene()
        {
            if (ImageContainer.IsFrontBufferAvailable)
            {
                if(sw == null)
                {
                    sw = new Stopwatch();
                    sw.Start();
                }

                Texture2D renderTarget = TextureContext.RenderTarget;
                ImageContainer.SetBackBufferSlimDX(renderTarget, RenderContext);
                CompositionTarget.Rendering += new EventHandler(Rendering);
            }
        }

        private void StopRenderingScene()
        {
            CompositionTarget.Rendering -= new EventHandler(Rendering);
        }

        Stopwatch sw;

        long lastMS = 0;
        private void Rendering(object sender, EventArgs e)
        {
            if (allowRender && TextureContext != null && ImageContainer != null && sw.ElapsedMilliseconds - lastMS > 1000 / _fps)
            {
                lock (nowRendering)
                {
                    lastMS = sw.ElapsedMilliseconds;
                    TextureContext.Render();
                    ImageContainer.InvalidateD3DImage(TextureContext.RenderTarget, RenderContext);
                    Render?.Invoke(this, new EventArgs());
                }
            }
        }
        private bool allowRender = true;
        private object nowRendering = new object();

        public event EventHandler AllowRenderingChanged;

        public bool AllowRendering
        {
            get
            {
                return allowRender;
            }
            set
            {
                if (allowRender != value)
                {
                    lock (nowRendering)
                    {
                        allowRender = value;
                        AllowRenderingChanged?.Invoke(this, null);
                    }
                }
            }
        }

        private void ImageContainer_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ImageContainer.IsFrontBufferAvailable)
            {
                BeginRenderingScene();
            }
            else
            {
                StopRenderingScene();
            }
        }

        private void WPFRenderControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (TextureContext != null)
            {
                TextureContext.Size = new System.Drawing.Size((int)e.NewSize.Width, (int)e.NewSize.Height);
            }
            if (ImageContainer != null && TextureContext != null)
            {
                ImageContainer.SetBackBufferSlimDX(TextureContext.RenderTarget, RenderContext);
            }
            if (!KeepAspectRatio && TextureContext != null)
            {
                TextureContext.MatrixManager.ProjectionMatrixManager.AspectRatio = (float)(e.NewSize.Width / e.NewSize.Height);
            }
        }
    }
}