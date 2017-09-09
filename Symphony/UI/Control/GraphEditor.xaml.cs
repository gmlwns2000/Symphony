using Symphony.Lyrics;
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
    public class KeySplineUpdatedArgs : EventArgs
    {
        public AnimationKeySpline KeySpline;

        public KeySplineUpdatedArgs(AnimationKeySpline ks)
        {
            KeySpline = ks;
        }
    }

    /// <summary>
    /// GraphEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GraphEditor : Window
    {
        Storyboard PopupOff;
        DispatcherTimer timerStart = new DispatcherTimer();
        DispatcherTimer timerEnd = new DispatcherTimer();

        AnimationKeySpline ks;
        public event EventHandler<KeySplineUpdatedArgs> Updated;

        public GraphEditor(Window Parent, AnimationKeySpline ks)
        {
            InitializeComponent();

            if (Parent != null)
            {
                Owner = Parent;
            }

            this.ks = ks;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            timerEnd.Interval = TimeSpan.FromMilliseconds(1);
            timerEnd.Tick += TimerEnd_Tick;

            timerStart.Interval = TimeSpan.FromMilliseconds(1);
            timerStart.Tick += TimerStart_Tick;

            Loaded += GraphEditor_Loaded;
        }

        private void GraphEditor_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePt();
        }

        private void UpdatePt()
        {
            Canvas.SetLeft(Cursor_Start, ks.ControlPoint1.X*canvas.ActualWidth - Cursor_Start.ActualWidth/2);
            Canvas.SetTop(Cursor_Start, (1-ks.ControlPoint1.Y)*canvas.ActualHeight - Cursor_Start.ActualHeight/2);
            Canvas.SetLeft(Cursor_End, ks.ControlPoint2.X * canvas.ActualWidth - Cursor_End.ActualWidth / 2);
            Canvas.SetTop(Cursor_End, (1 - ks.ControlPoint2.Y) * canvas.ActualHeight - Cursor_End.ActualHeight / 2);
            
            Line_Start.X1 = ks.ControlPoint1.X * canvas.ActualWidth;
            Line_Start.Y1 = (1 - ks.ControlPoint1.Y) * canvas.ActualHeight;
            Line_End.X1 = ks.ControlPoint2.X * canvas.ActualWidth;
            Line_End.Y1 = (1 - ks.ControlPoint2.Y) * canvas.ActualHeight;

            PathGeometry pg = new PathGeometry();

            PathFigure pf = new PathFigure();

            pf.StartPoint = new Point(0, canvas.ActualHeight);
            pf.IsClosed = false;
            pf.Segments.Add(new BezierSegment(new Point(ks.ControlPoint1.X * canvas.ActualWidth, (1 - ks.ControlPoint1.Y) * canvas.ActualHeight),
                new Point(ks.ControlPoint2.X * canvas.ActualWidth, (1 - ks.ControlPoint2.Y) * canvas.ActualHeight), new Point(canvas.ActualWidth, 0), true));

            pg.Figures.Add(pf);

            path.Data = pg;
        }

        private void TimerStart_Tick(object sender, EventArgs e)
        {
            //calcPos
            Point pt = Mouse.GetPosition(canvas);

            double x = Math.Max(0, Math.Min(1, pt.X / canvas.ActualWidth));
            double y = Math.Max(0, Math.Min(1, 1 - pt.Y / canvas.ActualHeight));

            ks = new AnimationKeySpline(new Point(x, y), ks.ControlPoint2);

            UpdatePt();

            Updated?.Invoke(this, new KeySplineUpdatedArgs(ks));

            if(Mouse.LeftButton == MouseButtonState.Released)
            {
                timerStart.Stop();
            }
        }

        private void TimerEnd_Tick(object sender, EventArgs e)
        {
            //calcPos
            Point pt = Mouse.GetPosition(canvas);

            double x = Math.Max(0, Math.Min(1, pt.X / canvas.ActualWidth));
            double y = Math.Max(0, Math.Min(1, 1 - pt.Y / canvas.ActualHeight));

            ks = new AnimationKeySpline(ks.ControlPoint1, new Point(x, y));

            UpdatePt();

            Updated?.Invoke(this, new KeySplineUpdatedArgs(ks));

            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                timerEnd.Stop();
            }
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopupOff.Begin();
        }

        private void Menu_General_New_Click(object sender, RoutedEventArgs e)
        {
            ks = new AnimationKeySpline();

            UpdatePt();

            Updated?.Invoke(this, new KeySplineUpdatedArgs(ks));
        }

        private void Menu_General_Close_Click(object sender, RoutedEventArgs e)
        {
            PopupOff.Begin();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Cursor_End_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (timerEnd.IsEnabled)
            {
                timerEnd.Stop();
            }

            timerEnd.Start();
        }

        private void Cursor_Start_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (timerStart.IsEnabled)
            {
                timerStart.Stop();
            }
            timerStart.Start();
        }
    }
}
