using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
    /// WaveFormTrackBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WaveFormTrackBar : UserControl
    {
        bool upposition = false;
        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ValueChanged;

        public WaveFormTrackBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty maxi =
                    DependencyProperty.Register
                    (
                        "Maximum", typeof(double), typeof(WaveFormTrackBar),
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

        public static readonly DependencyProperty MinutesRemainingProperty =
                DependencyProperty.Register
                (
                    "Value", typeof(double), typeof(WaveFormTrackBar),
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

        private float[] waveformDatas;
        private int waveformLength;
        private int preLength;

        public float WaveformHeightVolume = 0.85f;

        public void WaveformReady(object sender,Symphony.Player.WaveformReady e)
        {
            waveformLength = e.MaxLength;
            waveformDatas = e.Buffer;

            Canvas_Wave.Dispatcher.Invoke(new Action(updateGraph));
        }

        public void updateGraph()
        {
            if (preLength < waveformLength)
            {
                Canvas_Wave.OpacityMask = null;
            }
            preLength = waveformLength;
            if (waveformLength == waveformDatas.Length || WaveFormParent.Opacity > 0)
            {
                float[] datas = waveformDatas;
                int dataLength = waveformLength;

                int width, height;

                width = (int)(Canvas_Wave.ActualWidth);
                height = (int)(Canvas_Wave.ActualHeight );

                Bitmap bim = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bim);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.White, 1.5f);
                g.Clear(System.Drawing.Color.Transparent);

                PointF[] points = new PointF[datas.Length-1];
                for (int i = 0; i < datas.Length-1; i++)
                {
                    points[i] = new PointF((float)width / dataLength * i, (float)(height/2 + (datas[i+1]*((height/2)*WaveformHeightVolume))));
                }
                g.DrawLines(pen, points);

                MemoryStream ms = new MemoryStream();
                bim.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.CreateOptions = BitmapCreateOptions.None;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();

                ImageBrush ib = new ImageBrush(bi);
                ib.Stretch = Stretch.Fill;
                ib.Freeze();

                bim.Dispose();

                bi.Freeze();

                ms.Close();
                ms.Dispose();

                g.Dispose();

                Canvas_Wave.OpacityMask = ib;
            }
        }

        public static readonly DependencyProperty wheel =
                    DependencyProperty.Register
                    (
                        "WheelChange", typeof(double), typeof(WaveFormTrackBar),
                        new UIPropertyMetadata((double)2500)
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

        public System.Windows.Media.Color ForegroundColor = System.Windows.Media.Color.FromRgb(76, 215, 255);
        public System.Windows.Media.Color HighlightColor = System.Windows.Media.Color.FromRgb(255, 255, 255);

        private void updateColor(float point)
        {
            LinearGradientBrush brs = new LinearGradientBrush();
            GradientStopCollection gradient = new GradientStopCollection();

            if (point > 0.001f)
            {
                gradient.Add(new GradientStop(ForegroundColor, 1));
                gradient.Add(new GradientStop(ForegroundColor, point));
                gradient.Add(new GradientStop(HighlightColor, point - 0.001f));
                gradient.Add(new GradientStop(HighlightColor, 0));
                brs.GradientStops = gradient;
            }
            else
            {
                gradient.Add(new GradientStop(ForegroundColor, 1));
                gradient.Add(new GradientStop(ForegroundColor, 0));
                brs.GradientStops = gradient;
            }

            gradient.Freeze();
            brs.Freeze();

            Canvas_Wave.Fill = brs;
        }

        private void Bar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double left = Value / Maximum * Bar.ActualWidth - 13;
            PointCurs.Margin = new Thickness(left, 0, 0, 0);
            updateColor((float)Value / (float)Maximum);
        }

        private void Bar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double left = Value / Maximum * Bar.ActualWidth - 13;
            PointCurs.Margin = new Thickness(left, 0, 0, 0);
            updateColor((float)Value / (float)Maximum);
        }

        System.Windows.Point prePt = new System.Windows.Point();

        private void updateposition(System.Windows.Point pt)
        {
            if ((prePt.X != pt.X || prePt.Y != pt.Y) && pt.X >= 0 && pt.X <= control.ActualWidth && pt.Y >= 0 && pt.Y <= control.ActualHeight)
            {
                double var = pt.X / control.ActualWidth * Maximum;
                ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(Bar.Value, var));
                Bar.Value = var;
            }

            prePt = pt;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            prePt = new System.Windows.Point(-1, -1);
            updateposition(e.GetPosition(control));
            upposition = true;
        }

        private void control_MouseLeave(object sender, MouseEventArgs e)
        {
            upposition = false;
        }

        private void control_MouseUp(object sender, MouseButtonEventArgs e)
        {
            prePt = new System.Windows.Point(-1, -1);
            updateposition(e.GetPosition(control));
            upposition = false;
        }

        private void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (upposition)
            {
                updateposition(e.GetPosition(control));
            }
        }

        private void control_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Math.Abs(e.Delta) > 0)
            {
                double var = Math.Min(Maximum, Math.Max(0, Value + ((double)e.Delta / 120) *WheelChange));
                ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(Bar.Value, var));
                Bar.Value = var;
            }
        }
    }

}
