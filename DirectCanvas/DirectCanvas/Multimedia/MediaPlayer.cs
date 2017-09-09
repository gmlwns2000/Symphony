using System;
using System.Runtime.InteropServices;
using DirectCanvas.Misc;
using DirectCanvas.Rendering.Materials;
using DirectShowLib;
using SlimDX.Direct2D;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using System.IO;

namespace DirectCanvas.Multimedia
{
    /// <summary>
    /// The media player plays back multimedia files.
    /// </summary>
    public class MediaPlayer : IDisposable
    {
        /// <summary>
        /// Reference to the DirectCanvasFactory
        /// </summary>
        private readonly DirectCanvasFactory m_directCanvasFactory;

        /// <summary>
        /// The resource owner of this MediaPlayer
        /// </summary>
        private readonly Direct2DRenderTarget m_resourceOwner;

        /// <summary>
        /// The DirectShow graph
        /// </summary>
        private IGraphBuilder m_graph;

        /// <summary>
        /// The DirectShow media control.  Used to play, pause, stop, etc
        /// </summary>
        private IMediaControl m_mediaControl;

        /// <summary>
        /// The shared Direct3D surface
        /// </summary>
        private Texture m_sharedSurface;

        /// <summary>
        /// The Direct2D Bitmap that is shared with the _shared_ Direct3d surface
        /// </summary>
        private Bitmap m_internalBitmap;

        /// <summary>
        /// Flag to see if the video is ready to be drawn/rendered
        /// </summary>
        private bool m_isVideoReady;

        internal MediaPlayer(DirectCanvasFactory canvas, Direct2DRenderTarget resourceOwner)
        {
            m_directCanvasFactory = canvas;
            m_resourceOwner = resourceOwner;
        }

        internal Bitmap InternalBitmap
        {
            get
            {
                return m_internalBitmap;
            }
        }

        public void Open(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            /* Creates the GraphBuilder COM object */
            m_graph = new FilterGraphNoThread() as IGraphBuilder;

            if (m_graph == null)
                throw new Exception("Could not create a graph");


            IBaseFilter renderer = CreateVideoMixingRenderer9(m_graph, 1);

            var filterGraph = m_graph as IFilterGraph2;

            
            if (filterGraph == null)
                throw new Exception("Could not QueryInterface for the IFilterGraph2");

            IBaseFilter sourceFilter;

            /* Have DirectShow find the correct source filter for the Uri */
            int hr = filterGraph.AddSourceFilter(file, file, out sourceFilter);
            DsError.ThrowExceptionForHR(hr);

            /* We will want to enum all the pins on the source filter */
            IEnumPins pinEnum;

            hr = sourceFilter.EnumPins(out pinEnum);
            DsError.ThrowExceptionForHR(hr);

            IntPtr fetched = IntPtr.Zero;
            IPin[] pins = { null };

            /* Counter for how many pins successfully rendered */
            int pinsRendered = 0;


            /* Loop over each pin of the source filter */
            while (pinEnum.Next(pins.Length, pins, fetched) == 0)
            {
                if (filterGraph.RenderEx(pins[0],
                                         AMRenderExFlags.None,
                                         IntPtr.Zero) >= 0)
                    pinsRendered++;

                Marshal.ReleaseComObject(pins[0]);
            }

            Marshal.ReleaseComObject(pinEnum);
            Marshal.ReleaseComObject(sourceFilter);

            if (pinsRendered == 0)
                throw new Exception("Could not render any streams from the source Uri");

            m_mediaControl = m_graph as IMediaControl;
        }

        public void Play()
        {
            if(m_mediaControl == null)
                return;

            m_mediaControl.Run();
        }

        public void Pause()
        {
            if (m_mediaControl == null)
                return;

            m_mediaControl.Pause();
        }

        /// <summary>
        /// Creates a new VMR9 renderer and configures it with an allocator
        /// </summary>
        /// <returns>An initialized DirectShow VMR9 renderer</returns>
        private IBaseFilter CreateVideoMixingRenderer9(IGraphBuilder graph, int streamCount)
        {
            var vmr9 = new VideoMixingRenderer9() as IBaseFilter;

            var filterConfig = vmr9 as IVMRFilterConfig9;

            if (filterConfig == null)
                throw new Exception("Could not query filter configuration.");

            /* We will only have one video stream connected to the filter */
            int hr = filterConfig.SetNumberOfStreams(streamCount);
            DsError.ThrowExceptionForHR(hr);

            /* Setting the renderer to "Renderless" mode
             * sounds counter productive, but its what we
             * need to do for setting up a custom allocator */
            hr = filterConfig.SetRenderingMode(VMR9Mode.Renderless);
            DsError.ThrowExceptionForHR(hr);

            /* Query the allocator interface */
            var vmrSurfAllocNotify = vmr9 as IVMRSurfaceAllocatorNotify9;

            if (vmrSurfAllocNotify == null)
                throw new Exception("Could not query the VMR surface allocator.");

            var allocator = new Vmr9Allocator();

            IntPtr userId = new IntPtr(unchecked((int) 0xDEADBEEF));

            /* We supply our custom allocator to the renderer */
            hr = vmrSurfAllocNotify.AdviseSurfaceAllocator(userId, allocator);
            DsError.ThrowExceptionForHR(hr);

            hr = allocator.AdviseNotify(vmrSurfAllocNotify);
            DsError.ThrowExceptionForHR(hr);

            allocator.NewAllocatorSurface += OnNewAllocatorSurfaceCallback;
            hr = graph.AddFilter(vmr9, string.Format("Renderer: {0}", "VideoMixingRenderer9"));

            DsError.ThrowExceptionForHR(hr);

            return vmr9;
        }

        public event EventHandler IsVideoReadyChanged;

        public void InvokeIsVideoReadyChanged(EventArgs e)
        {
            EventHandler handler = IsVideoReadyChanged;
            if (handler != null) handler(this, e);
        }

        public bool IsVideoReady
        {
            get { return m_isVideoReady; }
            private set
            {
                var oldvalue = m_isVideoReady;
                m_isVideoReady = value;

                if(oldvalue != m_isVideoReady)
                    InvokeIsVideoReadyChanged(new EventArgs());
            }
        }

        internal Direct2DRenderTarget ResourceOwner
        {
            get
            {
                return m_resourceOwner;
            }
        }

        public Size NaturalSize { get; private set; }

        private void OnNewAllocatorSurfaceCallback(object sender, IntPtr pSurface, IntPtr sharedHandle, Misc.Size size)
        {
            /* Reset our fields */
            IsVideoReady = false;
            NaturalSize = new Size(0, 0);

            /* Free any resources that may be in use */
            if(m_internalBitmap != null)
            {
                m_internalBitmap.Dispose();
                m_internalBitmap = null;
            }

            if(m_sharedSurface != null)
            {
                m_sharedSurface.Dispose();
                m_sharedSurface = null;
            }

            Texture2D sharedTexture2D = null;
            var device = m_directCanvasFactory.DeviceContext.Device;

            if (sharedHandle == IntPtr.Zero)
                return;

            try
            {
                /* Open up the shared surface using the passed shared handle */
                sharedTexture2D = device.OpenSharedResource<Texture2D>(sharedHandle);
            }
            catch (Exception)
            {
            }
            
            if(sharedTexture2D == null)
            {
                return;
            }

            /* Wrap our Texture2D in our Texture class */
            m_sharedSurface = new Texture(sharedTexture2D);


            var props = new BitmapProperties();
            props.PixelFormat = new PixelFormat(Format.Unknown, 
                                                AlphaMode.Premultiplied);

            /* Create a Direct2D bitmap, using the shared surface provided by 
             * our custom allocator */
            m_internalBitmap = new Bitmap(ResourceOwner.InternalRenderTarget, 
                                          m_sharedSurface.InternalDxgiSurface, 
                                          props);

            NaturalSize = new Size(m_sharedSurface.Description.Width, 
                                   m_sharedSurface.Description.Height);
            IsVideoReady = true;
        }

        public void Dispose()
        {
            IsVideoReady = false;

            if (m_mediaControl != null)
            {
                m_mediaControl.Stop();
                Marshal.ReleaseComObject(m_mediaControl);
                m_mediaControl = null;
            }

            if (m_graph != null)
            {
                Marshal.ReleaseComObject(m_graph);
                m_graph = null;
            }

            if(m_internalBitmap != null)
            {
                m_internalBitmap.Dispose();
                m_internalBitmap = null;
            }

            if(m_sharedSurface != null)
            {
                m_sharedSurface.Dispose();
                m_sharedSurface = null;
            }
        }
    }
}
