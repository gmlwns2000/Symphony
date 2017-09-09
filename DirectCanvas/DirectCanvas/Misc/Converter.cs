using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFMedia = System.Windows.Media;
using DirectCanvas.Brushes;

namespace DirectCanvas.Misc
{
    public static class Converter
    {
        public static Brush ToBrush(DirectCanvasFactory factory, WPFMedia.Brush brush)
        {
            return WPFBrushConverter.ConvertFromBrush(factory, brush);
        }

        public static Brush ToBrush(DirectCanvasFactory factory, WPFMedia.Pen pen)
        {
            return ToBrush(factory, pen.Brush);
        }

        public static Rectangle ToRectangle(int x, int y, int w, int h)
        {
            return new Rectangle(x, y, w, h);
        }

        public static Rectangle ToRectangle(System.Windows.Rect Rect)
        {
            return ToRectangle((int)Rect.X, (int)Rect.Y, (int)Rect.Width, (int)Rect.Height);
        }

        public static Rectangle ToRectangle(System.Windows.Int32Rect Rect)
        {
            return ToRectangle(Rect.X, Rect.Y, Rect.Width, Rect.Height);
        }

        public static RectangleF ToRectangleF(float x, float y, float w, float h)
        {
            return new RectangleF(x, y, w, h);
        }

        public static RectangleF ToRectangleF(System.Windows.Rect rect)
        {
            return ToRectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
        }

        public static Size ToSize(int w, int h)
        {
            return new Size(w, h);
        }

        public static Size ToSize(System.Windows.Size size)
        {
            return ToSize((int)size.Width, (int)size.Height);
        }

        public static SizeF ToSizeF(float w, float h)
        {
            return new SizeF(w, h);
        }

        public static SizeF ToSizeF(System.Windows.Size size)
        {
            return ToSizeF((int)size.Width, (int)size.Height);
        }

        public static Point ToPoint(int x, int y)
        {
            return new Point(x, y);
        }

        public static Point ToPoint(System.Windows.Point pt)
        {
            return ToPoint((int)pt.X, (int)pt.Y);
        }

        public static PointF ToPointF(float x, float y)
        {
            return new PointF(x, y);
        }

        public static PointF ToPointF(System.Windows.Point pt)
        {
            return new PointF(pt.X, pt.Y);
        }
    }
}
