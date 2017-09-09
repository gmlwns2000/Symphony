using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D10.Device;

namespace DirectCanvas.Rendering.Materials
{
    /// <summary>
    /// A RenderTargetTexture wraps a Direct3D texture, similar
    /// to what the Texture class it inherits from does.  The difference
    /// is this will wrap a Direct3D texture that is set to be used
    /// as a render target.  This allows us to add specific features
    /// for render target textures.
    /// </summary>
    class RenderTargetTexture : ShaderResourceTexture
    {
        public RenderTargetTexture(Device device, 
                                   int width, 
                                   int height, 
                                   Format format = Format.B8G8R8A8_UNorm, 
                                   ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
            : base(new Texture2D(device, CreateTextureDescription(width, height, format, optionFlags)))
        {
            InternalRenderTargetView = new RenderTargetView(device, InternalTexture2D);
        }

        public RenderTargetTexture(Texture2D texture2D)
            : base(texture2D)
        {
            InternalRenderTargetView = new RenderTargetView(InternalDevice, InternalTexture2D);
        }


        /// <summary>
        /// The render target view allows us to use
        /// this texture as a render target
        /// </summary>
        public RenderTargetView InternalRenderTargetView { get; private set; }

        /// <summary>
        /// Sets the render target to recieve rendering commands
        /// </summary>
        public void SetRenderTarget()
        {
            SetRenderTarget(new Viewport(0, 0, Description.Width, Description.Height, 0.0f, 1.0f));
        }

        public void SetRenderTarget(Viewport viewport)
        {
            InternalDevice.OutputMerger.SetTargets(InternalRenderTargetView);

            InternalDevice.Rasterizer.SetViewports(viewport);
        }

        /// <summary>
        /// Clears the render target with the given color
        /// </summary>
        /// <param name="color">The color to clear the render target with</param>
        public void Clear(Color4 color)
        {
            var device = InternalDevice;
            device.ClearRenderTargetView(InternalRenderTargetView, color.InternalColor4);
        }

        private static Texture2DDescription CreateTextureDescription(int width, int height, Format format, ResourceOptionFlags optionFlags)
        {
            return new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = format,
                Height = height,
                Width = width,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = optionFlags
            };
        }

        public override void Dispose()
        {
            InternalRenderTargetView.Dispose();
            base.Dispose();
        }
    }
}
