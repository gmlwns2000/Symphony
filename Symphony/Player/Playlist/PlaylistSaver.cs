using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Player
{
    public class PlaylistSaver
    {
        public static void Save(string path, Playlist pl)
        {
            string filepath = path;

            if (!Path.IsPathRooted(path))
            {
                return;
            }

            string content = "";
            bool init = true;
            Logger.Log("start save playlist: " + filepath);

            foreach(PlaylistItem item in pl.Items)
            {
                string line = item.FilePath;
                if (!(item is FileItem))
                {
                    if (item is YoutubeItem)
                    {
                        YoutubeItem youtubeItem = (YoutubeItem)item;

                        line = "#sym youtube " + youtubeItem.Uri;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                if (init)
                {
                    init = false;
                    content = line;
                }
                else
                {
                    content += "\n" + line;
                }
            }

            File.WriteAllText(filepath, content);

            Logger.Log("end save playlist: " + filepath);
        }
    }
}
