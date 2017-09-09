using System;
using DirectCanvas.Rendering;
using System.Windows;

namespace DirectCanvas
{
    /// <summary>
    /// A presenter is a special type of DrawingLayer that allows
    /// rendering to other technologies, such as WPF or Windows Forms
    /// </summary>
    public abstract class Presenter : DrawingLayer
    {
        public event EventHandler<Size> SizeChanged;

        internal protected Presenter(DirectCanvasFactory directCanvas) : base(directCanvas)
        {

        }

        /// <summary>
        /// We override this property so our subclassed Presenters can change
        /// the "Layer" property instance, which is required for say a 
        /// swapchain backed Presenter
        /// </summary>
        internal override Direct2DRenderTarget D2DRenderTarget
        {
            get
            {
                return Layer.D2DRenderTarget;
            }
        }

        /// <summary>
        /// We override this property so our subclassed Presenters can change
        /// the "Layer" property instance, which is required for say a 
        /// swapchain backed Presenter
        /// </summary>
        internal override Rendering.Materials.RenderTargetTexture RenderTargetTexture
        {
            get
            {
                return Layer.RenderTargetTexture;
            }
        }

        /// <summary>
        /// Presents the DirectCanvas content
        /// </summary>
        public abstract void Present();

        /// <summary>
        /// The current drawing layer which the consumer should write to
        /// </summary>
        protected abstract DrawingLayer Layer { get; }

        /// <summary>
        /// Sets the size of the pres enter, in pixels.
        /// </summary>
        /// <param name="width">The pixel width to create</param>
        /// <param name="height">The pixel height to create</param>
        public virtual void SetSize(int width, int height)
        {
            EnsureSystemMemoryTexture();

            SizeChanged?.Invoke(this, new Size(width, height));
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
