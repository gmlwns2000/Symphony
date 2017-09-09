using System.Diagnostics;
using DirectCanvas.Rendering;
using Control = System.Windows.Forms.Control;

namespace DirectCanvas
{
    /// <summary>
    /// WindowsFormsPresenter renders DirectCanvas content to a Windows Forms control
    /// </summary>
    public sealed class WindowsFormsPresenter : Presenter
    {
        /// <summary>
        /// The Windows Forms control we are rendering to.
        /// </summary>
        private readonly Control m_control;

        /// <summary>
        /// The internal swap chain for the Hwnd
        /// </summary>
        private DxgiSwapChain m_swapChain;

        /// <summary>
        /// Because this is swap chain based, our current drawing layer changes
        /// after each present.  This is just used to cache which one was last.
        /// </summary>
        private DrawingLayer m_lastDrawingLayer;

        /// <summary>
        /// Flag to determine if we need to find the newest drawing layer from
        /// the swap chain.
        /// </summary>
        private bool m_drawingLayerInstanceDirty = true;

        /// <summary>
        /// Counter for how many times we have presented
        /// </summary>
        private long m_presentCounter = 0;

        /// <summary>
        /// Instantiates a new Windows Forms Presenter
        /// </summary>
        /// <param name="directCanvas">The DirectCanvasFactor this presenter belongs to</param>
        /// <param name="control"></param>
        public WindowsFormsPresenter(DirectCanvasFactory directCanvas, Control control) : base(directCanvas)
        {
            m_control = control;

            AttachToControl();
        }

        protected override DrawingLayer Layer
        {
            get
            {
                /* Check and see if we need to find the current drawing layer */
                if (m_drawingLayerInstanceDirty || m_lastDrawingLayer == null)
                    m_lastDrawingLayer = m_swapChain.GetCurrentDrawingLayer();

                /* Reset our flag */
                m_drawingLayerInstanceDirty = false;

                return m_lastDrawingLayer;
            }
        }

        /// <summary>
        /// Creates a new swapchain attached to our Windows Forms contro
        /// </summary>
        private void AttachToControl()
        {
            m_swapChain = new DxgiSwapChain10_1(Factory, m_control.Handle, m_control.ClientSize.Width, m_control.ClientSize.Height);
        }

        /// <summary>
        /// Sets the size of the presenter
        /// </summary>
        /// <param name="width">Pixel width of the presenter</param>
        /// <param name="height">Pixel height of the presenter</param>
        public override void SetSize(int width, int height)
        {
            m_swapChain.ResizeBuffers(width, height);
            base.SetSize(width, height);
        }

        /// <summary>
        /// Presents the DirectCanvas content to the Windows Forms control
        /// </summary>
        public override void Present()
        {
            m_presentCounter++;

            /* Where the actual presenting happens...*/
            m_swapChain.Present();

            /* Set our flag so we know to use the latest swapchain buffer */
            m_drawingLayerInstanceDirty = true;
        }

        public override void Dispose()
        {
            if(m_swapChain != null)
            {
                Layer.Dispose();
                m_lastDrawingLayer = null;
                m_swapChain.Dispose();
            }

            base.Dispose();
        }
    }
}