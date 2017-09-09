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

namespace Symphony.UI
{
    /// <summary>
    /// VolumeBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class VolumeBar : UserControl
    {
        private bool moving = false;
        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueChanged;

        public VolumeBar()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty MinutesRemainingProperty =
                    DependencyProperty.Register
                    (
                        "Value", typeof(double), typeof(VolumeBar),
                        new UIPropertyMetadata((double)10)
                    );

        public double Value
        {
            get
            {
                return (double)GetValue(MinutesRemainingProperty);
            }
            set
            {
                SetValue(MinutesRemainingProperty, value);
            }
        }

        public static readonly DependencyProperty maxi =
                    DependencyProperty.Register
                    (
                        "Maximum", typeof(double), typeof(VolumeBar),
                        new UIPropertyMetadata((double)100)
                    );

        public double Maximum
        {
            get
            {
                return (double)GetValue(maxi);
            }
            set
            {
                SetValue(maxi, value);
            }
        }

        public static readonly DependencyProperty wheel =
                    DependencyProperty.Register
                    (
                        "WheelChange", typeof(double), typeof(VolumeBar),
                        new UIPropertyMetadata((double)3)
                    );

        public double WheelChange
        {
            get
            {
                return (double)GetValue(wheel);
            }
            set
            {
                SetValue(wheel, value);
            }
        }

        public bool IsVerticalScroll { get; set; } = false;

        private void Bar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void Bar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            updatevalue(sender as ProgressBar, e);
            moving = true;
            e.Handled = true;
        }

        private void Bar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(moving)
                updatevalue(sender as ProgressBar, e);
            moving = false;
        }

        private void Bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                updatevalue(sender as ProgressBar, e);
            }
        }

        private void Bar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                updatevalue(sender as ProgressBar, e);
            }
            moving = false;
        }

        private void updatevalue(ProgressBar pg, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point pt = e.GetPosition(Bar);
                double val = Math.Min(Maximum, Math.Max(0, pt.X / Bar.ActualWidth * Maximum));
                if (!double.IsNaN(val))
                {
                    ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(Bar.Value, val));
                    SetValue(MinutesRemainingProperty, val);
                    Bar.Value = val;
                }
            }
        }

        private void Bar_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double val = Math.Min(Maximum, Math.Max(0, Value+((float)e.Delta/120)*WheelChange));
            ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(Bar.Value, val));
            SetValue(MinutesRemainingProperty, val);
            Bar.Value = val;
        }
    }
}
