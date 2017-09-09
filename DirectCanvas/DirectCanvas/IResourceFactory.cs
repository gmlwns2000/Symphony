using System.IO;
using DirectCanvas.Brushes;
using DirectCanvas.Imaging;
using DirectCanvas.Misc;
using DirectCanvas.Multimedia;
using DirectCanvas.Shapes;

namespace DirectCanvas
{
    public interface IResourceFactory
    {
        /// <summary>
        /// Creates a new MediaPlayer for playing of multimedia files
        /// </summary>
        /// <returns>A new instance of MediaPlayer</returns>
        MediaPlayer CreateMediaPlayer();

        /// <summary>
        /// Creates a new VideoBrush for using a MediaPlayer as a Brush
        /// </summary>
        /// <param name="player">The player to use for input of the VideoBrush</param>
        /// <returns>A new instance of VideoBrush</returns>
        VideoBrush CreateVideoBrush(MediaPlayer player);

        /// <summary>
        /// Creates a new DrawingLayer for drawing or rendering.
        /// </summary>
        /// <param name="width">The pixel width of the DrawingLayer</param>
        /// <param name="height">The pixel height of the DrawingLayer</param>
        /// <returns>An initialized drawing layer</returns>
        DrawingLayer CreateDrawingLayer(int width, int height);

        DrawingLayer CreateDrawingLayerFromImage(Image image);

        DrawingLayer CreateDrawingLayerFromFile(string filename);

        /// <summary>
        /// Creates a new SolidColorBrush.
        /// </summary>
        /// <param name="color">The color used for the SolidColorBrush</param>
        /// <returns>A new instance of SolidColorBrush</returns>
        SolidColorBrush CreateSolidColorBrush(Color4 color);

        /// <summary>
        /// Creates a new LinearGradientBrush for creating linear gradients.
        /// </summary>
        /// <param name="gradientStops">The gradient stop points</param>
        /// <param name="extendMode">The draw extend mode</param>
        /// <param name="startPoint">The gradient start point.  This is relative to the end point if the Alignment property is a relative type</param>
        /// <param name="endPoint">The gradient stop point.  This is relative to the start point if the Alignment property is a relative type</param>
        /// <returns>A new instance of LinearGradientBrush</returns>
        LinearGradientBrush CreateLinearGradientBrush(GradientStop[] gradientStops, ExtendMode extendMode, PointF startPoint, PointF endPoint);

        /// <summary>
        /// Creates a new CreateRadialGradientBrush for creating radial gradients.
        /// </summary>
        /// <param name="gradientStops">The gradient stop points</param>
        /// <param name="extendMode">The draw extend mode</param>
        /// <param name="centerPoint">The center point of the radial gradient. </param>
        /// <param name="gradientOriginOffset">This offset to move the origin, from center, of the radial gradient</param>
        /// <param name="radius">The radius of the gradiant</param>
        /// <returns>A new instance of RadialGradientBrush</returns>
        RadialGradientBrush CreateRadialGradientBrush(GradientStop[] gradientStops, ExtendMode extendMode, PointF centerPoint, PointF gradientOriginOffset, SizeF radius);

        /// <summary>
        /// Creates a new CreateDrawingLayerBrush, using a drawing layer as an input
        /// </summary>
        /// <param name="drawingLayer">The drawing layer to use as input</param>
        /// <returns>An intialized CreateDrawingLayerBrush</returns>
        DrawingLayerBrush CreateDrawingLayerBrush(DrawingLayer drawingLayer);

        /// <summary>
        /// Creates a new PathGeometry instance used for creating custom geometry
        /// </summary>
        /// <returns></returns>
        PathGeometry CreatePathGeometry();

        Image CreateImage(string filename);

        Image CreateImage(int width, int height);

        Image CreateImage(Stream stream);
    }
}