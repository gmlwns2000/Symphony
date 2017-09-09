using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Player
{
    public static class Pathes
    {
        public static string AlbumArtCacheFolder = "AlbumArtCache";

        public static string YoutubeVideoFolder
        {
            get
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YoutubeCache");

                DirectoryInfo di = new DirectoryInfo(path);

                if (!di.Exists)
                    di.Create();

                return path;
            }
        }
    }
}
