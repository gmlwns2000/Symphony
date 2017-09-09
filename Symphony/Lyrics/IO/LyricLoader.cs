using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Symphony.Util;

namespace Symphony.Lyrics
{
    public static class LyricLoader
    {
        public static bool Load(out Lyric Lyric, string FilePath)
        {
            if (File.Exists(FilePath))
            {
                using(XmlReader reader = XmlReader.Create(FilePath))
                {
                    reader.ReadStartElement("Lyric");

                    reader.ReadStartElement("Version");

                    string Version = reader.ReadContentAsString();
                    Logger.Log(string.Format("Lyric Version: {0}", Version));
                    
                    ReadEndElement(reader);

                    reader.Close();

                    using (XmlReader lyricReader = XmlReader.Create(FilePath))
                    {
                        lyricReader.ReadToFollowing("Lyric");

                        try
                        {
                            switch (Version)
                            {
                                case "1":
                                    LyricLoaderV1 v1 = new LyricLoaderV1();
                                    Lyric = v1.Do(lyricReader);
                                    break;
                                case "2":
                                    LyricLoaderV2 v2 = new LyricLoaderV2();
                                    Lyric = v2.Do(lyricReader);
                                    break;
                                case "3":
                                    LyricLoaderV3 v3 = new LyricLoaderV3();
                                    Lyric = v3.Do(lyricReader);
                                    break;
                                default:
                                    Lyric = null;
                                    return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("LyricLoader", ex);

                            Logger.Show(string.Format(LanguageHelper.FindText("Lang_Lyric_Loader_ErrorFormat"), Version, ((IXmlLineInfo)lyricReader).LineNumber, ((IXmlLineInfo)lyricReader).LinePosition, reader.Name, ex.ToString()));

                            Lyric = null;

                            return false;
                        }

                        lyricReader.Close();
                    }

                    Lyric.Version = Version;
                    Lyric.WorkingDirectory = Path.GetDirectoryName(FilePath);

                    Logger.Log("Lyric Working Directory " + Lyric.WorkingDirectory);
                }
                return true;
            }
            else
            {
                Lyric = null;
                return false;
            }
        }

        private static void ReadEndElement(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                reader.ReadEndElement();
            }
        }
    }
}
