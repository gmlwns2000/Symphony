using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Symphony.Player
{
    public class PlaylistLoader
    {
        public string FilePath;
        public string CurrentName;
        public int CurrentIndex;
        public int PlaylistItemCount;

        static object playlistReaderLocker = new object();

        List<PlaylistItem>[] itemLoaded;

        public PlaylistLoader()
        {

        }

        public PlaylistLoader(string FileName)
        {
            this.FilePath = FileName;
        }

        public Playlist LoadPlaylist()
        {
            if(FilePath == null)
            {
                return null;
            }
            else
            {
                return LoadPlaylist(FilePath);
            }
        }

        public Playlist LoadPlaylist(string path)
        {
            FilePath = path;

            string[] lines = File.ReadAllLines(path);

            List<string> list = new List<string>();
            for (int ii = 0; ii < lines.Length; ii++)
            {
                string line = lines[ii];

                if (!line.StartsWith("#"))
                {
                    if (File.Exists(line))
                    {
                        list.Add(line);
                    }
                }
                else if (line.ToLower().StartsWith("#sym"))
                {
                    list.Add(line);
                }
            }

            PlaylistItemCount = list.Count;
            CurrentIndex = 0;

            Playlist pl = new Playlist(Path.GetFileNameWithoutExtension(path));

            Logger.Log("Load AutoSave Playlist: " + path.ToString());

            int cores = Environment.ProcessorCount;

            //Multi Thread
            if (list.Count > cores * 2)
            {
                //init
                List<Thread> loaders = new List<Thread>();
                itemLoaded = new List<PlaylistItem>[cores];

                //start
                for (int i = 0; i < cores; i++)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(PlaylistReaderProc));
                    th.IsBackground = true;
                    th.Start(new PlaylistLoaderArgs(list, i, cores));
                    loaders.Add(th);
                }

                //join
                for (int i = 0; i < loaders.Count; i++)
                {
                    loaders[i].Join();
                }

                //merge
                for (int i = 0; i < itemLoaded.Length; i++)
                {
                    pl.AddRange(itemLoaded[i]);
                }
            }
            //Single Thread
            else if (list.Count > 0)
            {
                for (int ii = 0; ii < list.Count; ii++)
                {
                    CurrentName = list[ii];
                    CurrentIndex = ii;

                    PlaylistItem item = GetPlaylistItem(list[ii]);

                    pl.Add(item);
                }
            }

            list.Clear();
            list = null;

            return pl;
        }

        private void PlaylistReaderProc(object arg)
        {
            PlaylistLoaderArgs args = (PlaylistLoaderArgs)arg;
            int id = args.id;
            int cores = args.cores;
            List<string> list = args.list;

            int part = (int)Math.Ceiling((double)list.Count / cores);

            List<PlaylistItem> items = new List<PlaylistItem>();

            for (int i = part * id; i < Math.Min(list.Count, part * (id + 1)); i++)
            {
                CurrentName = list[i];

                CurrentIndex++;

                PlaylistItem item = GetPlaylistItem(list[i]);
                
                if(item != null)
                {
                    items.Add(item);
                }
            }

            itemLoaded[id] = items;
        }

        public static PlaylistItem GetPlaylistItem(string raw_line)
        {
            string line = raw_line.ToLower();

            if (line.StartsWith("#"))
            {
                if (line.StartsWith("#sym"))
                {
                    string[] spl = raw_line.Split(new char[] { ' ' }, 3);

                    if (spl.Length >= 3)
                    {
                        string type = spl[1].ToLower();

                        if (type == "youtube")
                        {
                            return new YoutubeItem(spl[2]);
                        }
                    }
                }
            }
            else
            {
                return new FileItem(raw_line);
            }

            return null;
        }
    }
}
