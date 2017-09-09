using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TextblockVsFormattedtext
{
    public class FormattedTextContainer : FrameworkElement
    {
        VisualCollection canvas;

        protected override Visual GetVisualChild(int index)
        {
            return canvas[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return canvas.Count;
            }
        }


        public static CultureInfo currentCulture = CultureInfo.CurrentCulture;
        public static Typeface tf = new Typeface("NanumBarunGothic");

        public FormattedTextContainer(string text, double x, double y)
        {
            canvas = new VisualCollection(this);
            DrawingVisual visual = new DrawingVisual();

            using(DrawingContext dc = visual.RenderOpen())
            {
                FormattedText ft = new FormattedText(text, currentCulture, FlowDirection.LeftToRight, tf, 12, Brushes.Black);

                Width = ft.Width;
                Height = ft.Height;

                dc.DrawText(ft, new Point(x,y));
            }

            canvas.Add(visual);
        }
    }
}
