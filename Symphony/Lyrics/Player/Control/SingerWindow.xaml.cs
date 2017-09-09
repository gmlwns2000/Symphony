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
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Interop;

namespace Symphony.Lyrics
{
    /// <summary>
    /// Singer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SingerWindow : Window
    {
        bool dragMoveble = true;
        MainWindow mw;

        public SingerWindow(bool dragMoveble, MainWindow mw = null)
        {
            this.dragMoveble = dragMoveble;

            InitializeComponent();

            SizeChanged += SingerWindow_SizeChanged;

            if (mw == null)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;

                HorizontalAlignment = HorizontalAlignment.Center;
                VerticalAlignment = VerticalAlignment.Center;
            }

            this.mw = mw;

            Closed += SingerWindow_Closed;
        }

        private void SingerWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double cw = e.NewSize.Width - e.PreviousSize.Width;
            double ch = e.NewSize.Height - e.PreviousSize.Height;

            Top -= ch / 2;
            Left -= cw / 2;
        }

        private void SingerWindow_Closed(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }

        public void Show(Window OwnerWindow)
        {
            Owner = OwnerWindow;

            Show();
        }

        public new void Show()
        {
            base.Show();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!dragMoveble)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;

                UI.WindowTransclick.set(hwnd);
            }
        }

        #region dragmove

        DispatcherTimer timer;

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1);
                timer.Tick += Timer_Tick;
            }

            if (timer.IsEnabled)
            {
                timer.Stop();
            }

            timer.Start();

            if (mw != null)
            {
                mw.SetRenderUI(false);
            }

            DragMove();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Released)
            {
                if (mw != null)
                {
                    mw.SetRenderUI(true);
                }

                timer.Stop();
            }
        }

        #endregion dragmove

        public int scrIndex = -1;
        public Thickness SingerMargin = new Thickness(24);

        public void SetScreen(int index)
        {
            double Top, Left, ScrWidth, ScrHeight;

            scrIndex = index;

            PresentationSource source = PresentationSource.FromVisual(this);

            double dpiY;

            if (source == null)
            {
                return;
            }
            else
            {
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }

            System.Windows.Forms.Screen target;

            if (index < 0 || index > System.Windows.Forms.Screen.AllScreens.Length - 1)
            {
                target = System.Windows.Forms.Screen.PrimaryScreen;
            }
            else
            {
                target = System.Windows.Forms.Screen.AllScreens[index];
            }

            Top = target.WorkingArea.Y / dpiY;
            Left = target.WorkingArea.X / dpiY;
            ScrWidth = target.WorkingArea.Width / dpiY;
            ScrHeight = target.WorkingArea.Height / dpiY;

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Left = Left + SingerMargin.Left;
                    break;
                case HorizontalAlignment.Center:
                    Left = Left + ScrWidth / 2 - Width / 2;
                    break;
                case HorizontalAlignment.Right:
                    Left = Left + ScrWidth - Width - SingerMargin.Left;
                    break;
                default:
                    break;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    Top = Top + SingerMargin.Top;
                    break;
                case VerticalAlignment.Center:
                    Top = Top + ScrHeight / 2 - Height / 2;
                    break;
                case VerticalAlignment.Bottom:
                    Top = Top + ScrHeight - Height - SingerMargin.Bottom;
                    break;
                default:
                    break;
            }

            this.Left = Left;
            this.Top = Top;
        }
    }
}
