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
using Symphony.Player;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Threading;
using Symphony.Dancer;
using Symphony.Lyrics;
using Symphony.Server;
using Microsoft.Win32;
using Symphony.Util;

namespace Symphony.UI
{
    /// <summary>
    /// MusicInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MusicInfo : Window
    {
        PlayerCore np;
        PlaylistItem item
        {
            get
            {
                if (np != null)
                    return np.Playlists[PlaylistIndex].Items[ItemIndex];
                else
                    return null;
            }
            set
            {
                if (np != null)
                    np.Playlists[PlaylistIndex].Items[ItemIndex] = value;
            }
        }

        MainWindow mw;
        int PlaylistIndex;
        int ItemIndex;
        MusicMetadata metadata;

        Storyboard PopupOff;

        ShadowWindow shadow;

        public MusicInfo(MainWindow Parent, ref PlayerCore np, int playlistIndex, int itemIndex)
        {
            InitializeComponent();

            shadow = new ShadowWindow(this, Parent, 20, 0.28, false);

            Owner = Parent;
            mw = Parent;
            mw.Closed += Mw_Closed;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            this.np = np;
            PlaylistIndex = playlistIndex;
            ItemIndex = itemIndex;

            if (playlistIndex * itemIndex < 0)
            {
                Close();
                return;
            }

            mw.SetRenderUI(false);

            UpdateUI();

            UpdateWeb();
        }

        private void Mw_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            mw.SetRenderUI(true);
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mw.SetRenderUI(false);
            PopupOff.Begin();
        }

        #region updater

        PlotSearchResult PlotSearchResult;
        LyricSearchResult LyricSearchResult;
        Song song;

        private async void UpdateWeb()
        {
            Menu_Lyric_Edit.IsEnabled = false;
            Menu_Lyric_Manage.IsEnabled = false;
            Menu_Lyric_Reset.IsEnabled = false;
            Menu_Lyric_Download.IsEnabled = false;
            Menu_Dance_Edit.IsEnabled = false;
            Menu_Dance_Manage.IsEnabled = false;
            Menu_Dance_Download.IsEnabled = false;
            Menu_Dance_Reset.IsEnabled = false;

            if (metadata == null)
                return;

            string preHeader = (string)Menu_Dance_Edit.Header;
            Menu_Dance_Edit.Header = LanguageHelper.FindText("Lang_MusicInfo_Checking_Server");
            string preHeader1 = (string)Menu_Lyric_Edit.Header;
            Menu_Lyric_Edit.Header = LanguageHelper.FindText("Lang_MusicInfo_Checking_Server");

            Task t = new Task(new Action(UpdateSong));

            t.Start();

            await t;

            Menu_Lyric_Edit.Header = preHeader1;
            Menu_Dance_Edit.Header = preHeader;

            UpdateLyric();

            UpdatePlot();
        }

        private void UpdateSong()
        {
            QueryResult result = SongSearcher.Search(metadata);

            if (result.Success && result.Tag != null)
            {
                song = ((SongCollection)result.Tag).TargetSong;
            }
        }

        private async void UpdateLyric()
        {
            Menu_Lyric_Edit.IsEnabled = false;
            Menu_Lyric_Manage.IsEnabled = false;
            Menu_Lyric_Reset.IsEnabled = false;
            Menu_Lyric_Download.IsEnabled = false;

            string preHeader = (string)Menu_Lyric_Download.Header;
            Menu_Lyric_Download.Header = LanguageHelper.FindText("Lang_MusicInfo_Checking_Server");

            Task<LyricSearchResult> tLocal = new Task<LyricSearchResult>(new Func<LyricSearchResult>(()=> { return LyricHelper.SearchLocal(metadata); }));
            tLocal.Start();
            LyricSearchResult localResult = await tLocal;

            LyricSearchResult = localResult;
            if(localResult.Exist == LyricExistState.LocalExist)
            {
                Menu_Lyric_Edit.IsEnabled = true;
            }

            Task<LyricSearchResult> t = new Task<LyricSearchResult>(new Func<LyricSearchResult>(() => { return LyricHelper.SearchWeb(metadata, song, localResult); }));
            t.Start();
            LyricSearchResult = await t;

            Menu_Lyric_Download.Header = preHeader;

            Menu_Lyric.IsEnabled = true;
            Menu_Lyric_Reset.IsEnabled = true;
            Menu_Lyric_Edit.IsEnabled = true;
            Menu_Lyric_Download.IsEnabled = true;

            switch (LyricSearchResult.Exist)
            {
                case LyricExistState.UnsureLocal:
                    Menu_Lyric_Download.IsEnabled = false;
                    break;
                case LyricExistState.LocalExist:
                    Menu_Lyric_Download.IsEnabled = false;
                    break;
                case LyricExistState.GlobalExist:
                    break;
                case LyricExistState.WebExitst:
                    Menu_Lyric_Edit.IsEnabled = false;
                    break;
                case LyricExistState.None:
                    Menu_Lyric_Download.IsEnabled = false;
                    Menu_Lyric_Edit.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private async void UpdatePlot()
        {
            Menu_Dance_Edit.IsEnabled = false;
            Menu_Dance_Manage.IsEnabled = false;
            Menu_Dance_Download.IsEnabled = false;
            Menu_Dance_Reset.IsEnabled = false;

            string preHeader = (string)Menu_Dance_Edit.Header;
            Menu_Dance_Edit.Header = LanguageHelper.FindText("Lang_MusicInfo_Checking_Server");

            Task<PlotSearchResult> t = new Task<PlotSearchResult>(new Func<PlotSearchResult>(() => { return PlSearch.Search(metadata); }));
            t.Start();
            PlotSearchResult = await t;

            Menu_Dance_Edit.Header = preHeader;

            Menu_Dance.IsEnabled = true;
            Menu_Dance_Reset.IsEnabled = true;
            Menu_Dance_Edit.IsEnabled = true;
            Menu_Dance_Download.IsEnabled = true;

            switch (PlotSearchResult.Exist)
            {
                case PlotExistState.UnsureLocal:
                    Menu_Dance_Download.IsEnabled = false;
                    break;
                case PlotExistState.LocalExist:
                    Menu_Dance_Download.IsEnabled = false;
                    break;
                case PlotExistState.GlobalExist:
                    break;
                case PlotExistState.WebExitst:
                    Menu_Dance_Edit.IsEnabled = false;
                    break;
                case PlotExistState.None:
                    Menu_Dance_Download.IsEnabled = false;
                    Menu_Dance_Edit.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private void ClearUI()
        {
            metadata = null;

            Menu_AlbumArt_Add.IsEnabled = false;
            Menu_AlbumArt_Clear.IsEnabled = false;

            Lb_Title.Text = LanguageHelper.FindText("Lang_Unknown_Title");
            Lb_Artist.Text = LanguageHelper.FindText("Lang_Unknown_Artist");
            Lb_Bitrate.Text = "- kbps";
            Lb_Codec.Text = LanguageHelper.FindText("Lang_Unknown");
            Lb_Genre.Text = LanguageHelper.FindText("Lang_Unknown");
            Lb_Sample.Text = "- Hz";
            Lb_Size.Text = "- MB";
            Lb_Track_Album.Text = LanguageHelper.FindText("Lang_Unknown");
            Lb_Year.Text = LanguageHelper.FindText("Lang_Unknown");
            Lb_FileName.Text = "-";
            Lb_FilePath.Text = "-";

            Menu_AlbumArt.Items.Clear();

            MenuItem mi = new MenuItem();
            mi.Header = LanguageHelper.FindText("Lang_MusicInfo_Oops_AlbumArt");
            mi.Height = 25;

            Menu_AlbumArt.Items.Add(mi);
        }

        private void UpdateUI()
        {
            ClearUI();

            if (item.Tag != null)
            {
                metadata = new MusicMetadata(item.Tag.Title, item.Tag.Artist, item.Tag.Album, "", item.FileName);

                if (!item.Tag.CanEditAlbumArt())
                {
                    Menu_AlbumArt_Add.IsEnabled = false;
                    Menu_AlbumArt_Clear.IsEnabled = false;
                }
                else
                {
                    Menu_AlbumArt_Add.IsEnabled = true;
                    Menu_AlbumArt_Clear.IsEnabled = true;
                }

                if (!string.IsNullOrWhiteSpace(item.Tag.Title))
                {
                    Lb_Title.Text = item.Tag.Title;
                }

                if (!string.IsNullOrWhiteSpace(item.Tag.Artist))
                {
                    Lb_Artist.Text = item.Tag.Artist;
                }

                if (!string.IsNullOrWhiteSpace(item.Tag.Year))
                {
                    Lb_Bitrate.Text = item.Tag.Bitrate.ToString("0kbps");
                }

                if (!string.IsNullOrWhiteSpace(item.Tag.Codec))
                {
                    Lb_Codec.Text = item.Tag.Codec;
                }

                if (!string.IsNullOrWhiteSpace(item.Tag.Genre))
                {
                    Lb_Genre.Text = item.Tag.Genre;
                }

                if (item.Tag.Frequency > 0)
                {
                    Lb_Sample.Text = item.Tag.Frequency.ToString("0,0 Hz");
                }

                if (!string.IsNullOrWhiteSpace(item.FilePath) && File.Exists(item.FilePath))
                {
                    Lb_Size.Text = ((new FileInfo(item.FilePath)).Length / 1000000).ToString("0.0 MB");
                }

                if (!string.IsNullOrWhiteSpace(item.Tag.Album) && !string.IsNullOrWhiteSpace(item.Tag.Track))
                {
                    //둘다 잇음
                    Lb_Track_Album.Text = item.Tag.Track.ToString() + " / " + item.Tag.Album;
                }
                else if (!string.IsNullOrWhiteSpace(item.Tag.Album))
                {
                    //앨범만
                    Lb_Track_Album.Text = item.Tag.Album;
                }
                else if (!string.IsNullOrWhiteSpace(item.Tag.Track))
                {
                    //트랙만
                    Lb_Track_Album.Text = item.Tag.Track.ToString();
                }

                if (!string.IsNullOrWhiteSpace(item.Tag.Year))
                {
                    Lb_Year.Text = item.Tag.Year.ToString();
                }

                if (item.Tag.Pictures != null)
                {
                    SolidColorBrush scb = new SolidColorBrush(Color.FromArgb(11, 0, 0, 0));
                    scb.Freeze();

                    Rct_AlbumArt_1.Fill = scb;
                    Rct_AlbumArt_2.Fill = scb;
                    Rct_AlbumArt_3.Fill = scb;

                    for (int i = 0; i < item.Tag.Pictures.Count && i < 3; i++)
                    {
                        BitmapImage bit = new BitmapImage();
                        try
                        {
                            bit.BeginInit();
                            bit.CacheOption = BitmapCacheOption.None;
                            bit.UriSource = new Uri(item.Tag.Pictures[i].FilePath);

                            PresentationSource source = PresentationSource.FromVisual(Owner);

                            double dpiX;

                            if (source != null)
                            {
                                dpiX = source.CompositionTarget.TransformToDevice.M11;
                            }
                            else
                            {
                                dpiX = 1;
                            }

                            switch (i)
                            {
                                case 0:
                                    bit.DecodePixelWidth = (int)(Rct_AlbumArt_1.Width * dpiX);
                                    break;
                                case 1:
                                    bit.DecodePixelWidth = (int)(Rct_AlbumArt_2.Width * dpiX);
                                    break;
                                case 2:
                                    bit.DecodePixelWidth = (int)(Rct_AlbumArt_3.Width * dpiX);
                                    break;
                            }

                            bit.EndInit();

                            ImageBrush AlbumArt = new ImageBrush(bit);
                            AlbumArt.SetValue(ImageBrush.StretchProperty, Stretch.UniformToFill);
                            AlbumArt.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.Fant);
                            AlbumArt.Freeze();

                            bit.Freeze();

                            switch (i)
                            {
                                case 0:
                                    Rct_AlbumArt_1.Fill = AlbumArt;
                                    break;
                                case 1:
                                    Rct_AlbumArt_2.Fill = AlbumArt;
                                    break;
                                case 2:
                                    Rct_AlbumArt_3.Fill = AlbumArt;
                                    break;
                            }
                        }
                        catch
                        {

                        }
                    }

                    if (item.Tag.Pictures.Count > 0)
                    {
                        Menu_AlbumArt.Items.Clear();

                        string format = LanguageHelper.FindText("Lang_MusicInfo_Extract_Image");

                        for (int i = 0; i < item.Tag.Pictures.Count; i++)
                        {
                            MenuItem mi = new MenuItem();
                            mi.Header = string.Format(format, i + 1);
                            mi.Tag = i;
                            mi.Height = 25;
                            mi.Click += new RoutedEventHandler(Mi_Click);

                            Menu_AlbumArt.Items.Add(mi);
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(item.FileName))
                {
                    Lb_Title.Text = item.FileName;
                }

                //LANGSUP
                if (item.IsAvailable)
                {
                    Lb_Artist.Text = "사용할 수 있음";
                }
                else
                {
                    Lb_Artist.Text = "사용할 수 없음";
                }
            }

            if (!string.IsNullOrWhiteSpace(item.FileName))
            {
                Lb_FileName.Text = item.FileName;
            }

            if (!string.IsNullOrWhiteSpace(item.FilePath))
            {
                Lb_FilePath.Text = item.FilePath;
            }
        }

        #endregion 

        SaveFileDialog sfd;
        bool sfd_okay = false;
        private void Mi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem s = (MenuItem)sender;

            if(sfd == null)
            {
                sfd = new SaveFileDialog();
                sfd.Title = LanguageHelper.FindText("Lang_Save");
                sfd.Filter = Util.IO.SupportedImageFilter;
                sfd.FileOk += Sfd_FileOk;
            }

            try
            {
                sfd_okay = false;

                MemoryStream ms = new MemoryStream(item.Tag.Pictures[(int)s.Tag].RawData);
                ms.Position = 0;

                BitmapDecoder decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                BitmapFrame frame = decoder.Frames[0];
                BitmapMetadata metadata = frame.Metadata as BitmapMetadata;
                string format = metadata.Format.ToLower();

                sfd.DefaultExt = "." + format;

                ms.Close();
                ms.Dispose();
                ms = null;

                sfd.ShowDialog();

                if (sfd_okay)
                {
                    File.WriteAllBytes(sfd.FileName, item.Tag.Pictures[(int)s.Tag].RawData);
                }
            }
            catch(Exception exc)
            {
                Logger.Error(this, exc);

                DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Error_FileSave"), LanguageHelper.FindText("Lang_Error"), DialogMessageType.Okay);
            }
        }

        private void Sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sfd_okay = !e.Cancel;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mw.StopRenderingWhileClicking();

            DragMove();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            mw.SetRenderUI(true);
        }

        private void Lb_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 3)
            {
                try
                {
                    Logger.Info("Copied : \"" + ((TextBlock)sender).Text + "\"");

                    Clipboard.SetText(((TextBlock)sender).Text);
                }
                catch (Exception ee)
                {
                    Logger.Error(this, ee);
                }
            }
        }

        #region Menu_Dance

        private void Menu_Dance_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (plotClearing)
            {
                DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Deleting_Cache"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.Okay);
                return;
            }

            PlotClearCache();
        }

        bool plotClearing = false;
        private async void PlotClearCache()
        {
            DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Confirm_Cache"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

            if (r == DialogMessageResult.Yes)
            {
                plotClearing = true;

                await PlHelper.ClearCacheAsync();

                plotClearing = false;

                DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Deleted_Dance_Cache"));

                UpdatePlot();
            }
        }

        private void Menu_Dance_Make_Click(object sender, RoutedEventArgs e)
        {
            DanceLiteEditor de = new DanceLiteEditor(new MusicMetadata(item));

            UserControlHostWindow host = new UserControlHostWindow(this, mw, LanguageHelper.FindText("Lang_MusicInfo_Dancer_Load"), de);
            host.ShowDialog();
        }

        private void Menu_Dance_Manage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_Dance_Download_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_Dance_Edit_Click(object sender, RoutedEventArgs e)
        {
            //Close();
            //mw.PlotEdit(PlaylistIndex, ItemIndex, PlotSearchResult.LocalPath);

            DanceLiteEditor de = new DanceLiteEditor(PlotSearchResult.LocalPath);

            UserControlHostWindow host = new UserControlHostWindow(this, mw, LanguageHelper.FindText("Lang_MusicInfo_Dancer_Load"), de);
            host.ShowDialog();
        }

        #endregion Menu_Dance

        #region Menu_Lyric

        private void Menu_Lyric_Make_Click(object sender, RoutedEventArgs e)
        {
            Close();
            mw.NewLyric(PlaylistIndex, ItemIndex);
        }

        private void Menu_Lyric_Edit_Click(object sender, RoutedEventArgs e)
        {
            Close();
            mw.LyricEdit(PlaylistIndex, ItemIndex, LyricSearchResult.LocalPath);
        }

        private void Menu_Lyric_Manage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_Lyric_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (lyricClearing)
            {
                DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Deleting_Cache"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.Okay);
                return;
            }
            LyricCacheClear();
        }

        private bool lyricClearing = false;
        private async void LyricCacheClear()
        {
            DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Confirm_Cache"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

            if (r == DialogMessageResult.Yes)
            {
                lyricClearing = true;

                await LyricHelper.ClearCacheAsync();

                lyricClearing = false;

                DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Deleted_Lyric_Cache"));

                UpdateLyric();
            }
        }

        private void Menu_Lyric_Download_Click(object sender, RoutedEventArgs e)
        {
            if(LyricSearchResult!= null && Menu_Lyric_Download.IsEnabled)
            {
                if (LyricSearchResult.Exist == LyricExistState.WebExitst)
                {
                    Lyrics.LyricDownloader ld = new Lyrics.LyricDownloader(this, LyricSearchResult.RegisteredLyric);
                    ld.Closed += Ld_Closed;
                    ld.Show();
                }
                else
                {
                    DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Overwrite_Cache"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

                    if(r == DialogMessageResult.Yes)
                    {
                        Lyrics.LyricDownloader ld = new Lyrics.LyricDownloader(this, LyricSearchResult.RegisteredLyric);
                        ld.Closed += Ld_Closed;
                        ld.Show();
                    }
                }
            }
        }

        private void Ld_Closed(object sender, EventArgs e)
        {
            UpdateWeb();
        }

        #endregion Menu_Lyric

        #region Menu_File

        private void Menu_File_Move_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = LanguageHelper.FindText("Lang_MusicInfo_File_Move");
            sfd.Filter = string.Format("{1}|*{0}|{2}|*.*", System.IO.Path.GetExtension(item.FilePath), LanguageHelper.FindText("Lang_SupportMusicFormat"), LanguageHelper.FindText("Lang_AllFileFormat"));
            sfd.DefaultExt = System.IO.Path.GetExtension(item.FilePath);

            if(sfd.ShowDialog() == true)
            {
                string source = item.FilePath;

                bool playing = false;
                int pos = 0;
                if(np.isPlay && np.CurrentPlaylistIndex == PlaylistIndex && np.CurrentPlaylist != null && np.CurrentPlaylist.Index == ItemIndex)
                {
                    playing = true;
                    pos = (int)np.GetPosition(TimeUnit.MilliSecond);

                    np.Stop();
                }

                try
                {
                    if (source.ToLower() != sfd.FileName.ToLower())
                    {
                        if (File.Exists(sfd.FileName))
                        {
                            File.Delete(sfd.FileName);
                        }

                        File.Move(source, sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_File_Move_Error"));

                    return;
                }

                np.Playlists[PlaylistIndex].Remove(ItemIndex);
                mw.UpdateList();

                item.FilePath = sfd.FileName;
                item.FileName = System.IO.Path.GetFileName(sfd.FileName);
                item.Tag.FilePath = sfd.FileName;

                UpdateUI();

                if (playing)
                {
                    np.Play(ItemIndex);
                    np.SetPosition(pos);
                }
            }
        }

        private void Menu_File_Copy_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = LanguageHelper.FindText("Lang_MusicInfo_File_Copy");
            sfd.Filter = string.Format("{1}|*{0}|{2}|*.*", System.IO.Path.GetExtension(item.FilePath), LanguageHelper.FindText("Lang_SupportMusicFormat"), LanguageHelper.FindText("Lang_AllFileFormat"));
            sfd.DefaultExt = System.IO.Path.GetExtension(item.FilePath);

            if (sfd.ShowDialog() == true)
            {
                FIleCopy fc = new FIleCopy(item.FilePath, sfd.FileName);

                ProgressWindow pw = new ProgressWindow(this, fc, LanguageHelper.FindText("Lang_MusicInfo_File_Copy"), System.IO.Path.GetDirectoryName(item.FilePath) + " > " + System.IO.Path.GetDirectoryName(sfd.FileName));

                pw.ShowDialog();
            }
        }

        private void Menu_File_Rename_Click(object sender, RoutedEventArgs e)
        {
            DialogText dt = new DialogText(this, LanguageHelper.FindText("Lang_MusicInfo_File_Edit_Name"), System.IO.Path.GetFileNameWithoutExtension(item.FilePath));
            DialogTextResult r = dt.ShowDialog();

            if (r.Okay)
            {
                bool playing = false;
                double pos = 0;

                if (np.isPlay)
                {
                    if(np.CurrentPlaylistIndex == PlaylistIndex && np.CurrentPlaylist != null && np.CurrentPlaylist.Index == ItemIndex)
                    {
                        playing = true;
                        pos = np.GetPosition(TimeUnit.MilliSecond);

                        np.Stop();
                    }
                }

                string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(item.FilePath), Util.TextTool.FileNameFix( r.Text ) + System.IO.Path.GetExtension(item.FilePath));

                if (item.FilePath.ToLower() != dest.ToLower())
                {
                    try
                    {
                        Logger.Log("Rename File: " + item.FilePath + " > " + dest);

                        File.Move(item.FilePath, dest);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(this, ex);

                        DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_File_Rename_Error"));

                        return;
                    }
                }

                item.FilePath = dest;
                item.FileName = System.IO.Path.GetFileName(dest);
                item.Tag.FilePath = dest;

                UpdateUI();

                if (playing)
                {
                    np.Play(ItemIndex);
                    np.SetPosition((int)pos);
                }
            }
        }

        private void Menu_File_Remove_Item_Click(object sender, RoutedEventArgs e)
        {
            if (np.isPlay && np.CurrentPlaylist != null && np.CurrentPlaylistIndex == PlaylistIndex && np.CurrentPlaylist.Index == ItemIndex)
            {
                np.Stop();
            }

            np.Playlists[PlaylistIndex].Remove(ItemIndex);
            mw.UpdateList();

            Close();
        }

        private void Menu_File_Delete_Click(object sender, RoutedEventArgs e)
        {
            DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_Confirm_Delete_File"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

            if(r == DialogMessageResult.Yes)
            {
                if (np.isPlay && np.CurrentPlaylist != null && np.CurrentPlaylistIndex == PlaylistIndex && np.CurrentPlaylist.Index == ItemIndex)
                {
                    np.Stop();
                }

                try
                {
                    Logger.Log("File Remove: " + item.FilePath);

                    File.Delete(item.FilePath);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_File_Remove_Error"));
                }

                np.Playlists[PlaylistIndex].Remove(ItemIndex);
                mw.UpdateList();

                Close();
            }
        }

        #endregion Menu_File

        #region Menu_Tag
        
        private void Menu_AlbumArt_Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = LanguageHelper.FindText("Lang_File_Open");
            string ft = LanguageHelper.FindText("Lang_SupportImageFormat") + "|";
            foreach (string format in Tags.AlbumArtAvailableFormats)
            {
                ft += "*." + format.Trim().Trim('.') + ";"; 
            }
            ft.Trim(';');
            ofd.Filter = ft;

            ofd.FileOk += delegate
            {
                if (File.Exists(ofd.FileName))
                {
                    int index = 10;
                    if(item.Tag.Pictures != null)
                    {
                        index = item.Tag.Pictures.Count + 10;
                    }

                    SetAlbumArt(index, ofd.FileName);
                }
            };

            ofd.ShowDialog(this);
        }

        private void Menu_AlbumArt_Clear_Click(object sender, RoutedEventArgs e)
        {
            DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Clear_Confirm"), LanguageHelper.FindText("Lang_Caution"), DialogMessageType.YesNo);

            if (r == DialogMessageResult.Yes)
            {
                try
                {
                    item.Tag.ClearAlbumArt();

                    DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Clear_Finish"));
                }
                catch(Exception ex)
                {
                    if(ex is System.IO.IOException)
                    {
                        DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Clear_FileUsing"));
                    }
                    else
                    {
                        DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Clear_Error"));
                    }
                }

                UpdateUI();
                mw.UpdateList();
            }
        }

        private void Menu_Tag_Refresh_Click(object sender, RoutedEventArgs e)
        {
            item.Tag.Refresh();

            UpdateUI();
            mw.UpdateList();
        }

        #endregion Menu_Tag

        #region AlbumArt

        private void ShowAlbumArtViewer(int index)
        {
            if (item != null && item.Tag != null && item.Tag.Pictures != null && item.Tag.Pictures.Count > 0 && item.Tag.Pictures.Count > index)
            {
                List<string> imgSources = new List<string>();
                foreach (TagImage img in item.Tag.Pictures)
                {
                    imgSources.Add(img.FilePath);
                }

                ImageViewerPopup iv = new ImageViewerPopup(mw, this, imgSources, index, item.FileName);
                iv.Show();
            }
        }

        private void Rct_AlbumArt_1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                ShowAlbumArtViewer(0);
            }
        }

        private void Rct_AlbumArt_2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                ShowAlbumArtViewer(1);
            }
        }

        private void Rct_AlbumArt_3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount > 1)
            {
                ShowAlbumArtViewer(2);
            }
        }
        
        private void SetAlbumArt(int index, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Length > 0 && File.Exists(files[0]) && Symphony.Player.Tags.IsImage(files[0]))
                {
                    SetAlbumArt(index, files[0]);
                }
            }
        }

        private void SetAlbumArt(int index, string path)
        {
            Logger.Log("Start Edit AlbumArt");

            string ext = System.IO.Path.GetExtension(item.FilePath);
            if(ext.EndsWith("wav") || ext.EndsWith("wave"))
            {
                DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Unsupported") + ": " + ext.ToUpper().Trim('.'));
                return;
            }

            string errMsg = LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Set_Failed");

            bool npStopped = false;
            int position = 0;
            if (np.CurrentMusic == item)
            {
                position = (int)np.GetPosition(TimeUnit.MilliSecond);
                np.Stop();
                npStopped = true;
            }

            bool result;
            try
            {
                result = item.Tag.SetAlbumArt(index, path);
            }
            catch (Exception ex)
            {
                Logger.Error(this, ex);
                result = false;

                if (ex is IOException)
                {
                    errMsg = LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Clear_FileUsing");
                }
            }

            if (npStopped)
            {
                np.Play(ItemIndex);
                np.SetPosition(position);
            }

            UpdateUI();
            mw.UpdateList();

            if (result)
            {
                DialogMessage.Show(this, LanguageHelper.FindText("Lang_MusicInfo_AlbumArt_Set_Finished"));
            }
            else
            {
                DialogMessage.Show(this, errMsg);
            }
        }

        private void Rct_AlbumArt_1_Drop(object sender, DragEventArgs e)
        {
            SetAlbumArt(0, e);
        }

        private void Rct_AlbumArt_2_Drop(object sender, DragEventArgs e)
        {
            SetAlbumArt(1, e);
        }

        private void Rct_AlbumArt_3_Drop(object sender, DragEventArgs e)
        {
            SetAlbumArt(2, e);
        }

        #endregion AlbumArt

        private void wd_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
            {
                Menu_Tag_Refresh_Click(this, null);
            }
        }
    }
}
