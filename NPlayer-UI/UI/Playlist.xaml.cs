using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NPlayer;
using System.Threading;

namespace NPlayer_UI
{
    /// <summary>
    /// Playlist.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Playlist : Window
    {
        public delegate void updatetxt(string label1, string label2, int max, int value);
        public delegate void updatedele();

        public nPlayerCore np;
        public OpenFileDialog ofd;
        public Prograss pg;

        private bool free = false;
        private bool updated = false;
        private string[] files;
        private int updating = 0;

        public Playlist(ref nPlayerCore npp)
        {
            InitializeComponent();

            np = npp;
            np.Playlists[np.CurrentPlaylistIndex].PlaylistUpdated += Playlist_PlaylistUpdated;

            update();
        }

        private void Playlist_PlaylistUpdated(object sender, PlaylistEventArgs e)
        {
            if (this.list.Dispatcher.CheckAccess())
            {
                update();
            }
            else
            {
                this.list.Dispatcher.Invoke(new updatedele(update));
                return;
            }
        }

        public void updatePos(int left, int top, int w, int h)
        {
            if (!free)
            {
                updating = 0;
                this.Left = left + w - 16;
                this.Top = top;
                this.Height = h;
            }
        }

        void update()
        {
            list.Items.Clear();
            updated = true;
            if (np.Playlists[np.CurrentPlaylistIndex] != null)
            {
                if (np.Playlists[np.CurrentPlaylistIndex].Items != null)
                {
                    for (int i = 0; i < np.Playlists[np.CurrentPlaylistIndex].Items.Count; i++)
                    {
                        ListBoxItem item = new ListBoxItem();
                        nPlayerPlaylistItem music = np.Playlists[np.CurrentPlaylistIndex].Items[i];

                        if (i == np.Playlists[np.CurrentPlaylistIndex].Index)
                        {
                            item.Content = ">| ";
                        }
                        else
                        {
                            item.Content = "- | ";
                        }

                        item.Content += music.FileName;

                        if (music.Tag.Album != null)
                        {
                            item.Content += " | " + music.Tag.Album;
                        }

                        if (music.Tag.Title != null)
                        {
                            item.Content += " | " + music.Tag.Title;
                        }

                        if (music.Tag.Duration != null)
                        {
                            item.Content += " | " + music.Tag.Duration;
                        }

                        item.MouseDoubleClick += Item_MouseDoubleClick;
                        item.MouseRightButtonUp += Item_MouseRightButtonUp;
                        item.Tag = i;
                        list.Items.Add(item);
                    }
                    lbCurrentList.Content = np.CurrentPlaylistIndex;
                    lbIndex.Content = np.Playlists[np.CurrentPlaylistIndex].Index + 1;
                    lbTitle.Content = np.Playlists[np.CurrentPlaylistIndex].Title;
                    comboBox.SelectedIndex = (int)np.Playlists[np.CurrentPlaylistIndex].Order;
                }
            }
        }

        private void Item_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
        }

        private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            np.Play((int)item.Tag);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((np != null))
            {
                np.SetPlaylistOrder((PlaylistOrder)comboBox.SelectedIndex);
            }
            else
            {
                updated = false;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)    //remove
        {
            if (np != null)
            {
                int[] indexes = new int[list.SelectedItems.Count];
                for (int i = 0; i < list.SelectedItems.Count; i++)
                {
                    ListBoxItem lbitem = (ListBoxItem)list.SelectedItems[i];
                    indexes[i] = (int)lbitem.Tag;
                }
                //np.CurrentPlaylist.Remove(indexes);
            }
        }

        public void Add()
        {
            for (int i = 0; i < files.Length; i++)
            {
                pg.Dispatcher.Invoke(new updatetxt(pg.update), files[i], i.ToString() + "/" + files.Length.ToString(), files.Length, i);
                np.Playlists[np.CurrentPlaylistIndex].Add(files[i]);
            }
            pg.Dispatcher.Invoke(new updatedele(pg.Close));
            pg = null;
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)      //add
        {
            ofd = new OpenFileDialog();
            ofd.Title = "파일 추가";
            ofd.Filter = np.FileFilter;
            ofd.Multiselect = true;
            ofd.ShowDialog();
            if (ofd.FileNames != null)
            {
                files = ofd.FileNames;
                pg = new Prograss();
                pg.Show();
                Thread th = new Thread(new ThreadStart(Add));
                th.Start();
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (updating < 4)
            {
                updating++;
            }
            else
            {
                free = true;
            }
        }
    }
}
