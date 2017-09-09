using Symphony.Lyrics;
using Symphony.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;

namespace Symphony.Util
{
    public static class XmlHelper
    {
        #region Writer

        public static void WriteString(XmlWriter writer, string ElementName, string Content)
        {
            writer.WriteStartElement(ElementName);

            writer.WriteString(Content);

            writer.WriteEndElement();
        }

        public static void WriteBoolAttribute(XmlWriter writer, string AttributeName, bool Value)
        {
            if (Value)
            {
                writer.WriteAttributeString(AttributeName, Bool2String(Value));
            }
            else
            {
                writer.WriteAttributeString(AttributeName, Bool2String(Value));
            }
        }

        public static void WriteValue(XmlWriter writer, string ElementName, string Value)
        {
            writer.WriteStartElement(ElementName);

            writer.WriteAttributeString("Value", Value);

            writer.WriteEndElement();
        }

        #endregion Writer

        #region Reader

        public static void ReadEndElement(XmlReader reader, string ElementName)
        {
            while (reader.Read())
            {
                if(reader.NodeType == XmlNodeType.EndElement && reader.Name == ElementName)
                {
                    return;
                }
            }
        }

        #endregion Reader

        #region Covert2String

        public static string Bool2String(bool b)
        {
            if (b)
                return "True";
            else
                return "False";
        }

        public static string Byte2String(byte bt)
        {
            return BitConverter.ToUInt16(new byte[2] { bt, 0x00 }, 0).ToString();
        }

        public static string KeySpline2String(AnimationKeySpline ks)
        {
            return string.Format("{0}, {1}, {2}, {3}", ks.ControlPoint1.X.ToString("0.000"), ks.ControlPoint1.Y.ToString("0.000"), ks.ControlPoint2.X.ToString("0.000"), ks.ControlPoint2.Y.ToString("0.000"));
        }

        public static string Color2String(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }

        public static string BarRenderType2String(BarRenderTypes type)
        {
            return Enum.GetName(typeof(BarRenderTypes), type);
        }

        public static string TextAlignment2String(TextAlignment ta)
        {
            return Enum.GetName(typeof(TextAlignment), ta);
        }

        public static string HorizontalAlignment2String(HorizontalAlignment ha)
        {
            return Enum.GetName(typeof(HorizontalAlignment), ha);
        }

        public static string VerticalAlignment2String(VerticalAlignment va)
        {
            return Enum.GetName(typeof(VerticalAlignment), va);
        }

        public static string FadeIn2String(LyricLine line)
        {
            return string.Format("{0}, {1}, ({2})", FadeInMode2String(line.FadeIn), line.FadeInLength.ToString("0.0000"), KeySpline2String(line.FadeInKeySpline));
        }

        public static string FadeOut2String(LyricLine line)
        {
            return string.Format("{0}, {1}, ({2})", FadeOutMode2String(line.FadeOut), line.FadeOutLength.ToString("0.0000"), KeySpline2String(line.FadeOutKeySpline));
        }

        public static string Sync2String(LyricLine line)
        {
            return string.Format("{0}, {1}", line.Duration.ToString("0"), line.Position.ToString("0"));
        }

        public static string FadeInMode2String(FadeInMode fm)
        {
            return Enum.GetName(typeof(FadeInMode), fm);
        }

        public static string FadeOutMode2String(FadeOutMode fm)
        {
            return Enum.GetName(typeof(FadeOutMode), fm);
        }

        public static string Stretch2String(Stretch st)
        {
            return Enum.GetName(typeof(Stretch), st);
        }

        public static string BitmapScalingMode2String(BitmapScalingMode sm)
        {
            return Enum.GetName(typeof(BitmapScalingMode), sm);
        }

        public static string ResamplingMode2String(Symphony.Player.ResamplingMode rm)
        {
            return Enum.GetName(typeof(Symphony.Player.ResamplingMode), rm);
        }

        public static string Thickness2String(Thickness th)
        {
            return string.Format("{0}, {1}, {2}, {3}", th.Left.ToString("0.00"), th.Top.ToString("0.00"), th.Right.ToString("0.00"), th.Bottom.ToString("0.00"));
        }

        public static string Point2String(Point pt)
        {
            return string.Format("{0}, {1}", pt.X.ToString("0.00"), pt.Y.ToString("0.00"));
        }

        public static string Size2String(Size sz)
        {
            return string.Format("{0}, {1}", sz.Width, sz.Height);
        }

        public static string Rotation2String(Rotation rt)
        {
            return string.Format("{0}, ({1}, {2})", rt.Angle, rt.X, rt.Y);
        }

        #endregion Convert2String

        #region String2Convert

        public static bool String2Bool(string Text)
        {
            if (Text.ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static byte String2Byte(string str)
        {
            return Convert.ToByte(Math.Min(255, Math.Max(0, Convert.ToDouble(str))));
        }

        public static AnimationKeySpline String2KeySpline(string text)
        {
            string[] spl = text.Split(',');

            Point pt1 = new Point(Convert.ToDouble(spl[0].Trim()), Convert.ToDouble(spl[1].Trim()));
            Point pt2 = new Point(Convert.ToDouble(spl[2].Trim()), Convert.ToDouble(spl[3].Trim()));

            return new AnimationKeySpline(pt1, pt2);
        }

        public static Color String2Color(string text)
        {
            return (Color)ColorConverter.ConvertFromString(text);
        }

        public static FontWeight String2FontWeight(string text)
        {
            return (FontWeight) new FontWeightConverter().ConvertFromString(text);
        }

        public static FontStyle String2FontStyle(string text)
        {
            return (FontStyle) new FontStyleConverter().ConvertFromString(text);
        }

        public static BarRenderTypes String2BarRenderType(string text)
        {
            return (BarRenderTypes)Enum.Parse(typeof(BarRenderTypes), text, true);
        }

        public static HorizontalAlignment String2HorizontalAlignment(string text)
        {
            return (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), text, true);
        }

        public static VerticalAlignment String2VerticalAlignment(string text)
        {
            return (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), text, true);
        }

        public static FadeInMode String2FadeInMode(string text)
        {
            return (FadeInMode)Enum.Parse(typeof(FadeInMode), text, true);
        }

        public static FadeOutMode String2FadeOutMode(string text)
        {
            return (FadeOutMode)Enum.Parse(typeof(FadeOutMode), text, true);
        }

        public static TextAlignment String2TextAlignment(string text)
        {
            return (TextAlignment)Enum.Parse(typeof(TextAlignment), text, true);
        }

        public static BitmapScalingMode String2ScalingMode(string text)
        {
            return (BitmapScalingMode)Enum.Parse(typeof(BitmapScalingMode), text, true);
        }

        public static Stretch String2Stretch(string text)
        {
            return (Stretch)Enum.Parse(typeof(Stretch), text, true);
        }

        public static Symphony.Player.ResamplingMode String2ResamplingMode(string text)
        {
            return (Symphony.Player.ResamplingMode)Enum.Parse(typeof(Symphony.Player.ResamplingMode), text, true);
        }

        public static Point String2Point(string text)
        {
            string[] spl = text.Split(',');
            Point pt = new Point();
            pt.X = Convert.ToDouble(spl[0].Trim());
            pt.Y = Convert.ToDouble(spl[1].Trim());
            return pt;
        }

        public static Size String2Size(string text)
        {
            string[] spl = text.Split(',');
            Size size = new Size();
            size.Width = Convert.ToDouble(spl[0].Trim());
            size.Height = Convert.ToDouble(spl[1].Trim());
            return size;
        }

        public static void String2Sync(ref LyricLine Line, string text)
        {
            string[] spl = text.Split(',');
            Line.Duration = Convert.ToDouble(spl[0].Trim());
            Line.Position = Convert.ToDouble(spl[1].Trim());
        }

        public static Thickness String2Thickness(string text)
        {
            string[] spl = text.Split(',');
            Thickness margin = new Thickness();
            margin.Left = Convert.ToDouble(spl[0].Trim());
            margin.Top = Convert.ToDouble(spl[1].Trim());
            margin.Right = Convert.ToDouble(spl[2].Trim());
            margin.Bottom = Convert.ToDouble(spl[3].Trim());
            return margin;
        }

        public static Rotation String2Rotation(string text)
        {
            string[] spl = text.Split(new char[1] { ',' }, 2);
            Rotation rt = new Rotation();
            rt.Angle = Convert.ToDouble(spl[0].Trim());
            Point pt = String2Point(spl[1].Trim(' ', '(', ')', ' '));
            rt.X = pt.X;
            rt.Y = pt.Y;
            return rt;
        }

        public static void String2FadeIn(ref LyricLine Line, string text)
        {
            string[] spl = text.Split(new char[1] { ',' }, 3);
            Line.FadeIn = String2FadeInMode(spl[0].Trim());
            Line.FadeInLength = Convert.ToDouble(spl[1].Trim());
            Line.FadeInKeySpline = String2KeySpline(spl[2].Trim(' ', '(', ')', ' '));
        }

        public static void String2FadeOut(ref LyricLine Line, string text)
        {
            string[] spl = text.Split(new char[1] { ',' }, 3);
            Line.FadeOut = String2FadeOutMode(spl[0].Trim());
            Line.FadeOutLength = Convert.ToDouble(spl[1].Trim());
            Line.FadeOutKeySpline = String2KeySpline(spl[2].Trim(' ', '(', ')', ' '));
        }

        #endregion String2Convert
    }
}
