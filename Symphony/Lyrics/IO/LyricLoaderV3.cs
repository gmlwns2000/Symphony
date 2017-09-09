using Symphony.Server;
using Symphony.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.Lyrics
{
    public class LyricLoaderV3 : LyricLoaderV2, ILyricLoader
    {
        /*
         <Lyric MinWidth="500" MinHeight="135">
	        <Version>3</Version>
	        <Metadata>
		        <Title>daze</Title>
		        <Artist>じん feat.メイリア from GARNiDELiA</Artist>
		        <Album>daze/days</Album>
		        <Author>AinL</Author>
		        <FileName>01. daze.flac</FileName>
	        </Metadata>
	        <Lines>
		        <Line   Sync="248055, 0" FadeOut="FadeOut, 0.0089, (0.819, 0.000, 1.000, 0.000)" FadeIn="FadeIn, 0.0268, (0.000, 0.800, 0.000, 1.000)" Opacity="1.00" Margin="10.00, 10.00, 10.00, 10.00" 
                        Offset="0.00, 0.00" MinSize="0, 0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" ContentRotation="0, (0.5, 0.5)" 
                        Blur.Radius="11.07" Shadow.Radius="12.95" Shadow.Opacity="0.83" Shadow.Depth="0.00" Shadow.Direction="0.00" Shadow.Color="#FF9E4FFF">
			        <Line.Content>
				        <ImageContent Resource="49c47af3.jpg" Width="500.00" Height="150.00" ScalingMode="NearestNeighbor" Stretch="UniformToFill" />
			        </Line.Content>
		        </Line>
		        <Line   Sync="1600, 6000" FadeOut="ZoomIn, 0.1000, (0.000, 0.000, 1.000, 0.000)" FadeIn="ZoomIn, 0.0700, (0.000, 0.000, 0.000, 1.000)" Opacity="1.00" Margin="0.00, 0.00, 0.00, 0.00" 
                        Offset="0.00, 0.00" MinSize="0, 0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" ContentRotation="0, (0.5, 0.5)" 
                        Blur.Radius="0.00" Shadow.Radius="5.00" Shadow.Opacity="1.00" Shadow.Depth="0.00" Shadow.Direction="0.00" Shadow.Color="#FF000000">
			        <Line.Content>
				        <TextContent Text="「daze」" TextAlignment="Center" FontFamily="Auto" FontSize="28.00" FontWeigth="Normal" FontStyle="Normal" Foreground="#FFFF487B" />
			        </Line.Content>
		        </Line>
            </Lines>
        </Lyric>
        */
        public new Lyric Do(XmlReader reader)
        {
            Lyric Lyric = new Lyric(new MusicMetadata());

            Lyric.MinHeight = Convert.ToDouble(reader["MinHeight"]);
            Lyric.MinWidth = Convert.ToDouble(reader["MinWidth"]);

            ReadMetadata(reader, ref Lyric);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Line")
                {
                    Lyric.Lines.Add(ReadLine(reader, ref Lyric));
                }
            }

            return Lyric;
        }

        public LyricLine ReadLine(XmlReader reader, ref Lyric Lyric)
        {
            LyricLine Line = new LyricLine();

            string strSync = reader["Sync"];
            XmlHelper.String2Sync(ref Line, strSync);

            string strFadeOut = reader["FadeOut"];
            XmlHelper.String2FadeOut(ref Line, strFadeOut);

            string strFadeIn = reader["FadeIn"];
            XmlHelper.String2FadeIn(ref Line, strFadeIn);

            string strOpacity = reader["Opacity"];
            Line.Opacity = Convert.ToDouble(strOpacity);

            string strMargin = reader["Margin"];
            Line.Margin = XmlHelper.String2Thickness(strMargin);

            string strOffset = reader["Offset"];
            Line.BoxOffset = XmlHelper.String2Point(strOffset);

            string strMinSize = reader["MinSize"];
            Line.MinSize = XmlHelper.String2Size(strMinSize);

            string strHorizontalAlignment = reader["HorizontalAlignment"];
            Line.HorizontalAlignment = XmlHelper.String2HorizontalAlignment(strHorizontalAlignment);

            string strVerticalAlignment = reader["VerticalAlignment"];
            Line.VerticalAlignment = XmlHelper.String2VerticalAlignment(strVerticalAlignment);

            string strContentRotation = reader["ContentRotation"];
            Line.ContentRotation = XmlHelper.String2Rotation(strContentRotation);

            LyricBlur Blur = new LyricBlur();

            string strBlurRadius = reader["Blur.Radius"];
            Blur.Radius = Convert.ToDouble(strBlurRadius);

            Line.Blur = Blur;

            LyricDropShadow Shadow = new LyricDropShadow();

            string strShadowRadius = reader["Shadow.Radius"];
            Shadow.Radius = Convert.ToDouble(strShadowRadius);

            string strShadowOpacity = reader["Shadow.Opacity"];
            Shadow.Opacity = Convert.ToDouble(strShadowOpacity);

            string strShadowDepth = reader["Shadow.Depth"];
            Shadow.Depth = Convert.ToDouble(strShadowDepth);

            string strShadowDirection = reader["Shadow.Direction"];
            Shadow.Direction = Convert.ToDouble(strShadowDirection);

            string strShadowColor = reader["Shadow.Color"];
            Shadow.Color = XmlHelper.String2Color(strShadowColor);

            Line.Shadow = Shadow;

            bool endElement = false;
            while(reader.Read() && !endElement)
            {
                if(reader.NodeType == XmlNodeType.EndElement)
                {
                    if(reader.Name == "Line")
                    {
                        endElement = true;
                        break;
                    }
                }
                else if(reader.NodeType == XmlNodeType.Element)
                {
                    if(reader.Name == "Line.Content")
                    {
                        bool contentRead = false;
                        while (reader.Read() && !contentRead)
                        {
                            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Line.Content")
                            {
                                contentRead = true;
                                break;
                            }
                            else if(reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "TextContent":
                                        Line.Content = ReadTextContent(reader);
                                        break;
                                    case "ImageContent":
                                        Line.Content = ReadImageContent(reader, ref Lyric);
                                        break;
                                    default:
                                        throw new NotImplementedException("Unknown Content");
                                }
                            }
                        }
                    }
                }
            }

            return Line;
        }

        //<TextContent Text="「daze」" TextAlignment="Center" FontFamily="Auto" FontSize="28.00" FontWeigth="Normal" FontStyle="Normal" Foreground="#FFFF487B" />
        public TextContent ReadTextContent(XmlReader reader)
        {
            TextContent content = new TextContent();

            content.Text = reader["Text"];

            string strTextAlignment = reader["TextAlignment"];
            content.TextAlignment = XmlHelper.String2TextAlignment(strTextAlignment);

            content.FontFamily = reader["FontFamily"];

            string strFontSize = reader["FontSize"];
            content.FontSize = Convert.ToDouble(strFontSize);

            string strFontWeight = reader["FontWeight"];
            content.FontWeight = XmlHelper.String2FontWeight(strFontWeight);

            string strFontStyle = reader["FontStyle"];
            content.FontStyle = XmlHelper.String2FontStyle(strFontStyle);

            string strForeground = reader["Foreground"];
            content.Foreground = XmlHelper.String2Color(strForeground);

            return content;
        }

        //<ImageContent Resource="49c47af3.jpg" Width="500.00" Height="150.00" ScalingMode="NearestNeighbor" Stretch="UniformToFill" />
        public ImageContent ReadImageContent(XmlReader reader, ref Lyric Lyric)
        {
            ImageContent content = new ImageContent(Lyric);

            string strResource = reader["Resource"];
            content.Resource.Open(strResource);

            string strWidth = reader["Width"];
            content.Width = Convert.ToDouble(strWidth);

            string strHeight = reader["Height"];
            content.Height = Convert.ToDouble(strHeight);

            string strScalingMode = reader["ScalingMode"];
            content.ScalingMode = XmlHelper.String2ScalingMode(strScalingMode);

            string strStretch = reader["Stretch"];
            content.Stretch = XmlHelper.String2Stretch(strStretch);

            return content;
        }
    }
}
