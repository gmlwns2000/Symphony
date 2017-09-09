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
using System.Windows.Threading;

namespace Symphony.UI
{
    /// <summary>
    /// AngleEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AngleEditor : UserControl
    {
        public static DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(AngleEditor), new FrameworkPropertyMetadata(new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs arg) =>
        {
            AngleEditor editor = (AngleEditor)sender;

            double angle = (double)arg.NewValue;

            editor.AngleChanged?.Invoke(editor, new RoutedPropertyChangedEventArgs<double>(editor.Angle, angle));

            editor.UpdateAngle(angle);
        })));

        public double Angle
        {
            get
            {
                return (double)GetValue(AngleProperty);
            }
            set
            {
                SetValue(AngleProperty, value);
            }
        }

        DispatcherTimer timer;
        public event EventHandler<RoutedPropertyChangedEventArgs<double>> AngleChanged;
        public AngleEditor()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;

            Angle = Angle;
        }

        private void UpdateAngle(double angle)
        {
            Rotate.Angle = -angle;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point pt = Mouse.GetPosition(this);
                pt.X -= Width / 2;
                pt.Y += Height / 2;
                pt.Y = Height - pt.Y;
                if (pt.X == 0 && pt.Y == 0)
                    return;
                double a = Math.Acos(pt.X / Math.Sqrt(pt.X * pt.X + pt.Y * pt.Y)) / Math.PI * 180;
                if(pt.Y < 0)
                {
                    a = 360-a;
                }

                Angle = a;
            }
            else
            {
                timer.Stop();
            }
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (timer.IsEnabled)
                timer.Stop();

            timer.Start();
        }
    }
}
