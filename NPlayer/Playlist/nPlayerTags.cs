using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace NPlayer
{
    public class nPlayerTags : IDisposable
    { 
        //STATIC FIELDS
        private static nPlayerLog log = new nPlayerLog("Tag");
        private static string[] _albumArtFolders = new string[] 
        {
            "cover.jpg",
            "cover.png",
            "cover.bmp",
            "albumart.png",
            "albumart.jpg",
            "Scans/",
            "Scan/",
            "BK/",
            "../BK/",
            "../Scans/",
            "../Scan/"
        };
        public static string[] AlbumArtFolders
        {
            get
            {
                return _albumArtFolders;
            }
            set
            {
                _albumArtFolders = value;
            }
        }

        private static readonly string[] _albumArtAvailableFormats = new string[] 
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".gif",
            ".bmp",
            ".tiff"
        };
        /// <summary>
        /// Support Image Format.
        /// <para/>.jpg; .jpeg; .png; .gif; .bmp; .tiff;
        /// <para/>
        /// </summary>
        public static string[] AlbumArtAvailableFormats
        {
            get
            {
                return _albumArtAvailableFormats;
            }
        }

        private static bool _useLocalAlbumArts = true;
        public static bool UseLocalAlbumArts
        {
            get
            {
                return _useLocalAlbumArts;
            }
            set
            {
                _useLocalAlbumArts = value;
            }
        }

        //CLASS FIELDS
        public string FilePath { get; set; }
        public string Artist { get; private set; }
        public string Album { get; private set; }
        public string Title { get; private set; }
        public string BPM { get; private set; }
        public string Composers { get; private set; }
        public string Genre { get; private set; }
        public string Lyrics { get; private set; }
        public string Track { get; private set; }
        public string Year { get; private set; }
        public string Codec { get; private set; }
        public int Bitrate { get; private set; }
        public TimeSpan Duration { get; private set; }
        public int Frequency { get; private set; }
        public List<nPlayerTagImage> Pictures { get; private set; }

        public nPlayerTags()
        {

        }

        public nPlayerTags(string path)
        {
            try
            {
                SetTarget(path);
            }
            catch (Exception e)
            {
                log.derr(e.ToString());
            }
        }

        #region Init

        public void SetTarget(string path)
        {
            FilePath = path;
            Codec = GetCodecMessage(System.IO.Path.GetExtension(path).ToUpper());
            GetId3Tag(this);
            FixTag(this);
        }

        public void Refresh()
        {
            if(FilePath != null)
            {
                Codec = GetCodecMessage(Path.GetExtension(FilePath));
                GetId3Tag(this);
                FixTag(this);
            }
        }

        public static string GetCodecMessage(string extention)
        {
            extention = extention.ToLower();
            switch (extention)
            {
                case ".flac":
                    return "Free Lossless Audio Codec";
                case ".ogg":
                    return "OGG";
                case ".aiff":
                    return "Audio Interchange File Format";
                case ".aif":
                    return "Audio Interchange File Format";
                case ".aifc":
                    return "Audio Interchange File Format";
                case ".wav":
                    return "Waveform audio format";
                case ".ac3":
                    return "AC3";
                case ".aac":
                    return "Advanced Audio Coding";
                case ".m4a":
                    return "MPEG-Layered 4";
                case ".mp3":
                    return "MPEG-Layered 3";
                default:
                    return extention.ToUpper().Trim('.');
            }
        }

        private static void GetId3Tag(nPlayerTags tag)
        {
            try
            {
                using (TagLib.File tagfile = TagLib.File.Create(tag.FilePath))
                {
                    if (tagfile.Tag != null)
                    {
                        if (tagfile.Tag.Title != null)
                        {
                            tag.Title = tagfile.Tag.Title;
                        }

                        if (tagfile.Tag.Performers != null)
                        {
                            tag.Artist = "";

                            for (int i = 0; i < tagfile.Tag.Performers.Length; i++)
                            {
                                tag.Artist += FixString(tagfile.Tag.Performers[i]);
                                tag.Artist += ", ";
                            }
                        }

                        if (tagfile.Tag.Composers != null)
                        {
                            if (string.IsNullOrEmpty(tag.Artist))
                            {
                                tag.Artist = "";
                            }

                            for (int i = 0; i < tagfile.Tag.Composers.Length; i++)
                            {
                                tag.Artist += FixString(tagfile.Tag.Composers[i]);
                                tag.Artist += ", ";
                            }
                        }

                        if (tag.Artist != null)
                        {
                            tag.Artist = tag.Artist.Trim();
                            tag.Artist = tag.Artist.Trim(',');
                        }

                        if (tagfile.Tag.Album != null)
                        {
                            tag.Album = tagfile.Tag.Album;
                        }

                        if (tagfile.Tag.BeatsPerMinute > 0)
                        {
                            tag.BPM = tagfile.Tag.BeatsPerMinute.ToString();
                        }

                        if (tagfile.Tag.JoinedComposers != null)
                        {
                            tag.Composers = tagfile.Tag.JoinedComposers;
                        }

                        if (tagfile.Tag.JoinedGenres != null)
                        {
                            tag.Genre = tagfile.Tag.JoinedGenres;
                        }

                        if (tagfile.Tag.Lyrics != null)
                        {
                            tag.Lyrics = tagfile.Tag.Lyrics;
                        }

                        tag.Track = tagfile.Tag.Track.ToString();
                        if (!StringEmpty(tag.Track))
                        {
                            if (tagfile.Tag.TrackCount >= 1)
                            {
                                tag.Track += "/" + tagfile.Tag.TrackCount.ToString();
                            }
                        }

                        if (tagfile.Tag.Year > 0)
                        {
                            tag.Year = tagfile.Tag.Year.ToString();
                        }

                        if (tagfile.Properties != null)
                        {
                            if (tagfile.Properties.AudioBitrate > 0)
                            {
                                tag.Bitrate = tagfile.Properties.AudioBitrate;
                            }

                            if (tagfile.Properties.Duration != null)
                            {
                                tag.Duration = tagfile.Properties.Duration;
                            }

                            if (tagfile.Properties.AudioSampleRate > 0)
                            {
                                tag.Frequency = tagfile.Properties.AudioSampleRate;
                            }
                        }

                        if (tag.Pictures != null)
                        {
                            tag.Pictures.Clear();
                            tag.Pictures = null;
                        }

                        if (tagfile.Tag.Pictures != null)
                        {
                            if (tag.Pictures == null)
                            {
                                tag.Pictures = new List<nPlayerTagImage>();
                            }

                            for (int i = 0; i < tagfile.Tag.Pictures.Length; i++)
                            {
                                tag.Pictures.Add(new nPlayerTagImage(tagfile.Tag.Pictures[i].Data.Data));
                            }
                        }

                        string musicFolder = Path.GetDirectoryName(tag.FilePath);

                        for (int i = 0; i < AlbumArtFolders.Length && UseLocalAlbumArts; i++)
                        {
                            if (tag.Pictures == null)
                            {
                                tag.Pictures = new List<nPlayerTagImage>();
                            }

                            string current = AlbumArtFolders[i];

                            try
                            {
                                if (current.EndsWith("/") || current.EndsWith("\\"))
                                {
                                    string folderPath = Path.Combine(musicFolder, current);

                                    if (Directory.Exists(folderPath))
                                    {
                                        DirectoryInfo di = new DirectoryInfo(folderPath);
                                        FileInfo[] fiList = di.GetFiles();
                                        foreach (FileInfo fi in fiList)
                                        {
                                            if (IsImage(fi.FullName))
                                            {
                                                tag.Pictures.Add(new nPlayerTagImage(fi.FullName));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    string path = Path.Combine(musicFolder, current);

                                    if (IsImage(path) && File.Exists(path))
                                    {
                                        tag.Pictures.Add(new nPlayerTagImage(path));
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.derr(ex.ToString());
            }
        }

        #endregion |nit

        #region AlbumArt

        public bool SetAlbumArt(int index, string path)
        {
            if (!System.IO.File.Exists(path) || !IsImage(path))
            {
                return false;
            }

            using(TagLib.File tagfile = TagLib.File.Create(FilePath))
            {
                TagLib.Picture pic = new TagLib.Picture(path);
                TagLib.Id3v2.AttachedPictureFrame picFrame = new TagLib.Id3v2.AttachedPictureFrame(pic);

                if (tagfile.Tag.Pictures == null || (tagfile.Tag.Pictures != null && tagfile.Tag.Pictures.Length == 0) )
                {
                    //새로 만들기
                    TagLib.IPicture[] pictures = new TagLib.IPicture[1];

                    pictures[0] = picFrame;

                    tagfile.Tag.Pictures = pictures;
                }
                else if (tagfile.Tag.Pictures != null && tagfile.Tag.Pictures.Length > 0 && tagfile.Tag.Pictures.Length - 1 < index)
                {
                    //마지막에 추가
                    TagLib.IPicture[] pictures = tagfile.Tag.Pictures;

                    Array.Resize(ref pictures, pictures.Length + 1);

                    pictures[pictures.Length - 1] = picFrame;

                    tagfile.Tag.Pictures = pictures;
                }
                else
                {
                    //정상 위치에 삽입
                    TagLib.IPicture[] pictures = tagfile.Tag.Pictures;
                    
                    pictures[index] = picFrame;

                    tagfile.Tag.Pictures = pictures;
                }

                log.dlog("AlbumArt is Edited Start Saving");
                tagfile.Save();
                log.dlog("AlbumArt is Edited End Saving");
            }

            log.dlog("AlbumArt is Edited Update ID3 Tag");
            GetId3Tag(this);

            return true;
        }

        public void ClearAlbumArt()
        {
            using(TagLib.File tagfile = TagLib.File.Create(FilePath))
            {
                tagfile.Tag.Pictures = new TagLib.IPicture[0];

                tagfile.Save();
            }

            GetId3Tag(this);
        }

        #endregion AlbumArt

        #region Tool

        public static bool IsImage(string path)
        {
            foreach (string s in AlbumArtAvailableFormats)
            {
                if (path.ToLower().EndsWith(s))
                {
                    return true;
                }
            }
            return false;
        }

        public static void FixTag(nPlayerTags tag)
        {
            tag.Artist = FixString(tag.Artist);
            tag.Album = FixString(tag.Album);
            tag.Title = FixString(tag.Title);
            tag.BPM = FixString(tag.BPM);
            tag.Composers = FixString(tag.Composers);
            tag.Genre = FixString(tag.Genre);
            tag.Lyrics = FixString(tag.Lyrics);
            tag.Track = FixString(tag.Track);
            tag.Year = FixString(tag.Year);
        }

        private static bool StringEmpty(string str)
        {
            if (str == null)
            {
                return true;
            }
            else
            {
                if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static string FixString(string inp)
        {
            if(inp == null)
            {
                return "";
            }
            else
            {
                if(string.IsNullOrEmpty(inp) || string.IsNullOrWhiteSpace(inp))
                {
                    return "";
                }
                else
                {
                    inp = Regex.Replace(inp, "[\x00-\x08\x0B\x0C\x0E-\x1F]", string.Empty, RegexOptions.Compiled);

                    return inp.Trim();
                }
            }
        }

        public static void NullTag(nPlayerTags tag)
        {
            tag.Artist = "";
            tag.Album = "";
            tag.Title = "";
            tag.BPM = "";
            tag.Composers = "";
            tag.Genre = "";
            tag.Lyrics = "";
            tag.Track = "";
            tag.Year = "";
            tag.Codec = "";
            tag.Bitrate = 0;
            tag.Duration = new TimeSpan();
            tag.Frequency = 0;
            tag.Pictures = null;
        }

        #endregion Tool

        public void Dispose()
        {
            if(Pictures != null)
            {
                for(int i =0; i<Pictures.Count; i++)
                {
                    Pictures[i].Dispose();
                }

                Pictures.Clear();

                Pictures = null;
            }
        }
    }
}