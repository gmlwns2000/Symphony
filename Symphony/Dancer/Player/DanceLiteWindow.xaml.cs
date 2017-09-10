using MMF.Matricies.Camera.CameraMotion;
using MMF.Model.PMX;
using MMF.Motion;
using Symphony.Player;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.Dancer
{
    /// <summary>
    /// DanceLiteRenderer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DanceLiteWindow : Window
    {
        PlotLite pl;
        PlayerCore np;
        WindowResizer wr;
        Brush borderBrush;
        MainWindow mw;

        public DanceLiteControl control;

        public DanceLiteWindow(DanceLiteControl control, MainWindow mw, PlayerCore np)
        {
            this.mw = mw;
            this.np = np;

            wr = new WindowResizer(this);
            wr.Margin = 5;

            InitializeComponent();

            this.control = control;
            grid.Children.Clear();
            grid.Children.Add(control);

            borderBrush = new SolidColorBrush(Color.FromArgb(66, 0, 0, 0));
            borderBrush.Freeze();

            Closed += DanceLiteRenderer_Closed;

            Show();
        }

        #region Window

        private void DanceLiteRenderer_Closed(object sender, EventArgs e)
        {
            grid.Children.Clear();
        }

        DispatcherTimer timer;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                Point pt = e.GetPosition(this);

                if (pt.Y < wr.Margin || pt.X < wr.Margin || pt.X > ActualWidth - wr.Margin || pt.Y > ActualHeight - wr.Margin)
                {
                    wr.resizeWindow();
                }
                else if(Keyboard.Modifiers == ModifierKeys.None)
                {
                    if (timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromMilliseconds(17);
                        timer.Tick += Timer_Tick;
                    }

                    if (timer.IsEnabled)
                    {
                        timer.Stop();
                    }
                    timer.Start();

                    if(control.RenderControl != null)
                        control.RenderControl.AllowRendering = false;

                    DragMove();
                }
            }

            SolidColorBrush scb = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255));
            scb.Freeze();
            Background = scb;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Released)
            {
                Background = Brushes.Transparent;

                if (control.RenderControl != null && control.motion != null)
                {
                    control.RenderControl.AllowRendering = true;
                    if (np.isPaused)
                    {
                        if (control.motion != null)
                        {
                            control.motion.Stop();
                        }
                    }
                    else
                    {
                        control.Np_PlaySeeked();
                    }
                }

                timer.Stop();
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Background = Brushes.Transparent;
        }

        DispatcherTimer timerBorder;
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(this);

            wr.MouseMove(pt, ActualWidth, ActualHeight);

            if(timerBorder == null)
            {
                timerBorder = new DispatcherTimer();
                timerBorder.Interval = TimeSpan.FromMilliseconds(750);
                timerBorder.Tick += TimerBorder_Tick;
            }
            if (timerBorder.IsEnabled)
            {
                timerBorder.Stop();
            }
            border.BorderBrush = borderBrush;
            timerBorder.Start();
        }

        private void TimerBorder_Tick(object sender, EventArgs e)
        {
            border.BorderBrush = Brushes.Transparent;
        }

        #endregion Window
    }
}
