using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;
using System.Windows.Media;

namespace Symphony.UI
{
    public class LvItemPlaylist
    {
        public LvItemPlaylist(Playlist playlist)
        {
            Title = playlist.Title;

            Count = playlist.Items.Count.ToString();

            if (playlist.Duration == null)
            {
                Duration = "-:-:-";
            }
            else
            {
                if (playlist.Duration.Days > 0)
                {
                    Duration = playlist.Duration.ToString(@"dd\:hh\:mm\:ss");
                }
                else if(playlist.Duration.Hours > 0)
                {
                    Duration = playlist.Duration.ToString(@"hh\:mm\:ss");
                }
                else
                {
                    Duration = playlist.Duration.ToString(@"mm\:ss");
                }
            }
        }
        public ImageBrush PlayingCover { get; set; }
        public string Count { get; private set; }
        public string Title { get; private set; }
        public string Duration { get; private set; }
    }
}
