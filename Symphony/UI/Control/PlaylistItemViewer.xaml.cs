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
using System.Windows.Threading;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media.Animation;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls.Primitives;

namespace Symphony.UI
{
    /// <summary>
    /// PlaylistItemViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PlaylistItemViewer : UserControl
    {
        public event EventHandler<RoutedEventArgs> GoBack;
        public event EventHandler<ScrollChangedEventArgs> ScrollChanged;

        public bool Updated { get; set; } = false;

        double dpiX;
        double ItemHeight = 45;

        int PlaylistIndex = -1;
        Playlist Playlist
        {
            get
            {
                if(np != null && np.Playlists.Count > 0 && PlaylistIndex > -1 && PlaylistIndex < np.Playlists.Count)
                {
                    return np.Playlists[PlaylistIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        int maxUndo = 75;

        PlayerCore np;
        MainWindow mw;

        List<LvItemPlaylistItem> items = new List<LvItemPlaylistItem>();
        RoutedEventArgs eventArgs;

        ImageBrush Brush_Play;

        Storyboard Story_ViewOn;
        Storyboard Story_ViewOff;
        Storyboard Story_EditOn;
        Storyboard Story_EditOff;
        Storyboard Story_DragPanelOn;
        Storyboard Story_DragPanelOff;
        Storyboard Story_DragPanelTrashOn;
        Storyboard Story_DragPanelTrashOff;
        Storyboard Story_DragPanelCancelOn;
        Storyboard Story_DragPanelCancelOff;

        List<List<PlaylistItem>> undoQue = new List<List<PlaylistItem>>(75);

        public PlaylistItemViewer()
        {
            InitializeComponent();

            DecodeUpdater.Interval = TimeSpan.FromSeconds(0.25);
            DecodeUpdater.Tick += DecodeUpdater_Tick;

            lstData.ItemsSource = items;

            Brush_Play = (ImageBrush)FindResource("Brush_Play");

            Story_ViewOff = (Storyboard)FindResource("ViewOff");
            Story_ViewOff.Completed += Story_ViewOff_Completed;

            Story_ViewOn = (Storyboard)FindResource("ViewOn");

            Story_EditOff = (Storyboard)FindResource("EditOff");
            Story_EditOff.Completed += Story_EditOff_Completed;

            Story_EditOn = (Storyboard)FindResource("EditOn");
            Story_EditOn.Completed += Story_EditOn_Completed;

            Story_DragPanelCancelOff = (Storyboard)FindResource("DragPanelCancelOff");

            Story_DragPanelCancelOn = (Storyboard)FindResource("DragPanelCancelOn");

            Story_DragPanelOff = (Storyboard)FindResource("DragPanelOff");
            Story_DragPanelOff.Completed += Story_DragPanelOff_Completed;

            Story_DragPanelOn = (Storyboard)FindResource("DragPanelOn");

            Story_DragPanelTrashOff = (Storyboard)FindResource("DragPanelTrashOff");

            Story_DragPanelTrashOn = (Storyboard)FindResource("DragPanelTrashOn");
        }

        #region Init

        public void Init(ref PlayerCore np, MainWindow mw)
        {
            this.mw = mw;
            this.mw.Closed += Mw_Closed;

            this.np = np;

            PlaylistItem.ItemUpdated += PlaylistItem_ItemUpdated;
        }

        private void PlaylistItem_ItemUpdated(object sender, PlaylistItem e)
        {
            if (Playlist.HasItem(e))
            {
                int index = Playlist.IndexOf(e);

                bool hasIt = false;
                foreach(LvItemPlaylistItem item in items)
                {
                    if(item.ParentID == e.UID)
                    {
                        hasIt = true;
                        break;
                    }
                }

                if (hasIt)
                {
                    items[index].Update(e);
                    items[index].DecodeAlbumArt(dpiX);

                    if (Playlist.Index == index && np.isPlay)
                    {
                        items[index].AlbumArtCover = Visibility.Visible;
                    }

                    Dispatcher.Invoke(lstData.Items.Refresh);
                }
                else
                {
                    Dispatcher.Invoke(new Action(UpdateList));
                }
            }
        }

        public void SetTarget(int index)
        {
            if (PlaylistIndex != index)
            {
                Updated = false;
                PlaylistIndex = index;
            }
        }

        public void UpdateList()
        {
            if (PlaylistIndex < 0 || PlaylistIndex >= np.Playlists.Count)
            {
                Logger.Log("Update Playlist Skipped by index Errored");
                return;
            }

            if (Visibility == Visibility.Hidden)
            {
                if (Updated)
                {
                    Logger.Log("Update Playlist Skipped by Hidden Errored");
                    return;
                }
            }

            Lb_Playlist_Title.Text = Playlist.Title;
            List<PlaylistItem> itemlist = Playlist.Items;

            items.Clear();

            for (int i = 0; i < itemlist.Count; i++)
            {
                if (items.Count > i)
                {
                    if (items[i].FileName != itemlist[i].FileName)
                    {
                        items[i] = new LvItemPlaylistItem(itemlist[i]);
                    }
                }
                else
                {
                    items.Add(new LvItemPlaylistItem(itemlist[i]));
                }
            }

            if (np != null && np.isPlay && np.CurrentPlaylistIndex == PlaylistIndex && np.CurrentPlaylist.Index >= 0)
            {
                items[np.CurrentPlaylist.Index].AlbumArtCover = Visibility.Visible;
            }

            lstData.Items.Refresh();

            RefreshBitmapCache();

            PresentationSource source = PresentationSource.FromVisual(lstData);

            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
            }
            else
            {
                dpiX = 1.5;
            }

            DecodeStart(Environment.ProcessorCount);

            Updated = true;
        }

        private void RefreshBitmapCache()
        {
            ScrollViewer sv = (ScrollViewer)lstData.Template.FindName("scrollViewer", lstData);
            if (sv == null)
                return;

            ScrollContentPresenter con = (ScrollContentPresenter)sv.Template.FindName("PART_ScrollContentPresenter", sv);

            con.CacheMode = null;
            con.CacheMode = new BitmapCache();
        }

        #endregion Init

        #region MultiThread

        bool DecoderRunning = false;
        int decoderCores;
        List<Thread> Decoders = new List<Thread>();
        DispatcherTimer DecodeUpdater = new DispatcherTimer();

        private void DecodeStart(int DecoderCores)
        {
            Decoders.Clear();

            if (DecoderRunning)
            {
                DecodeStop();
            }

            DecoderRunning = true;
            decoderCores = DecoderCores;
            decoderComp = 0;

            for(int i=0; i<DecoderCores; i++)
            {
                Decoders.Add(new Thread(new ParameterizedThreadStart(DecoderProc)));
                Decoders[i].IsBackground = true;
                Decoders[i].Start(i);
            }

            DecodeUpdater.Start();
        }

        private void DecodeStop()
        {
            if (DecoderRunning)
            {
                for (int i = 0; i < Decoders.Count; i++)
                {
                    if (Decoders[i].IsAlive)
                    {
                        Decoders[i].Abort();
                        Decoders[i].Join();
                    }
                }

                if (DecodeUpdater.IsEnabled)
                {
                    DecodeUpdater.Stop();
                }

                DecoderRunning = false;
            }
        }

        private void DecodeUpdater_Tick(object sender, EventArgs e)
        {
            lstData.ItemsSource = items;
            lstData.Items.Refresh();
        }

        double decoderComp = 0;
        static object decoderLocker = new object();
        private void DecoderProc(object objId)
        {
            int id = (int)objId;
            for(int i=id; i<items.Count; i += decoderCores)
            {
                items[i].DecodeAlbumArt(dpiX);
            }

            lock (decoderLocker)
            {
                decoderComp++;
            }

            if (decoderComp >= Decoders.Count)
            {
                Thread.Sleep((int)DecodeUpdater.Interval.TotalMilliseconds);
                if (DecodeUpdater.IsEnabled)
                {
                    DecodeUpdater.Stop();
                }
            }
        }

        #endregion MultiThread

        #region Undo
        private void saveUndoData()
        {
            if(undoQue.Count >= maxUndo)
            {
                undoQue.RemoveAt(0);
            }

            List<PlaylistItem> p = new List<PlaylistItem>();

            for(int i=0; i<Playlist.Items.Count; i++)
            {
                p.Add(Playlist.Items[i]);
            }

            undoQue.Add(p);
        }

        public void resetUndoData()
        {
            undoQue.Clear();
        }

        private void Undo()
        {
            if (undoQue.Count > 0 && Visibility == Visibility.Visible)
            {
                Playlist.Items = undoQue[undoQue.Count - 1];

                undoQue.RemoveAt(undoQue.Count - 1);

                Playlist.UpdateTotalTime();

                UpdateList();

                Logger.Log("Undod. Queue left " + undoQue.Count.ToString());
            }
        }

        #endregion Undo

        #region Player Events
        
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ScrollChanged != null)
            {
                ScrollChanged(sender, e);
            }
        }

        private void Story_ViewOff_Completed(object sender, EventArgs e)
        {
            if (GoBack != null)
            {
                GoBack(sender, eventArgs);
            }
        }

        public void ViewOn()
        {
            Story_ViewOn.Begin();
        }

        public void PlayerStarted()
        {
            if (PlaylistIndex == np.CurrentPlaylistIndex)
            {
                if (items.Count > Playlist.Index)
                {
                    items[Playlist.Index].AlbumArtCover = Visibility.Visible;

                    double count = Playlist.Items.Count;

                    lstData.ItemsSource = items;
                    lstData.Items.Refresh();

                    VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic, null, lstData, null);

                    double scrollHeight2 = vsp.ScrollOwner.ScrollableHeight;
                    double offsetItem = Math.Floor(vsp.ActualHeight / ItemHeight);
                    double scrollHeight = scrollHeight2 + offsetItem;

                    double offset = Math.Min(scrollHeight, Math.Max(0, scrollHeight / count * Playlist.Index - offsetItem / 2));

                    vsp.SetVerticalOffset(Math.Round(offset));
                }
            }
        }

        public void PlayerStopped()
        {
            if (Visibility == Visibility.Hidden)
            {
                Updated = false;
                return;
            }

            if (np.CurrentPlaylistIndex == PlaylistIndex)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].AlbumArtCover == Visibility.Visible)
                    {
                        items[i].AlbumArtCover = Visibility.Hidden;
                        break;
                    }
                }

                lstData.ItemsSource = items;
                lstData.Items.Refresh();
            }
        }

        #endregion Player Events

        private void Mw_Closed(object sender, EventArgs e)
        {
            DecodeStop();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            np.CurrentPlaylistIndex = PlaylistIndex;
            np.Stop();
            np.Play(lstData.SelectedIndex);
        }

        private void ListBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                np.CurrentPlaylistIndex = PlaylistIndex;
                np.Stop();
                np.Play(lstData.SelectedIndex);
            }
            else if(e.Key == Key.Back)
            {
                eventArgs = e;
                Story_ViewOff.Begin();
            }
        }

        #region ItemControl

        private void Bt_Add_Item_Click(object sender, RoutedEventArgs e)
        {
            mw.FileOpen(PlaylistIndex);
        }

        private void Bt_Delete_Click(object sender, RoutedEventArgs e)
        {
            saveUndoData();

            while (lstData.SelectedItems.Count > 0)
            {
                RemoveAt(lstData.SelectedIndex, true);
            }
        }

        private void RemoveAt(int index, bool refresh)
        {
            if (index >= 0)
            {
                Logger.Log("Remove Item: " + index.ToString());

                Playlist.Remove(index);
                items.RemoveAt(index);

                if (refresh)
                {
                    lstData.Items.Refresh();
                }

                if(items.Count == 0)
                {
                    RefreshBitmapCache();
                }
            }
        }

        private void lstData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                saveUndoData();

                while (lstData.SelectedItems.Count > 0)
                {
                    RemoveAt(lstData.SelectedIndex, true);
                }
            }
            else if(e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Undo();
            }
            else if(e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                mw.SavePlaylists();
            }
        }

        private void Bt_Go_Back_Click(object sender, RoutedEventArgs e)
        {
            eventArgs = e;

            Story_ViewOff.Begin();
        }

        #endregion ItemControl

        #region TitleChange
        bool editmode = false;
        private void Lb_Playlist_Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2 && !editmode)
            {
                Story_EditOn.Begin();
                mw.IsAllowHotkey = false;
                Tb_Playlist_Title.Text = Lb_Playlist_Title.Text;
                Tb_Playlist_Title.Visibility = Visibility.Visible;
                editmode = true;
            }
        }

        private void Tb_Playlist_Title_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (editmode)
            {
                string title = "";
                char[] arr = Tb_Playlist_Title.Text.ToCharArray();

                for(int i=0; i<arr.Length; i++)
                {
                    if (!(arr[i] == '\\' || arr[i] == '/' || arr[i] == '*' || arr[i] == ':' || arr[i] == '?' || arr[i] == '\"' || arr[i] == '<' || arr[i] == '>' || arr[i] == '|'))
                    {
                        title += arr[i];
                    }
                }

                Tb_Playlist_Title.Text = title;

                title = title.Trim().TrimEnd('.');

                for (int i=0; i<np.Playlists.Count; i++)
                {
                    if(np.Playlists[i].Title == title)
                    {
                        return;
                    }
                }
                Lb_Playlist_Title.Text = title;
                Playlist.SetTitle(title);
            }
        }

        private void Tb_Playlist_Title_LostFocus(object sender, RoutedEventArgs e)
        {
            if (editmode)
            {
                Story_EditOff.Begin();
                mw.IsAllowHotkey = true;
                Lb_Playlist_Title.Visibility = Visibility.Visible;
                editmode = false;
            }
        }

        private void Story_EditOn_Completed(object sender, EventArgs e)
        {
            Lb_Playlist_Title.Visibility = Visibility.Hidden;
        }

        private void Story_EditOff_Completed(object sender, EventArgs e)
        {
            Tb_Playlist_Title.Visibility = Visibility.Hidden;
        }

        private void Tb_Playlist_Title_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Escape)
            {
                Story_EditOff.Begin();
                Lb_Playlist_Title.Visibility = Visibility.Visible;
                mw.IsAllowHotkey = true;
                editmode = false;
            }
        }
        #endregion TitleChange

        #region ItemMenu
        private void ItemMenu_Properties_Click(object sender, RoutedEventArgs e)
        {
            MusicInfo mi = new MusicInfo(mw, ref np, PlaylistIndex, lstData.SelectedIndex);
            mi.ShowDialog();
        }

        private void ItemMenu_Remove_Click(object sender, RoutedEventArgs e)
        {
            saveUndoData();

            while (lstData.SelectedItems.Count > 0)
            {
                RemoveAt(lstData.SelectedIndex, true);
            }
        }

        private void ItemMenu_FilePath_Open_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Playlist.Items[lstData.SelectedIndex].FilePath;
            if (File.Exists(filePath))
            {
                ProcessStartInfo info = new ProcessStartInfo("explorer", "/select,"+filePath);
                info.CreateNoWindow = true;
                info.UseShellExecute = false;
                Process.Start(info);
            }
        }

        private void ItemMenu_Play_Click(object sender, RoutedEventArgs e)
        {
            np.CurrentPlaylistIndex = PlaylistIndex;
            np.Play(lstData.SelectedIndex);
        }
        #endregion ItemMenu

        #region AddMenu

        private void AddMenu_File_Click(object sender, RoutedEventArgs e)
        {
            mw.FileOpen(PlaylistIndex);
        }

        private void AddMenu_Folder_Click(object sender, RoutedEventArgs e)
        {
            mw.FolderOpen(PlaylistIndex);
        }

        private void AddMenu_Youtube_Click(object sender, RoutedEventArgs e)
        {
            mw.YoutubeOpen(PlaylistIndex);
        }

        #endregion AddMenu

        #region Drag

        Point DragStartPoint;
        private DragAdorner _adorner;
        public static BitmapSource CreateBitmapSourceFromVisual(Size size, Visual visualToRender, bool undoTransformation)
        {
            if (visualToRender == null)
            {
                return null;
            }
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height), 96, 96, PixelFormats.Pbgra32);

            if (undoTransformation)
            {
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(visualToRender);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), size));
                }
                bmp.Render(dv);
            }
            else
            {
                bmp.Render(visualToRender);
            }
            bmp.Freeze();

            return bmp;
        }

        private void InitialiseAdorner(UIElement listViewItem)
        {
            Size size = listViewItem.RenderSize;

            ImageBrush brush = new ImageBrush(CreateBitmapSourceFromVisual(size, listViewItem, true));
            brush.Freeze();

            _adorner = new DragAdorner(lstData, listViewItem.RenderSize, brush);
            _adorner.Opacity = 0.66;
            _adorner.CacheMode = new BitmapCache() { SnapsToDevicePixels = true };
            _adorner.CacheMode.Freeze();
            _adorner.IsHitTestVisible = false;

            AdornerLayer.GetAdornerLayer(this).Add(_adorner);
        }

        private void Brush_Changed(object sender, EventArgs e)
        {
            Console.WriteLine("updated");
        }

        private void lstData_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("playlistDragList") || sender == e.Source)
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void ListViewDragLeave(object sender, DragEventArgs e)
        {

        }

        private Rect BoundsRelativeTo(FrameworkElement element, Visual relativeTo)
        {
            return element.TransformToVisual(relativeTo).TransformBounds(LayoutInformation.GetLayoutSlot(element));
        }

        private void ListViewDragOver(object sender, DragEventArgs args)
        {
            if (_adorner != null)
            {
                Window w = Window.GetWindow(this);
                Point p = args.GetPosition(w);
                Rect r = DragStartBound;
                double bound = Math.Min(ItemHeight * 1.5, r.Height * 0.33);

                _adorner.OffsetLeft = p.X - ItemHeight;
                _adorner.OffsetTop = p.Y - ItemHeight / 2;
                
                if(p.Y < r.Y + bound)
                {
                    double def = r.Y + bound - p.Y;
                    double offset = vsp.VerticalOffset - Math.Max(0, Math.Min(2, (def / ItemHeight) - 1));

                    vsp.SetVerticalOffset(offset);
                }

                if(p.Y > r.Y+r.Height-bound)
                {
                    double def = p.Y - (r.Y + r.Height - bound);
                    double offset = vsp.VerticalOffset + Math.Min(2, (def / ItemHeight));

                    vsp.SetVerticalOffset(offset);
                }
            }
        }

        List<LvItemPlaylistItem> DragMoveData;
        VirtualizingStackPanel vsp;
        double DragStartOffset;
        Rect DragStartBound;
        private void lstData_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window w = Window.GetWindow(this);

            vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic, null, lstData, null);

            DragStartBound = BoundsRelativeTo(vsp, w);
            DragStartPoint = e.GetPosition(w);
            DragStartOffset = vsp.HorizontalOffset;

            DragMoveData = lstData.SelectedItems.OfType<LvItemPlaylistItem>().ToList();
            dragStart = true;
        }

        private void lstData_MouseLeave(object sender, MouseEventArgs e)
        {
            dragStart = false;
        }

        bool dragStart = false;

        private void lstData_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !mw.IsOnResizing && dragStart)
            {
                Point position = e.GetPosition(Window.GetWindow(this));

                if (Math.Abs(position.X - DragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - DragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    BeginDrag(e);
                }
            }
        }

        //https://fxmax.wordpress.com/2010/10/05/wpf/

        bool dragging = false;
        private void BeginDrag(MouseEventArgs e)
        {
            Window w = Window.GetWindow(this);
            ListView listView = lstData;
            ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

            if (listViewItem == null)
                return;

            // get the data for the ListViewItem

            //setup the drag adorner.
            mw.SetRenderUI(false);
            InitialiseAdorner(listViewItem);

            //add handles to update the adorner.
            Story_DragPanelOn.Begin();
            dragging = true;
            DragCanceled = false;
            w.Cursor = Cursors.None;
            w.PreviewDragOver += ListViewDragOver;
            listView.DragLeave += ListViewDragLeave;
            listView.DragEnter += lstData_DragEnter;

            //sort list.

            if (!DragMoveData.Contains(lstData.SelectedItem))
            {
                DragMoveData = lstData.SelectedItems.OfType<LvItemPlaylistItem>().ToList();
            }

            List<int> indexes = new List<int>();
            foreach(LvItemPlaylistItem i in DragMoveData)
            {
                indexes.Add(lstData.Items.IndexOf(i));
            }
            indexes.Sort();

            DragMoveData.Clear();

            foreach (int i in indexes)
            {
                DragMoveData.Add(items[i]);
            }

            DataObject data = new DataObject("playlistDragList", DragMoveData);
            DragDropEffects de = DragDrop.DoDragDrop(lstData, data, DragDropEffects.Move);

            Story_DragPanelOff.Begin();
            dragStart = false;
            dragging = false;
            mw.SetRenderUI(true);
            w.Cursor = Cursors.Arrow;
            w.PreviewDragOver -= ListViewDragOver;
            listView.DragLeave -= ListViewDragLeave;
            listView.DragEnter -= lstData_DragEnter;

            if (DragCanceled)
            {
                if(_adorner != null)
                {
                    Storyboard sb = new Storyboard();
                    Duration duration = new Duration(TimeSpan.FromMilliseconds(300));

                    DoubleAnimationUsingKeyFrames offsetX = new DoubleAnimationUsingKeyFrames();
                    offsetX.KeyFrames.Add(new SplineDoubleKeyFrame(_adorner.OffsetLeft, KeyTime.FromPercent(0)));
                    offsetX.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromPercent(1), new KeySpline(0, 0, 0, 1)));
                    offsetX.Duration = duration;

                    Storyboard.SetTarget(offsetX, _adorner);
                    Storyboard.SetTargetProperty(offsetX, new PropertyPath(DragAdorner.OffsetLeftProperty));

                    DoubleAnimationUsingKeyFrames offsetY = new DoubleAnimationUsingKeyFrames();
                    offsetY.KeyFrames.Add(new SplineDoubleKeyFrame(_adorner.OffsetTop, KeyTime.FromPercent(0)));
                    offsetY.KeyFrames.Add(new SplineDoubleKeyFrame(DragStartPoint.Y - ItemHeight/2, KeyTime.FromPercent(1), new KeySpline(0,0,0,1)));
                    offsetY.Duration = duration;

                    Storyboard.SetTarget(offsetY, _adorner);
                    Storyboard.SetTargetProperty(offsetY, new PropertyPath(DragAdorner.OffsetTopProperty));

                    DoubleAnimationUsingKeyFrames opacity = new DoubleAnimationUsingKeyFrames();
                    opacity.KeyFrames.Add(new SplineDoubleKeyFrame(_adorner.Opacity, KeyTime.FromPercent(0)));
                    opacity.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromPercent(1), new KeySpline(0, 0, 1, 1)));
                    opacity.Duration = duration;

                    Storyboard.SetTarget(opacity, _adorner);
                    Storyboard.SetTargetProperty(opacity, new PropertyPath(OpacityProperty));

                    sb.Children.Add(opacity);
                    sb.Children.Add(offsetX);
                    sb.Children.Add(offsetY);

                    DragAdorner ad = _adorner;

                    sb.Completed += delegate 
                    {
                        ad.Dispose();
                        AdornerLayer.GetAdornerLayer(this).Remove(ad);
                        ad = null;
                    };
                    sb.Begin();
                }
            }
            else
            {
                if (_adorner != null)
                {
                    Storyboard sb = new Storyboard();
                    Duration duration = new Duration(TimeSpan.FromMilliseconds(240));

                    DoubleAnimationUsingKeyFrames opacity = new DoubleAnimationUsingKeyFrames();
                    opacity.Duration = duration;
                    opacity.KeyFrames.Add(new SplineDoubleKeyFrame(_adorner.Opacity, KeyTime.FromPercent(0)));
                    opacity.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromPercent(1), new KeySpline(0, 0, 1, 1)));

                    Storyboard.SetTarget(opacity, _adorner);
                    Storyboard.SetTargetProperty(opacity, new PropertyPath(OpacityProperty));

                    DoubleAnimationUsingKeyFrames offsetX = new DoubleAnimationUsingKeyFrames();
                    offsetX.Duration = duration;
                    offsetX.KeyFrames.Add(new SplineDoubleKeyFrame(_adorner.OffsetTop, KeyTime.FromPercent(0)));
                    offsetX.KeyFrames.Add(new SplineDoubleKeyFrame(_adorner.OffsetTop + 18, KeyTime.FromPercent(1), new KeySpline(0, 0, 1, 0)));

                    Storyboard.SetTarget(offsetX, _adorner);
                    Storyboard.SetTargetProperty(offsetX, new PropertyPath(DragAdorner.OffsetTopProperty));

                    sb.Children.Add(offsetX);
                    sb.Children.Add(opacity);

                    DragAdorner ad = _adorner;

                    sb.Completed += delegate
                    {
                        ad.Dispose();
                        AdornerLayer.GetAdornerLayer(this).Remove(ad);
                        ad = null;
                    };
                    sb.Begin();
                }
            }
        }

        private void Story_DragPanelOff_Completed(object sender, EventArgs e)
        {
            Story_DragPanelCancelOff.Begin();
            Story_DragPanelCancelOff.SkipToFill();
            Story_DragPanelTrashOff.Begin();
            Story_DragPanelTrashOff.SkipToFill();
        }

        //remove drop
        private void Rect_Remove_Drop(object sender, DragEventArgs e)
        {
            Story_DragPanelTrashOff.Begin();

            if (e.Data.GetDataPresent("playlistDragList"))
            {
                List<LvItemPlaylistItem> movelist = e.Data.GetData("playlistDragList") as List<LvItemPlaylistItem>;

                if (movelist != null)
                {
                    saveUndoData();

                    foreach(LvItemPlaylistItem item in movelist)
                    {
                        int ind = items.IndexOf(item);

                        RemoveAt(ind, false);
                    }

                    lstData.Items.Refresh();
                }
            }
        }

        //cancel drop
        bool DragCanceled = false;
        private void Rect_Cancel_Drop(object sender, DragEventArgs e)
        {
            Story_DragPanelCancelOff.Begin();
            DragCanceled = true;
        }

        //list drop
        private void lstData_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("playlistDragList"))
            {
                List<LvItemPlaylistItem> movelist = e.Data.GetData("playlistDragList") as List<LvItemPlaylistItem>;
                ListViewItem listViewItem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if (listViewItem != null && movelist != null)
                {
                    LvItemPlaylistItem replaceItem = (LvItemPlaylistItem)lstData.ItemContainerGenerator.ItemFromContainer(listViewItem);
                    int index = lstData.Items.IndexOf(replaceItem);

                    if (index >= 0)
                    {
                        saveUndoData();

                        List<PlaylistItem> pItems = new List<PlaylistItem>();

                        int p_index = index;
                        bool skip = false;

                        foreach (LvItemPlaylistItem item in movelist)
                        {
                            int ind = items.IndexOf(item);

                            if (ind < p_index)
                            {
                                if(skip)
                                    index--;

                                skip = true;
                            }

                            PlaylistItem pi = Playlist.Items[ind];
                            pItems.Add(pi);
                        }
                        
                        int iii = 0;
                        foreach(LvItemPlaylistItem item in movelist)
                        {
                            items.Remove(item);
                            Playlist.Items.Remove(pItems[iii]);
                            iii++;
                        }
                        
                        for (int i=movelist.Count-1; i >= 0; i--)
                        {
                            items.Insert(index, movelist[i]);
                            Playlist.Items.Insert(index, pItems[i]);
                        }
                        
                        for(int i=0; i<items.Count; i++)
                        {
                            if(items[i].AlbumArtCover == Visibility.Visible)
                            {
                                Playlist.Index = i;
                                break;
                            }
                        }

                        lstData.Items.Refresh();
                    }
                }
            }
            else if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Point pt = e.GetPosition(lstData);

                VirtualizingStackPanel vsp = (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic, null, lstData, null);
                
                int ind = (int)Math.Floor((pt.Y - 37) / ItemHeight) + (int)vsp.VerticalOffset;

                if (files != null && files.Length > 0)
                {
                    if (ind < np.Playlists[this.PlaylistIndex].Items.Count && ind > -1)
                    {
                        saveUndoData();

                        //TODO: LOADING DIALOG
                        foreach(string path in files)
                        {
                            try
                            {
                                if (PlayerCore.IsSupport(path))
                                {
                                    np.Playlists[this.PlaylistIndex].Insert(ind, new FileItem(path));
                                }
                            }
                            catch
                            {

                            }
                        }

                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].AlbumArtCover == Visibility.Visible)
                            {
                                np.Playlists[this.PlaylistIndex].Index = i;
                                break;
                            }
                        }

                        UpdateList();
                    }
                    else if (ind > -1)
                    {
                        saveUndoData();

                        //TODO: LOADING DIALOG
                        foreach (string path in files)
                        {
                            try
                            {
                                if (PlayerCore.IsSupport(path))
                                {
                                    np.Playlists[this.PlaylistIndex].Add(new FileItem(path));
                                }
                            }
                            catch
                            {

                            }
                        }

                        UpdateList();
                    }
                }
            }
        }

        // Helper to search up the VisualTree
        private static T FindAnchestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        #endregion Drag
    }
}
