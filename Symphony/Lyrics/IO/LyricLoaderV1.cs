using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Symphony.Lyrics
{
    /* SAMPLE VERSION 1
    <Lyric>
        <Version>1</Version>
        <Metadata>
            <Title/>
            <Artist/>
            <Album/>
            <Author>알 수 없음</Author>
            <FileName>supercell feat. Gazelle - Melt 3M MIX.mp3</FileName>
        </Metadata>
        <Lines>
            <Count>1</Count>
            <Line   Shadow.Color.R="0" 
                    Shadow.Color.G="0" 
                    Shadow.Color.B="0" 
                    Shadow.Color.A="255" 
                    Shadow.Direction="0.00" 
                    Shadow.Depth="0.00" 
                    Shadow.Opacity="1.00" 
                    Shadow.Radius="5.00" 
                    Blur.Radius="0.00" 
                    Position="1521.00" 
                    Duration="-1.00" 
                    R="255" 
                    G="255" 
                    B="255" 
                    A="255" 
                    TextAlignment="Center" 
                    Text="가사를 입력 해주세요 1" 
                    TextSize="28.00" 
                    Opacity="1.00" 
                    FadeInLength="0.10" 
                    FadeIn="FadeIn" 
                    FadeOutLength="0.10" 
                    FadeOut="FadeOut"/>
        </Lines>
    </Lyric>
    */
    public class LyricLoaderV1 : ILyricLoader
    {
        public Lyric Do(XmlReader reader)
        {
            Lyric Lyric = new Lyric(new Server.MusicMetadata());

            ReadMetadata(reader, ref Lyric);

            reader.ReadToFollowing("Lines");

            reader.ReadToFollowing("Count");

            int count = Convert.ToInt32(reader.ReadElementContentAsString());

            for (int i = 0; i < count; i++) 
            {
                Lyric.Add(ReadLine(reader));
            }

            return Lyric;
        }

        public void ReadMetadata(XmlReader reader, ref Lyric Lyric)
        {
            reader.ReadToFollowing("Metadata");
            bool run = true;
            while (run)
            {
                reader.Read();
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if(reader.Name == "Title")
                        {
                            Lyric.Metadata.Title = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name == "Artist")
                        {
                            Lyric.Metadata.Artist = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name == "Album")
                        {
                            Lyric.Metadata.Album = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name == "Author")
                        {
                            Lyric.Metadata.Author = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name == "FileName")
                        {
                            Lyric.Metadata.FileName = reader.ReadElementContentAsString();
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if(reader.Name == "Metadata")
                        {
                            run = false;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        [Obsolete]
        public LyricLine ReadLine(XmlReader reader)
        {
            LyricLine line = new LyricLine();
            TextContent content = new TextContent();

            reader.ReadToFollowing("Line");

            byte SR = String2Byte(reader.GetAttribute("Shadow.Color.R"));
            byte SG = String2Byte(reader.GetAttribute("Shadow.Color.G"));
            byte SB = String2Byte(reader.GetAttribute("Shadow.Color.B"));
            byte SA = String2Byte(reader.GetAttribute("Shadow.Color.A"));
            double SDirection = Convert.ToDouble(reader.GetAttribute("Shadow.Direction"));
            double SDepth = Convert.ToDouble(reader.GetAttribute("Shadow.Depth"));
            double SOpacity = Convert.ToDouble(reader.GetAttribute("Shadow.Opacity"));
            double SRadius = Convert.ToDouble(reader.GetAttribute("Shadow.Radius"));
            LyricDropShadow shadow = new LyricDropShadow();
            shadow.Color = Color.FromArgb(SA, SR, SG, SB);
            shadow.Direction = SDirection;
            shadow.Depth = SDepth;
            shadow.Opacity = SOpacity;
            shadow.Radius = SRadius;
            line.Shadow = shadow;
            
            double BRadius = Convert.ToDouble(reader.GetAttribute("Blur.Radius"));
            line.Blur = new LyricBlur(BRadius);

            double Position = Convert.ToDouble(reader.GetAttribute("Position"));
            line.Position = Position;
            double Duration = Convert.ToDouble(reader.GetAttribute("Duration"));
            line.Duration = Duration;

            byte R = String2Byte(reader.GetAttribute("R"));
            byte G = String2Byte(reader.GetAttribute("G"));
            byte B = String2Byte(reader.GetAttribute("B"));
            byte A = String2Byte(reader.GetAttribute("A"));
            Color color = Color.FromArgb(A, R, G, B);
            content.Foreground = color;

            string strTextAlignment = reader.GetAttribute("TextAlignment");
            TextAlignment TextAlignment = TextAlignment.Center;
            switch (strTextAlignment)
            {
                case "Left":
                    TextAlignment = TextAlignment.Left;
                    break;
                case "Right":
                    TextAlignment = TextAlignment.Right;
                    break;
                case "Center":
                    TextAlignment = TextAlignment.Center;
                    break;
                case "Justify":
                    TextAlignment = TextAlignment.Justify;
                    break;
                default:
                    break;
            }
            content.TextAlignment = TextAlignment;

            string Text = reader.GetAttribute("Text");
            content.Text = Text;
            double TextSize = Convert.ToDouble(reader.GetAttribute("TextSize"));
            content.FontSize = TextSize;

            double Opacity = Convert.ToDouble(reader.GetAttribute("Opacity"));
            line.Opacity = Opacity;

            double FadeInLength = Convert.ToDouble(reader.GetAttribute("FadeInLength"));
            line.FadeInLength = FadeInLength;
            string strFadeIn = reader.GetAttribute("FadeIn");
            double FadeOutLength = Convert.ToDouble(reader.GetAttribute("FadeOutLength"));
            line.FadeOutLength = FadeOutLength;
            string strFadeOut = reader.GetAttribute("FadeOut");

            FadeInMode FadeIn = FadeInMode.Auto;
            switch (strFadeIn)
            {
                case "Auto":
                    FadeIn = FadeInMode.Auto;
                    break;
                case "None":
                    FadeIn = FadeInMode.None;
                    break;
                case "FadeIn":
                    FadeIn = FadeInMode.FadeIn;
                    break;
                case "BlurIn":
                    FadeIn = FadeInMode.BlurIn;
                    break;
                case "ZoomIn":
                    FadeIn = FadeInMode.ZoomIn;
                    break;
                case "ZoomOut":
                    FadeIn = FadeInMode.ZoomOut;
                    break;
                case "SlideFromLeft":
                    FadeIn = FadeInMode.SlideFromLeft;
                    break;
                case "SlideFromRight":
                    FadeIn = FadeInMode.SlideFromRight;
                    break;
                case "SlideFromTop":
                    FadeIn = FadeInMode.SlideFromTop;
                    break;
                case "SlideFromBottom":
                    FadeIn = FadeInMode.SlideFromBottom;
                    break;
                default:
                    break;
            }
            line.FadeIn = FadeIn;

            FadeOutMode FadeOut = FadeOutMode.Auto;
            switch (strFadeOut)
            {
                case "Auto":
                    FadeOut = FadeOutMode.Auto;
                    break;
                case "None":
                    FadeOut = FadeOutMode.None;
                    break;
                case "FadeOut":
                    FadeOut = FadeOutMode.FadeOut;
                    break;
                case "BlurOut":
                    FadeOut = FadeOutMode.BlurOut;
                    break;
                case "ZoomIn":
                    FadeOut = FadeOutMode.ZoomIn;
                    break;
                case "ZoomOut":
                    FadeOut = FadeOutMode.ZoomOut;
                    break;
                case "SlideToLeft":
                    FadeOut = FadeOutMode.SlideToLeft;
                    break;
                case "SlideToRight":
                    FadeOut = FadeOutMode.SlideToRight;
                    break;
                case "SlideToTop":
                    FadeOut = FadeOutMode.SlideToTop;
                    break;
                case "SlideToBottom":
                    FadeOut = FadeOutMode.SlideToBottom;
                    break;
                default:
                    break;
            }
            line.FadeOut = FadeOut;

            line.FadeInKeySpline = new AnimationKeySpline(0,0,0,1);

            line.FadeOutKeySpline = new AnimationKeySpline(0,0,1,0);
            
            line.Content = content;

            return line;
        }

        [Obsolete]
        public byte String2Byte(string str)
        {
            return Convert.ToByte(Math.Min(255, Math.Max(0, Convert.ToDouble(str))));
        }
    }
}
