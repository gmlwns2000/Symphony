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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Symphony.Player;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using Symphony.UI;
using System.ComponentModel;
using System.Diagnostics;
using Symphony.Util;

namespace Symphony.UI
{
    /// <summary>
    /// PlaylistsViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 
    public class IndexChangedArgs:EventArgs
    {
        public int index;

        public IndexChangedArgs(int indexx)
        {
            index = indexx;
        }
    }
    public partial class PlaylistsViewer : UserControl
    {
        ImageBrush Brush_Play;
        List<LvItemPlaylist> items = new List<LvItemPlaylist>();
        public EventHandler<IndexChangedArgs> ListBoxItemDoubleClicked;
        PlayerCore np;
        Storyboard Story_ViewOn;
        Storyboard Story_ViewOff;
        OpenFileDialog ofd = new OpenFileDialog();
        PlaylistItemViewer playlistitemViewer;
        MainWindow mw;

        public PlaylistsViewer()
        {
            InitializeComponent();

            lstData.ItemsSource = items;

            ofd.Title = LanguageHelper.FindText("Lang_Playlist_Open");
            ofd.Multiselect = true;
            LanguageHelper.LangaugeChanged += delegate (object s, EventArgs arg)
            {
                ofd.Title = LanguageHelper.FindText("Lang_Playlist_Open");
                ofd.Filter = LanguageHelper.FindText("Lang_M3U8File") + "|*.m3u8|" + LanguageHelper.FindText("Lang_M3UFile") + "|*.m3u|" + LanguageHelper.FindText("Lang_AllFileFormat") + "|*.*";
            };
            ofd.Filter = LanguageHelper.FindText("Lang_M3U8File") + "|*.m3u8|" + LanguageHelper.FindText("Lang_M3UFile") + "|*.m3u|" + LanguageHelper.FindText("Lang_AllFileFormat") + "|*.*";
            ofd.FileOk += Ofd_FileOk;

            Brush_Play = this.FindResource("Brush_Play") as ImageBrush;
            Story_ViewOff = (Storyboard)this.FindResource("ViewOff");
            Story_ViewOff.Completed += Story_ViewOff_Completed;
            Story_ViewOn = (Storyboard)this.FindResource("ViewOn");
        }

        private void Ofd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ofd.FileNames.Length > 0)
            {
                for (int i = 0; i<ofd.FileNames.Length; i++)
                {
                    List<string> pl = new List<string>();

                    string title = System.IO.Path.GetFileNameWithoutExtension(ofd.FileNames[i]);
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileNames[i]);
                    bool skip = false;

                    Debug.WriteLine("np.Playlists.Count: " + np.Playlists.Count.ToString());
                    for(int ii = 0; ii < np.Playlists.Count; ii++)
                    {
                        if(np.Playlists[ii].Title == title)
                        {
                            AskMerge am = new AskMerge(mw);
                            am.ShowDialog();
                            if(am.result == MergeMode.Merge)
                            {
                                for(int iii=0; iii< np.Playlists[ii].Items.Count; iii++)
                                {
                                    pl.Add(np.Playlists[ii].Items[iii].FilePath);
                                }
                                np.Playlists.RemoveAt(ii);
                            }
                            else if(am.result == MergeMode.Change)
                            {
                                np.Playlists.RemoveAt(ii);
                            }else
                            {
                                skip = true;
                            }
                            break;
                        }
                    }

                    if (!skip)
                    {
                        for (int ii = 0; ii < lines.Length; ii++)
                        {
                            string line = lines[ii];

                            if (line[0].CompareTo('#') != 0)
                            {
                                try
                                {
                                    if (!System.IO.Path.IsPathRooted(line))
                                    {
                                        line = System.IO.Path.GetDirectoryName(ofd.FileNames[i]) + "\\" + line;
                                    }
                                    pl.Add(line);
                                }
                                catch { }
                            }
                        }
                        mw.AddPlaylist(title, pl.ToArray());
                    }
                }
            }
        }

        private void Story_ViewOff_Completed(object sender, EventArgs e)
        {
            if (ListBoxItemDoubleClicked != null)
            {
                ListBoxItemDoubleClicked(sender, new IndexChangedArgs(lstData.SelectedIndex));
            }
        }

        private void Bt_Open_List_Click(object sender, RoutedEventArgs e)
        {
            mw.SetRenderUI(false);
            ofd.ShowDialog();
            mw.SetRenderUI(true);
        }

        public void Init(Window wd, ref PlaylistItemViewer plv)
        {
            mw = (MainWindow)wd;
            playlistitemViewer = plv;
        }

        private void Bt_Add_Item_Click(object sender, RoutedEventArgs e)
        {
            NewPlaylist nwp = new NewPlaylist(mw, np.Playlists,LanguageHelper.FindText("Lang_Playlist")+" "+(items.Count+1).ToString());
            nwp.ShowDialog();
            string title = nwp.name;
            if (title != null && !Util.TextTool.StringEmpty(title))
            {
                np.Playlists.Add(new Playlist(title, PlaylistOrder.Once));

                if (np.Playlists.Count == 1)
                {
                    np.CurrentPlaylistIndex = 0;
                }

                updateList();
            }
        }

        private void Bt_Delete_Click(object sender, RoutedEventArgs e)
        {
            while (lstData.SelectedItems.Count > 0 && lstData.SelectedIndex >= 0)
            {
                removeAt(lstData.SelectedIndex);
                playlistitemViewer.Updated = false;
                lstData.Items.Refresh();
            }
        }

        private void removeAt(int index)
        {
            if (index < np.CurrentPlaylistIndex)
            {
                np.CurrentPlaylistIndex--;
            }
            else if (index == np.CurrentPlaylistIndex)
            {
                np.CurrentPlaylistIndex = -1;
            }

            np.Playlists.RemoveAt(lstData.SelectedIndex);
            items.RemoveAt(lstData.SelectedIndex);
        }

        public void ViewOn()
        {
            this.Story_ViewOn.Begin();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Story_ViewOff.Begin();
        }

        private void ListBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Story_ViewOff.Begin();
            }
        }

        public void PlayStart()
        {
            if (items.Count > np.CurrentPlaylistIndex)
            {
                items[np.CurrentPlaylistIndex].PlayingCover = Brush_Play;
            }
            lstData.ItemsSource = items;
            lstData.Items.Refresh();
        }

        public void PlayStopped()
        {
            if (items.Count > np.CurrentPlaylistIndex && np.CurrentPlaylistIndex>=0)
            {
                items[np.CurrentPlaylistIndex].PlayingCover = null;
            }
            lstData.ItemsSource = items;
            lstData.Items.Refresh();
        }

        public void setTarget(ref PlayerCore np)
        {
            this.np = np;
        }

        public void updateList()
        {
            items.Clear();

            for (int i=0; i< np.Playlists.Count; i++)
            {
                items.Add(new LvItemPlaylist(np.Playlists[i]));
            }

            if (np.isPlay && np.CurrentPlaylistIndex>=0)
            {
                items[np.CurrentPlaylistIndex].PlayingCover = Brush_Play;
            }

            lstData.ItemsSource = items;
            lstData.Items.Refresh();
        }
    }
}
