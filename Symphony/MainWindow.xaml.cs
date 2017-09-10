using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Markup;
using System.Linq;
using System.IO.Pipes;

using Symphony.Dancer;
using Symphony.DSP;
using Symphony.Lyrics;
using Symphony.Server;
using Symphony.UI;
using Symphony.Util;
using Symphony.Player;
using Symphony.Player.DSP;
using Symphony.Player.DSP.CSCore;
using System.Threading.Tasks;

namespace Symphony
{
    public enum WindowMode
    {
        Big = 0,
        Mid = 1,
        Small = 2
    }

    public partial class MainWindow : Window, IDisposable
    {
        OpenFileDialog ofd;
        System.Windows.Forms.FolderBrowserDialog folderBrowser;

        int ofd_Target = 0;

        System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
        System.Timers.Timer systemCounter;

        Storyboard ImageOn;
        Storyboard ImageOff;
        Storyboard MidOn;
        Storyboard MidOff;
        Storyboard SmallOn;
        Storyboard ShadowOff;
        Storyboard ShadowOn;
        Storyboard MovieOn;
        Storyboard MovieOff;

        BitmapImage Img_AlbumArt;
        BitmapImage Img_Play;
        BitmapImage Img_Pause;
        BitmapImage Img_Bar_Order_Once;
        BitmapImage Img_Bar_Order_Random;
        BitmapImage Img_Bar_Order_Repeat;
        BitmapImage Img_Bar_Order_RepeatOne;

        ImageBrush Brush_Play;
        ImageBrush Brush_Pause;
        ImageBrush Brush_AlbumArt;
        ImageBrush Brush_AlbumArt_LowQ;

        WindowMode _windowMode = WindowMode.Big;
        public event EventHandler WindowModeChanged;
        public WindowMode WindowMode
        {
            get
            {
                return _windowMode;
            }
            private set
            {
                _windowMode = value;

                WindowModeChanged?.Invoke(this, null);
            }
        }

        Window DummyToolWindow;
        SpectrumAnalysisVisualization specVisualizer;
        VuVisualization vuVisualizer;
        DebugOsiloscopeVisualization debugOsVisualizer;
        OsiloscopeVisualization osVisualizer;

        int calcFrame = 0;
        int fps = 0;
        
        double dpiX = 1.0;
        double dpiY = 1.0;

        public event EventHandler<bool> UpdateAllowChanged;
        public event EventHandler<bool> MovingStateChanged;

        bool windowModeChanging = false;
        bool IsShadowOn = false;
        bool needUiResize = false;
        bool _isMoving = false;
        public bool isMoving
        {
            get
            {
                return _isMoving;
            }
            set
            {
                _isMoving = value;

                MovingStateChanged?.Invoke(this, value);
            }
        }
        bool _isFrameUpdateAllowed = true;
        public bool isFrameUpadteAllowed
        {
            get
            {
                return _isFrameUpdateAllowed;
            }
            set
            {
                if (_isFrameUpdateAllowed != value)
                {
                    _isFrameUpdateAllowed = value;

                    UpdateAllowChanged?.Invoke(this, value);
                }
            }
        }
        bool _isAllowHotkey = true;
        public bool IsAllowHotkey
        {
            get
            {
                return _isAllowHotkey;
            }
            set
            {
                _isAllowHotkey = value;
            }
        }

        public bool IsOnResizing
        {
            get
            {
                return shadowWindow.IsOnResizing;
            }
        }

        private bool _debugger = false;
        public bool Debugger
        {
            get
            {
                return _debugger;
            }
            set
            {
                if(_debugger != value)
                {
                    if (value)
                    {
                        debugOsVisualizer.SetOn(true);
                    }
                    else
                    {
                        debugOsVisualizer.SetOn(false);
                    }
                }
                _debugger = value;
            }
        }

        public static string versionText = "Symphony Beta 3.1.16";

        Settings setting => settingListener.Settings;
        SettingListener settingListener;
        ShadowWindow shadowWindow;
        PlayerCore np;

        bool StopUpdateWhenScrollPlaylist = true;
        bool isDebug;
        bool showFpsLogger = false;

        public MainWindow(SplashWindow splash)
        {
            clock.Start();
            
            #if DEBUG
            isDebug = true;
#else
            isDebug = false;
#endif

            splash.Update("프로그램 시작 중");

            Logger.Info (" ####  #     # #     # ####    ##   #    # #     # ");
            Logger.Info ("#       #   #  ##   ## #   #  #  #  ##   #  #   #  ");
            Logger.Info (" ####    # #   ##   ## #   # #    # # #  #   # #   ");
            Logger.Info ("     #    #    # # # # ####  #    # #  # #    #    ");
            Logger.Info ("     #    #    # # # # #      #  #  #   ##    #    ");
            Logger.Info ("#####     #    #  #  # #       ##   #    #    #    ");
            Logger.Info ("                 Since 2014/3/10~                  ");
            Logger.Error("          Made by AinL, MattMatt, Jinkwan          ");
            Logger.Info ("          b                 A       333            ");
            Logger.Info ("         b             t   AA          3           ");
            Logger.Info ("         bBb    EEE  tTTt A aA     3333            ");
            Logger.Info ("        B   b  EeeeE  T  AaaaA        3            ");
            Logger.Info ("        bBBb____Eee__tT_a_____a___3333             ");

            Logger.Info("Rendering Tier : " + RenderCapability.Tier.ToString());

            splash.Update("UI 초기화");

            InitializeComponent();
            
            MidOn = FindResource("MidOn") as Storyboard;
            MidOff = FindResource("MidOff") as Storyboard;
            SmallOn = FindResource("SmallOn") as Storyboard;
            ImageOn = FindResource("ImageOn") as Storyboard;
            ImageOff = FindResource("ImageOff") as Storyboard;
            ShadowOff = FindResource("ShadowOff") as Storyboard;
            ShadowOn = FindResource("ShadowOn") as Storyboard;
            MovieOn = FindResource("MovieOn") as Storyboard;
            MovieOff = FindResource("MovieOff") as Storyboard;

            MidOn.Completed += MidOn_Completed;
            MidOff.Completed += MidOff_Completed;
            SmallOn.Completed += SmallOn_Completed;
            ImageOff.Completed += ImageOff_Completed;
            MovieOff.Completed += MovieOff_Completed;
            MovieOn.Completed += MovieOn_Completed;
            
            shadowWindow = new ShadowWindow(this, this);
            shadowWindow.IgnoreFreezeMainWindow = true;
            shadowWindow.MouseLeftButtonDown_Custom += ShadowWindow_MouseLeftButtonDown;

            splash.Update("플레이어 초기화");

            np = new PlayerCore(isDebug);

            np.PlayStarted += Np_PlayStarted;
            np.PlayStarted += PlaylistViewer.PlayStart;
            np.PlayStarted += PlaylistItemViewer.PlayerStarted;
            np.PlayStopped += Np_PlayStopped;
            np.PlayStopped += PlaylistViewer.PlayStopped;
            np.PlayStopped += PlaylistItemViewer.PlayerStopped;
            np.PlayResumed += Np_PlayResumed;
            np.PlayPaused += Np_PlayPaused;
            np.FileOpenFaild += Np_FileOpenFaild;
            np.FormatUnknown += Np_FormatUnknown;
            np.WaveformReady += Sld_Big_Position.WaveformReady;
            
            //init visualizers
            specVisualizer = new SpectrumAnalysisVisualization(np);
            vuVisualizer = new VuVisualization(grid2, np);
            debugOsVisualizer = new DebugOsiloscopeVisualization();
            osVisualizer = new OsiloscopeVisualization();

            splash.Update("설정 로드 중");
            LoadSettings();

            splash.Update("UI 설정 중");

            PlaylistViewer.setTarget(ref np);
            PlaylistViewer.ListBoxItemDoubleClicked += ListBoxItemDoubleClicked;
            PlaylistItemViewer.SetTarget(np.CurrentPlaylistIndex);
            PlaylistItemViewer.GoBack += GoBack;
            PlaylistItemViewer.ScrollChanged += ScrollChanged;
            PlaylistViewer.updateList();

            ofd = new OpenFileDialog();
            ofd.Filter = PlayerCore.FileFilter;
            ofd.Title = LanguageHelper.FindText("Lang_File_Open");
            ofd.Multiselect = true;
            ofd.FileOk += Ofd_FileOk;
            LanguageHelper.LangaugeChanged += delegate (object sen, EventArgs arg)
            {
                ofd.Title = LanguageHelper.FindText("Lang_File_Open");
                ofd.Filter = PlayerCore.FileFilter;
            };

            //LANGSUP
            folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.ShowNewFolderButton = false;
            folderBrowser.Description = "폴더 추가";

            systemCounter = new System.Timers.Timer(1000);
            systemCounter.Elapsed += SystemCounter_Elapsed;

            Init_VU();
            Init_Visualizer();

            VisualParent.Init(this);

            VisualParent.Visualizers.Add(specVisualizer);
            VisualParent.Visualizers.Add(osVisualizer);
            VisualParent.Visualizers.Add(debugOsVisualizer);
            VisualParent.Visualizers.Add(vuVisualizer);

            splash.Update("플레이 리스트 읽는 중");

            LoadPlaylists(splash);

            PlaylistItemViewer.Init(ref np, this);
            PlaylistViewer.Init(this, ref PlaylistItemViewer);

            splash.Update("리소스 준비 중");

            Img_Pause = FindResource("Img_Pause") as BitmapImage;
            Img_Play = FindResource("Img_Play") as BitmapImage;
            Img_Bar_Order_Once = FindResource("Img_Bar_Order_Once") as BitmapImage;
            Img_Bar_Order_Random = FindResource("Img_Bar_Order_Random") as BitmapImage;
            Img_Bar_Order_Repeat = FindResource("Img_Bar_Order_Repeat") as BitmapImage;
            Img_Bar_Order_RepeatOne = FindResource("Img_Bar_Order_RepeatOne") as BitmapImage;

            Brush_Play = new ImageBrush(Img_Play);
            Brush_Pause = new ImageBrush(Img_Pause);

            Brush_Play.Freeze();
            Brush_Pause.Freeze();

            Img_Pause.Freeze();
            Img_Play.Freeze();
            Img_Bar_Order_Once.Freeze();
            Img_Bar_Order_Random.Freeze();
            Img_Bar_Order_Repeat.Freeze();
            Img_Bar_Order_RepeatOne.Freeze();

            Sld_Footer_Volume.ValueChanged += Sld_Footer_Volume_ValueChanged;
            Sld_Big_Position.ValueChanged += Sld_Big_Position_ValueChanged;

            Bar_Next.Click += Bar_Next_Click;
            Bar_Play.Click += Bar_Play_Click;
            Bar_Previous.Click += Bar_Previous_Click;
            Bar_Order.Click += Bar_Order_Click;
            Bar_Stop.Click += Bar_Stop_Click;

            RenderOptions.SetBitmapScalingMode(Img_Back_AlbumArt, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetBitmapScalingMode(Img_Big_AlbumArt, BitmapScalingMode.HighQuality);
            RenderOptions.SetBitmapScalingMode(Img_Small_AlbumArt, BitmapScalingMode.HighQuality);

            CompositionTarget.Rendering += Renderer_Tick;

            PlaylistItemViewer.UpdateList();

            systemCounter.Start();
            Reset_Meta(false);

            Loaded += MainWindow_Loaded;
            WindowModeChanged += MainWindow_WindowModeChanged;

            Logger.Log("Initial Time: " + clock.ElapsedMilliseconds.ToString("0,0") + "ms");
        }

        #region Window Controls

        private void MainWindow_WindowModeChanged(object sender, EventArgs e)
        {
            UpdateVisualParentAllowRender();
        }

        private void Load_StateChanged(object sender, EventArgs e)
        {
            UpdateVisualParentAllowRender();
        }

        private void UpdateVisualParentAllowRender()
        {
            if(WindowState == WindowState.Minimized || WindowMode == WindowMode.Small)
            {
                VisualParent.AllowRender = false;
            }
            else
            {
                VisualParent.AllowRender = true;
            }
        }

        DispatcherTimer stateHandler;
        bool NowStateChanging = false;
        private void StateChanging(WindowState ws)
        {
            NowStateChanging = true;
            shadowWindow.AllowUpdate = false;

            if (stateHandler == null)
            {
                stateHandler = new DispatcherTimer();
                stateHandler.Tick += StateHandler_Tick;
                stateHandler.Interval += TimeSpan.FromMilliseconds(400);
            }

            if (stateHandler.IsEnabled)
            {
                stateHandler.Stop();
            }
            stateHandler.Start();

            WindowState = ws;
        }

        private void StateHandler_Tick(object sender, EventArgs e)
        {
            NowStateChanging = false;
            stateHandler.Stop();

            if (shadowWindow != null)
            {
                shadowWindow.Update(true);
                shadowWindow.AllowUpdate = true;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(sender as Grid);

            if (pt.Y < 40)
            {
                if (e.ClickCount >= 2 && WindowMode!=WindowMode.Small)
                {
                    if (WindowState == WindowState.Maximized)
                    {
                        StateChanging(WindowState.Normal);
                    }
                    else
                    {
                        StateChanging(WindowState.Maximized);
                    }
                }
                else if(e.ClickCount == 1)
                {
                    isMoving = true;
                    isFrameUpadteAllowed = false;
                    DragMove();
                }
            }
        }

        private void Load_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isFrameUpadteAllowed = true;
        }

        private void Bt_Title_Exit_Click(object sender, RoutedEventArgs e)
        {
            np.Stop();
            this.Close();
        }

        private void Bt_Title_Max_Click(object sender, RoutedEventArgs e)
        {
            if (WindowMode != WindowMode.Small)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    StateChanging(WindowState.Normal);
                }
                else
                {
                    StateChanging(WindowState.Maximized);
                }
            }
        }

        private void Bt_Title_Mini_Click(object sender, RoutedEventArgs e)
        {
            StateChanging(WindowState.Minimized);
        }

        private void ShadowWindow_MouseLeftButtonDown(object sender, MouseLeftDownArgs e)
        {
            Point pt = Mouse.GetPosition(Grid_Background);

            if ((pt.X < 0 || pt.X >= Grid_Background.ActualWidth
                || pt.Y < 0 || pt.Y >= Grid_Background.ActualHeight) 
                && !windowModeChanging && !isEditor && WindowState != WindowState.Maximized)
            {
                if(e.ClickCount >= 2)
                {
                    MovieMode(false);
                }

                if (e.ClickCount >= 2 && shadowWindow.wr.GetStatus() == "right")
                {
                    if (WindowMode == WindowMode.Big)
                    {
                        MidOn.Begin();
                        shadowWindow.AllowUpdate = false;
                        WindowMode = WindowMode.Mid;
                        windowModeChanging = true;
                    }
                    else if (WindowMode == WindowMode.Mid || WindowMode == WindowMode.Small)
                    {
                        MidOff.Begin();
                        shadowWindow.AllowUpdate = false;
                        WindowMode = WindowMode.Big;
                        windowModeChanging = true;
                    }
                }
                else if (e.ClickCount >= 2 && shadowWindow.wr.GetStatus() == "bottom")
                {
                    if(WindowMode == WindowMode.Mid)
                    {
                        SmallOn.Begin();
                        shadowWindow.AllowUpdate = false;
                        WindowMode = WindowMode.Small;
                        windowModeChanging = true;
                    }else if(WindowMode == WindowMode.Small)
                    {
                        MidOn.Begin();
                        shadowWindow.AllowUpdate = false;
                        WindowMode = WindowMode.Mid;
                        windowModeChanging = true;
                    }
                }
                else if(e.ClickCount >= 2 && shadowWindow.wr.GetStatus() == "bottomRight")
                {
                    if (WindowMode == WindowMode.Big)
                    {
                        SmallOn.Begin();
                        shadowWindow.AllowUpdate = false;
                        WindowMode = WindowMode.Small;
                        windowModeChanging = true;
                    }
                    else
                    {
                        MidOff.Begin();
                        shadowWindow.AllowUpdate = false;
                        WindowMode = WindowMode.Big;
                        windowModeChanging = true;
                    }
                }
            }
        }

        private void MidOn_Completed(object sender, EventArgs e)
        {
            windowModeChanging = false;
            shadowWindow.AllowUpdate = true;
        }

        private void MidOff_Completed(object sender, EventArgs e)
        {
            windowModeChanging = false;
            shadowWindow.AllowUpdate = true;
        }

        private void SmallOn_Completed(object sender, EventArgs e)
        {
            windowModeChanging = false;
            shadowWindow.AllowUpdate = true;
        }

        private void Load_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(Grid_Background);

            if(pt.X > 0 && pt.X < Grid_Background.ActualWidth - 85 && pt.Y < 36 && pt.Y > 0 && IsMovieMode)
            {
                if(e.ClickCount >= 2)
                {
                    if(WindowState == WindowState.Maximized)
                    {
                        StateChanging(WindowState.Normal);
                    }
                    else
                    {
                        StateChanging(WindowState.Maximized);
                    }
                }
                else
                {
                    isFrameUpadteAllowed = false;
                    isMoving = true;
                    DragMove();
                }
            }
        }

        bool windowMaximized = false;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                if (WindowMode == WindowMode.Small)
                {
                    StateChanging(WindowState.Normal);
                }
                else
                {
                    Grid_Background.Margin = new Thickness(7);

                    windowMaximized = true;
                }
            }
            else
            {
                if (windowMaximized)
                {
                    Grid_Background.Margin = new Thickness(0);
                    windowMaximized = false;
                }
            }

            needUiResize = true;
        }

        bool IsMovieMode = false;
        DispatcherTimer movieUpdater;

        private void MovieMode(bool on)
        {
            if (WindowMode != WindowMode.Big || IsMovieMode == on)
                return;

            IsMovieMode = on;

            if (movieUpdater == null)
            {
                movieUpdater = new DispatcherTimer();
                movieUpdater.Tick += MovieUpdater_Tick;
                movieUpdater.Interval = TimeSpan.FromMilliseconds(1000/40);
            }

            if (on)
            {
                MovieOff.Stop();
                MovieOn.Begin();
            }
            else
            {
                MovieOn.Stop();
                MovieOff.Begin();
            }
            movieUpdater.Start();
        }

        private void MovieUpdater_Tick(object sender, EventArgs e)
        {
            VU_Opacity = VU_Opacity;
        }

        private void MovieOn_Completed(object sender, EventArgs e)
        {
            if (movieUpdater != null && movieUpdater.IsEnabled)
            {
                movieUpdater.Stop();
            }
            VU_Opacity = VU_Opacity;
        }

        private void MovieOff_Completed(object sender, EventArgs e)
        {
            if (movieUpdater != null && movieUpdater.IsEnabled)
            {
                movieUpdater.Stop();
            }
            VU_Opacity = VU_Opacity;
        }

        #endregion

        #region Player Control

        private void Load_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HotKeyDown(sender, e);
        }

        public void HotKeyDown(object sender, KeyEventArgs e)
        {
            if (isDebug)
            {
                switch (e.Key)
                {
                    default:
                        break;
                }
            }

            if (IsAllowHotkey)
            {
                switch (e.Key)
                {
                    case Key.Space:
                        e.Handled = true;
                        if (np != null && np.isPlay)
                        {
                            np.PlayPause();
                        }
                        break;
                    case Key.Left:
                        e.Handled = true;
                        if (np.isPlay)
                        {
                            np.SetPosition((int)np.GetPosition(TimeUnit.MilliSecond) - 3000);
                        }
                        break;
                    case Key.Right:
                        e.Handled = true;
                        if (np.isPlay)
                        {
                            np.SetPosition((int)np.GetPosition(TimeUnit.MilliSecond) + 3000);
                        }
                        break;
                    case Key.Up:
                        e.Handled = true;
                        Sld_Footer_Volume.Value = Math.Min(100, Sld_Footer_Volume.Value + 3);
                        UpdateVolume();
                        break;
                    case Key.Down:
                        e.Handled = true;
                        Sld_Footer_Volume.Value = Math.Max(0, Sld_Footer_Volume.Value - 3);
                        UpdateVolume();
                        break;
                    case Key.PageUp:
                        e.Handled = true;
                        np.Previous();
                        update_play();
                        break;
                    case Key.PageDown:
                        e.Handled = true;
                        np.Next();
                        update_play();
                        break;
                    case Key.F9:
                        if (!e.IsRepeat)
                        {
                            PlayerMiniControlShow = !PlayerMiniControlShow;
                            e.Handled = true;
                        }
                        break;
                    case Key.F11:
                        e.Handled = true;
                        MovieMode(!IsMovieMode);
                        break;
                    case Key.F12:
                        e.Handled = true;
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            DialogTextResult r = new DialogText(this, "debug input\ndebugOsiloLength", "").ShowDialog();
                            if (r.Okay)
                            {
                                switch (r.Text)
                                {
                                    case "debugOsiloLength":
                                        r = new DialogText(this, "get length (ms)", "").ShowDialog();
                                        if (r.Okay)
                                        {
                                            try
                                            {
                                                debugOsVisualizer.ViewerSize = (int)Convert.ToDouble(r.Text);
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            Debugger = !Debugger;
                        }
                        break;
                }
            }
        }

        public void FileOpen(int targetIndex)
        {
            isFrameUpadteAllowed = false;

            ofd_Target = targetIndex;
            ofd.ShowDialog(this);

            isFrameUpadteAllowed = true;
        }

        public void FolderOpen(int targetIndex)
        {
            isFrameUpadteAllowed = false;
            
            if(System.Windows.Forms.DialogResult.OK == folderBrowser.ShowDialog())
            {
                string targetDir = folderBrowser.SelectedPath;

                if (Directory.Exists(targetDir))
                {
                    List<string> list = PlayerCore.FolderSearch(new DirectoryInfo(targetDir));

                    if (list != null && list.Count > 0)
                    {
                        MusicLoading ml = new MusicLoading(this, np.Playlists[targetIndex], list.ToArray());

                        ml.Closed += Ml_Closed;

                        ml.Show();

                        ml.Worker.RunWorkerAsync();
                    }
                }
            }

            isFrameUpadteAllowed = true;
        }

        public void YoutubeOpen(int ofdTarget)
        {
            DialogText text = new DialogText(this, "유튜브 주소를 입력해주세요", "");

            DialogTextResult result = text.ShowDialog();

            if (result.Okay)
            {
                if(result.Text != null)
                {
                    try
                    {
                        YoutubeItem item = new YoutubeItem(result.Text);
                        np.Playlists[ofdTarget].Add(item);
                    }
                    catch(Exception ex)
                    {
                        DialogMessage.Show(this, "유튜브 주소가 아닌것같습니다\n"+ex.ToString(), "오류", DialogMessageType.Okay);
                    }

                    PlaylistViewer.updateList();
                    PlaylistItemViewer.Updated = false;
                    PlaylistItemViewer.UpdateList();
                }
            }
        }

        private void Bt_controls_open_Click(object sender, RoutedEventArgs e)
        {
            if (np.CurrentPlaylistIndex > -1)
            {
                FileOpen(np.CurrentPlaylistIndex);
            }
        }

        private void Ofd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MusicLoading ml = new MusicLoading(this, np.Playlists[ofd_Target], ofd.FileNames);
            ml.Closed += Ml_Closed;
            ml.Show();
            ml.Worker.RunWorkerAsync();
        }

        private void Ml_Closed(object sender, EventArgs e)
        {
            PlaylistViewer.updateList();
            PlaylistItemViewer.Updated = false;
            PlaylistItemViewer.UpdateList();
        }

        private void update_play()
        {
            if (np.isPlay)
            {
                if (np.isPaused)
                {
                    Bt_controls_play.Background = Brush_Play;
                    Bar_Play.ImageSource = Img_Play;
                }
                else
                {
                    Bar_Play.ImageSource = Img_Pause;
                    Bt_controls_play.Background = Brush_Pause;
                }
            }
            else
            {
                Bar_Play.ImageSource = Img_Play;
                Bt_controls_play.Background = Brush_Play;
            }
        }

        private void update_order(bool updateOrderControl = true)
        {
            if (np.CurrentPlaylist != null)
            {
                PlaylistOrder order = np.CurrentPlaylist.Order;

                if (miniControlWindow != null)
                    miniControlWindow.UpdateOrder(order);

                if(updateOrderControl)
                    Bt_Small_Order.PlaylistOrder = order;

                if (order == PlaylistOrder.Once)
                {
                    Bar_Order.ImageSource = Img_Bar_Order_Once;
                    Bar_Order.Description = LanguageHelper.FindText("Lang_Player_Repeat_PlayOnceAll");
                }
                else if (order == PlaylistOrder.Repeat)
                {
                    Bar_Order.ImageSource = Img_Bar_Order_Repeat;
                    Bar_Order.Description = LanguageHelper.FindText("Lang_Player_Repeat_RepeatAll");
                }
                else if (order == PlaylistOrder.RepeatOne)
                {
                    Bar_Order.ImageSource = Img_Bar_Order_RepeatOne;
                    Bar_Order.Description = LanguageHelper.FindText("Lang_Player_Repeat_RepeatOne");
                }
                else if (order == PlaylistOrder.Random)
                {
                    Bar_Order.ImageSource = Img_Bar_Order_Random;
                    Bar_Order.Description = LanguageHelper.FindText("Lang_Player_Repeat_Random");
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
        
        private void Bt_Small_Order_Changed(object sender, PlaylistOrder e)
        {
            if (np.CurrentPlaylist != null)
            {
                np.CurrentPlaylist.Order = e;
                update_order(false);
            }
        }

        private void Bt_controls_previous_Click(object sender, RoutedEventArgs e)
        {
            np.Previous();
            update_play();
        }

        private void Bt_controls_stop_Click(object sender, RoutedEventArgs e)
        {
            np.Stop();
        }

        private void Bt_controls_play_Click(object sender, RoutedEventArgs e)
        {
            if (np.isPlay)
            {
                np.PlayPause();
            }
            else
            {
                np.Next();
            }
            update_play();
        }

        private void Bt_controls_next_Click(object sender, RoutedEventArgs e)
        {
            np.Next();
            update_play();
        }

        bool isSettingOpened = false;
        SettingWindow sw;
        private void Bt_controls_setting_Click(object sender, RoutedEventArgs e)
        {
            if(!isSettingOpened)
            {
                sw = new SettingWindow(this, ref np);
                isSettingOpened = true;
                sw.Closed += Sw_Closed;
                sw.Show();
            }
            else
            {
                sw.Topmost = true;
                sw.Topmost = false;
                sw.Focus();
            }
        }

        private void Sw_Closed(object sender, EventArgs e)
        {
            isSettingOpened = false;

            SaveSettings();

            sw = null;
        }

        private void Bar_Order_Click(object sender, EventArgs e)
        {
            Bt_Small_Order.Next();
        }

        bool barAvailable = true;

        private void Bar_Previous_Click(object sender, EventArgs e)
        {
            if (barAvailable)
            {
                barAvailable = false;
                np.Previous();
                update_play();
            }
        }

        private void Bar_Play_Click(object sender, EventArgs e)
        {
            if (np.isPlay)
            {
                np.PlayPause();
            }
            else
            {
                np.Next();
            }
            update_play();
        }

        private void Bar_Next_Click(object sender, EventArgs e)
        {
            if (barAvailable)
            {
                Logger.Log("Taskbar button Next Clicked");
                barAvailable = false;
                np.Next();
                update_play();
            }
        }

        private void Bar_Stop_Click(object sender, EventArgs e)
        {
            np.Stop();
        }

        #endregion

        #region PlaylistControl

        double scrollStart = 0;
        double scrollStopDuration = 100;

        public void UpdateList()
        {
            PlaylistViewer.updateList();
            PlaylistItemViewer.UpdateList();
        }

        private void ListBoxItemDoubleClicked(object sender, IndexChangedArgs e)
        {
            PlaylistItemViewer.SetTarget(e.index);
            PlaylistItemViewer.resetUndoData();
            PlaylistItemViewer.UpdateList();
            PlaylistItemViewer.ViewOn();
            PlaylistItemViewer.Visibility = Visibility.Visible;
            PlaylistViewer.Visibility = Visibility.Hidden;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            PlaylistViewer.updateList();
            PlaylistItemViewer.Visibility = Visibility.Hidden;
            PlaylistViewer.ViewOn();
            PlaylistViewer.Visibility = Visibility.Visible;
        }

        public void AddPlaylist(string title, string[] Files)
        {
            np.Playlists.Add(new Playlist(title));

            if(np.Playlists.Count == 1)
            {
                np.CurrentPlaylistIndex = 0;
            }

            MusicLoading ml = new MusicLoading(this, np.Playlists[np.Playlists.Count-1], Files);
            ml.Closed += Ml_Closed;
            ml.Show();
            ml.Worker.RunWorkerAsync();
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            StopUpdateWhenScrollPlaylist = true;
            scrollStart = clock.ElapsedMilliseconds;
        }

        #endregion PlaylistControl

        #region Playback Event

        private void Np_FormatUnknown(string msg)
        {
            Dispatcher.Invoke(new Action(() => { UI.DialogMessage.Show(this, msg, LanguageHelper.FindText("Lang_Error"), DialogMessageType.Okay); }));
        }

        private void Np_FileOpenFaild(string msg)
        {
            Dispatcher.Invoke(new Action(() => { UI.DialogMessage.Show(this, msg, LanguageHelper.FindText("Lang_Error"), DialogMessageType.Okay); }));
        }

        private void Np_PlayPaused()
        {
            update_play();
        }

        private void Np_PlayResumed()
        {
            update_play();
        }

        private void Np_PlayStopped()
        {
            if (!isEditor)
            {
                SearchMetadataStop();
            }

            if (UpdateMetaThread != null)
            {
                UpdateMetaThread.Abort();
                UpdateMetaThread = null;
            }

            ImageOffBegin();

            Reset_Meta(true);

            update_play();
        }

        Thread UpdateMetaThread;
        private void Np_PlayStarted()
        {
            VisualParent.InitSample(AudioDesiredLantency, GUIUpdate, np.DSPMaster);

            if (!isEditor)
            {
                SearchMetadataStart();
                np.DSPMaster.SampleEnd += VisualParent.Sample;
            }

            if (specVisualizer.SpectrumAnalysis != null)
            {
                specVisualizer.SpectrumAnalysis.MaximumFrequency = SpecMaxFreq;
                specVisualizer.SpectrumAnalysis.MinimumFrequency = SpecMinFreq;
                specVisualizer.SpectrumAnalysis.IsXLogScale = SpecUseLogScale;
                specVisualizer.SpectrumAnalysis.UseResampling = SpecUseResampler;
                specVisualizer.SpectrumAnalysis.ResamplingMode = SpecResampleMode;
                specVisualizer.SpectrumAnalysis.ScalingStrategy = Spec_ScalingMode;
            }

            barAvailable = true;
            
            update_order();
            update_play();

            if(UpdateMetaThread != null)
            {
                UpdateMetaThread.Abort();
                UpdateMetaThread = null;
            }

            UpdateMetaThread = Update_Meta_Async();
        }

        private void Sld_Small_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            np.SetPosition((int)e.NewValue);
        }

        private void Sld_Big_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            np.SetPosition((int)e.NewValue);
        }

        private void Sld_Footer_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if ((int)e.NewValue <= 0)
            {
                if (np != null)
                {
                    np.SetVolume(0);
                }
                Lb_Footer_Volume.Text = LanguageHelper.FindText("Lang_Player_Mute");
            }
            else {
                if (np != null)
                {
                    np.SetVolume((float)(Math.Pow(10, (float)e.NewValue / 100f) / 10 - 0.1) * 1.1111111f);
                }
                Lb_Footer_Volume.Text = ((int)e.NewValue).ToString() + "%";
            }
        }

        #endregion Playback Event

        #region Rendering

        int calc_ui_fps = 0;
        bool vz_ready = false;
        double renderer_pre = 0;
        double stemp_UI = 0;
        int calc_os_frame = 0;
        int calc_vu_frame = 0;
        double ui_rendering = 0;
        double renderer_rendering = 0;
        double sum_renderer = 0;

        public void SetRenderUI(bool Allow)
        {
            isFrameUpadteAllowed = Allow;
        }

        public void StopRenderingWhileClicking()
        {
            SetRenderUI(false);

            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromMilliseconds(10);
            t.Tick += (object sender1, EventArgs e1) =>
            {
                if (Mouse.LeftButton == MouseButtonState.Released)
                {
                    SetRenderUI(true);
                    ((DispatcherTimer)sender1).Stop();
                }
            };
            t.Start();
        }

        private void Renderer_Tick(object sender, EventArgs e)
        {
            if (WindowState != WindowState.Minimized && !isEditor)
            {
                renderer_rendering = Math.Max(renderer_rendering, clock.ElapsedTicks - renderer_pre);
                sum_renderer += clock.ElapsedTicks - renderer_pre;
                renderer_pre = clock.ElapsedTicks;

                double renderStart = clock.ElapsedTicks;

                //Start Rendering

                if (StopUpdateWhenScrollPlaylist)
                {
                    if (scrollStart > clock.ElapsedMilliseconds - scrollStopDuration)
                    {
                        isFrameUpadteAllowed = false;
                    }
                    else
                    {
                        StopUpdateWhenScrollPlaylist = false;
                        isFrameUpadteAllowed = true;
                    }
                }

                if (!NowStateChanging && isFrameUpadteAllowed && !isMoving && stemp_UI < clock.ElapsedMilliseconds - GUIUpdate)
                {
                    stemp_UI = clock.ElapsedMilliseconds;

                    if (needUiResize)
                    {
                        q_index = 0;
                        needUiResize = false;
                    }

                    if (WindowMode != WindowMode.Small)
                    {
                        VisualParent.Update();
                    }

                    if (np.isPlay)
                    {
                        double len = np.GetLength(TimeUnit.MilliSecond);
                        double pos = np.GetPosition(TimeUnit.MilliSecond);
                        Sld_Small_Position.Maximum = len;
                        Sld_Small_Position.Value = pos;
                        Sld_Big_Position.Maximum = len;
                        Sld_Big_Position.Value = pos;
                        Lb_Small_Position.Text = np.GetPosition(@"hh\:mm\:ss");
                    }
                    calc_ui_fps++;
                    calc_os_frame++;
                }
                else if(isMoving)
                {
                    if(Mouse.LeftButton == MouseButtonState.Released)
                    {
                        isMoving = false;
                    }
                }

                ui_rendering = Math.Max(ui_rendering, clock.ElapsedTicks - renderStart);
                calcFrame++;
            }
        }

        private void SystemCounter_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            fps = calcFrame;
            ui_fps = calc_ui_fps;

            calcFrame = 0;
            calc_ui_fps = 0;

            if (calc_vu_frame != 0)
            {
                calc_vu_frame = 0;
                calc_os_frame = 0;
            }

            if (isDebug && showFpsLogger)
            {
                fps_msg = "WPF Thread: " + fps.ToString()
                       + " | WPF Time: " + renderer_rendering.ToString("0,0.0") + "Ticks, " + (renderer_rendering / 1000).ToString("0,0.0") + "ms"
                       + " | WPFDrawFPS: " + ((int)(1000000 / renderer_rendering)).ToString("0,0.0")
                       + " | WPFDrawAvgFPS: " + ((int)(1000000 / (sum_renderer / fps))).ToString("0,0.0")
                       + " | UI Tread: " + ui_fps.ToString()
                       + " | UI Time: " + ui_rendering.ToString("0,0.0") + "Ticks, " + (ui_rendering / 1000).ToString("0,0.0") + "ms"
                       + " | UIDrawFPS: " + ((int)(1000000 / ui_rendering)).ToString("0,0.0")
                       + " | TotalMemory: " + GC.GetTotalMemory(false).ToString("0,000") ;

                if (fps >= 24)
                {
                    Logger.Log(fps_msg);
                }
                else
                {
                    Logger.Error("[LOW FPS! WARN!]" + fps_msg);
                }
            }

            renderer_rendering = 0;
            ui_rendering = 0;
            sum_renderer = 0;
        }

        #endregion Rendering

        #region Updating UI

        int ui_fps = 0;
        string fps_msg = "";

        private void UpdateVolume()
        {
            if ((int)Sld_Footer_Volume.Value <= 0)
            {
                if (np != null)
                {
                    np.SetVolume(0);
                }
                Lb_Footer_Volume.Text = LanguageHelper.FindText("Lang_Player_Mute");
            }
            else
            {
                if (np != null)
                {
                    np.SetVolume((float)(Math.Pow(10, (float)Sld_Footer_Volume.Value / 100f) / 10 - 0.1) * 1.1111111f);
                }
                Lb_Footer_Volume.Text = ((int)Sld_Footer_Volume.Value).ToString() + "%";
            }
        }

        private Thread Update_Meta_Async()
        {
            Thread t = new Thread(new ThreadStart(Update_Meta));

            t.Start();

            return t;
        }

        DispatcherOperation Op_AlbumArtOperation;
        private void Update_Meta()
        {
            PlaylistItem item = np.CurrentPlaylist.GetCurrent();

            Tags tag = item.Tag;

            if (item != null)
            {
                Dispatcher.BeginInvoke(new Action(()=> 
                {
                    PresentationSource source = PresentationSource.FromVisual(this);

                    if (source != null)
                    {
                        dpiX = source.CompositionTarget.TransformToDevice.M11;
                    }
                    else
                    {
                        dpiX = 2;
                    }

                    Update_Footer(tag);

                    Lb_Small_Position.Text = "00:00:00";

                    if (tag != null)
                    {
                        if (!TextTool.StringEmpty(item.Tag.Title))
                        {
                            Title = item.Tag.Title;
                            Lb_Small_Title.Text = item.Tag.Title;
                            Lb_Big_Title.Text = item.Tag.Title;
                        }
                        else
                        {
                            Title = item.FileName;
                            Lb_Small_Title.Text = item.FileName;
                            Lb_Big_Title.Text = item.FileName;
                        }

                        Lb_Big_Album.Text = item.Tag.Album;
                        Lb_Big_Artist.Text = item.Tag.Artist;
                        Lb_Big_FileName.Text = item.FileName;
                        Lb_Big_Gerne.Text = item.Tag.Genre;
                        Lb_Big_Track.Text = item.Tag.Track;
                        Lb_Big_Year.Text = item.Tag.Year;
                    }
                    else
                    {
                        Title = item.FileName;

                        Lb_Small_Title.Text = item.FileName;

                        Lb_Big_Title.Text = item.FileName;
                        Lb_Big_Album.Text = "";
                        Lb_Big_Artist.Text = "";
                        Lb_Big_FileName.Text = item.FileName;
                        Lb_Big_Gerne.Text = "";
                        Lb_Big_Track.Text = "";
                        Lb_Big_Year.Text = "";
                    }
                }));

                try
                {
                    if ((np.CurrentMusic.Tag != null) && (np.CurrentMusic.Tag.Pictures!=null) && (np.CurrentMusic.Tag.Pictures.Count > 0 && np.CurrentMusic.Tag.Pictures[0] != null))
                    {
                        BitmapDecoder dec = BitmapDecoder.Create(new Uri(np.CurrentMusic.Tag.Pictures[0].FilePath), BitmapCreateOptions.None, BitmapCacheOption.None);
                        BitmapFrame frame = dec.Frames[0];
                        double pHeight = frame.PixelHeight;
                        double pWidth = frame.PixelWidth;
                        frame = null;
                        dec = null;

                        Img_AlbumArt = new BitmapImage();
                        Img_AlbumArt.BeginInit();
                        Img_AlbumArt.CreateOptions = BitmapCreateOptions.None;
                        Img_AlbumArt.CacheOption = BitmapCacheOption.OnDemand;
                        Img_AlbumArt.UriSource = new Uri(np.CurrentMusic.Tag.Pictures[0].FilePath);
                        if(pHeight > pWidth)
                        {
                            Img_AlbumArt.DecodePixelWidth = (int)(Img_Big_AlbumArt.ActualWidth * dpiX * 2.5);
                        }
                        else
                        {
                            Img_AlbumArt.DecodePixelHeight = (int)(Img_Big_AlbumArt.ActualHeight * dpiX * 2.5);
                        }
                        Img_AlbumArt.EndInit();

                        Brush_AlbumArt = new ImageBrush(Img_AlbumArt);
                        Brush_AlbumArt.SetValue(ImageBrush.StretchProperty, Stretch.UniformToFill);
                        Brush_AlbumArt.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.Linear);
                        Brush_AlbumArt.Freeze();

                        Brush_AlbumArt_LowQ = new ImageBrush(Img_AlbumArt);
                        Brush_AlbumArt_LowQ.SetValue(ImageBrush.StretchProperty, Stretch.UniformToFill);
                        Brush_AlbumArt_LowQ.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.NearestNeighbor);
                        Brush_AlbumArt_LowQ.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                        Brush_AlbumArt_LowQ.Freeze();

                        Img_AlbumArt.Freeze();
                    }
                    else
                    {
                        Img_AlbumArt = null;
                    }
                }
                catch ( Exception e)
                {
                    Logger.Error(this, e);

                    Img_AlbumArt = null;
                }

                if(Op_AlbumArtOperation != null)
                {
                    if(Op_AlbumArtOperation.Status == DispatcherOperationStatus.Pending || Op_AlbumArtOperation.Status == DispatcherOperationStatus.Executing)
                    {
                        Op_AlbumArtOperation.Abort();
                        Op_AlbumArtOperation = null;
                    }
                }

                Op_AlbumArtOperation = Dispatcher.BeginInvoke(new Action(() => 
                {
                    if (!IsShadowOn)
                    {
                        IsShadowOn = true;
                        //ShadowOn.Begin();
                    }

                    if (Img_AlbumArt != null)
                    {
                        Img_Small_AlbumArt.Fill = Brush_AlbumArt;
                        Img_Big_AlbumArt.Fill = Brush_AlbumArt;
                    }
                    else
                    {
                        Img_Small_AlbumArt.Fill = null;
                        Img_Big_AlbumArt.Fill = null;
                    }

                    ImageOffBegin();
                }));
            }
            else
            {
                if (Op_AlbumArtOperation != null)
                {
                    if (Op_AlbumArtOperation.Status == DispatcherOperationStatus.Pending || Op_AlbumArtOperation.Status == DispatcherOperationStatus.Executing)
                    {
                        Op_AlbumArtOperation.Abort();
                        Op_AlbumArtOperation = null;
                    }
                }

                Op_AlbumArtOperation = Dispatcher.BeginInvoke(new Action(() => {

                    Reset_Meta(true);

                    Img_Small_AlbumArt.Fill = null;
                    Img_Big_AlbumArt.Fill = null;
                }));
            }
        }

        DispatcherTimer ResetMetaTimer;

        private void Reset_Meta(bool wait)
        {
            if (wait)
            {
                if (ResetMetaTimer == null)
                {
                    ResetMetaTimer = new DispatcherTimer();
                    ResetMetaTimer.Interval = TimeSpan.FromMilliseconds(33);
                    ResetMetaTimer.Tick += ResetMetaTimer_Tick;
                }
                if (ResetMetaTimer.IsEnabled)
                {
                    ResetMetaTimer.Stop();
                }
                ResetMetaTimer.Start();
            }
            else
            {
                ResetMetaTimer_Tick(null, null);
            }
        }

        private void ResetMetaTimer_Tick(object sender, EventArgs e)
        {
            if (!np.isPlay)
            {
                Title = versionText;
                
                Bar_Info.Description = versionText;

                Lb_Small_Position.Text = "00:00:00";
                Lb_Small_Title.Text = LanguageHelper.FindText("Lang_Please_Play_File");

                Lb_Big_Album.Text = "";
                Lb_Big_Artist.Text = "";
                Lb_Big_FileName.Text = "";
                Lb_Big_Gerne.Text = "";
                Lb_Big_Title.Text = LanguageHelper.FindText("Lang_Please_Play_File");
                Lb_Big_Track.Text = "";
                Lb_Big_Year.Text = "";

                Sld_Small_Position.Value = 0;

                Lb_Footer_Info.Text = versionText;

                if (!IsShadowOn)
                {
                    IsShadowOn = true;
                    //ShadowOn.Begin();
                }

                Img_Big_AlbumArt.Fill = null;
                Img_Small_AlbumArt.Fill = null;

                if (dlrenderer != null)
                {
                    dlrenderer.Close();
                    dlrenderer = null;
                    MemoryManagement.FlushMemory();
                }

                if (singer != null)
                {
                    singer.Dispose();
                    singer = null;
                }
            }
            if (ResetMetaTimer != null && ResetMetaTimer.IsEnabled)
            {
                ResetMetaTimer.Stop();
            }
        }

        private void Update_Footer(Tags tag)
        {
            int bitrate = 0;
            int sampleRate = 0;
            TimeSpan duration = new TimeSpan() ;

            if(tag != null)
            {
                bitrate = tag.Bitrate;
                sampleRate = tag.Frequency;
                duration = tag.Duration;
            }

            Lb_Footer_Info.Text = (bitrate).ToString("0,0kbps ") + sampleRate.ToString("0,0Hz ") + duration.ToString(@"hh\:mm\:ss");
        }

        #endregion Updating UI

        #region RemoteControl

        private RemoteControllerWindow miniControlWindow;

        private void OpenMiniControl()
        {
            if (miniControlWindow == null)
            {
                RemoteControllerWindow r = new RemoteControllerWindow(this, np);

                r.UpdateAlbumArt(Img_AlbumArt);

                miniControlWindow = r;

                r.Closed += delegate (object obj, EventArgs arg)
                {
                    RemoteControllerWindow wd = (RemoteControllerWindow)obj;

                    if (miniControlWindow != null && miniControlWindow.UID == wd.UID)
                    {
                        miniControlWindow = null;
                        _playerMiniControlShow = false;
                    }
                };

                r.Show();

                r.Topmost = PlayerMiniControlTopmost;
                if (PlayerMiniControlSavePosition && PlayerMiniControlTop != -1)
                {
                    r.Top = PlayerMiniControlTop;
                    r.Left = PlayerMiniControlLeft;
                }

                r.LocationChanged += delegate (object obj, EventArgs arg)
                {
                    if (_playerMiniControlSetPosition)
                        return;

                    PlayerMiniControlTop = r.Top;
                    PlayerMiniControlLeft = r.Left;
                };

                r.Update();
            }
            else
            {
                miniControlWindow.Activate();
            }
        }

        private void CloseMiniControl()
        {
            if(miniControlWindow != null)
            {
                miniControlWindow.Close();

                miniControlWindow = null;
            }
            _playerMiniControlShow = false;
        }

        #endregion RemoteControl

        #region VU Meter

        private void Init_VU()
        {
            if(Brush_VU_Dark == null)
            {
                Brush_VU_Dark = new SolidColorBrush(Color.FromArgb(80, 100, 100, 100));
            }
            if (Brush_VU_White == null)
            {
                Brush_VU_White = new SolidColorBrush(Color.FromArgb(80, 160, 160, 160));
            }
        }

        #endregion VU Meter

        #region Osilo

        int q_index = 0;
        List<double> osilo_samples = new List<double>();

        private void Init_Visualizer()
        {
            if (OsiloFill == null) 
            {
                GradientStopCollection osilo_gradient_stops = new GradientStopCollection();
                
                osilo_gradient_stops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#AAFFFFFF"), 0));
                osilo_gradient_stops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFFFFFFF"), 0.85));
                osilo_gradient_stops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#55FFFFFF"), 0.85));
                osilo_gradient_stops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#55FFFFFF"), 1));

                OsiloFill = new LinearGradientBrush(osilo_gradient_stops, new Point(0.5, 1), new Point(0.5, 0));
            }
            
            if(SpecFill == null)
            {
                GradientStopCollection spec_gradient_stops = new GradientStopCollection();

                spec_gradient_stops.Add(new GradientStop(Color.FromArgb(244, 255, 255, 255), 0));
                spec_gradient_stops.Add(new GradientStop(Color.FromArgb(244, 255, 255, 255), 0.72));

                spec_gradient_stops.Add(new GradientStop(Color.FromArgb(95, 255, 255, 255), 0.72));
                spec_gradient_stops.Add(new GradientStop(Color.FromArgb(95, 255, 255, 255), 0.90));

                spec_gradient_stops.Add(new GradientStop(Color.FromArgb(30, 255, 255, 255), 0.90));
                spec_gradient_stops.Add(new GradientStop(Color.FromArgb(30, 255, 255, 255), 0.98));
                spec_gradient_stops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 1.0));

                SpecFill = new LinearGradientBrush(spec_gradient_stops, new Point(0.5, 0), new Point(0.5, 1));
            }
        }

        #endregion Osilo

        #region ImageAnimation

        private void ImageOff_Completed(object sender, EventArgs e)
        {
            Img_Back_AlbumArt.Fill = null;
            Img_Back_AlbumArt.Visibility = Visibility.Hidden;
            if (np.isPlay)
            {
                if (Img_AlbumArt != null)
                {
                    Img_Back_AlbumArt.Fill = Brush_AlbumArt_LowQ;
                    Img_Back_AlbumArt.Visibility = Visibility.Visible;

                    if (IsShadowOn)
                    {
                        //ShadowOff.Begin();
                        IsShadowOn = false;
                    }

                    ImageOnBegin();
                }
                else
                {
                    ImageOnBegin();
                }
            }
            else
            {
                ImageOnBegin();
            }
        }

        private void ImageOffBegin()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (miniControlWindow != null)
                {
                    miniControlWindow.UpdateAlbumArt(Img_AlbumArt);
                }

                if (UseImageAnimation)
                {
                    ImageOff.Begin();
                }
                else
                {
                    ImageOff_Completed(null, new EventArgs());
                }
            }));
        }

        private void ImageOnBegin()
        {
            if (UseImageAnimation)
            {
                ImageOn.Begin();
            }
            else
            {
                Img_Back_AlbumArt.Opacity = 1;
                Img_Back_AlbumArt.Visibility = Visibility.Visible;
            }
        }

        #endregion ImageAnimation

        #region Pipe

        public void PipePlayJustAdded()
        {
            np.Play(np.CurrentPlaylist.Items.Count - 1);

            update_order();
            update_play();
        }

        NamedPipeServerStream Pipe;
        Thread PipeServerThread;

        private void StartPipe()
        {
            PipeServerThread = new Thread(new ThreadStart(PipeServer));

            PipeServerThread.Start();
        }

        bool PipeOn = true;
        private void PipeServer()
        {
            while (PipeOn)
            {
                try
                {
                    if (Pipe != null)
                    {
                        Pipe.Disconnect();
                        Pipe.Close();
                        Pipe.Dispose();
                        Pipe = null;
                    }

                    Pipe = new NamedPipeServerStream(App.PipeName, PipeDirection.In);
                    if (Pipe.CanTimeout)
                        Pipe.ReadTimeout = 5000;
                }
                catch
                {
                    Logger.Log("Pipe: Wait for another instance is closed");

                    Pipe = null;

                    Thread.Sleep(2500);
                }

                if (Pipe != null)
                {
                    Logger.Log("Pipe, Wait For Connection");

                    Pipe.WaitForConnection();

                    StreamString ss = new StreamString(Pipe);

                    string readed = ss.ReadString();

                    if (readed != null)
                    {
                        string[] files = readed.Split('\n');

                        int playlist_index = -1;

                        if (np.CurrentPlaylist != null)
                        {
                            playlist_index = np.CurrentPlaylistIndex;
                        }
                        else
                        {
                            for (int i = 0; i < np.Playlists.Count; i++)
                            {
                                if (np.Playlists[i].Title == LanguageHelper.FindText("Lang_Playlist_Default_Playlist"))
                                {
                                    playlist_index = i;
                                    break;
                                }
                            }

                            if (playlist_index < 0)
                            {
                                np.Playlists.Add(new Playlist(LanguageHelper.FindText("Lang_Playlist_Default_Playlist")));

                                playlist_index = np.Playlists.Count - 1;
                            }
                        }

                        int item_index = -1;
                        for (int i = 0; i < files.Length; i++)
                        {
                            Logger.Log("FILE ADDED (TO: " + i.ToString() + "): " + files[i]);

                            np.Playlists[playlist_index].Add(new FileItem(files[i]));

                            item_index = np.Playlists[playlist_index].Items.Count - 1;
                        }

                        np.CurrentPlaylistIndex = playlist_index;

                        Dispatcher.Invoke(new Action(() =>
                        {
                            PlaylistViewer.updateList();
                            PlaylistItemViewer.Updated = false;
                            PlaylistItemViewer.UpdateList();

                            if (np.isPlay)
                            {
                                np.Stop();
                            }

                            np.Play(item_index);
                            update_order();
                            update_play();
                        }));
                    }

                    Thread.Sleep(50);

                    Logger.Log("PIPE READED!!!! = " + readed);
                }
            }
        }

        private void StopPipe()
        {
            PipeOn = false;

            PipeServerThread.Join(200);

            if (Pipe != null)
            {
                Pipe.Close();
            }
        }

        #endregion Pipe

        #region Closed

        public void Dispose()
        {
            if (np != null)
            {
                np.Dispose();
            }
            if (systemCounter != null)
            {
                systemCounter.Dispose();
            }
        }

        private void window_Closed(object sender, EventArgs e)
        {
            StopPipe();

            np.Stop();
            np.Dispose();
            
            if (dlrenderer != null)
            {
                dlrenderer.Close();
                dlrenderer = null;
                MemoryManagement.FlushMemory();
            }

            if (singer != null)
            {
                singer.Dispose();
                singer = null;
            }

            if(DummyToolWindow != null)
            {
                DummyToolWindow.Close();
                DummyToolWindow = null;
            }

            SearchMetadataStop();

            SaveSettings();

            SavePlaylists();
            
            TagImage.ClearCache();

            Environment.Exit(0);
        }

        private void XmlWriteValue(XmlWriter writer, string Name, string Value)
        {
            writer.WriteStartElement(Name);

            writer.WriteAttributeString("Value", Value);

            writer.WriteEndElement();
        }

        private int SavingMaxRetry = 5;

        public void SaveSettings()
        {
            DirectoryInfo di_setting = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings"));
            if (!di_setting.Exists)
            {
                di_setting.Create();
            }

            SaveSetting(di_setting, 0);

            DSPChainSaver.Save(Path.Combine(di_setting.FullName, "Effects.DSPs"), np.DSPs);
        }

        private void SaveSetting(DirectoryInfo di, int retry)
        {
            if(retry > SavingMaxRetry)
            {
                return;
            }

            try
            {
                string xmlFile = Path.Combine(di.FullName, "Settings.xml");

                XmlWriterSettings wsetting = new XmlWriterSettings();
                wsetting.Indent = true;
                wsetting.IndentChars = "\t";

                using (XmlWriter writer = XmlWriter.Create(xmlFile, wsetting))
                {
                    writer.WriteStartElement("Visual");

                    /*
                    TODO LISTS!

                    case "Waveform.Zoom":
                    case "Visualizer.Use":
                    case "Osilo.Use"
                    case "VU.Use":
                    */

                    //Theme Data Fields
                    writer.WriteStartElement("Theme");

                    XmlWriteValue(writer, "CurrentTheme", CurrentTheme);

                    writer.WriteEndElement();

                    //GUI Data Fields
                    writer.WriteStartElement("GUI");

                    if (UseImageAnimation) XmlWriteValue(writer, "UseImageAnimation", "true");
                    else XmlWriteValue(writer, "UseImageAnimation", "false");

                    XmlWriteValue(writer, "GUIUpdate", GUIUpdate.ToString());

                    if (UseFooterInfoText) XmlWriteValue(writer, "UseFooterInfoText", "true");
                    else XmlWriteValue(writer, "UseFooterInfoText", "false");

                    if (SaveWindowMode) XmlWriteValue(writer, "SaveWindowMode", "true");
                    else XmlWriteValue(writer, "SaveWindowMode", "false");

                    switch (WindowMode)
                    {
                        case WindowMode.Big:
                            XmlWriteValue(writer, "WindowMode", "Big");
                            break;
                        case WindowMode.Mid:
                            XmlWriteValue(writer, "WindowMode", "Mid");
                            break;
                        case WindowMode.Small:
                            XmlWriteValue(writer, "WindowMode", "Small");
                            break;
                        default:
                            throw new ArgumentNullException();
                    }

                    writer.WriteEndElement();

                    //Player Data Field
                    writer.WriteStartElement("Player");

                    if (PlayerUseSearchLocalAlbumArt) XmlWriteValue(writer, "Player.UseSearchLocalAlbumArt", "true");
                    else XmlWriteValue(writer, "Player.UseSearchLocalAlbumArt", "false");

                    writer.WriteStartElement("Player.AlbumArtSearchPaths");

                    foreach(string path in PlayerAlbumArtSearchPathes)
                    {
                        XmlWriteValue(writer, "Player.AlbumArtSearchPath", path);
                    }

                    writer.WriteEndElement();

                    XmlWriteValue(writer, "Player.LanguageFileName", LanguageHelper.LanguageFileName);

                    XmlWriteValue(writer, "Player.MiniControlShow", XmlHelper.Bool2String(PlayerMiniControlShow));

                    XmlWriteValue(writer, "Player.MiniControlSavePosition", XmlHelper.Bool2String(PlayerMiniControlSavePosition));

                    XmlWriteValue(writer, "Player.MiniControlTopmost", XmlHelper.Bool2String(PlayerMiniControlTopmost));

                    XmlWriteValue(writer, "Player.MiniControlTop", PlayerMiniControlTop.ToString("0.00"));

                    XmlWriteValue(writer, "Player.MiniControlLeft", PlayerMiniControlLeft.ToString("0.00"));

                    writer.WriteEndElement();

                    //Audio Data Field
                    writer.WriteStartElement("Audio");

                    XmlWriteValue(writer, "Audio.DesiredLantency", AudioDesiredLantency.ToString());

                    XmlWriteValue(writer, "Audio.Volume", AudioVolume.ToString());

                    if (AudioUseDspProcessing) XmlWriteValue(writer, "Audio.UseDspProcessing", "true");
                    else XmlWriteValue(writer, "Audio.UseDspProcessing", "false");

                    if (AudioUseDspLimit) XmlWriteValue(writer, "Audio.UseDspLimit", "true");
                    else XmlWriteValue(writer, "Audio.UseDspLimit", "false");

                    XmlWriteValue(writer, "Audio.DspLimitSampleRate", AudioDspLimitSampleRate.ToString());

                    writer.WriteEndElement();

                    //Singer Data Fields
                    writer.WriteStartElement("Singer");

                    switch (SingerHorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            XmlWriteValue(writer, "Singer.HorizontalAlignment", "Left");
                            break;
                        case HorizontalAlignment.Center:
                            XmlWriteValue(writer, "Singer.HorizontalAlignment", "Center");
                            break;
                        case HorizontalAlignment.Right:
                            XmlWriteValue(writer, "Singer.HorizontalAlignment", "Right");
                            break;
                        case HorizontalAlignment.Stretch:
                            XmlWriteValue(writer, "Singer.HorizontalAlignment", "Stretch");
                            break;
                        default:
                            break;
                    }

                    switch (SingerVerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            XmlWriteValue(writer, "Singer.VerticalAlignment", "Top");
                            break;
                        case VerticalAlignment.Center:
                            XmlWriteValue(writer, "Singer.VerticalAlignment", "Center");
                            break;
                        case VerticalAlignment.Bottom:
                            XmlWriteValue(writer, "Singer.VerticalAlignment", "Bottom");
                            break;
                        case VerticalAlignment.Stretch:
                            XmlWriteValue(writer, "Singer.VerticalAlignment", "Stretch");
                            break;
                        default:
                            break;
                    }

                    switch (SingerDefaultFadeInMode)
                    {
                        case FadeInMode.Auto:
                            throw new NullReferenceException("Unknown SingerDefaultFadeInMode. MainWindow.1908");
                        case FadeInMode.None:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "None");
                            break;
                        case FadeInMode.FadeIn:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "FadeIn");
                            break;
                        case FadeInMode.BlurIn:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "BlurIn");
                            break;
                        case FadeInMode.ZoomIn:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "ZoomIn");
                            break;
                        case FadeInMode.ZoomOut:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "ZoomOut");
                            break;
                        case FadeInMode.SlideFromLeft:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "SlideFromLeft");
                            break;
                        case FadeInMode.SlideFromRight:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "SlideFromRight");
                            break;
                        case FadeInMode.SlideFromTop:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "SlideFromTop");
                            break;
                        case FadeInMode.SlideFromBottom:
                            XmlWriteValue(writer, "Singer.DefaultFadeInMode", "SlideFromBottom");
                            break;
                        default:
                            throw new NullReferenceException("Unknown SingerDefaultFadeInMode. MainWindow.1937");
                    }

                    switch (SingerDefaultFadeOutMode)
                    {
                        case FadeOutMode.Auto:
                            throw new NullReferenceException("Unknown SingerDefaultFadeOutMode. MainWindow.1943");
                        case FadeOutMode.None:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "None");
                            break;
                        case FadeOutMode.FadeOut:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "FadeOut");
                            break;
                        case FadeOutMode.BlurOut:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "BlurOut");
                            break;
                        case FadeOutMode.ZoomIn:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "ZoomIn");
                            break;
                        case FadeOutMode.ZoomOut:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "ZoomOut");
                            break;
                        case FadeOutMode.SlideToLeft:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "SlideToLeft");
                            break;
                        case FadeOutMode.SlideToRight:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "SlideToRight");
                            break;
                        case FadeOutMode.SlideToTop:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "SlideToTop");
                            break;
                        case FadeOutMode.SlideToBottom:
                            XmlWriteValue(writer, "Singer.DefaultFadeOutMode", "SlideToBottom");
                            break;
                        default:
                            throw new NullReferenceException("Unknown SingerDefaultFadeOutMode. MainWindow.1972");
                    }

                    XmlWriteValue(writer, "Singer.Top", SingerTop.ToString());

                    XmlWriteValue(writer, "Singer.Left", SingerLeft.ToString());

                    if (SingerResetPosition) XmlWriteValue(writer, "Singer.ResetPosition", "true");
                    else XmlWriteValue(writer, "Singer.ResetPosition", "false");

                    if (SingerCanDragmove) XmlWriteValue(writer, "Singer.CanDragmove", "true");
                    else XmlWriteValue(writer, "Singer.CanDragmove", "false");

                    XmlWriteValue(writer, "Singer.Zoom", SingerZoom.ToString());

                    if (SingerShow) XmlWriteValue(writer, "Singer.Show", "true");
                    else XmlWriteValue(writer, "Singer.Show", "false");

                    if (SingerWindowMode) XmlWriteValue(writer, "Singer.WindowMode", "true");
                    else XmlWriteValue(writer, "Singer.WindowMode", "false");

                    XmlWriteValue(writer, "Singer.Opacity", SingerOpacity.ToString());

                    writer.WriteEndElement();

                    //Composer Data Fields
                    writer.WriteStartElement("Composer");

                    if (ComposerTopmost) XmlWriteValue(writer, "Composer.Topmost", "true");
                    else XmlWriteValue(writer, "Composer.Topmost", "false");

                    if (ComposerUse) XmlWriteValue(writer, "Composer.Use", "true");
                    else XmlWriteValue(writer, "Composer.Use", "false");

                    XmlWriteValue(writer, "Composer.Opacity", ComposerOpacity.ToString());

                    XmlWriteValue(writer, "Composer.WindowMode", XmlHelper.Bool2String(ComposerWindowMode));

                    writer.WriteEndElement();
                    
                    //Osilo Data Fields
                    writer.WriteStartElement("Osilo");

                    XmlWriteValue(writer, "Osilo.RenderType", XmlHelper.BarRenderType2String(OsiloRenderType));

                    XmlWriteValue(writer, "Osilo.Height", OsiloHeight.ToString());

                    XmlWriteValue(writer, "Osilo.View", OsiloView.ToString());

                    XmlWriteValue(writer, "Osilo.Strength", OsiloStrength.ToString());

                    XmlWriteValue(writer, "Osilo.Width", OsiloWidth.ToString());

                    XmlWriteValue(writer, "Osilo.Dash", OsiloDash.ToString());

                    XmlWriteValue(writer, "Osilo.Top", OsiloTop.ToString());

                    XmlWriteValue(writer, "Osilo.Opacity", OsiloOpacity.ToString());

                    XmlWriteValue(writer, "Osilo.UseInvert", XmlHelper.Bool2String(OsiloUseInvert));

                    XmlWriteValue(writer, "Osilo.GridShow", XmlHelper.Bool2String(OsiloGridShow));

                    XmlWriteValue(writer, "Osilo.GridTextHorizontalAlignment", XmlHelper.HorizontalAlignment2String(OsiloGridTextHorizontalAlignment));

                    writer.WriteEndElement();

                    //Vu Data Fields
                    writer.WriteStartElement("VU");

                    XmlWriteValue(writer, "VU.Opacity", VU_Opacity.ToString());

                    XmlWriteValue(writer, "VU.Senstive", VU_Senstive.ToString());

                    writer.WriteEndElement();

                    //Spec Data Fields
                    writer.WriteStartElement("Spec");

                    XmlWriteValue(writer, "Spec.RenderType", XmlHelper.BarRenderType2String(SpecRenderType));

                    XmlWriteValue(writer, "Spec.Top", SpecTop.ToString());

                    XmlWriteValue(writer, "Spec.Height", SpecHeight.ToString());

                    if (SpecInvert) XmlWriteValue(writer, "Spec.Invert", "true");
                    else XmlWriteValue(writer, "Spec.Invert", "false");

                    if (SpecUseLogScale) XmlWriteValue(writer, "Spec.UseLogScale", "true");
                    else XmlWriteValue(writer, "Spec.UseLogScale", "false");

                    XmlWriteValue(writer, "Spec.MaxFreq", SpecMaxFreq.ToString());

                    XmlWriteValue(writer, "Spec.MinFreq", SpecMinFreq.ToString());

                    XmlWriteValue(writer, "Spec.Opacity", SpecOpacity.ToString());

                    XmlWriteValue(writer, "Spec.Strength", SpecStrength.ToString());

                    if (SpecUseResampler) XmlWriteValue(writer, "Spec.UseResampler", "true");
                    else XmlWriteValue(writer, "Spec.UseResampler", "false");

                    XmlWriteValue(writer, "Spec.ResamplingMode", XmlHelper.ResamplingMode2String(SpecResampleMode));

                    string strSpecScalingMode = "";
                    switch (Spec_ScalingMode)
                    {
                        case ScalingStrategy.Decibel:
                            strSpecScalingMode = "Decibel";
                            break;
                        case ScalingStrategy.Linear:
                            strSpecScalingMode = "Linear";
                            break;
                        case ScalingStrategy.Sqrt:
                            strSpecScalingMode = "Sqrt";
                            break;
                        default:
                            strSpecScalingMode = "Decibel";
                            break;
                    }
                    XmlWriteValue(writer, "Spec.ScalingMode", strSpecScalingMode);

                    XmlWriteValue(writer, "Spec.Width", Spec_Width.ToString());

                    XmlWriteValue(writer, "Spec.Dash", Spec_Dash.ToString());

                    XmlWriteValue(writer, "Spec.GridShow", XmlHelper.Bool2String(SpecGridShow));

                    XmlWriteValue(writer, "Spec.GridTextHorizontalAlignment", XmlHelper.HorizontalAlignment2String(SpecGridTextHorizontalAlignment));

                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.Flush();

                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());

                SaveSetting(di, retry + 1);
            }
        }

        private string Color2String(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }

        public void SavePlaylists()
        {
            string playlistPath = AppDomain.CurrentDomain.BaseDirectory + "Playlists";

            Logger.Log("playlist save path: " + playlistPath);

            if (!Directory.Exists(playlistPath))
            {
                Directory.CreateDirectory(playlistPath);
            }
            else
            {
                DirectoryInfo dt = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Playlists");
                FileInfo[] files = dt.GetFiles("*.m3u8");
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i].FullName);
                }
            }

            for (int i = 0; i < np.Playlists.Count; i++)
            {
                if (np.Playlists[i].Items.Count >= 1)
                {
                    PlaylistSaver.Save(playlistPath + "\\" + np.Playlists[i].Title + ".m3u8", np.Playlists[i]);
                }
            }
        }

        #endregion Closed

        #region Loading

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }
            else
            {
                Logger.Error("display init errored");
            }

            np.WaveformWidth = (int)(Sld_Big_Position.ActualWidth * dpiX);
            
            DummyToolWindow = new Window();
            DummyToolWindow.Top = -10000;
            DummyToolWindow.Left = -10000;
            DummyToolWindow.Width = 0;
            DummyToolWindow.Height = 0;
            DummyToolWindow.WindowStyle = WindowStyle.ToolWindow;
            DummyToolWindow.ShowInTaskbar = false;
            DummyToolWindow.Show();

            StartPipe();
        }
        
        private int LoadingMaxRetry = 5;

        private void LoadSettings()
        {
            _playerMiniControlSetPosition = true;

            DirectoryInfo di_setting = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings"));
            di_setting.Create();

            FileInfo fi_visual = new FileInfo(Path.Combine(di_setting.FullName, "Settings.xml"));
            LoadSetting(fi_visual, 0);

            FileInfo fi_dsp = new FileInfo(Path.Combine(di_setting.FullName, "Effects.DSPs"));
            if (fi_dsp.Exists)
            {
                try
                {
                    List<DSPBase> dsps = DSPChainLoader.Load(fi_dsp.FullName, ref np);
                    if (dsps != null)
                    {
                        np.DSPs = dsps;
                        np.UpdateDSP();
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error(this, ex);
                }
            }

            if (Util.TextTool.StringEmpty(CurrentTheme))
            {
                CurrentTheme = "";
            }

            if (Util.TextTool.StringEmpty(LanguageHelper.LanguageFileName))
            {
                LanguageHelper.LanguageFileName = "";
            }

            DirectoryInfo di_theme = new DirectoryInfo(Path.Combine(di_setting.FullName, CurrentTheme));
            ThemeHelper.LoadTheme(di_theme, this);

            FileInfo fi_lang = new FileInfo(Path.Combine(LanguageHelper.LibraryDirectory, LanguageHelper.LanguageFileName));
            LoadLanguagePack(fi_lang, 0);

            //fix Lang Mute
            if(Sld_Footer_Volume.Value == 0)
            {
                Sld_Footer_Volume.Value = 0.001;
                Sld_Footer_Volume.Value = 0;
            }

            _playerMiniControlSetPosition = false;
        }

        private void LoadLanguagePack(FileInfo fi, int retry)
        {
            if(retry >= 5 || !fi.Exists)
            {
                LanguageHelper.LoadDefaultPack();

                return;
            }

            try
            {
                LanguageHelper.LoadLanguagePack(fi);
            }
            catch (Exception e)
            {
                Logger.Error(this, e);

                LoadLanguagePack(fi, retry + 1);
            }
        }

        private void LoadSetting(FileInfo fi, int retry)
        {
            settingListener = new SettingListener();
            settingListener.SettingChanged += OnSettingChanged;
            settingListener.SettingPropertyChanged += OnSettingPropertyChanged;
            return;

            if(retry > LoadingMaxRetry || !fi.Exists)
            {
                return;
            }

            try {
                WindowMode wmread = WindowMode.Big;

                using (XmlReader reader = XmlReader.Create(fi.FullName))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    //Theme Data Fields
                                    case "Visual":
                                        break;
                                    case "Theme":
                                        break;
                                    case "GUI":
                                        break;
                                    case "CurrentTheme":
                                        CurrentTheme = reader.GetAttribute("Value");
                                        break;
                                    case "UseImageAnimation":
                                        string strUseImageAnimation = reader.GetAttribute("Value");
                                        if (strUseImageAnimation.ToLower() == "true") UseImageAnimation = true;
                                        else UseImageAnimation = false;
                                        break;
                                    case "GUIUpdate":
                                        GUIUpdate = (int)Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "UseFooterInfoText":
                                        string strUseFooterInfoText = reader.GetAttribute("Value");
                                        if (strUseFooterInfoText.ToLower() == "true") UseFooterInfoText = true;
                                        else UseFooterInfoText = false;
                                        break;
                                    case "SaveWindowMode":
                                        if (XmlHelper.String2Bool(reader.GetAttribute("Value")))
                                        {
                                            SaveWindowMode = true;
                                        }
                                        else
                                        {
                                            SaveWindowMode = false;
                                        }
                                        break;
                                    case "WindowMode":
                                        string wmtxt = reader.GetAttribute("Value").ToLower();
                                        switch (wmtxt)
                                        {
                                            case "big":
                                                wmread = WindowMode.Big;
                                                break;
                                            case "small":
                                                wmread = WindowMode.Small;
                                                break;
                                            case "mid":
                                                wmread = WindowMode.Mid;
                                                break;
                                        }
                                        break;
                                    case "Waveform.Zoom":
                                        break;

                                    //Audio Data Fields
                                    case "Audio":
                                        break;
                                    case "Audio.DesiredLantency":
                                        AudioDesiredLantency = (int)Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Audio.Volume":
                                        AudioVolume = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Audio.UseDspProcessing":
                                        AudioUseDspProcessing = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Audio.UseDspLimit":
                                        AudioUseDspLimit = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Audio.DspLimitSampleRate":
                                        AudioDspLimitSampleRate = (int)Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;

                                    //Player Data Fields
                                    case "Player":
                                        break;
                                    case "Player.AlbumArtSearchPaths":
                                        List<string> playerAlbumArtPath = new List<string>();
                                        while (reader.Read())
                                        {
                                            if(reader.NodeType == XmlNodeType.Element && reader.Name == "Player.AlbumArtSearchPath")
                                            {
                                                playerAlbumArtPath.Add(reader.GetAttribute("Value"));
                                            }
                                            else if(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Player.AlbumArtSearchPaths")
                                            {
                                                break;
                                            }
                                        }
                                        PlayerAlbumArtSearchPathes = playerAlbumArtPath.ToArray();
                                        break;
                                    case "Player.UseSearchLocalAlbumArt":
                                        PlayerUseSearchLocalAlbumArt = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Player.LanguageFileName":
                                        LanguageHelper.LanguageFileName = reader.GetAttribute("Value");
                                        break;
                                    case "Player.MiniControlShow":
                                        PlayerMiniControlShow = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Player.MiniControlSavePosition":
                                        PlayerMiniControlSavePosition = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Player.MiniControlTopmost":
                                        PlayerMiniControlTopmost = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Player.MiniControlTop":
                                        PlayerMiniControlTop = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Player.MiniControlLeft":
                                        PlayerMiniControlLeft = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    //Visualizer Data Fields
                                    case "Visualizer.Use":
                                        break;

                                    //Osilo Data Fields
                                    case "Osilo":
                                        break;
                                    case "Osilo.RenderType":
                                        OsiloRenderType = XmlHelper.String2BarRenderType(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.View":
                                        OsiloView = (int)Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.Use":
                                        break;
                                    case "Osilo.Height":
                                        OsiloHeight = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.Opacity":
                                        OsiloOpacity = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.Width":
                                        OsiloWidth = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.Dash":
                                        OsiloDash = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.Top":
                                        OsiloTop = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.Strength":
                                        OsiloStrength = (float)Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.UseInvert":
                                        OsiloUseInvert = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.GridShow":
                                        OsiloGridShow = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Osilo.GridTextHorizontalAlignment":
                                        OsiloGridTextHorizontalAlignment = XmlHelper.String2HorizontalAlignment(reader.GetAttribute("Value"));
                                        break;

                                    //VU Data Fields
                                    case "VU":
                                        break;
                                    case "VU.Opacity":
                                        VU_Opacity = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "VU.Senstive":
                                        VU_Senstive = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "VU.Use":
                                        break;

                                    //Spectrum Data Fields
                                    case "Spec":
                                        break;
                                    case "Spec.RenderType":
                                        SpecRenderType = XmlHelper.String2BarRenderType(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.Top":
                                        SpecTop = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.Height":
                                        SpecHeight = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.Invert":
                                        SpecInvert = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.UseLogScale":
                                        SpecUseLogScale = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.MaxFreq":
                                        SpecMaxFreq = (int)Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.MinFreq":
                                        SpecMinFreq = (int)Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.Opacity":
                                        SpecOpacity = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.Strength":
                                        SpecStrength = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.UseResampler":
                                        SpecUseResampler = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.ResamplingMode":
                                        SpecResampleMode = XmlHelper.String2ResamplingMode(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.ScalingMode":
                                        string strSpecScalingMode = reader.GetAttribute("Value").ToLower();
                                        switch (strSpecScalingMode)
                                        {
                                            case "decibel":
                                                Spec_ScalingMode = ScalingStrategy.Decibel;
                                                break;
                                            case "linear":
                                                Spec_ScalingMode = ScalingStrategy.Linear;
                                                break;
                                            case "sqrt":
                                                Spec_ScalingMode = ScalingStrategy.Sqrt;
                                                break;
                                            default:
                                                throw new NotImplementedException();
                                        }
                                        break;
                                    case "Spec.Width":
                                        Spec_Width = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.Dash":
                                        Spec_Dash = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.GridShow":
                                        SpecGridShow = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    case "Spec.GridTextHorizontalAlignment":
                                        SpecGridTextHorizontalAlignment = XmlHelper.String2HorizontalAlignment(reader.GetAttribute("Value"));
                                        break;

                                    //Singer Data Fields
                                    case "Singer":
                                        break;
                                    case "Singer.HorizontalAlignment":
                                        string strSingerHorizontalAlignment = reader.GetAttribute("Value");
                                        switch (strSingerHorizontalAlignment)
                                        {
                                            case "Left":
                                                SingerHorizontalAlignment = HorizontalAlignment.Left;
                                                break;
                                            case "Center":
                                                SingerHorizontalAlignment = HorizontalAlignment.Center;
                                                break;
                                            case "Right":
                                                SingerHorizontalAlignment = HorizontalAlignment.Right;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "Singer.VerticalAlignment":
                                        string strSingerVerticalAlignment = reader.GetAttribute("Value");
                                        switch (strSingerVerticalAlignment)
                                        {
                                            case "Top":
                                                SingerVerticalAlignment = VerticalAlignment.Top;
                                                break;
                                            case "Center":
                                                SingerVerticalAlignment = VerticalAlignment.Center;
                                                break;
                                            case "Bottom":
                                                SingerVerticalAlignment = VerticalAlignment.Bottom;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "Singer.DefaultFadeInMode":
                                        string strSingerDefaultFadeInMode = reader.GetAttribute("Value");
                                        switch (strSingerDefaultFadeInMode)
                                        {
                                            case "None":
                                                SingerDefaultFadeInMode = FadeInMode.None;
                                                break;
                                            case "FadeIn":
                                                SingerDefaultFadeInMode = FadeInMode.FadeIn;
                                                break;
                                            case "BlurIn":
                                                SingerDefaultFadeInMode = FadeInMode.BlurIn;
                                                break;
                                            case "ZoomIn":
                                                SingerDefaultFadeInMode = FadeInMode.ZoomIn;
                                                break;
                                            case "ZoomOut":
                                                SingerDefaultFadeInMode = FadeInMode.ZoomOut;
                                                break;
                                            case "SlideFromLeft":
                                                SingerDefaultFadeInMode = FadeInMode.SlideFromLeft;
                                                break;
                                            case "SlideFromRight":
                                                SingerDefaultFadeInMode = FadeInMode.SlideFromRight;
                                                break;
                                            case "SlideFromBottom":
                                                SingerDefaultFadeInMode = FadeInMode.SlideFromBottom;
                                                break;
                                            case "SlideFromTop":
                                                SingerDefaultFadeInMode = FadeInMode.SlideFromTop;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "Singer.DefaultFadeOutMode":
                                        string strSingerDefaultFadeOutMode = reader.GetAttribute("Value");
                                        switch (strSingerDefaultFadeOutMode)
                                        {
                                            case "None":
                                                SingerDefaultFadeOutMode = FadeOutMode.None;
                                                break;
                                            case "FadeOut":
                                                SingerDefaultFadeOutMode = FadeOutMode.FadeOut;
                                                break;
                                            case "BlurOut":
                                                SingerDefaultFadeOutMode = FadeOutMode.BlurOut;
                                                break;
                                            case "ZoomIn":
                                                SingerDefaultFadeOutMode = FadeOutMode.ZoomIn;
                                                break;
                                            case "ZoomOut":
                                                SingerDefaultFadeOutMode = FadeOutMode.ZoomOut;
                                                break;
                                            case "SlideToLeft":
                                                SingerDefaultFadeOutMode = FadeOutMode.SlideToLeft;
                                                break;
                                            case "SlideToRight":
                                                SingerDefaultFadeOutMode = FadeOutMode.SlideToRight;
                                                break;
                                            case "SlideToTop":
                                                SingerDefaultFadeOutMode = FadeOutMode.SlideToTop;
                                                break;
                                            case "SlideToBottom":
                                                SingerDefaultFadeOutMode = FadeOutMode.SlideToBottom;
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    case "Singer.ResetPosition":
                                        string strSingerResetPosition = reader.GetAttribute("Value");
                                        if(strSingerResetPosition.ToLower() == "true")
                                        {
                                            SingerResetPosition = true;
                                        }
                                        else
                                        {
                                            SingerResetPosition = false;
                                        }
                                        break;
                                    case "Singer.Top":
                                        SingerTop = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Singer.Left":
                                        SingerLeft = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Singer.CanDragmove":
                                        string strSingerCanDragMove = reader.GetAttribute("Value");
                                        if (strSingerCanDragMove.ToLower() == "true") SingerCanDragmove = true;
                                        else SingerCanDragmove = false;
                                        break;
                                    case "Singer.Show":
                                        string strSingerShow = reader.GetAttribute("Value");
                                        if (strSingerShow.ToLower() == "true") SingerShow = true;
                                        else SingerShow = false;
                                        break;
                                    case "Singer.Opacity":
                                        SingerOpacity = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Singer.Zoom":
                                        SingerZoom = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Singer.WindowMode":
                                        SingerWindowMode = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;

                                    //Composer Data Fields
                                    case "Composer":
                                        break;
                                    case "Composer.Topmost":
                                        string strComposerTopmost = reader.GetAttribute("Value");
                                        if (strComposerTopmost.ToLower() == "true") ComposerTopmost = true;
                                        else ComposerTopmost = false;
                                        break;
                                    case "Composer.Use":
                                        string strComposerUse = reader.GetAttribute("Value");
                                        if (strComposerUse.ToLower() == "true") ComposerUse = true;
                                        else ComposerUse = false;
                                        break;
                                    case "Composer.Opacity":
                                        ComposerOpacity = Convert.ToDouble(reader.GetAttribute("Value"));
                                        break;
                                    case "Composer.WindowMode":
                                        ComposerWindowMode = XmlHelper.String2Bool(reader.GetAttribute("Value"));
                                        break;
                                    default:
                                        Logger.Log("Unknown Name in Setting file. Is it not implemented? (" + reader.Name + ")");
                                        break;
                                }
                                break;
                        }
                    }
                    
                    reader.Close();
                }

                //End of Reading.
                if(WindowMode != wmread && SaveWindowMode)
                {
                    WindowMode = wmread;
                    switch (WindowMode)
                    {
                        case WindowMode.Big:
                            MidOff.Begin();
                            //MidOff.SkipToFill();
                            break;
                        case WindowMode.Mid:
                            MidOn.Begin();
                            //MidOn.SkipToFill();
                            //MidOn.Stop();
                            break;
                        case WindowMode.Small:
                            SmallOn.Begin();
                            //SmallOn.SkipToFill();
                            //SmallOn.Stop();
                            break;
                    }
                }
            }
            catch
            {
                LoadSetting(fi, retry + 1);
            }
        }

        private void OnSettingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnSettingChanged(object sender, Settings e)
        {
            throw new NotImplementedException();
        }

        public void LoadPlaylists(SplashWindow window)
        {
            Logger.Log("Found AutoSave Path: " + AppDomain.CurrentDomain.BaseDirectory + "Playlists");
            DirectoryInfo dt = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Playlists");

            if (!dt.Exists)
            {
                dt.Create();
            }

            FileInfo[] files = dt.GetFiles("*.m3u8");
            List<FileInfo> orderedFIles = new List<FileInfo>(files);
            orderedFIles = orderedFIles.OrderByDescending(x => x.LastWriteTime).ToList();
            files = orderedFIles.ToArray();

            if (files.Length > 0)
            {
                np.Playlists.Clear();

                Logger.Log("Found AutoSave Playlist: " + files.Length.ToString());

                PlaylistLoader loader = null;

                bool threadon = true;
                Thread th = new Thread(() => 
                {
                    string previous = "-1";
                    while (threadon)
                    {
                        if (loader != null && !TextTool.StringEmpty(loader.CurrentName) && previous != loader.CurrentName && window != null)
                        {
                            window.Update(string.Format("플레이 리스트 읽는 중 ({0}/{1})\n{2}", loader.CurrentIndex + 1, loader.PlaylistItemCount, System.IO.Path.GetFileName(loader.CurrentName)));

                            previous = loader.CurrentName;
                        }
                        Thread.Sleep(17);
                    }
                });

                th.Start();

                foreach(FileInfo targetFile in files)
                {
                    Logger.Log("Open AutoSave Playlist: " + targetFile.Name.ToString());

                    loader = new PlaylistLoader(targetFile.FullName);

                    np.Playlists.Add(loader.LoadPlaylist());
                }

                threadon = false;

                th.Join();
                th = null;
            }
        }

        #endregion Loading

        #region Metadata

        Thread MetadataSearcherThread;
        Thread LyricSearcherThread;
        Thread PlotSearcherThread;
        bool MetadataSearchCancelPadding = false;

        private void SearchMetadataStart()
        {
            if(MetadataSearcherThread != null)
            {
                MetadataSearcherThread.Abort();
                MetadataSearcherThread = null;
            }

            if(singer != null)
            {
                singer.Dispose();
                singer = null;
            }
            if(dlrenderer != null)
            {
                dlrenderer.Close();
                dlrenderer = null;
                MemoryManagement.FlushMemory();
            }

            MetadataSearcherThread = new Thread(new ThreadStart(SearchMetadata));
            MetadataSearcherThread.IsBackground = true;
            MetadataSearcherThread.Start();
        }

        private void SearchMetadata()
        {
            if (np.CurrentMusic.Tag != null)
            {
                MusicMetadata meta = new MusicMetadata(np.CurrentMusic.Tag.Title, np.CurrentMusic.Tag.Artist, np.CurrentMusic.Tag.Album, "", np.CurrentMusic.FileName);

                Song song = null;
                QueryResult r = SongSearcher.Search(meta);

                if (r.Success && r.Tag != null)
                {
                    song = ((SongCollection)r.Tag).TargetSong;
                }

                if (!MetadataSearchCancelPadding)
                {
                    if (LyricSearcherThread != null)
                    {
                        LyricSearcherThread.Abort();
                        LyricSearcherThread = null;
                    }

                    LyricSearcherThread = new Thread(new ThreadStart(() => { LyricSearchStart(meta, song); }));
                    LyricSearcherThread.IsBackground = true;
                    LyricSearcherThread.Start();

                    if (PlotSearcherThread != null)
                    {
                        PlotSearcherThread.Abort();
                        PlotSearcherThread = null;
                    }
                    PlotSearcherThread = new Thread(new ThreadStart(() => { PlotSearchStart(meta, song); }));
                    PlotSearcherThread.IsBackground = true;
                    PlotSearcherThread.Start();
                }
            }
        }

        private void SearchMetadataStop()
        {
            MetadataSearchCancelPadding = true;

            if(MetadataSearcherThread != null)
            { 
                MetadataSearcherThread.Abort();
                MetadataSearcherThread = null;
            }

            if (LyricSearcherThread != null) 
            {
                LyricSearcherThread.Abort();
                LyricSearcherThread = null;
            }

            if (PlotSearcherThread != null)
            {
                PlotSearcherThread.Abort();
                PlotSearcherThread = null;
            }

            MetadataSearchCancelPadding = false;
        }

        #endregion Metadata

        #region Editor Common

        bool isEditor = false;
        PlaylistOrder Editor_PreOrder;
        WindowMode Editor_PreWM;
        WindowState Editor_PreWS;
        bool Editor_PreFPS = false;
        int Editor_PlaylistIndex = 0;
        int Editor_ItemIndex = 0;
        int Editor_PreLentancy = 0;
        List<DSPBase> Editor_PreDSPs;

        private void InitEditor()
        {
            Editor_PreWM = WindowMode;
            WindowMode = WindowMode.Small;

            Editor_PreWS = WindowState;
            WindowState = WindowState.Minimized;

            Editor_PreFPS = showFpsLogger;
            showFpsLogger = false;

            Editor_PreOrder = np.CurrentPlaylist.Order;
            np.CurrentPlaylist.Order = PlaylistOrder.RepeatOne;

            Editor_PreLentancy = np.DesiredLatency;

            if (np.isPlay)
            {
                np.Stop();
            }

            Editor_PreDSPs = new List<DSPBase>();
            Editor_PreDSPs.AddRange(np.DSPs);

            np.DSPs.Clear();

            if (singer != null)
            {
                singer.Dispose();
                singer = null;
            }

            if (dlrenderer != null)
            {
                dlrenderer.Close();
                dlrenderer = null;
                MemoryManagement.FlushMemory();
            }

            if (sw != null)
            {
                sw.WindowState = WindowState.Minimized;
                sw.Hide();
            }

            if (miniControlWindow != null)
            {
                miniControlWindow.WindowState = WindowState.Minimized;
                miniControlWindow.Hide();
            }

            if(np.CurrentPlaylist != null)
                Editor_ItemIndex = np.CurrentPlaylist.Index;

            Editor_PlaylistIndex = np.CurrentPlaylistIndex;

            np.RenderWaveform = false;
            isEditor = true;

            Hide();
        }

        private void ExitEditor()
        {
            if (np.isPlay)
            {
                np.Stop();
            }

            np.DSPs.Clear();
            np.DSPs.AddRange(Editor_PreDSPs);
            Editor_PreDSPs = null;

            WindowMode = Editor_PreWM;

            showFpsLogger = Editor_PreFPS;

            isEditor = false;

            np.DesiredLatency = Editor_PreLentancy;

            np.RenderWaveform = true;
            
            Show();

            if(miniControlWindow != null)
            {
                miniControlWindow.Show();
                miniControlWindow.WindowState = WindowState.Normal;
            }

            if (sw != null)
            {
                sw.Show();
                sw.WindowState = WindowState.Normal;
            }

            WindowState = Editor_PreWS;

            if (np.CurrentPlaylist != null)
            {
                np.CurrentPlaylist.Order = Editor_PreOrder;

                np.Play(np.CurrentPlaylist.Index);
            }
        }

        #endregion Editor Common

        #region Dance

        DanceLiteRenderer dlrenderer;
        PlotLite MusicPlot;

        private void InitPlotEditor(int playlistIndex, int itemIndex)
        {
            np.CurrentPlaylistIndex = playlistIndex;

            InitEditor();

            np.DesiredLatency = 100;

            np.Play(itemIndex);

            Hide();
        }

        public void NewPlot(int playlistIndex, int itemIndex)
        {
            InitPlotEditor(playlistIndex, itemIndex);

            Dancer.DanceMain DE_Main = new Dancer.DanceMain(this, ref np, Editor_PlaylistIndex, Editor_ItemIndex);
            DE_Main.Closed += DE_Main_Closed;
            DE_Main.Show();
        }

        public void PlotEdit(int playlistIndex, int itemIndex, string workingDirectory)
        {
            InitPlotEditor(playlistIndex, itemIndex);

            Dancer.DanceMain DE_Main = new Dancer.DanceMain(this, ref np, Editor_PlaylistIndex, Editor_ItemIndex, true, workingDirectory);
            DE_Main.Closed += DE_Main_Closed;
            DE_Main.Show();
        }

        private void DE_Main_Closed(object sender, EventArgs e)
        {
            ExitEditor();
        }

        /// <summary>
        /// 탐색후 재생
        /// </summary>
        private void PlotSearchStart(MusicMetadata meta, Song song)
        {
            PlotSearchResult result =  PlSearch.Search(meta);
            switch (result.Exist)
            {
                case PlotExistState.UnsureLocal:
                    PlayPlot(result.LocalPath);
                    break;
                case PlotExistState.LocalExist:
                    PlayPlot(result.LocalPath);
                    break;
                case PlotExistState.GlobalExist:
                    PlayPlot(result.LocalPath);
                    break;
                case PlotExistState.WebExitst:
                    break;
                case PlotExistState.None:
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        if (dlrenderer != null)
                        {
                            dlrenderer.Close();
                            dlrenderer = null;
                            MemoryManagement.FlushMemory();
                        }
                    }));
                    break;
                default:
                    break;
            }
        }

        private void PlayPlot(string path)
        {
            MusicPlot = null;
            MusicPlot = new PlotLite(path);

            this.Dispatcher.Invoke(
                new Action(() => 
                {
                    if (ComposerUse)
                    {
                        if (dlrenderer != null)
                        {
                            dlrenderer.Close();
                            dlrenderer = null;
                            MemoryManagement.FlushMemory();

                            dlrenderer = new DanceLiteRenderer(this, np, MusicPlot);
                        }
                        else
                        {
                            dlrenderer = new DanceLiteRenderer(this, np, MusicPlot);
                        }
                        dlrenderer.WindowMode = ComposerWindowMode;

                        dlrenderer.Show();

                        dlrenderer.Topmost = ComposerTopmost;
                        dlrenderer.Opacity = ComposerOpacity;
                    }
                    else
                    {
                        if (dlrenderer != null)
                        {
                            dlrenderer.Close();
                            dlrenderer = null;
                            MemoryManagement.FlushMemory();
                        }
                    }
                }));
        }

        #endregion

        #region Lyric

        private void InitLyricEditor(int playlistIndex , int itemIndex)
        {
            np.CurrentPlaylistIndex = playlistIndex;

            InitEditor();

            np.DesiredLatency = 100;

            np.Play(itemIndex);

            Hide();
        }

        public void NewLyric(int playlistIndex, int itemIndex)
        {
            InitLyricEditor(playlistIndex, itemIndex);

            LyricsEditor LE = new LyricsEditor(ref np, this, new MusicMetadata(np.CurrentMusic.Tag.Title, np.CurrentMusic.Tag.Album, np.CurrentMusic.Tag.Artist, LanguageHelper.FindText("Lang_Unknown"), np.CurrentMusic.FileName));
            LE.Closed += LE_Closed;
        }

        public void LyricEdit(int playlistIndex, int itemIndex, string workingDirectory)
        {
            InitLyricEditor(playlistIndex, itemIndex);

            Lyrics.LyricsEditor LE = new Lyrics.LyricsEditor(ref np, this, System.IO.Path.Combine(workingDirectory,"lyric.xml"));
            LE.Closed += LE_Closed;
        }

        private void LE_Closed(object sender, EventArgs e)
        {
            ExitEditor();
        }

        Singer singer;
        Lyric Lyric;

        private void PlayLyric(string path)
        {
            Lyric Lyric;
            bool okay = LyricLoader.Load(out Lyric, Path.Combine(path, "lyric.xml"));
            Dispatcher.Invoke(
                new Action(() => 
                {
                    if (okay)
                    {
                        this.Lyric = Lyric;
                        CreateSinger(Lyric);
                    }
                    else
                    {
                        if (singer != null)
                        {
                            singer.Dispose();
                            singer = null;
                        }
                        this.Lyric = null;
                    }
                }));
        }

        private void CreateSinger(Lyric Lyric)
        {
            if (SingerShow)
            {
                if (singer == null)
                {
                    singer = new Singer(ref Lyric, np, this, SingerCanDragmove, SingerWindowMode);
                    singer.Zoom = SingerZoom;
                    singer.Refresh();
                    singer.Closed += Singer_Closed;
                    singer.Show(DummyToolWindow);
                }
                else
                {
                    singer.Init(ref Lyric);
                    singer.Zoom = SingerZoom;
                }

                singer.Optimize = true;
                singer.Topmost = true;
                singer.Opacity = SingerOpacity;
                singer.HorizontalAlignment = SingerHorizontalAlignment;
                singer.VerticalAlignment = SingerVerticalAlignment;

                if (!SingerResetPosition && SingerTop != double.NegativeInfinity)
                {
                    singer.Top = SingerTop;
                    singer.Left = SingerLeft;
                }

                if (SingerCanDragmove || !SingerResetPosition)
                {
                    singer.ResetPosition = false;
                }
                else
                {
                    singer.ResetPosition = true;
                }

                if (SingerResetPosition)
                {
                    singer.SetScreen(singer.scrIndex);
                }
            }
            else
            {
                if (singer != null)
                {
                    singer.Dispose();
                    singer = null;
                }
            }
        }

        private void Singer_Closed(object sender, EventArgs e)
        {
            SingerTop = singer.Top;
            SingerLeft = singer.Left;
        }

        private void LyricSearchStart(MusicMetadata meta, Song song)
        {
            LyricSearchResult localResult = LyricHelper.SearchLocal(meta);

            if (localResult.Exist == LyricExistState.LocalExist)
            {
                PlayLyric(localResult.LocalPath);
            }
            else
            {
                LyricSearchResult result = LyricHelper.SearchWeb(meta, song, localResult);

                switch (result.Exist)
                {
                    case LyricExistState.UnsureLocal:
                        PlayLyric(result.LocalPath);
                        break;
                    case LyricExistState.LocalExist:
                        PlayLyric(result.LocalPath);
                        break;
                    case LyricExistState.GlobalExist:
                        PlayLyric(result.LocalPath);
                        break;
                    case LyricExistState.WebExitst:
                        Server.LyricDownloader downloader = new Server.LyricDownloader();
                        QueryResult r = downloader.Download(result.RegisteredLyric);
                        if (r.Success)
                        {
                            PlayLyric((string)r.Tag);
                        }
                        break;
                    case LyricExistState.None:
                        Dispatcher.Invoke(
                            new Action(() =>
                            {
                                if (singer != null)
                                {
                                    singer.Dispose();
                                    singer = null;
                                }
                            }));
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
