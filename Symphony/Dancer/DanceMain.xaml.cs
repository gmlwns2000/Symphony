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
using Symphony;
using Symphony.UI;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Win32;
using System.IO;

namespace Symphony.Dancer
{
    /// <summary>
    /// DanceMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DanceMain : Window
    {
        MainWindow mw;
        Window w;
        PlayerCore np;
        WindowResizer wr;
        DispatcherTimer timer;
        ImageBrush Brush_Play;
        ImageBrush Brush_Pause;
        BitmapImage Img_Play;
        BitmapImage Img_Pause;
        Plot Plot;
        Composer composer;
        DispatcherTimer DragMoveTimer;

        int scrIndex = -1;
        int PlaylistIndex = -1;
        int ItemIndex = -1;

        /// <summary>
        /// 댄서 애디터의 매인 윈도우입니다. 애디팅을 하려면 editMode와 workingDirectory를 채워 주십시오.
        /// </summary>
        public DanceMain(MainWindow mw, ref PlayerCore np, int playlistIndex, int itemIndex, bool editMode = false, string workingDirectory = "")
        {
            //init
            InitializeComponent();
            this.mw = mw;
            mw.Closed += Mw_Closed;
            Closed += DanceMain_Closed;
            this.np = np;
            Brush_Play = FindResource("Brush_Play") as ImageBrush;
            Brush_Pause = FindResource("Brush_Pause") as ImageBrush;
            np.PlayPaused += UpdatePlay;
            np.PlayResumed += UpdatePlay;
            wr = new WindowResizer(this);

            DragMoveTimer = new DispatcherTimer();
            DragMoveTimer.Interval = TimeSpan.FromMilliseconds(33);
            DragMoveTimer.Tick += DragMoveTimer_Tick;

            //load
            PlaylistIndex = playlistIndex;
            ItemIndex = itemIndex;

            if (editMode)
            {
                PlotLoader.Load(workingDirectory, out Plot);
            }
            else
            {
                Plot = new Plot(new Server.MusicMetadata(np.CurrentMusic.Tag.Title, np.CurrentMusic.Tag.Artist, np.CurrentMusic.Tag.Album, PlotHelper.TextUnknown, np.CurrentMusic.FileName), new Ratio(16, 9));
            }

            if (!Plot.Inited)
            {
                DialogMessage.Show(null, "플롯을 초기화 하지 못햇습니다.", "오류", DialogMessageType.Okay);
                Close();
                return;
            }

            composer = new Composer(ref Plot, ref np);
            composer.RendererClosed += Composer_RendererClosed;

            //setup Editor
            Loaded += DanceMain_Loaded;
            Closing += DanceMain_Closing;

            Img_Pause = FindResource("Img_Pause") as BitmapImage;
            Img_Play = FindResource("Img_Play") as BitmapImage;

            Bar_Next.Click += Bar_Next_Click;
            Bar_NextSkip.Click += Bar_NextSkip_Click;
            Bar_Play.Click += Bar_Play_Click;
            Bar_PreSkip.Click += Bar_PreSkip_Click;
            Bar_Previous.Click += Bar_Previous_Click;

            w = new Window();
            w.Top = -100;
            w.Left = -100;
            w.Width = 1;
            w.Height = 1;
            w.WindowStyle = WindowStyle.ToolWindow;
            w.ShowInTaskbar = false;
            w.Show();
            
            w.Hide();

            UpdatePlay();
        }

        #region Window Control

        private void DanceMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DialogMessageResult result = DialogMessage.Show(this, "정말로 에디터를 종료 하시겠습니까?\n저장하지않은 데이터는 사라집니다.", "확인", DialogMessageType.YesNo);

            if (result == DialogMessageResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Mw_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void Composer_RendererClosed(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DanceMain_Closed(object sender, EventArgs e)
        {
            np.Pause(true);
            np.Stop();

            if (timer.IsEnabled)
            {
                timer.Stop();
            }
            if (DragMoveTimer.IsEnabled)
            {
                DragMoveTimer.Stop();
            }

            np.PlayPaused -= UpdatePlay;
            np.PlayResumed -= UpdatePlay;

            if (Plot.Inited)
            {
                composer.Destroy();

                w.Close();
            }

            w = null;

            composer = null;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(Grid_Position);
            if (pt.Y > 0 && pt.Y < 50)
            {
                //TODO: RENDER STOP
                if (DragMoveTimer.IsEnabled) { DragMoveTimer.Stop(); }
                DragMoveTimer.Start();
                wr.dragWindow();
            }
        }

        private void DragMoveTimer_Tick(object sender, EventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Released)
            {
                DragMoveTimer.Stop();
                //TODO: RENDER RESTART
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(Grid_Background);
            wr.MouseMove(pt, Grid_Background.ActualWidth, Grid_Background.ActualHeight);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(Grid_Background);
            if(pt.X < 0 || pt.X > Grid_Background.ActualWidth ||
               pt.Y < 0 || pt.Y > Grid_Background.ActualHeight)
            {
                wr.resizeWindow();
            }
        }

        #endregion Window region

        #region Player Control

        private void DanceMain_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(np != null && np.isPlay)
            {
                Bar_Position.Maximum = np.GetLength(TimeUnit.MilliSecond);
                Bar_Position.Value = np.GetPosition(TimeUnit.MilliSecond);
            }
        }

        private void Bar_Previous_Click(object sender, EventArgs e)
        {
            Bt_Previous_Click(sender, new RoutedEventArgs());
        }

        private void Bar_PreSkip_Click(object sender, EventArgs e)
        {
            Bt_PreSkip_Click(sender, new RoutedEventArgs());
        }

        private void Bar_Play_Click(object sender, EventArgs e)
        {
            Bt_Play_Click(sender, new RoutedEventArgs());
        }

        private void Bar_NextSkip_Click(object sender, EventArgs e)
        {
            Bt_NextSkip_Click(sender, new RoutedEventArgs());
        }

        private void Bar_Next_Click(object sender, EventArgs e)
        {
            Bt_Next_Click(sender, new RoutedEventArgs());
        }

        private void Bt_Previous_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Clamp(composer.Renderer.position - 1000, 0, np.GetLength(TimeUnit.MilliSecond)));
            np.Pause(true);
        }

        private void Bt_PreSkip_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Clamp(composer.Renderer.position-3000,0,np.GetLength(TimeUnit.MilliSecond)));
        }

        private void Bt_Play_Click(object sender, RoutedEventArgs e)
        {
            np.PlayPause();
        }

        private void Bt_NextSkip_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Clamp(composer.Renderer.position + 3000, 0, np.GetLength(TimeUnit.MilliSecond)));
        }

        private void Bt_Next_Click(object sender, RoutedEventArgs e)
        {
            np.SetPosition((int)Clamp(composer.Renderer.position + 1000, 0, np.GetLength(TimeUnit.MilliSecond)));
            np.Pause(true);
        }

        private void Bar_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            np.SetPosition((int)e.NewValue);
        }

        private double Clamp(double val, double min, double max)
        {
            return Math.Max(min, Math.Min(max, val));
        }

        private void UpdatePlay()
        {
            if(np != null && np.isPlay)
            {
                if (np.isPaused)
                {
                    Bt_Play.Background = Brush_Play;
                    Bar_Play.ImageSource = Img_Play;
                }
                else
                {
                    Bt_Play.Background = Brush_Pause;
                    Bar_Play.ImageSource = Img_Pause;
                }
            }
        }

        #endregion Player Control
        
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                composer.Renderer.GetFocus();
                Topmost = true;
                Topmost = false;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            composer.Renderer.GetFocus();
            Topmost = true;
            Topmost = false;
        }

        private void UpdateList()
        {
            Lst_Data.ItemsSource = Plot.Instances;
            Lst_Data.Items.Refresh();
        }

        private void Bt_Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "열기";
            ofd.Filter = "PMX모델 파일(*.pmx)|*.pmx";

            MikuInstance inst = null;
            try
            {
                if (ofd.ShowDialog() == true)
                {
                    //COPY PMX
                    string originalPMX = System.IO.Path.GetDirectoryName(ofd.FileName);
                    string[] pmxspl = originalPMX.Split(System.IO.Path.DirectorySeparatorChar);
                    string pmxCopy = System.IO.Path.Combine(Plot.WorkingDirectory, "PMX", pmxspl[pmxspl.Length - 1]);
                    string pmxRelative = System.IO.Path.Combine("PMX", pmxspl[pmxspl.Length - 1], System.IO.Path.GetFileName(ofd.FileName));
                    Util.IO.DirectoryCopy(originalPMX, pmxCopy);

                    OpenFileDialog ofd2 = new OpenFileDialog();
                    ofd2.Filter = "vmd 모션 파일 (* .vmd) | * .vmd";
                    if (ofd2.ShowDialog() == true)
                    {
                        //COPY VMD
                        string originalVMD = System.IO.Path.GetFileName(ofd2.FileName);
                        string copyDist = System.IO.Path.Combine(Plot.WorkingDirectory, "VMD");
                        if (!Directory.Exists(copyDist))
                        {
                            Directory.CreateDirectory(copyDist);
                        }
                        File.Copy(ofd2.FileName, System.IO.Path.Combine(copyDist, originalVMD), true);

                        inst = new MikuInstance(System.IO.Path.GetFileNameWithoutExtension(ofd.FileName),
                            np.GetPosition(TimeUnit.MilliSecond), double.PositiveInfinity, pmxRelative, System.IO.Path.Combine("VMD", originalVMD));
                    }
                    else
                    {
                        inst = new MikuInstance(System.IO.Path.GetFileNameWithoutExtension(ofd.FileName),
                            np.GetPosition(TimeUnit.MilliSecond), double.PositiveInfinity, pmxRelative, null);
                    }
                    Plot.Instances.Add(inst);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(this, ex);
                UI.DialogMessage.Show(this, "모델을 불러오는 도중 오류가 발생했습니다.");
            }

            UpdateList();

            if(inst != null)
                composer.Renderer.AddInstance(inst);
        }

        private void Bt_Del_Click(object sender, RoutedEventArgs e)
        {
            while(Lst_Data.SelectedItems != null && Lst_Data.SelectedItems.Count > 0)
            {
                composer.Renderer.RemoveInstance(Plot.Instances[Lst_Data.SelectedIndex]);
                Plot.Instances.RemoveAt(Lst_Data.SelectedIndex);
                Lst_Data.Items.Refresh();
            }
        }

        private void Bt_Properties_Click(object sender, RoutedEventArgs e)
        {
            Lyrics.MetadataEditor me = new Lyrics.MetadataEditor(Plot.Metadata);
            me.Updated += delegate (object s, Lyrics.MetadataUpdated arg)
            {
                Plot.Metadata = arg.Metadata;
            };

            UserControlHostWindow host = new UserControlHostWindow(this, mw, "춤 정보 수정", me);
            host.ShowDialog();
        }

        private void Bt_Save_Click(object sender, RoutedEventArgs e)
        {
            //SAVE
        }

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Lst_ItemDoubleClick(object sender, EventArgs e)
        {
            //EDITOR
        }
    }
}
