using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Symphony.UI
{
    public class LvItemPlaylistItem : IDisposable
    {
        private static int idCounter = -1;
        private static int GetID()
        {
            idCounter++;

            return idCounter;
        }

        public int UID = GetID();
        public int ParentID;

        private TagImage AlbumArtCache;

        public Visibility AlbumArtCover { get; set; }
        public Brush AlbumArt { get; set; }
        public string Album { get; set; }
        public string Album_Artist { get; set; }
        public string Artist { get; set; }
        public string Codec { get; set; }
        public string Bitrate { get; set; }
        public string FileName { get; set; }
        public string SampleRate { get; set; }
        public string Title { get; set; }
        public string Track { get; set; }

        public LvItemPlaylistItem(PlaylistItem item)
        {
            Update(item);
        }

        public void Update(PlaylistItem item)
        {
            ParentID = item.UID;

            AlbumArtCover = Visibility.Hidden;

            if (item.Tag != null)
            {
                if (!String.IsNullOrWhiteSpace(item.Tag.Title))
                {
                    Title = item.Tag.Title;
                }
                else
                {
                    Title = item.FileName;
                }

                if (!String.IsNullOrWhiteSpace(item.Tag.Album))
                {
                    Album = item.Tag.Album;
                }
                else
                {
                    Album = "알 수 없는 앨범";
                }

                if (!String.IsNullOrWhiteSpace(item.Tag.Artist))
                {
                    Artist = item.Tag.Artist;
                }
                else
                {
                    Artist = "알 수 없는 아티스트";
                }

                if (!String.IsNullOrWhiteSpace(item.Tag.Album) && !String.IsNullOrWhiteSpace(item.Tag.Artist))
                {
                    Album_Artist = item.Tag.Album + " - " + item.Tag.Artist;
                }
                else
                {
                    Album_Artist = "알 수 없습니다.";
                }

                if (!String.IsNullOrWhiteSpace(item.Tag.Track))
                {
                    Track = item.Tag.Track;
                }
                else
                {
                    Track = "-";
                }

                if (!String.IsNullOrWhiteSpace(item.Tag.Bitrate.ToString()))
                {
                    Bitrate = item.Tag.Bitrate.ToString() + "kbps";
                }
                else
                {
                    Bitrate = "-";
                }

                if (!String.IsNullOrWhiteSpace(item.Tag.Frequency.ToString()))
                {
                    SampleRate = item.Tag.Frequency.ToString("0,0") + "hz";
                }
                else
                {
                    SampleRate = "-";
                }

                if (item.Tag.Pictures != null)
                {
                    if (item.Tag.Pictures.Count > 0)
                    {
                        if (item.Tag.Pictures[0] != null)
                        {
                            AlbumArtCache = item.Tag.Pictures[0];
                        }
                        else
                        {
                            AlbumArtCache = null;
                        }
                    }
                    else
                    {
                        AlbumArtCache = null;
                    }
                }

                if (!String.IsNullOrWhiteSpace(item.Tag.Codec))
                {
                    Codec = item.Tag.Codec;
                }
                else
                {
                    Codec = "-";
                }
            }
            else
            {
                Title = item.FileName;
            }

            if (!String.IsNullOrWhiteSpace(item.FileName))
            {
                FileName = item.FileName;
            }
            else
            {
                FileName = "-";
            }
        }

        public bool DecodeAlbumArt(double dpi)
        {
            try
            {
                if (AlbumArtCache != null && AlbumArtCache.FilePath != null)
                {
                    BitmapImage test = new BitmapImage(new Uri(AlbumArtCache.FilePath));

                    BitmapImage bit = new BitmapImage();
                    bit.BeginInit();
                    bit.CreateOptions = BitmapCreateOptions.None;
                    bit.CacheOption = BitmapCacheOption.None;
                    bit.DecodePixelHeight = (int)(41 * dpi);
                    bit.UriSource = new Uri(AlbumArtCache.FilePath);
                    bit.EndInit();

                    AlbumArt = new ImageBrush(bit);
                    AlbumArt.SetValue(ImageBrush.StretchProperty, Stretch.UniformToFill);
                    AlbumArt.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.Linear);
                    AlbumArt.Freeze();

                    bit.Freeze();
                    return true;
                }
                else
                {
                    AlbumArt = null;
                    return false;
                }
            }
            catch
            {
                Logger.Error(this, "ERRORED ON DECODING IMAGE OF " + FileName);

                AlbumArt = null;
                return false;
            }
        }

        public void Dispose()
        {
            AlbumArt = null;
            AlbumArtCache = null;
        }
    }
}
