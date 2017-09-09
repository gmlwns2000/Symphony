using System;
using System.IO;
using DirectCanvas.Brushes;
using DirectCanvas.Effects;
using DirectCanvas.Imaging;
using DirectCanvas.Imaging.WIC;
using DirectCanvas.Misc;
using DirectCanvas.Rendering;
using DirectCanvas.Rendering.Effects;
using DirectCanvas.Rendering.Materials;
using SlimDX.Direct3D10;

namespace DirectCanvas
{
    /// <summary>
    /// The DirectCanvasFactory is the central instance to create resources
    /// such as drawing layers, brushes and media players.  All resources created
    /// with a DirectCanvasFactory instance can interoperate, where applicable, with
    /// other resources created with that DirectCanvasFactory instance. 
    /// </summary>
    public sealed class DirectCanvasFactory : IDisposable, IResourceFactory
    {
        private CompositionDrawStateManagement m_composeStateManagement;

        /// <summary>
        /// The composer engine.
        /// </summary>
        private Composer m_composer;

        /// <summary>
        /// The shader renderer.  This allows effects to be processed.
        /// </summary>
        private ShaderRenderer m_shaderRenderer;

        /// <summary>
        /// The real resource factory.  
        /// Both DirectCanvasFactory and ResourceFactory implement IResourceFactory
        /// </summary>
        private ResourceFactory m_resources;

        /// <summary>
        /// The WIC Factory for dealing with imaging.
        /// </summary>
        private IWICImagingFactory m_wicImagingFactory;

        /// <summary>
        /// Creates a new instance of DirectCanvasFactory
        /// </summary>
        public DirectCanvasFactory()
        {
            m_composeStateManagement = new CompositionDrawStateManagement();

            InitializeDeviceIndependantResources();
            InitializeDeviceResources();
        }

        private void InitializeDeviceIndependantResources()
        {
            m_wicImagingFactory = new WICImagingFactory() as IWICImagingFactory;
        }

        private void InitializeDeviceResources()
        {
            var deviceSettings = new DeviceSettings10_1();
            
            /* Direct2D needs BgraSupport support flag to be set */
            deviceSettings.CreationFlags = DeviceCreationFlags.BgraSupport;

            /* Creates the new device */
            DeviceContext = new DeviceContext10_1(deviceSettings);

            /* Instantiate our composer */
            m_composer = new Composer(DeviceContext);

            /* Make sure we can create pixel shader effects... */
            m_shaderRenderer = new ShaderRenderer(this);

            /* Make sure we can create resources :) */
            m_resources = new ResourceFactory(this);
        }

        /// <summary>
        /// The DeviceContext of the DirectCanvasFactory
        /// </summary>
        internal DeviceContext10_1 DeviceContext { get; private set; }

        internal IWICImagingFactory WicImagingFactory
        {
            get { return m_wicImagingFactory; }
        }

        /// <summary>
        /// Begins the composition process.  BeingCompose always needs 
        /// to be eventually followed by an EndCompose.
        /// </summary>
        /// <param name="renderTarget">The RenderTargetTexture to begin composition on.</param>
        /// <param name="blendStateMode">The blend state to use</param>
        internal void BeginCompose(RenderTargetTexture renderTarget, BlendStateMode blendStateMode)
        {
            m_composeStateManagement.BeginDrawState();
            m_composer.BeginDraw(renderTarget, blendStateMode);
        }

        /// <summary>
        /// Ends the composition process and flushes any remaining commands.  
        /// A BeginCompose must have been call first.
        /// </summary>
        internal void EndCompose()
        {
            m_composeStateManagement.EndDrawState();

            /* Flushes all the commands */
            m_composer.EndDraw();
        }

        /// <summary>
        /// Composes a DrawingLayer with another DrawingLayer.  
        /// This allows for things like scaling, blending, 2D and 3D transformations
        /// </summary>
        /// <param name="layer">The DrawingLayer that is used as the input</param>
        /// <param name="sourceArea">The area over the input DrawingLayer</param>
        /// <param name="destinationArea">The output area to draw the source area</param>
        /// <param name="rotationTransform">The rotation parameters</param>
        /// <param name="tint">The color to tint the source layer on to the output</param>
        internal void Compose(DrawingLayer layer, ref RectangleF sourceArea, ref RectangleF destinationArea, ref RotationParameters rotationTransform, ref Color4 tint)
        {
            m_composeStateManagement.DrawPreamble();

            m_composer.ComposeDrawingLayer(layer, ref sourceArea, ref destinationArea, ref rotationTransform, ref tint);
        }

        /// <summary>
        /// Composes a DrawingLayer with another DrawingLayer.  
        /// This allows for things like scaling, blending, 2D and 3D transformations
        /// </summary>
        /// <param name="layer">The DrawingLayer that is used as the input</param>
        /// <param name="sourceArea">The area over the input DrawingLayer</param>
        /// <param name="destinationArea">The output area to draw the source area</param>
        /// <param name="rotationTransform">The rotation parameters</param>
        /// <param name="tint">The color to tint the source layer on to the output</param>
        internal void Compose(DrawingLayer layer, RectangleF sourceArea, RectangleF destinationArea, RotationParameters rotationTransform, Color4 tint)
        {
            m_composeStateManagement.DrawPreamble();

            m_composer.ComposeDrawingLayer(layer, sourceArea, destinationArea, rotationTransform, tint);
        }

        /// <summary>
        /// Applys a shader effect
        /// </summary>
        /// <param name="effect">The shader effect to apply</param>
        /// <param name="input">The DrawingLayer to be used as input</param>
        /// <param name="output">The DrawingLayer to be used as output</param>
        /// <param name="clearOutput">Clear the output before writing</param>
        internal void ApplyEffect(ShaderEffect effect, DrawingLayer input, DrawingLayer output, bool clearOutput = true)
        {
            m_shaderRenderer.Apply(effect, input.RenderTargetTexture, output.RenderTargetTexture, clearOutput );
        }

        internal void ApplyEffect(ShaderEffect effect, DrawingLayer input, DrawingLayer output, Rectangle targetRect, bool clearOutput = true)
        {
            m_shaderRenderer.Apply(effect, input.RenderTargetTexture, output.RenderTargetTexture, targetRect, clearOutput);
        }

        /// <summary>
        /// Creates a new MediaPlayer for playing of multimedia files
        /// </summary>
        /// <returns>A new instance of MediaPlayer</returns>
        public Multimedia.MediaPlayer CreateMediaPlayer()
        {
            return m_resources.CreateMediaPlayer();
        }

        /// <summary>
        /// Creates a new VideoBrush for using a MediaPlayer as a Brush
        /// </summary>
        /// <param name="player">The player to use for input of the VideoBrush</param>
        /// <returns>A new instance of VideoBrush</returns>
        public Brushes.VideoBrush CreateVideoBrush(Multimedia.MediaPlayer player)
        {
            return m_resources.CreateVideoBrush(player);
        }

        /// <summary>
        /// Creates a new DrawingLayer for drawing or rendering.
        /// </summary>
        /// <param name="width">The pixel width of the DrawingLayer</param>
        /// <param name="height">The pixel height of the DrawingLayer</param>
        /// <returns>An initialized drawing layer</returns>
        public DrawingLayer CreateDrawingLayer(int width, int height)
        {
            return m_resources.CreateDrawingLayer(width, height);
        }

        /// <summary>
        /// Creates a new SolidColorBrush.
        /// </summary>
        /// <param name="color">The color used for the SolidColorBrush</param>
        /// <returns>A new instance of SolidColorBrush</returns>
        public Brushes.SolidColorBrush CreateSolidColorBrush(Color4 color)
        {
            return m_resources.CreateSolidColorBrush(color);
        }

        /// <summary>
        /// Creates a new LinearGradientBrush for creating linear gradients.
        /// </summary>
        /// <param name="gradientStops">The gradient stop points</param>
        /// <param name="extendMode">The draw extend mode</param>
        /// <param name="startPoint">The gradient start point.  This is relative to the end point if the Alignment property is a relative type</param>
        /// <param name="endPoint">The gradient stop point.  This is relative to the start point if the Alignment property is a relative type</param>
        /// <returns>A new instance of LinearGradientBrush</returns>
        public Brushes.LinearGradientBrush CreateLinearGradientBrush(Brushes.GradientStop[] gradientStops, Brushes.ExtendMode extendMode, PointF startPoint, PointF endPoint)
        {
            return m_resources.CreateLinearGradientBrush(gradientStops, extendMode, startPoint, endPoint);
        }

        /// <summary>
        /// Creates a new CreateRadialGradientBrush for creating radial gradients.
        /// </summary>
        /// <param name="gradientStops">The gradient stop points</param>
        /// <param name="extendMode">The draw extend mode</param>
        /// <param name="centerPoint">The center point of the radial gradient. </param>
        /// <param name="gradientOriginOffset">This offset to move the origin, from center, of the radial gradient</param>
        /// <param name="radius">The radius of the gradiant</param>
        /// <returns>A new instance of RadialGradientBrush</returns>
        public Brushes.RadialGradientBrush CreateRadialGradientBrush(Brushes.GradientStop[] gradientStops, Brushes.ExtendMode extendMode, PointF centerPoint, PointF gradientOriginOffset, SizeF radius)
        {
            return m_resources.CreateRadialGradientBrush(gradientStops, extendMode, centerPoint, gradientOriginOffset, radius);
        }

        /// <summary>
        /// Creates a new DrawingLayerBrush, using a drawing layer as an input
        /// </summary>
        /// <param name="drawingLayer">The drawing layer to use as input</param>
        /// <returns>An intialized DrawingLayerBrush</returns>
        public DrawingLayerBrush CreateDrawingLayerBrush(DrawingLayer drawingLayer)
        {
            return m_resources.CreateDrawingLayerBrush(drawingLayer);
        }

        /// <summary>
        /// Creates a new PathGeometry instance used for creating custom geometry
        /// </summary>
        /// <returns></returns>
        public Shapes.PathGeometry CreatePathGeometry()
        {
            return m_resources.CreatePathGeometry();
        }

        public Image CreateImage(string filename)
        {
            return m_resources.CreateImage(filename);
        }

        public DrawingLayer CreateDrawingLayerFromFile(string filename)
        {
            return m_resources.CreateDrawingLayerFromFile(filename);
        }

        public DrawingLayer CreateDrawingLayerFromImage(Image image)
        {
            return m_resources.CreateDrawingLayerFromImage(image);
        }

        public Image CreateImage(int width, int height)
        {
            return m_resources.CreateImage(width, height);
        }

        public Image CreateImage(Stream stream)
        {
            return m_resources.CreateImage(stream);
        }
        
        public void Dispose()
        {
            if (m_composer != null)
            {
                m_composer.Dispose();
                m_composer = null;
            }

            if(m_shaderRenderer != null)
            {
                m_shaderRenderer.Dispose();
            }

            if (m_resources != null)
            {
                m_resources.Dispose();
                m_resources = null;
            }
        }
    }
}
