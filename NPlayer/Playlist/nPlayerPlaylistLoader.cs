using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NPlayer
{
    public class nPlayerPlaylistLoader
    {
        public string FilePath;
        public string CurrentName;
        public int CurrentIndex;
        public int PlaylistItemCount;

        static object playlistReaderLocker = new object();

        List<nPlayerPlaylistItem>[] itemLoaded;

        public nPlayerPlaylistLoader()
        {

        }

        public nPlayerPlaylistLoader(string FileName)
        {
            this.FilePath = FileName;
        }

        public nPlayerPlaylist LoadPlaylist()
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

        public nPlayerPlaylist LoadPlaylist(string path)
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
            }

            PlaylistItemCount = list.Count;
            CurrentIndex = 0;

            nPlayerPlaylist pl = new nPlayerPlaylist(Path.GetFileNameWithoutExtension(path));

            Logger.Log("Load AutoSave Playlist: " + path.ToString());

            int cores = Environment.ProcessorCount;

            //Multi Thread
            if (list.Count > cores * 2)
            {
                //init
                List<Thread> loaders = new List<Thread>();
                itemLoaded = new List<nPlayerPlaylistItem>[cores];

                //start
                for (int i = 0; i < cores; i++)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(PlaylistReader));
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

                    pl.Add(new nPlayerPlaylistItem(list[ii]));
                }
            }

            list.Clear();
            list = null;

            return pl;
        }

        private void PlaylistReader(object arg)
        {
            PlaylistLoaderArgs args = (PlaylistLoaderArgs)arg;
            int id = args.id;
            int cores = args.cores;
            List<string> list = args.list;

            int part = (int)Math.Ceiling((double)list.Count / cores);

            List<nPlayerPlaylistItem> items = new List<nPlayerPlaylistItem>();

            for (int i = part * id; i < Math.Min(list.Count, part * (id + 1)); i++)
            {
                CurrentName = list[i];
                CurrentIndex++;

                items.Add(new nPlayerPlaylistItem(list[i]));
            }

            itemLoaded[id] = items;
        }
    }
}
