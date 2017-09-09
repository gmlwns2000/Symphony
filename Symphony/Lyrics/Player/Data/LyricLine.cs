using Symphony.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Symphony.Lyrics
{
    public class LyricLine
    {
        //editor support
        public static int InstanceCount = 0;

        public readonly int ID;
        public string EditorText
        {
            get
            {
                if(Content is TextContent)
                {
                    return ((TextContent)Content).Text;
                }
                else if(Content is ImageContent)
                {
                    return LanguageHelper.FindText("Lang_Lyric_Content_ImageContent");
                }
                else
                {
                    return LanguageHelper.FindText("Lang_Lyric_Content_UnknownContent");
                }
            }
        }
        public string EditorComment
        {
            get
            {
                return string.Format("{0} | {1} | {2}", EditorPosition, EditorDuration, EditorType);
            }
        }

        public string EditorType
        {
            get
            {
                if(Content is TextContent)
                {
                    return LanguageHelper.FindText("Lang_Lyric_Content_TextContent");
                }
                else if(Content is ImageContent)
                {
                    return LanguageHelper.FindText("Lang_Lyric_Content_ImageContent");
                }
                else
                {
                    return LanguageHelper.FindText("Lang_Lyric_Content_UnknownContent");
                }
            }
        }

        public string EditorDuration
        {
            get
            {
                if(Duration < 0)
                {
                    return LanguageHelper.FindText("Lang_Auto");
                }
                else if(Duration < 60000)
                {
                    return TimeSpan.FromMilliseconds(Duration).ToString(@"ss\:fff");
                }
                else
                {
                    return TimeSpan.FromMilliseconds(Duration).ToString(@"mm\:ss\:fff");
                }
            }
        }

        public string EditorPosition
        {
            get
            {
                if (Duration < 60000)
                {
                    return TimeSpan.FromMilliseconds(Position).ToString(@"ss\:fff");
                }
                else
                {
                    return TimeSpan.FromMilliseconds(Position).ToString(@"mm\:ss\:fff");
                }
            }
        }

        //content
        public IContent Content { get; set; }

        //Layout > box
        public Rotation ContentRotation { get; set; }
        public Point BoxOffset { get; set; }
        public Size MinSize { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public Thickness Margin { get; set; }

        //Animation > box
        public double Position { get; set; }
        public FadeInMode FadeIn { get; set; }
        public FadeOutMode FadeOut { get; set; }
        public double Duration { get; set; }
        public double FadeInLength { get; set; }
        public double FadeOutLength { get; set; }
        public AnimationKeySpline FadeInKeySpline { get; set; }
        public AnimationKeySpline FadeOutKeySpline { get; set; }
        public bool View { get; set; }

        //Effect > content
        public LyricBlur Blur { get; set; }
        public LyricDropShadow Shadow { get; set; }
        public double Opacity { get; set; }

        public LyricLine(IContent Content, 
            Rotation ContentRotation, Point BoxOffset, Size MinSize, HorizontalAlignment HorizontalAlignment, VerticalAlignment VerticalAlignment, Thickness Margin,
            double Position, double Duration, FadeInMode FadeIn, double FadeInLength, AnimationKeySpline FadeInKeySpline, FadeOutMode FadeOut, double FadeOutLength, AnimationKeySpline FadeOutKeySpline, 
            LyricBlur Blur, LyricDropShadow Shadow, double Opacity)
        {
            ID = InstanceCount;
            InstanceCount++;

            this.Content = Content;

            this.ContentRotation = ContentRotation;
            this.BoxOffset = BoxOffset;
            this.MinSize = MinSize;
            this.HorizontalAlignment = HorizontalAlignment;
            this.VerticalAlignment = VerticalAlignment;
            this.Margin = Margin;

            this.Position = Position;
            this.Duration = Duration;
            this.FadeIn = FadeIn;
            this.FadeInLength = FadeInLength;
            this.FadeInKeySpline = FadeInKeySpline;
            this.FadeOut = FadeOut;
            this.FadeOutLength = FadeOutLength;
            this.FadeOutKeySpline = FadeOutKeySpline;

            this.Blur = Blur;
            this.Shadow = Shadow;
            this.Opacity = Opacity;

            View = false;
        }

        public LyricLine()
        {
            ID = InstanceCount;
            InstanceCount++;

            Content = new TextContent();
            
            ContentRotation = new Rotation(0, 0.5, 0.5);
            MinSize = new Size(0, 0);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            
            Blur = new LyricBlur(0);
            Shadow = new LyricDropShadow(new Color(), 0, 0, 0, 0);

            FadeInKeySpline = new AnimationKeySpline(0,0,0,1);
            FadeOutKeySpline = new AnimationKeySpline(0,0,1,0);
        }
    }

    public struct LyricBlur
    {
        public double Radius;

        public LyricBlur(double Radius)
        {
            this.Radius = Radius;
        }
    }

    public struct LyricDropShadow
    {
        public Color Color;
        public double Radius;
        public double Depth;
        public double Opacity;
        public double Direction;

        public LyricDropShadow(Color Color, double Radius, double Depth, double Direction, double Opacity)
        {
            this.Color = Color;
            this.Radius = Radius;
            this.Depth = Depth;
            this.Direction = Direction;
            this.Opacity = Opacity;
        }
    }
}
