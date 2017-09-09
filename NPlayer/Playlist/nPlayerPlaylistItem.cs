using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NPlayer
{
    public class nPlayerPlaylistItem
    {
        private static int idCounter = -1;
        private static int GetID()
        {
            idCounter++;

            return idCounter;
        }

        public int id = GetID();
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public nPlayerTags Tag { get; private set; }

        public nPlayerPlaylistItem(string filepath)
        {
            FilePath = filepath;
            FileName = Path.GetFileName(filepath);
            Tag = new nPlayerTags(filepath);
        }
    }
}
