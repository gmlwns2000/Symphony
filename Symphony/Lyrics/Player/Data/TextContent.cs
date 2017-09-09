using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Symphony.Lyrics
{
    public class TextContent : IContent
    {
        //TextField
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    RequireUpdate = true;
                }
            }
        }

        private TextAlignment _textAlignment;
        public TextAlignment TextAlignment
        {
            get
            {
                return _textAlignment;
            }
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    RequireUpdate = true;
                }
            }
        }

        //FontField
        private double _fontSize;
        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (_fontSize != value)
                    RequireUpdate = true;
                _fontSize = value;
            }
        }

        private FontFamily _fontFamily;
        private string _fontFamilyName;
        public string FontFamily
        {
            get
            {
                return _fontFamilyName;
            }
            set
            {
                if(_fontFamilyName != value)
                {
                    if (value == "Auto")
                    {
                        _fontFamily = DefaultFont;
                    }
                    else
                    {
                        _fontFamily = new FontFamily(value);
                    }
                    RequireUpdate = true;
                }

                _fontFamilyName = value;
            }
        }

        private static FontFamily _defaultFontFamily;
        private static FontFamily DefaultFont
        {
            get
            {
                if (_defaultFontFamily == null)
                {
                    _defaultFontFamily = new FontFamily(_defaultFontFamilyName);
                }

                return _defaultFontFamily;
            }
        }

        private static string _defaultFontFamilyName = "NanumBarunGothic";
        public static string DefaultFontFamily
        {
            get
            {
                return _defaultFontFamilyName;
            }
            set
            {
                if(_defaultFontFamilyName != value)
                {
                    _defaultFontFamily = new FontFamily(value);
                }

                _defaultFontFamilyName = value;
            }
        }

        private FontStyle _fontStyle;
        public FontStyle FontStyle
        {
            get
            {
                return _fontStyle;
            }
            set
            {
                if (value != _fontStyle)
                    RequireUpdate = true;
                _fontStyle = value;
            }
        }

        private FontWeight _fontWeight;
        public FontWeight FontWeight
        {
            get
            {
                return _fontWeight;
            }
            set
            {
                if (_fontWeight != value)
                    RequireUpdate = true;

                _fontWeight = value;
            }
        }

        //BrushField
        private Color _foreground;
        public Color Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                if (_foreground != value)
                    RequireUpdate = true;
                _foreground = value;
            }
        }

        public TextContent()
        {
            Text = "";
            TextAlignment = TextAlignment.Center;
            FontSize = 28;
            FontFamily = "Auto";
            FontStyle = FontStyles.Normal;
            FontWeight = FontWeights.Normal;
            Foreground = Colors.White;
        }

        public TextContent(string Text, TextAlignment TextAlignment, double FontSize, string FontFamily, FontStyle FontStyle, FontWeight FontWeight, Color Foreground)
        {
            this.Text = Text;
            this.TextAlignment = TextAlignment;
            this.FontSize = FontSize;
            this.FontFamily = FontFamily;
            this.FontStyle = FontStyle;
            this.FontWeight = FontWeight;
            this.Foreground = Foreground;
        }

        public override void Update()
        {
            if (RequireUpdate)
            {
                RequireUpdate = false;
                
                TextBlock tb = new TextBlock();

                tb.UseLayoutRounding = UseLayoutRounding;

                tb.Text = _text;
                tb.TextAlignment = _textAlignment;

                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                if (UseLayoutRounding)
                    tb.FontSize = Math.Round(_fontSize);
                else
                    tb.FontSize = _fontSize;
                tb.FontStyle = _fontStyle;
                tb.FontWeight = _fontWeight;
                tb.FontFamily = _fontFamily;

                SolidColorBrush brush = new SolidColorBrush(Foreground);
                brush.Freeze();
                tb.Foreground = brush;

                tb.CacheMode = new BitmapCache() { SnapsToDevicePixels = true, RenderAtScale = 2 };

                Content = tb;
            }
        }
    }
}
