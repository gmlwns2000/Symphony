using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Symphony.UI
{
    public class ShadowedTextBlock : FrameworkElement
    {
        VisualCollection canvas;

        private double _shadowX = 0;
        public double ShadowX
        {
            get
            {
                return _shadowX;
            }
            set
            {
                if (_shadowX != value)
                {
                    _shadowX = value;
                    UpdateText();
                }
            }
        }

        private double _shadowY = 1;
        public double ShadowY
        {
            get
            {
                return _shadowY;
            }
            set
            {
                if(_shadowY != value)
                {
                    _shadowY = value;
                    UpdateText();
                }
            }
        }

        public static readonly DependencyProperty ShadowProperty = DependencyProperty.Register("Shadow", typeof(Brush), typeof(ShadowedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
        public Brush Shadow
        {
            get
            {
                return (Brush)GetValue(ShadowProperty);
            }
            set
            {
                SetValue(ShadowProperty, value);
            }
        }
        
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(ShadowedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        private string _fontFamily = "NanumBarunGothic Light";
        public string FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                if(_fontFamily != value)
                {
                    _fontFamily = value;
                    UpdateText();
                }
            }
        }

        private double _fontSize = 12;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if(_fontSize != value)
                {
                    _fontSize = value;
                    UpdateText();
                }
            }
        }

        
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ShadowedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnPropertyChanged(DependencyObject iobject, DependencyPropertyChangedEventArgs e)
        {
            ShadowedTextBlock t = (ShadowedTextBlock)iobject;
            t.UpdateText();
        }

        public ShadowedTextBlock()
        {
            canvas = new VisualCollection(this);
        }

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

        public void UpdateText()
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                string t = Text;
                if (t != null)
                {
                    Typeface tf = new Typeface(new FontFamily(_fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                    double fontsize = FontSize;

                    if (Shadow != null)
                    {
                        FormattedText ft = new FormattedText(t, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, fontsize, Shadow);

                        dc.DrawText(ft, new Point(_shadowX, _shadowY));
                    }


                    if (Foreground != null)
                    {
                        FormattedText ft = new FormattedText(t, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, fontsize, Foreground);

                        dc.DrawText(ft, new Point(0, 0));

                        double w = Math.Round(ft.Width);
                        double h = Math.Round(ft.Height);
                        RenderSize = new Size(w,h);
                        Width = w;
                        Height = h;
                    }
                }
            }

            canvas.Clear();
            canvas.Add(visual);
        }
    }
}
