using Symphony.Player;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.UI
{
    /// <summary>
    /// RemoteControllerWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RemoteControllerWindow : Window
    {
        public Util.Settings Settings { get; set; } = Util.Settings.Current;

        private static int InstanceCount = 0;
        public int UID;

        bool inited = false;
        MainWindow mw;
        PlayerCore np;

        BitmapImage Img_Play;
        BitmapImage Img_Pause;

        BitmapImage Img_Pin_Pinned;
        BitmapImage Img_Pin_UnPinned;

        BitmapImage Img_AlbumArt;

        ImageBrush Brush_Play;
        ImageBrush Brush_Pause;

        ImageBrush Brush_Pin_Pinned;
        ImageBrush Brush_Pin_UnPinned;

        Storyboard PopupOff;
        Storyboard MouseFocus;
        Storyboard MouseUnFocus;
        Storyboard ImageOff;
        Storyboard ImageOn;

        DispatcherTimer timer;

        public RemoteControllerWindow(MainWindow mw, PlayerCore np)
        {
            UID = InstanceCount;
            InstanceCount++;

            InitializeComponent();

            orderSelector.OrderChanged += OrderSelector_OrderChanged;

            this.mw = mw;
            this.np = np;

            Img_Play = (BitmapImage)FindResource("Img_Play");
            Img_Pause = (BitmapImage)FindResource("Img_Pause");

            Img_Pin_Pinned = (BitmapImage)FindResource("Img_Pin_Pinned");
            Img_Pin_UnPinned = (BitmapImage)FindResource("Img_Pin_UnPinned");

            Brush_Play = new ImageBrush(Img_Play);
            Brush_Play.Freeze();
            Brush_Pause = new ImageBrush(Img_Pause);
            Brush_Pause.Freeze();

            Brush_Pin_Pinned = new ImageBrush(Img_Pin_Pinned);
            Brush_Pin_Pinned.Freeze();
            Brush_Pin_UnPinned = new ImageBrush(Img_Pin_UnPinned);
            Brush_Pin_UnPinned.Freeze();

            PopupOff = (Storyboard)FindResource("PopupOff");
            PopupOff.Completed += PopupOff_Completed;

            MouseFocus = (Storyboard)FindResource("MouseFocus");
            MouseFocus.Completed += MouseFocus_Completed;
            MouseUnFocus = (Storyboard)FindResource("MouseUnFocus");

            ImageOff = (Storyboard)FindResource("ImageOff");
            ImageOff.Completed += ImageOff_Completed;
            ImageOn = (Storyboard)FindResource("ImageOn");

            np.PlaySeeked += Update;
            np.PlayPaused += Update;
            np.PlayResumed += Update;
            np.PlayStarted += Update;
            np.PlayStopped += Update;

            mw.UpdateAllowChanged += Mw_UpdateAllowChanged;

            Closed += RemoteControllerWindow_Closed;
            StateChanged += RemoteControllerWindow_StateChanged;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            Update();
        }

        private bool mwOrderCall = false;
        private void OrderSelector_OrderChanged(object sender, PlaylistOrder e)
        {
            if (mwOrderCall)
                return;

            mw.Bt_Small_Order.PlaylistOrder = e;
        }

        public void UpdateOrder(PlaylistOrder o)
        {
            mwOrderCall = true;

            orderSelector.PlaylistOrder = o;

            mwOrderCall = false;
        }

        private void RemoteControllerWindow_StateChanged(object sender, EventArgs e)
        {
            if(WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            else if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }

        private void Mw_UpdateAllowChanged(object sender, bool e)
        {
            if (textAni != null)
            {
                if (e)
                {
                    textAni.Resume();
                }
                else
                {
                    textAni.Pause();
                }
            }
        }

        private void MouseFocus_Completed(object sender, EventArgs e)
        {
            mousefocusOn = false;
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        public new void Close()
        {
            PopupOff.Begin();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (inited)
            {
                if (np.isPlay)
                {
                    Sld_Position.Value = np.GetPosition().TotalMilliseconds;

                    Sld_Position.Maximum = np.GetLength().TotalMilliseconds;
                }

                if (Tb_Title.Text != mw.Lb_Small_Title.Text)
                {
                    Tb_Title.Text = mw.Lb_Small_Title.Text;
                }

                if (Sld_Volume.Value != Settings.AudioVolume)
                {
                    Sld_Volume.Value = Settings.AudioVolume;
                }
            }
        }

        private void RemoteControllerWindow_Closed(object sender, EventArgs e)
        {
            np.PlaySeeked -= Update;
            np.PlayPaused -= Update;
            np.PlayResumed -= Update;
            np.PlayStarted -= Update;
            np.PlayStopped -= Update;

            mw.UpdateAllowChanged -= Mw_UpdateAllowChanged;

            if(textAni != null)
            {
                textAni.Stop();
                textAni = null;
            }

            timer.Stop();
            timer = null;
        }

        public void Update()
        {
            inited = false;

            Dispatcher.Invoke(new Action(() => 
            {
                Sld_Volume.Value = Settings.AudioVolume;

                if(np.CurrentPlaylist != null)
                    UpdateOrder(np.CurrentPlaylist.Order);

                if (np.isPlay)
                {
                    Sld_Position.Value = np.GetPosition().TotalMilliseconds;
                    Sld_Position.Maximum = np.GetLength().TotalMilliseconds;
                }

                if (np.isPaused)
                {
                    Bt_Play.Background = Brush_Play;
                }
                else
                {
                    Bt_Play.Background = Brush_Pause;
                }

                if (!np.isPlay)
                {
                    Bt_Play.Background = Brush_Play;
                }

                if (Topmost)
                {
                    Bt_Topmost.Background = Brush_Pin_Pinned;
                }
                else
                {
                    Bt_Topmost.Background = Brush_Pin_UnPinned;
                }

                if (Tb_Title.Text != mw.Lb_Small_Title.Text)
                {
                    Tb_Title.Text = mw.Lb_Small_Title.Text;
                }

                if(Sld_Volume.Value != Settings.AudioVolume)
                {
                    Sld_Volume.Value = Settings.AudioVolume;
                }
            }));

            inited = true;
        }

        private void Sld_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                Settings.AudioVolume = e.NewValue;
            }
        }

        private void Sld_Position_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (np.isPlay)
                {
                    np.SetPosition((int)e.NewValue);
                }
            }
        }

        private void Bt_Close_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                Close();
            }
        }

        private void Bt_Topmost_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                Topmost = !Topmost;

                Update();
            }
        }

        private void Bt_Previous_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                np.Previous();
            }
        }

        private void Bt_Play_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                if (np.isPlay)
                {
                    np.Pause(!np.isPaused);
                }
                else
                {
                    np.Next();
                }

                Update();
            }
        }

        private void Bt_Next_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                np.Next();

                Update();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                mw.StopRenderingWhileClicking();

                DragMove();
            }
            else
            {
                if (mw.WindowState == WindowState.Minimized)
                {
                    mw.WindowState = WindowState.Normal;

                    mw.Activate();
                }
                else if(mw.WindowState == WindowState.Maximized)
                {
                    mw.Activate();
                }
                else if(mw.WindowState == WindowState.Normal)
                {
                    mw.WindowState = WindowState.Minimized;
                }
            }
        }

        Storyboard textAni;

        private void Tb_Title_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(textAni != null)
            {
                textAni.Stop();
                textAni = null;
            }
            
            double len = Math.Max(0, Tb_Title.ActualWidth - titleParent.ActualWidth);

            if (len == 0)
            {
                Tb_Title.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                Tb_Title.HorizontalAlignment = HorizontalAlignment.Left;

                Storyboard sb = new Storyboard();

                ThicknessAnimation ani = new ThicknessAnimation(new Thickness(0), new Thickness(-len, 0, 0, 0), new Duration(TimeSpan.FromSeconds(Math.Max(3.5, len / 7))));
                ani.AutoReverse = true;

                Storyboard.SetTargetProperty(ani, new PropertyPath(MarginProperty));
                Storyboard.SetTarget(ani, Tb_Title);

                sb.Children.Add(ani);

                sb.RepeatBehavior = RepeatBehavior.Forever;

                Timeline.SetDesiredFrameRate(sb, 7);

                sb.Freeze();

                sb.Begin();

                textAni = sb;
            }
        }

        DispatcherTimer mouseleave;
        bool mousefocusOn = false;
        private void wd_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!mousefocusOn)
            {
                MouseFocus.Begin();

                mousefocusOn = true;
            }

            if (mouseleave == null)
            {
                mouseleave = new DispatcherTimer();
                mouseleave.Interval = TimeSpan.FromSeconds(1.5);
                mouseleave.Tick += delegate (object obj, EventArgs arg)
                {
                    MouseFocus.Stop();

                    MouseUnFocus.Begin();

                    mouseleave.Stop();
                };
            }

            if (mouseleave.IsEnabled)
            {
                mouseleave.Stop();
            }

            mouseleave.Start();
        }

        private void wd_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            mw.HotKeyDown(sender, e);
        }
        
        public void UpdateAlbumArt(BitmapImage image)
        {
            Img_AlbumArt = image;

            Dispatcher.Invoke(delegate
            {
                if (Settings.UseImageAnimation)
                {
                    ImageOff.Begin();
                }
                else
                {
                    ImageOff.Begin(this, true);
                    ImageOff.SkipToFill();
                }
            });
        }

        private void ImageOff_Completed(object sender, EventArgs e)
        {
            Img_Back_AlbumArt.Source = Img_AlbumArt;

            if (Img_AlbumArt != null)
            {
                if (Settings.UseImageAnimation)
                {
                    ImageOn.Begin();
                }
                else
                {
                    ImageOn.Begin(this, true);
                    ImageOn.SkipToFill();
                }
            }
        }
    }
}
