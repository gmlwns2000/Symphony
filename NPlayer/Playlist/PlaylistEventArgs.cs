using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPlayer
{
    public class PlaylistEventArgs : EventArgs
    {
        public List<nPlayerPlaylistItem> Items;
        public PlaylistEventArgs(List<nPlayerPlaylistItem> list)
        {
            Items = list;
        }
    }
}
