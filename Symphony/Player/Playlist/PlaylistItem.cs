using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Symphony.Player
{
    public abstract class PlaylistItem
    {
        public static event EventHandler<PlaylistItem> ItemUpdated;

        private static int idCounter = -1;
        private static int GetID()
        {
            idCounter++;
            return idCounter;
        }

        public int UID = GetID();

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public virtual bool IsAvailable
        {
            get
            {
                if (FilePath == null)
                    return false;

                return File.Exists(FilePath);
            }
        }

        public Tags Tag { get; internal set; }

        public abstract Stream GetStream();

        internal void InvokeUpdated(object sender, PlaylistItem item)
        {
            ItemUpdated?.Invoke(sender, item);
        }
    }
}
