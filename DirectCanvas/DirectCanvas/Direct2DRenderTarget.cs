using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectCanvas.Rendering;
using SlimDX.Direct2D;
using SlimDX.DXGI;

namespace DirectCanvas
{
    internal class Direct2DRenderTarget : IDisposable
    {
        private readonly DeviceContext10_1 m_deviceContext10;
        private readonly Surface m_surface;
        private readonly Format m_format;
        private RenderTargetProperties m_renderTargetProperties;
        private RenderTarget m_renderTarget;

        public Direct2DRenderTarget(DeviceContext10_1 deviceContext10, Surface surface, Format format = Format.B8G8R8A8_UNorm)
        {
            m_deviceContext10 = deviceContext10;
            m_surface = surface;
            m_format = format;
            InitializeResources(surface);
        }

        public RenderTarget InternalRenderTarget
        {
            get { return m_renderTarget; }
            private set { m_renderTarget = value; }
        }

        internal Bitmap InternalBitmap { get; private set; }

        private void InitializeResources(Surface surface)
        {
            m_renderTargetProperties = new RenderTargetProperties();

            m_renderTargetProperties.HorizontalDpi = 96;
            m_renderTargetProperties.VerticalDpi = 96;
            m_renderTargetProperties.PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied);
            m_renderTargetProperties.Usage = RenderTargetUsage.None;
            m_renderTargetProperties.MinimumFeatureLevel = FeatureLevel.Direct3D10;
            
            InternalRenderTarget = RenderTarget.FromDXGI(m_deviceContext10.Direct2DFactory, 
                                                         surface,
                                                         m_renderTargetProperties);

            InternalRenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            InternalRenderTarget.TextAntialiasMode = TextAntialiasMode.Grayscale;

            var bitmapProperties = new BitmapProperties();
            bitmapProperties.PixelFormat = new PixelFormat(m_format, AlphaMode.Premultiplied);

            InternalBitmap = new Bitmap(InternalRenderTarget, surface, bitmapProperties);
        }

        public void Dispose()
        {
            InternalBitmap.Dispose();
            InternalRenderTarget.Dispose();
            m_surface.Dispose();
        }
    }
}
