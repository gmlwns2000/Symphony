using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symphony.Player
{
    public class Playlist
    {
        private int _index;
        public int Index {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set {
                title = value;
                PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
            }
        }

        public TimeSpan Duration { get; private set; }

        public event EventHandler<PlaylistEventArgs> PlaylistUpdated;

        public List<PlaylistItem> Items = new List<PlaylistItem>();

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

        public Playlist()
        {
            Order = PlaylistOrder.Once;
            Index = -1;
            Title = "Untitled";

            EndInit();
        }

        public Playlist(string Name)
        {
            Order = PlaylistOrder.Once;
            Index = -1;
            Title = Name;

            EndInit();
        }

        public Playlist(string Name, PlaylistOrder order)
        {
            Order = order;
            Index = -1;
            Title = Name;

            EndInit();
        }

        private void EndInit()
        {
            PlaylistItem.ItemUpdated += PlaylistItem_ItemUpdated;
        }

        #endregion Init

        #region Collection Control

        public void Insert(int index, PlaylistItem item)
        {
            lock (Items)
            {
                if (item == null)
                    return;

                Items.Insert(index, item);

                if (item.Tag != null && item.Tag.Duration != null)
                    Duration.Add(item.Tag.Duration);

                if (index <= Index)
                {
                    Index++;
                }
            }
        }

        public void Remove(int i)
        {
            lock (Items)
            {
                if (Items.Count != 0)
                {
                    if (i <= Index)
                    {
                        Index--;
                    }

                    if (Items[i].Tag != null)
                    {
                        Duration.Subtract(Items[i].Tag.Duration);
                    }

                    Items.RemoveAt(i);
                    PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
                }
            }
        }

        public void Replace(int index, PlaylistItem item)
        {
            Items[index] = item;
            PlaylistUpdated(this,new PlaylistEventArgs(Items));
        }

        public void Add(string filepath)
        {
            Add(new FileItem(filepath));
        }

        public void Add(PlaylistItem item)
        {
            lock (Items)
            {
                if (Items == null)
                {
                    Items = new List<PlaylistItem>();
                    Items.Add(item);
                }
                else
                {
                    Items.Add(item);
                }

                if (item.IsAvailable && item.Tag != null)
                {
                    Duration = TimeSpan.FromSeconds(item.Tag.Duration.TotalSeconds + Duration.TotalSeconds);
                }

                PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
            }
        }

        public void AddRange(List<PlaylistItem> collection)
        {
            lock (Items)
            {
                if (Items == null)
                {
                    Items = new List<PlaylistItem>(collection.Count);
                }

                foreach (PlaylistItem item in collection)
                {
                    Items.Add(item);

                    if (item.IsAvailable && item.Tag != null)
                    {
                        Duration = TimeSpan.FromSeconds(item.Tag.Duration.TotalSeconds + Duration.TotalSeconds);
                    }

                    PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
                }
            }
        }

        public bool HasItem (PlaylistItem item)
        {
            lock (Items)
            {
                foreach (PlaylistItem i in Items)
                {
                    if (i.UID == item.UID)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public int IndexOf(PlaylistItem item)
        {
            lock (Items)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].UID == item.UID)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        #endregion Collection Control

        #region Playlist Control

        public void UpdateTotalTime()
        {
            lock (Items)
            {
                TimeSpan ts = new TimeSpan();

                foreach (PlaylistItem item in Items)
                {
                    if (item.Tag != null)
                        ts.Add(item.Tag.Duration);
                }

                Duration = ts;
            }
        }

        public void SetTitle(string title)
        {
            Title = title;
            PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
        }

        public PlaylistItem GetCurrent()
        {
            if (_index > -1 && _index < Items.Count)
            {
                return Items[_index];
            }
            else
            {
                return null;
            }
        }

        private void PlaylistItem_ItemUpdated(object sender, PlaylistItem e)
        {
            if (HasItem(e))
            {
                UpdateTotalTime();
            }
        }

        #endregion Playlist Control

        #region Next Pre

        List<int> IndexList = new List<int>();
        int NowIndex = -1;
        
        public void GoNext()
        {
            if (Items != null)
            {
                int count = 0;
                int index = _index;
                do
                {
                    index = InternalNext(index);

                    if (index < 0 || index >= Items.Count)
                    {
                        break;
                    }

                    if (Items[index].IsAvailable)
                        break;

                    count++;
                }
                while (count < Items.Count);

                _index = index;

                PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
            }
        }

        private int InternalNext(int index)
        {
            if (Order == PlaylistOrder.Once)
            {
                if ((Items.Count > 0) && (index < Items.Count - 1))
                {
                    if (index == -1)
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                }
                else if ((index >= Items.Count - 1))
                {
                    index = -1;
                }
            }
            else if (Order == PlaylistOrder.Repeat)
            {
                if ((Items.Count > 0) && (index < Items.Count - 1))
                {
                    if (index == -1)
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                }
                else if ((index >= Items.Count - 1))
                {
                    index = 0;
                }
            }
            else if (Order == PlaylistOrder.Random)
            {
                Random rd = new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds));
                index = rd.Next(0, Items.Count - 1);
                IndexList.Add(_index);
                NowIndex = IndexList.Count - 1;
            }
            else if (Order == PlaylistOrder.RepeatOne)
            {
                return index;
            }

            return index;
        }

        public void GoPrevious()
        {
            if (Items != null)
            {
                int count = 0;
                int index = _index;
                do
                {
                    index = InternalPrevious(index);

                    if(index < 0 || index >= Items.Count)
                    {
                        break;
                    }

                    if (Items[index].IsAvailable)
                        break;

                    count++;
                }
                while (count < Items.Count);

                _index = index;

                PlaylistUpdated?.Invoke(this, new PlaylistEventArgs(Items));
            }
        }

        public int InternalPrevious(int index)
        {
            if (Order == PlaylistOrder.Once)
            {
                if ((Items.Count > 0) && (index > 0))
                {
                    if (index == -1)
                    {
                        index = Items.Count - 1;
                    }
                    else
                    {
                        index--;
                    }
                }
                else if ((index == 0))
                {
                    index = -1;
                }
            }
            else if (Order == PlaylistOrder.Repeat)
            {
                if ((Items.Count > 0) && (index > 0))
                {
                    if (index == -1)
                    {
                        index = Items.Count - 1;
                    }
                    else
                    {
                        index--;
                    }
                }
                else if ((index == 0))
                {
                    index = Items.Count - 1;
                }
            }
            else if (Order == PlaylistOrder.Random)
            {
                if (IndexList.Count > 0 && NowIndex > 0)
                {
                    NowIndex--;
                    index = IndexList[NowIndex];
                }
                else
                {
                    Random rd = new Random((int)(DateTime.Now.TimeOfDay.TotalMilliseconds));
                    index = rd.Next(0, Items.Count - 1);
                    IndexList.Add(index);
                    NowIndex = IndexList.Count - 1;
                }
            }

            return index;
        }

        #endregion Next Pre
    }
}
