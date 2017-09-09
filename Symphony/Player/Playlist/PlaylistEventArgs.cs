using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symphony.Player
{
    public class PlaylistEventArgs : EventArgs
    {
        public List<PlaylistItem> Items;
        public PlaylistEventArgs(List<PlaylistItem> list)
        {
            Items = list;
        }
    }
}
