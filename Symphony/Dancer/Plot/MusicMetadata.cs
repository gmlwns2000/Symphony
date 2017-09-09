using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.Dancer
{
    public class MusicMetadata
    {
        public string Title = "";
        public string Author = "";
        public string Album = "";
        public string Artist = "";
        public string FileName = "";
        public bool inited = false;

        public MusicMetadata(string Title, string Artist, string Album, string Author, string FileName)
        {
            this.Title = Title;
            this.Artist = Artist;
            this.Album = Album;
            this.Author = Author;
            this.FileName = FileName;
        }

        public MusicMetadata(string plotFile)
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

                    inited = true;
                }
            }
        }
    }
}
