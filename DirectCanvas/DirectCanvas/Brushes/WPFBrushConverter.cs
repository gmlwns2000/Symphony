using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpf = System.Windows.Media;

namespace DirectCanvas.Brushes
{
    public static class WPFBrushConverter
    {
        public static Brush ConvertFromBrush(DirectCanvasFactory factory, wpf.Brush brush)
        {
            if(brush is wpf.SolidColorBrush)
            {
                return ConvertFromSolidColorBrush(factory, (wpf.SolidColorBrush)brush);
            }
            else if(brush is wpf.GradientBrush)
            {
                return ConvertFromGradientBrush(factory, (wpf.GradientBrush)brush);
            }
            else if(brush is wpf.ImageBrush)
            {
                return ConvertFromImageBrush(factory, (wpf.ImageBrush)brush);
            }
            else
            {
                throw new ArgumentException("Unknown Brush");
            }
        }

        public static Brush ConvertFromSolidColorBrush(DirectCanvasFactory factory, wpf.SolidColorBrush brush)
        {
            SolidColorBrush convert = factory.CreateSolidColorBrush(new Color4(brush.Color));

            convert.Opacity *= (float)brush.Opacity;

            return convert;
        }

        public static Brush ConvertFromGradientBrush(DirectCanvasFactory factory, wpf.GradientBrush brush)
        {
            GradientStop[] stops = new GradientStop[brush.GradientStops.Count];

            int ind = 0;
            foreach (wpf.GradientStop g in brush.GradientStops)
            {
                stops[ind] = new GradientStop(new Color4(g.Color), (float)g.Offset);
                ind++;
            }

            if (brush is wpf.LinearGradientBrush)
            {
                return ConvertFromLinearGradientBrush(factory, (wpf.LinearGradientBrush)brush, stops);
            }
            else if(brush is wpf.RadialGradientBrush)
            {
                return ConvertFromRadialGradientBrush(factory, (wpf.RadialGradientBrush)brush, stops);
            }

            return null;
        }

        public static Brush ConvertFromLinearGradientBrush(DirectCanvasFactory factory, wpf.LinearGradientBrush brush, GradientStop[] stops)
        {
            LinearGradientBrush linear = factory.CreateLinearGradientBrush(stops, ExtendMode.Clamp, Misc.Converter.ToPointF(brush.StartPoint), Misc.Converter.ToPointF(brush.EndPoint));

            linear.Alignment = BrushAlignment.GeometryRelative;

            linear.Opacity = (float)brush.Opacity;

            return linear;
        }

        public static Brush ConvertFromRadialGradientBrush(DirectCanvasFactory factory, wpf.RadialGradientBrush brush, GradientStop[] stops)
        {
            RadialGradientBrush radial = factory.CreateRadialGradientBrush(stops, ExtendMode.Clamp, Misc.Converter.ToPointF(brush.Center), new Misc.PointF(0, 0), new Misc.SizeF(brush.RadiusX, brush.RadiusY));

            radial.Alignment = BrushAlignment.GeometryRelative;

            radial.Opacity = (float)brush.Opacity;

            return radial;
        }

        public static Brush ConvertFromImageBrush(DirectCanvasFactory factory, wpf.ImageBrush brush)
        {
            throw new NotImplementedException("This code was not tested. Disable this exception to use it.");

            Stream bmpStream = StreamFromBitmapSource((BitmapSource)brush.ImageSource);
            var brushImg = new Imaging.Image(bmpStream, factory);

            bmpStream.Dispose();

            var layer = factory.CreateDrawingLayerFromImage(brushImg);
            brushImg.Dispose();

            var retBrush = factory.CreateDrawingLayerBrush(layer);
            layer.Dispose();

            return retBrush;
        }

        private static Stream StreamFromBitmapSource(BitmapSource bitmapSrc)
        {
            Stream bmpStream = new MemoryStream();

            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSrc));
            encoder.Save(bmpStream);

            return bmpStream;
        }

    }
}
