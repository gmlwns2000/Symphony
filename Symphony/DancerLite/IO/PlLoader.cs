using Symphony.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.DancerLite
{
    public static class PlLoader
    {
        public static bool Load(string xmlFile, PlotLite pl)
        {
            using(XmlReader reader = XmlReader.Create(xmlFile))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if(reader.Name == "Metadata")
                            {
                                bool read = true;
                                MusicMetadata Metadata = new MusicMetadata("", "", "", "", "");
                                while(read && reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        if (reader.Name == "Artist")
                                        {
                                            Metadata.Artist = reader.ReadElementContentAsString();
                                        }
                                        else if (reader.Name == "Album")
                                        {
                                            Metadata.Album = reader.ReadElementContentAsString();
                                        }
                                        else if (reader.Name == "Title")
                                        {
                                            Metadata.Title = reader.ReadElementContentAsString();
                                        }
                                        else if (reader.Name == "FileName")
                                        {
                                            Metadata.FileName = reader.ReadElementContentAsString();
                                        }
                                        else if (reader.Name == "Author")
                                        {
                                            Metadata.Author = reader.ReadElementContentAsString();
                                        }
                                    }
                                    else if(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Metadata")
                                    {
                                        pl.Metadata = Metadata;
                                        read = false;
                                        break;
                                    }
                                }
                            }
                            else if(reader.Name == "Version")
                            {
                                pl.Version = reader.ReadElementContentAsString();
                            }
                            else if(reader.Name == "Data")
                            {
                                if(pl.Version == "1")
                                {
                                    PlLoaderV1 v1 = new PlLoaderV1();
                                    bool r = v1.Load(reader, pl);
                                    reader.Close();
                                    return r;
                                }
                                else
                                {
                                    Logger.Log("Unknown PL version");

                                    reader.Close();
                                    return false;
                                }
                            }
                            break;
                    }
                }

                reader.Close();
            }

            return false;
        }
    }
}
