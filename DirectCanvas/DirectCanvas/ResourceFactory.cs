using System;
using System.IO;
using DirectCanvas.Brushes;
using DirectCanvas.Imaging;
using DirectCanvas.Misc;
using DirectCanvas.Multimedia;
using DirectCanvas.Rendering.Materials;
using DirectCanvas.Shapes;

namespace DirectCanvas
{
    /// <summary>
    /// Internal factory for creating most resources.  This class belongs to the
    /// DirectCanvasFactory so all resources will belong to it.  This also
    /// creates a central point for creating resources so in the future they can be tracked
    /// and we can implement device lost strategies.
    /// </summary>
    class ResourceFactory : IDisposable, IResourceFactory
    {
        private const int MINIMUM_RENDERTARGET_WIDTH = 128;
        private const int MINIMUM_RENDERTARGET_HEIGHT = 128;

        /// <summary>
        /// Reference to the DirectCanvasFactory
        /// </summary>
        private readonly DirectCanvasFactory m_directCanvasFactory;

        /// <summary>
        /// The RenderTargetTexture texture that backs the Direct2DRenderTarget 
        /// </summary>
        private RenderTargetTexture m_textureResourceOwner;

        /// <summary>
        /// The Direct2DRenderTarget "owns" all resources created by this factory
        /// </summary>
        private Direct2DRenderTarget m_renderTargetResourceOwner;

        /// <summary>
        /// Creates a new instance of the resource factory
        /// </summary>
        /// <param name="canvas">The DirectCanvasFactory this factory belongs to</param>
        public ResourceFactory(DirectCanvasFactory canvas)
        {
            m_directCanvasFactory = canvas;
            m_textureResourceOwner = new RenderTargetTexture(m_directCanvasFactory.DeviceContext.Device, 
                                                             MINIMUM_RENDERTARGET_WIDTH, 
                                                             MINIMUM_RENDERTARGET_HEIGHT);

            m_renderTargetResourceOwner = new Direct2DRenderTarget(m_directCanvasFactory.DeviceContext, 
                                                                   m_textureResourceOwner.InternalDxgiSurface);
        }

        public MediaPlayer CreateMediaPlayer()
        {
            return new MediaPlayer(m_directCanvasFactory, m_renderTargetResourceOwner);
        }

        public VideoBrush CreateVideoBrush(MediaPlayer player)
        {
            return new VideoBrush(player);
        }

        public SolidColorBrush CreateSolidColorBrush(Color4 color)
        {
            return new SolidColorBrush(m_renderTargetResourceOwner, color);
        }

        public LinearGradientBrush CreateLinearGradientBrush(GradientStop[] gradientStops, ExtendMode extendMode, PointF startPoint, PointF endPoint)
        {
            return new LinearGradientBrush(m_renderTargetResourceOwner, gradientStops, extendMode, startPoint, endPoint);
        }

        public RadialGradientBrush CreateRadialGradientBrush(GradientStop[] gradientStops, ExtendMode extendMode, PointF centerPoint, PointF gradientOriginOffset, SizeF radius)
        {
            return new RadialGradientBrush(m_renderTargetResourceOwner, gradientStops, extendMode, centerPoint, gradientOriginOffset, radius);
        }

        public DrawingLayerBrush CreateDrawingLayerBrush(DrawingLayer drawingLayer)
        {
            return new DrawingLayerBrush(drawingLayer);
        }

        public DrawingLayer CreateDrawingLayer(int width, int height)
        {
            return new DrawingLayer(m_directCanvasFactory, width, height);
        }

        public DrawingLayer CreateDrawingLayerFromImage(Image image)
        {
            var layer = CreateDrawingLayer(image.Width, image.Height);

            image.CopyToDrawingLayer(layer);

            return layer;
        }

        public DrawingLayer CreateDrawingLayerFromFile(string filename)
        {
            DrawingLayer layer = null;
            using(var image = CreateImage(filename))
            {
                layer = CreateDrawingLayerFromImage(image);
            }

            return layer;
        }

        public PathGeometry CreatePathGeometry()
        {
            return new PathGeometry(m_renderTargetResourceOwner);
        }

        public Image CreateImage(string filename)
        {
            return new Image(filename, m_directCanvasFactory);
        }

        public Image CreateImage(int width, int height)
        {
            return new Image(width, height, m_directCanvasFactory);
        }

        public Image CreateImage(Stream stream)
        {
            return new Image(stream, m_directCanvasFactory);
        }

        public void Dispose()
        {
           if(m_renderTargetResourceOwner != null)
           {
               m_renderTargetResourceOwner.Dispose();
               m_renderTargetResourceOwner = null;
           }

            if(m_textureResourceOwner != null)
            {
                m_textureResourceOwner.Dispose();
                m_textureResourceOwner = null;
            }
        }
    }
}
