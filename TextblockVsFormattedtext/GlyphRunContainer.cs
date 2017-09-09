using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TextblockVsFormattedtext
{
    public class GlyphRunContainer : FrameworkElement
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

        public static Typeface tf = new Typeface("NanumBarunGothic");
        public static GlyphTypeface glyphTypeface = null;

        public GlyphRunContainer(string text, double x, double y)
        {
            canvas = new VisualCollection(this);
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                if (glyphTypeface == null)
                {
                    if (!tf.TryGetGlyphTypeface(out glyphTypeface))
                        throw new InvalidOperationException("No glyphtypeface found");
                }

                double size = 12;

                ushort[] glyphIndexes = new ushort[text.Length];
                double[] advanceWidths = new double[text.Length];

                double totalWidth = 0;

                for (int n = 0; n < text.Length; n++)
                {
                    ushort glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
                    glyphIndexes[n] = glyphIndex;

                    double width = glyphTypeface.AdvanceWidths[glyphIndex] * size;
                    advanceWidths[n] = width;

                    totalWidth += width;
                }

                Point origin = new Point(x, y);

                GlyphRun glyphRun = new GlyphRun(glyphTypeface, 0, false, size,
                    glyphIndexes, origin, advanceWidths, null, null, null, null,
                    null, null);

                dc.DrawGlyphRun(Brushes.Black, glyphRun);
            }

            canvas.Add(visual);
        }
    }
}
