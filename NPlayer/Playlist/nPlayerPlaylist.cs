using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPlayer
{
    public class nPlayerPlaylist
    {
        private int index;
        public int Index {
            get
            {
                return index;
            }
            set
            {
                index = value;
                if (PlaylistUpdated != null)
                {
                    PlaylistUpdated(this, new PlaylistEventArgs(Items));
                }
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set {
                title = value;
                if (PlaylistUpdated != null)
                {
                    PlaylistUpdated(this, new PlaylistEventArgs(Items));
                }
            }
        }

        public TimeSpan Duration { get; private set; }

        public event EventHandler<PlaylistEventArgs> PlaylistUpdated;

        public List<nPlayerPlaylistItem> Items = new List<nPlayerPlaylistItem>();

        private PlaylistOrder order;
        public PlaylistOrder Order
        {
            get { return order; }
            set
            {
                order = value;
                if ((PlaylistUpdated != null))
                {
                    PlaylistUpdated(this, new PlaylistEventArgs(Items));
                }
            }
        }

        #region Init
        public nPlayerPlaylist()
        {
            Order = PlaylistOrder.Once;
            Title = "Untitled";
            Index = -1;
        }

        public nPlayerPlaylist(string Name)
        {
            Order = PlaylistOrder.Once;
            Index = -1;
            Title = Name;
        }

        public nPlayerPlaylist(string Name, PlaylistOrder order)
        {
            Order = order;
            Index = -1;
            Title = Name;
        }
        #endregion Init
        
        public nPlayerPlaylistItem GetCurrent()
        {
            if (index > -1 && index < Items.Count) 
            {
                return Items[index];
            }
            else
            {
                return null;
            }
        }

        public void Remove(int i)
        {
            if (Items.Count != 0)
            {
                if (i <= Index)
                {
                    Index--;
                }
                Duration.Subtract(Items[i].Tag.Duration);
                Items.RemoveAt(i);
                if (PlaylistUpdated!=null) { PlaylistUpdated(this, new PlaylistEventArgs(Items)); }
            }
        }

        public void Replace(int index, nPlayerPlaylistItem item)
        {
            Items[index] = item;
            PlaylistUpdated(this,new PlaylistEventArgs(Items));
        }

        public void Add(string filepath)
        {
            Add(new nPlayerPlaylistItem(filepath));
        }

        public void Add(nPlayerPlaylistItem plitem)
        {
            if (Items == null)
            {
                Items = new List<nPlayerPlaylistItem>();
                Items.Add(plitem);
            }
            else
            {
                Items.Add(plitem);
            }
            Duration = TimeSpan.FromSeconds(plitem.Tag.Duration.TotalSeconds + Duration.TotalSeconds);
            PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
        }

        public void AddRange(List<nPlayerPlaylistItem> collection)
        {
            if(Items == null)
            {
                Items = new List<nPlayerPlaylistItem>(collection.Count);
            }

            for(int i=0; i<collection.Count; i++)
            {
                Items.Add(collection[i]);
                Duration = TimeSpan.FromSeconds(collection[i].Tag.Duration.TotalSeconds + Duration.TotalSeconds);
                PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
            }
        }

        public void SetTitle(string title)
        {
            Title = title;
            PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
        }

        Queue<int> qIndex = new Queue<int>();
        int backIndex = -1;
        
        public void GoNext()
        {
            if (Items != null)
            {
                if (Order == PlaylistOrder.Once)
                {
                    if ((Items.Count > 0) && (Index < Items.Count - 1))
                    {
                        if (Index == -1)
                        {
                            Index = 0;
                        }
                        else
                        {
                            Index++;
                        }
                    }
                    else if ((Index >= Items.Count - 1))
                    {
                        Index = -1;
                    }
                }
                else if (Order == PlaylistOrder.Repeat)
                {
                    if ((Items.Count > 0) && (Index < Items.Count - 1))
                    {
                        if (Index == -1)
                        {
                            Index = 0;
                        }
                        else
                        {
                            Index++;
                        }
                    }
                    else if ((Index >= Items.Count - 1))
                    {
                        Index = 0;
                    }
                }
                else if(Order == PlaylistOrder.Random)
                {
                    Random rd = new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds));
                    Index = rd.Next(0,Items.Count-1);
                    qIndex.Enqueue(Index);
                    backIndex = qIndex.Count - 1;
                }
                else if(Order == PlaylistOrder.RepeatOne)
                {
                    Index = Index;  
                }
                if (PlaylistUpdated != null) { PlaylistUpdated(this, new PlaylistEventArgs(Items)); }
            }
        }

        public void GoPrevious()
        {
            if (Items != null)
            {
                if (Order == PlaylistOrder.Once)
                {
                    if ((Items.Count > 0) && (Index > 0))
                    {
                        if (Index == -1)
                        {
                            Index = Items.Count - 1;
                        }
                        else
                        {
                            Index--;
                        }
                    }
                    else if ((Index == 0))
                    {
                        Index = -1;
                    }
                }else if(Order == PlaylistOrder.Repeat)
                {
                    if ((Items.Count > 0) && (Index > 0))
                    {
                        if (Index == -1)
                        {
                            Index = Items.Count - 1;
                        }
                        else
                        {
                            Index--;
                        }
                    }
                    else if ((Index == 0))
                    {
                        Index = Items.Count - 1;
                    }
                }
                else if(Order == PlaylistOrder.Random)
                {
                    if (qIndex.Count > 0 && backIndex > 0)
                    {
                        backIndex--;
                        Index = qIndex.ToArray()[backIndex];
                    }
                    else
                    {
                        Random rd = new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds));
                        Index = rd.Next(0, Items.Count - 1);
                        qIndex.Enqueue(Index);
                        backIndex = qIndex.Count - 1;
                    }
                }
                if (PlaylistUpdated != null) { PlaylistUpdated(this, new PlaylistEventArgs(Items)); }
            }
        }

        public void Insert(int index, nPlayerPlaylistItem item)
        {
            if (item == null)
                return;

            Items.Insert(index, item);

            if (item.Tag != null && item.Tag.Duration != null)
                Duration.Add(item.Tag.Duration);

            if(index <= Index)
            {
                Index++;
            }
        }

        public void UpdateTotalTime()
        {
            TimeSpan ts = new TimeSpan();

            foreach( nPlayerPlaylistItem item in Items)
            {
                if(item.Tag.Duration != null)
                    ts.Add(item.Tag.Duration);
            }

            Duration = ts;
        }
    }
}
