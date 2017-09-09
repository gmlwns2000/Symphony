using System;
using SlimDX;
using SlimDX.DXGI;

namespace DirectCanvas.Rendering
{
    /// <summary>
    /// The DxgiSwapChain is an internal class used to handle DXGI swapchain
    /// specifics.  This is an abstract class that should be inherited to implement
    /// specific versions of Direct3D
    /// </summary>
    abstract class DxgiSwapChain : IDisposable
    {
        private readonly ComObject m_device;
        private readonly Factory m_factory;
        private readonly IntPtr m_hWnd;
        private readonly int m_width;
        private readonly int m_height;

        protected DxgiSwapChain(ComObject device, Factory factory, IntPtr hWnd, int width, int height)
        {
            m_device = device;
            m_factory = factory;
            m_hWnd = hWnd;
            m_width = width;
            m_height = height;

            InitializeResources();
        }

        private void InitializeResources()
        {
            /* Creates a new DXGI swap chain */
            InternalSwapChain = new SwapChain(m_factory, m_device, new SwapChainDescription
            {
                BufferCount = 1,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                ModeDescription = new ModeDescription(m_width, m_height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                OutputHandle = m_hWnd,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput | Usage.ShaderInput
            });
        }

        /// <summary>
        /// Resizes the swapchain backbuffers.
        /// </summary>
        /// <param name="width">The width in pixels</param>
        /// <param name="height">The height in pixels</param>
        public void ResizeBuffers(int width, int height)
        {
            var description = InternalSwapChain.Description;

            OnResizeBuffers(width, height);

            InternalSwapChain.ResizeBuffers(1 /* Buffer count */,
                                            width,
                                            height, 
                                            description.ModeDescription.Format, 
                                            description.Flags);
        }

        /// <summary>
        /// Called when a resize of the the backbuffers is requested
        /// </summary>
        /// <param name="width">The width in pixels</param>
        /// <param name="height">The height in pixels</param>
        protected abstract void OnResizeBuffers(int width, int height);

        /// <summary>
        /// Returns the current backbuffer from the swapchain used for drawing
        /// </summary>
        public abstract DrawingLayer GetCurrentDrawingLayer();
        
        /// <summary>
        /// Internal reference to the DXGI swap chain
        /// </summary>
        public SwapChain InternalSwapChain { get; private set; }

        /// <summary>
        /// Presents the Swapchain
        /// </summary>
        public void Present()
        {
            InternalSwapChain.Present(0, PresentFlags.None);
        }

        virtual public void Dispose()
        {
            if(InternalSwapChain != null)
            {
                InternalSwapChain.Dispose();
                InternalSwapChain = null;
            }
        }
    }
}
