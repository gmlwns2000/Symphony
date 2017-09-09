using Symphony.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Symphony.Util;
using System.IO;

namespace Symphony.Lyrics
{
    public class Lyric : ResourcesParent
    {
        public string Version { get; set; }

        public string _workingDirecotry;
        public string WorkingDirectory
        {
            get
            {
                return _workingDirecotry;
            }
            set
            {
                _workingDirecotry = value;
                DirectoryInfo di = new DirectoryInfo(value);
                if (!di.Exists)
                {
                    di.Create();
                }
                ResourceDirectory = di.CreateSubdirectory(LyricHelper.ResoureceDirectoryName).FullName;
            }
        }

        public List<LyricLine> Lines { get; set; } = new List<LyricLine>();
        public MusicMetadata Metadata { get; set; } = new MusicMetadata();

        //canvas
        public double MinWidth { get; set; } = 500;
        public double MinHeight { get; set; } = 135;
        public Brush Background { get; set; }

        public Lyric(string Title, string Artist, string Album, string Author, string FileName)
        {
            Metadata = new MusicMetadata(Title, Artist, Album, Author, FileName);
        }

        public Lyric(MusicMetadata meta)
        {
            Metadata = meta;
        }

        public int IndexOf(LyricLine line)
        {
            int index = -1;

            for (int i = 0; i < Lines.Count; i++)
            {
                if(line.ID == Lines[i].ID)
                {
                    return i;
                }
            }

            return index;
        }
        
        public void RemoveAt(int index)
        {
            if (Lines[index].Content != null)
            {
                if (Lines[index].Content is IRemovable)
                {
                    ((IRemovable)Lines[index].Content).Remove();
                }

                if (Lines[index].Content is IDisposable)
                {
                    ((IDisposable)Lines[index].Content).Dispose();
                }

                ResourceGarbageCollection();

                Lines.RemoveAt(index);
            }
        }

        public void Remove(LyricLine line)
        {
            RemoveAt(IndexOf(line));
        }

        public void RemoveItems(List<LyricLine> lines)
        {
            foreach(LyricLine line in lines)
            {
                Remove(line);
            }
        }

        public void Solt()
        {
            bool going = false;
            int index = 0;
            while (true)
            {
                if(index < Lines.Count - 1)
                {
                    if(Lines[index].Position > Lines[index + 1].Position)
                    {
                        LyricLine temp = Lines[index + 1];
                        Lines[index + 1] = Lines[index];
                        Lines[index] = temp;
                        going = true;
                    }
                    index++;
                }
                else
                {
                    if (!going)
                    {
                        break;
                    }
                    going = false;
                    index = 0;
                }
            }
        }

        public int Add(LyricLine line)
        {
            if (Lines.Count > 0) {
                int index = -1;
                for (int i = 0; i < Lines.Count; i++)
                {
                    if (Lines[i].Position > line.Position)
                    {
                        index = i;
                        break;
                    }
                }
                if(index == -1)
                {
                    Lines.Add(line);
                    return Lines.Count - 1;
                }
                else
                {
                    Lines.Insert(index, line);
                    return index;
                }
            }
            else
            {
                Lines.Add(line);
                return Lines.Count - 1;
            }
        }
    }
}
