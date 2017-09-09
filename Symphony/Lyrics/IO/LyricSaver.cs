using Symphony.Dancer;
using Symphony.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Symphony.Lyrics
{
    public static class LyricSaver
    {
        public static int Version = 3;

        public static void Save(string saveFile, bool overwrite, Lyric Lyric)
        {
            try
            {
                if (File.Exists(saveFile))
                {
                    if (overwrite)
                    {
                        File.Delete(saveFile);
                    }
                    else
                    {
                        return;
                    }
                }

                DirectoryInfo lyric_di = new DirectoryInfo(Lyric.WorkingDirectory);

                Logger.Log("LyricSaver", "Lyric WorkingDirectory: " + lyric_di.FullName);

                Lyric.ResourceGarbageCollection();

                XmlWriterSettings writerSetting = new XmlWriterSettings();
                writerSetting.Indent = true;
                writerSetting.IndentChars = "\t";

                using(XmlWriter writer = XmlWriter.Create(Path.Combine(lyric_di.FullName, "lyric.xml"), writerSetting))
                {
                    writer.WriteStartDocument();

                    writer.WriteStartElement("Lyric");

                    writer.WriteAttributeString("MinWidth", Lyric.MinWidth.ToString());
                    writer.WriteAttributeString("MinHeight", Lyric.MinHeight.ToString());

                    WriteString(writer, "Version", Version.ToString());
                    
                    WriteMetadata(writer, Lyric);

                    writer.WriteStartElement("Lines");
                    foreach (LyricLine line in Lyric.Lines)
                    {
                        WriteLine(writer, line);
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.WriteEndDocument();

                    writer.Flush();

                    writer.Close();
                }
                
                using (var zip = new Ionic.Zip.ZipFile(Encoding.UTF8))
                {
                    zip.Comment = string.Format("Symphony Lyric File Version {0}\nTitle: {1}\nArtist: {2}\nAlbum: {3}\nAuthor: {4}", 
                        Version, Lyric.Metadata.Title, Lyric.Metadata.Artist, Lyric.Metadata.Album, Lyric.Metadata.Author);
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.Level9;
                    zip.CompressionMethod = Ionic.Zip.CompressionMethod.Deflate;
                    zip.AlternateEncoding = Encoding.UTF8;

                    zip.AddDirectory(lyric_di.FullName, lyric_di.Name);

                    zip.Save(saveFile);
                }
            }
            catch (Exception e)
            {
                Logger.Error("LyricSaver", e);

                if (e is IOException)
                {
                    UI.DialogMessage.Show(null, LanguageHelper.FindText("Lang_Lyric_Loader_FileUsing"));
                }
            }
        }
        
        private static void WriteString(XmlWriter writer, string ElementName, string Content)
        {
            writer.WriteStartElement(ElementName);

            writer.WriteString(Content);

            writer.WriteEndElement();
        }

        private static void WriteMetadata(XmlWriter writer, Lyric Lyric)
        {
            writer.WriteStartElement("Metadata");

            WriteString(writer, "Title", Lyric.Metadata.Title);
            WriteString(writer, "Artist", Lyric.Metadata.Artist);
            WriteString(writer, "Album", Lyric.Metadata.Album);
            WriteString(writer, "Author", Lyric.Metadata.Author);
            WriteString(writer, "FileName", Lyric.Metadata.FileName);

            writer.WriteEndElement();
        }

        private static void WriteLine(XmlWriter writer, LyricLine Line)
        {
            writer.WriteStartElement("Line");

            writer.WriteAttributeString("Sync", XmlHelper.Sync2String(Line));
            writer.WriteAttributeString("FadeOut", XmlHelper.FadeOut2String(Line));
            writer.WriteAttributeString("FadeIn", XmlHelper.FadeIn2String(Line));

            writer.WriteAttributeString("Opacity", Line.Opacity.ToString("0.00"));
            writer.WriteAttributeString("Margin", XmlHelper.Thickness2String(Line.Margin));
            writer.WriteAttributeString("Offset", XmlHelper.Point2String(Line.BoxOffset));
            writer.WriteAttributeString("MinSize", XmlHelper.Size2String(Line.MinSize));
            writer.WriteAttributeString("HorizontalAlignment", XmlHelper.HorizontalAlignment2String(Line.HorizontalAlignment));
            writer.WriteAttributeString("VerticalAlignment", XmlHelper.VerticalAlignment2String(Line.VerticalAlignment));
            writer.WriteAttributeString("ContentRotation", XmlHelper.Rotation2String(Line.ContentRotation));

            writer.WriteAttributeString("Blur.Radius", Line.Blur.Radius.ToString("0.00"));
            writer.WriteAttributeString("Shadow.Radius", Line.Shadow.Radius.ToString("0.00"));
            writer.WriteAttributeString("Shadow.Opacity", Line.Shadow.Opacity.ToString("0.00"));
            writer.WriteAttributeString("Shadow.Depth", Line.Shadow.Depth.ToString("0.00"));
            writer.WriteAttributeString("Shadow.Direction", Line.Shadow.Direction.ToString("0.00"));
            writer.WriteAttributeString("Shadow.Color", XmlHelper.Color2String(Line.Shadow.Color));

            writer.WriteStartElement("Line.Content");
            if(Line.Content is TextContent)
            {
                WriteTextContent(writer, (TextContent)Line.Content);
            }
            else if (Line.Content is ImageContent)
            {
                WriteImageContent(writer, (ImageContent)Line.Content);
            }
            else
            {
                throw new NotImplementedException("Unknown Content Type");
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private static void WriteTextContent(XmlWriter writer, TextContent content)
        {
            writer.WriteStartElement("TextContent");
            
            writer.WriteAttributeString("Text", content.Text);
            writer.WriteAttributeString("TextAlignment", XmlHelper.TextAlignment2String(content.TextAlignment));

            writer.WriteAttributeString("FontFamily", content.FontFamily);
            writer.WriteAttributeString("FontSize", content.FontSize.ToString("0.00"));
            writer.WriteAttributeString("FontWeight", new FontWeightConverter().ConvertToString(content.FontWeight));
            writer.WriteAttributeString("FontStyle", new FontStyleConverter().ConvertToString(content.FontStyle));

            writer.WriteAttributeString("Foreground", XmlHelper.Color2String(content.Foreground));

            writer.WriteEndElement();
        }

        private static void WriteImageContent(XmlWriter writer, ImageContent content)
        {
            writer.WriteStartElement("ImageContent");

            writer.WriteAttributeString("Resource", content.Resource.FileName);
            writer.WriteAttributeString("Width", content.Width.ToString("0.00"));
            writer.WriteAttributeString("Height", content.Height.ToString("0.00"));
            writer.WriteAttributeString("ScalingMode", XmlHelper.BitmapScalingMode2String(content.ScalingMode));
            writer.WriteAttributeString("Stretch", XmlHelper.Stretch2String(content.Stretch));

            writer.WriteEndElement();
        }
    }
}
