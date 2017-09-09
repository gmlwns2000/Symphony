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
using System.Threading;

namespace DirectCanvas
{
    /// <summary>
    /// WPFPresenter renders DirectCanvas content to WPF airspace.
    /// </summary>
    public sealed class WPFBlitPresenter : Presenter
    {
        /// <summary>
        /// Remains for the duration of the application
        /// </summary>
        private static Direct3DEx m_direct3D;

        /// <summary>
        /// Direct3D9 Device.  Remains for duration of the application
        /// </summary>
        private static DeviceEx m_device;

        /// <summary>
        /// Cached pixel width size of presenter layer
        /// </summary>
        private int m_width;

        /// <summary>
        /// Cached pixel hight size of presenter layer
        /// </summary>
        private int m_height;

        /// <summary>
        /// The WPF Image we are rendering to
        /// </summary>
        private readonly Image m_image;

        /// <summary>
        /// The WPF D3DImage class that is set to the WPF Image source
        /// to render our Direct3D content
        /// </summary>
        private D3DImage m_d3dImage;
        public ImageSource ImageSource
        {
            get
            {
                return m_d3dImage;
            }
        }
        /// <summary>
        /// The internal layer which everything is drawn to
        /// </summary>
        private DrawingLayer m_layer;

        /// <summary>
        /// First blit layer.  We need two layers to blit to
        /// to avoid flickering in WPF.  This is temporary until
        /// we build proper syncronization of the rendering
        /// </summary>
        private DrawingLayer m_blitLayer;

        /// <summary>
        /// Second blit layer.  We need two layers to blit to
        /// to avoid flickering in WPF.  This is temporary until
        /// we build proper syncronization of the rendering
        /// </summary>
        private DrawingLayer m_blitLayer2;

        /// <summary>
        /// The shared texture between DX10 and DX9 for WPF interop. Equivalent to the front buffer.
        /// </summary>
        private Texture m_sharedTexture;

        /// <summary>
        /// The source texture that we are drawing to. Equivalent to the back buffer.
        /// </summary>
        private Texture m_sourceTexture;

        /// <summary>
        /// P/Invoke method to get a valid hwnd.  Needed to initialize
        /// Direct3D9
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// Initializes Direct3D9 and creates a static D3D9 device for interop
        /// </summary>
        private static void CreateDevice()
        {
            m_direct3D = new Direct3DEx();

            /* Create the device present parameters */
            var param = new PresentParameters();
            param.Windowed = true;
            param.SwapEffect = SwapEffect.Discard;
            param.BackBufferFormat = Format.A8R8G8B8;
            param.PresentationInterval = PresentInterval.Immediate;

            /* Creates the Direct3D9Ex device.  This must be an "Ex" device
             * to allow resource sharing with D3D10 */
            m_device = new DeviceEx(m_direct3D,
                                    0,
                                    DeviceType.Hardware,
                                    GetDesktopWindow(),
                                    CreateFlags.HardwareVertexProcessing | CreateFlags.Multithreaded | CreateFlags.FpuPreserve,
                                    param);
        }

        /// <summary>
        /// Helper method to get a shared handle from a Direct3D10 texture
        /// </summary>
        /// <param name="texture">The texture to get a shared handle from</param>
        /// <returns>The shared handle of the texture</returns>
        private static IntPtr GetSharedHandle(Texture2D texture)
        {
            var resource = new SlimDX.DXGI.Resource(texture);
            IntPtr result = resource.SharedHandle;
            resource.Dispose();
            return result;
        }

        /// <summary>
        /// Static constructor.  Initiates resources that need to remain
        /// for the length of the application
        /// </summary>
        static WPFBlitPresenter()
        {
            CreateDevice();
        }

        public WPFBlitPresenter(DirectCanvasFactory directCanvas, int width, int height, Dispatcher dispatcher)
            : base(directCanvas)
        {
            m_width = width;
            m_height = height;

            /* Check and see if we are on the UI thread as we may want to support
             * multi-threaded scenarios */
            if (!dispatcher.CheckAccess())
            {
                dispatcher.Invoke((Action)delegate
                {
                    m_d3dImage = new D3DImage();
                });
            }
            else
            {
                m_d3dImage = new D3DImage();
            }

            /* Initialize our special layers */
            CreateLayer(m_width, m_height);
        }

        /// <summary>
        /// Creates a new WPFPresenter instance.
        /// </summary>
        /// <param name="directCanvas">The DirectCanvas factor this presenter belongs to</param>
        /// <param name="width">The pixel width of the presenter</param>
        /// <param name="height">The pixel height of the presenter</param>
        /// <param name="image">The WPF Image to render to</param>
        public WPFBlitPresenter(DirectCanvasFactory directCanvas, int width, int height, Image image)
            : base(directCanvas)
        {
            m_width = width;
            m_height = height;
            m_image = image;

            /* Check and see if we are on the UI thread as we may want to support
             * multi-threaded scenarios */
            if (!image.Dispatcher.CheckAccess())
            {
                image.Dispatcher.Invoke((Action)delegate
                {
                    m_d3dImage = new D3DImage();
                    image.Source = m_d3dImage;
                });
            }
            else
            {
                m_d3dImage = new D3DImage();
                image.Source = m_d3dImage;
            }

            /* Initialize our special layers */
            CreateLayer(m_width, m_height);
        }

        /// <summary>
        /// Creates the special drawing layer the presenter will use to render content
        /// </summary>
        /// <param name="width">Pixel width of the device</param>
        /// <param name="height">Pixel height of the device</param>
        private void CreateLayer(int width, int height)
        {
            /* Release any used resources.  Other resources may
             * already exist if the presenter is being resized */
            ReleaseResources();

            /* This will be the main layer that content is written to */
            m_layer = new DrawingLayer(Factory,
                                       width,
                                       height,
                                       SlimDX.DXGI.Format.B8G8R8A8_UNorm,
                                       ResourceOptionFlags.Shared);

            var sourceHandle = GetSharedHandle(m_layer.RenderTargetTexture.InternalTexture2D);
            m_sourceTexture = new Texture(m_device,
                                          width,
                                          height,
                                          1,
                                          Usage.RenderTarget,
                                          Format.A8R8G8B8,
                                          Pool.Default,
                                          ref sourceHandle);

            

            /* First blit layer.  We need two layers to blit to
             * to avoid flickering in WPF.  This is temporary until
             * we build proper syncronization of the rendering */
            m_blitLayer = new DrawingLayer(Factory,
                                           width,
                                           height,
                                           SlimDX.DXGI.Format.B8G8R8A8_UNorm,
                                           ResourceOptionFlags.KeyedMutex);

            /* Second blit layer.  We need two layers to blit to
             * to avoid flickering in WPF.  This is temporary until
             * we build proper syncronization of the rendering */
            m_blitLayer2 = new DrawingLayer(Factory,
                                            width,
                                            height,
                                            SlimDX.DXGI.Format.B8G8R8A8_UNorm,
                                            ResourceOptionFlags.Shared);

            /* Get the shared handle associated to our last blit texture */
            var sharedHandle = GetSharedHandle(m_blitLayer2.RenderTargetTexture.InternalTexture2D);

            /* This initializes the shared surface on our Direct3D9Ex device so WPF can access it
             * It needs to be pulled in using the exact same size/format/etc as the original */
            m_sharedTexture = new Texture(m_device,
                                          width,
                                          height,
                                          1,
                                          Usage.RenderTarget,
                                          Format.A8R8G8B8,
                                          Pool.Default,
                                          ref sharedHandle);

            Surface s = m_device.GetRenderTarget(0);

            /* We get the surface from our shared texture...*/
            using (Surface surface = m_sharedTexture.GetSurfaceLevel(0))
            {
                InvalidateD3DSurface(surface.ComPointer);
            }
        }

        protected override DrawingLayer Layer
        {
            get { return m_layer; }
        }

        /// <summary>
        /// Sets the size of the presenter
        /// </summary>
        /// <param name="width">Pixel width of the presenter</param>
        /// <param name="height">Pixel height of the presenter</param>
        public override void SetSize(int width, int height)
        {
            m_width = width;
            m_height = height;

            /* Will release the old layers and create new ones */
            CreateLayer(m_width, m_height);

            base.SetSize(width, height);
        }

        /// <summary>
        /// Presents the DirectCanvas content to WPF
        /// </summary>
        public override void Present()
        {
            if (m_sharedTexture == null)
                return;
            Factory.DeviceContext.Device.Flush();
            Thread.Sleep(2);
            NewPresent();
            //OldSlowPresent();
        }

        private void NewPresent()
        {
            using (Surface surfTarget = m_sharedTexture.GetSurfaceLevel(0))
            {
                using (Surface surfSource = m_sourceTexture.GetSurfaceLevel(0))
                {
                    m_device.StretchRectangle(surfSource, surfTarget, TextureFilter.Linear);
                    InvalidateD3DSurface(IntPtr.Zero);
                }
            }
        }

        private void InvalidateD3DSurface(IntPtr targetPointer)
        {
            if (!m_d3dImage.Dispatcher.CheckAccess())
            {
                m_d3dImage.Dispatcher.Invoke((Action)delegate
                    {
                        InvalidateD3DSurface(targetPointer);
                    });
                return;
            }

            if (!m_d3dImage.IsFrontBufferAvailable)
                return;

            try
            {
                m_d3dImage.Lock();
                if (targetPointer != IntPtr.Zero)
                {
                    m_d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, targetPointer);
                }
                m_d3dImage.AddDirtyRect(new Int32Rect(0, 0, m_layer.Width, m_layer.Height));
                m_d3dImage.Unlock();
            }
            catch (ArgumentException ex)
            {
                //Backbuffer might be lost after sleeping
                Uninit();
            }
        }

        private void Uninit()
        {
            throw new NotImplementedException();
        }

        private void OldSlowPresent()
        {
            /* Blit from our rendered layer to our first blitting layer */
            using (var mutex = new KeyedMutex(m_blitLayer.RenderTargetTexture.InternalTexture2D))
            {
                var result = mutex.Acquire(0, 30);
                m_blitLayer.Clear();
                m_blitLayer.BeginDraw();
                m_blitLayer.DrawLayer(Layer);
                m_blitLayer.EndDraw();
                mutex.Release(0);
            }

            /* Blit from our first blitting layer to our second */
            using (var mutex = new KeyedMutex(m_blitLayer.RenderTargetTexture.InternalTexture2D))
            {
                var result = mutex.Acquire(0, 30);
                m_blitLayer2.Clear();
                m_blitLayer2.BeginDraw();
                m_blitLayer2.DrawLayer(m_blitLayer);
                //m_blitLayer2.DrawLayer(Layer);
                m_blitLayer2.EndDraw();
                mutex.Release(0);
            }

            /* Always make sure this is invoked on the UI thread */
            //m_d3dImage.Dispatcher.Invoke((Action) delegate
            //{
            //    m_d3dImage.Lock();
            //    m_d3dImage.AddDirtyRect(new Int32Rect(0, 0, m_width, m_height));
            //    m_d3dImage.Unlock();
            //});
            InvalidateD3DSurface(IntPtr.Zero);

        }

        private void ReleaseResources()
        {
            if (m_sharedTexture != null)
            {
                m_sharedTexture.Dispose();
                m_sharedTexture = null;
            }

            if (m_blitLayer != null)
            {
                m_blitLayer.Dispose();
                m_blitLayer = null;
            }

            if (m_blitLayer2 != null)
            {
                m_blitLayer2.Dispose();
                m_blitLayer2 = null;
            }

            if (m_layer != null)
            {
                m_layer.Dispose();
                m_layer = null;
            }
        }

        public override void Dispose()
        {
            if (m_d3dImage != null)
                m_d3dImage.Dispatcher.Invoke((Action)delegate
                {
                    m_d3dImage.Lock();
                    m_d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
                    m_d3dImage.Unlock();
                    m_d3dImage = null;
                });

            ReleaseResources();

            base.Dispose();
        }
    }
}