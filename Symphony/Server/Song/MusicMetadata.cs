using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.Server
{
    public class MusicMetadata
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Album { get; set; }
        public string Artist { get; set; }
        public string FileName { get; set; }

        public MusicMetadata()
        {

        }

        public MusicMetadata(string Title, string Artist, string Album, string Author, string FileName)
        {
            this.Title = Title;
            this.Artist = Artist;
            this.Album = Album;
            this.Author = Author;
            this.FileName = FileName;
        }

        public MusicMetadata(Symphony.Player.PlaylistItem item)
        {
            Title = item.Tag.Title;
            Artist = item.Tag.Artist;
            Album = item.Tag.Album;
            Author = "";
            FileName = item.FileName;
        }

        public MusicMetadata(Lyrics.Lyric Lyric)
        {
            this.Title = Lyric.Metadata.Title;
            this.Artist = Lyric.Metadata.Artist;
            this.Album = Lyric.Metadata.Album;
            this.Author = Lyric.Metadata.Author;
            this.FileName = Lyric.Metadata.FileName;
        }

        public MusicMetadata(Dancer.Plot Plot)
        {
            this.Title = Plot.Metadata.Title;
            this.Artist = Plot.Metadata.Artist;
            this.Album = Plot.Metadata.Album;
            this.Author = Plot.Metadata.Author;
            this.FileName = Plot.Metadata.FileName;
        }

        public MusicMetadata(string xmlFile)
        {
            if(Path.GetFileName(xmlFile) == "plot.xml")
            {
                ReadPlot(xmlFile);
            }
            else if (Path.GetFileName(xmlFile) == "lyric.xml")
            {
                ReadLyric(xmlFile);
            }
            else if(Path.GetFileName(xmlFile) == "pl.xml")
            {
                ReadPlotLite(xmlFile);
            }
            else
            {
                Debug.WriteLine("ERROR Unknown xmlFile Type. MusicMetadata.41");
            }
        }

        private void ReadPlotLite(string plotFile)
        {
            if (File.Exists(plotFile))
            {
                using (XmlReader reader = XmlReader.Create(plotFile))
                {
                    reader.ReadToFollowing("Metadata");
                    while (true)
                    {
                        reader.Read();
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "Title":
                                    Title = reader.ReadElementContentAsString();
                                    break;
                                case "Artist":
                                    Artist = reader.ReadElementContentAsString();
                                    break;
                                case "Album":
                                    Album = reader.ReadElementContentAsString();
                                    break;
                                case "Author":
                                    Author = reader.ReadElementContentAsString();
                                    break;
                                case "FileName":
                                    FileName = reader.ReadElementContentAsString();
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement)
                        {
                            if (reader.Name == "Metadata")
                            {
                                break;
                            }
                        }
                    }

                    reader.Close();
                }
            }
        }

        private void ReadPlot(string plotFile)
        {
            FileInfo fi = new FileInfo(plotFile);

            if (fi.Exists)
            {
                using (XmlReader reader = XmlReader.Create(plotFile))
                {
                    reader.ReadStartElement("Plot");

                    reader.ReadToFollowing("Version");

                    string Version = reader.ReadElementContentAsString();

                    switch (Version)
                    {
                        case "1":
                            reader.ReadToFollowing("Metadata");
                            while (true)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    switch (reader.Name)
                                    {
                                        case "Title":
                                            Title = reader.ReadElementContentAsString();
                                            break;
                                        case "Artist":
                                            Artist = reader.ReadElementContentAsString();
                                            break;
                                        case "Album":
                                            Album = reader.ReadElementContentAsString();
                                            break;
                                        case "Author":
                                            Author = reader.ReadElementContentAsString();
                                            break;
                                        case "FileName":
                                            FileName = reader.ReadElementContentAsString();
                                            break;
                                    }
                                }
                                else if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (reader.Name == "Metadata")
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void ReadLyric(string lyricFile)
        {
            FileInfo fi = new FileInfo(lyricFile);

            if (fi.Exists)
            {
                using (XmlReader reader = XmlReader.Create(lyricFile))
                {
                    reader.ReadStartElement("Lyric");

                    reader.ReadToFollowing("Version");

                    string Version = reader.ReadElementContentAsString();

                    if(Version == "1" || Version == "2" || Version == "3")
                    {
                        reader.ReadToFollowing("Metadata");
                        while (true)
                        {
                            reader.Read();
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                switch (reader.Name)
                                {
                                    case "Title":
                                        Title = reader.ReadElementContentAsString();
                                        break;
                                    case "Artist":
                                        Artist = reader.ReadElementContentAsString();
                                        break;
                                    case "Album":
                                        Album = reader.ReadElementContentAsString();
                                        break;
                                    case "Author":
                                        Author = reader.ReadElementContentAsString();
                                        break;
                                    case "FileName":
                                        FileName = reader.ReadElementContentAsString();
                                        break;
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (reader.Name == "Metadata")
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Logger.Error("Music Metadata", string.Format("Unknown Lyric Version {0} : {1}", Version, lyricFile));
                    }
                }
            }
        }

        public override string ToString()
        {
            string ret = "";

            if (!Util.TextTool.StringEmpty(Title))
            {
                ret += "Title: " + Title;
            }

            if (!Util.TextTool.StringEmpty(Album))
            {
                ret += " Album: " + Album;
            }

            if (!Util.TextTool.StringEmpty(Artist))
            {
                ret += " Artist: " + Artist;
            }

            if (!Util.TextTool.StringEmpty(FileName))
            {
                ret += " FileName: " + FileName;
            }

            if (!Util.TextTool.StringEmpty(Author))
            {
                ret += " Author: " + Author;
            }

            return ret.TrimStart();
        }
    }
}
