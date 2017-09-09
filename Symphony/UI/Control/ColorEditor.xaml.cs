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
    public class ColorUpdatedArgs
    {
        public Color NewColor;


        public ColorUpdatedArgs(Color NewColor)
        {
            this.NewColor = NewColor;
        }
    }

    /// <summary>
    /// ColorEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ColorEditor : UserControl
    {
        public event EventHandler<ColorUpdatedArgs> ColorUpdated;

        public static DependencyProperty ColorBrushProperty = DependencyProperty.Register("ColorBrush", typeof(Brush), typeof(ColorEditor), new FrameworkPropertyMetadata(null));
        public Brush ColorBrush
        {
            get
            {
                return (Brush)GetValue(ColorBrushProperty);
            }
            private set
            {
                SetValue(ColorBrushProperty, value);
            }
        }

        double H = 0;
        double S = 0;
        double V = 0;
        double A = 1;

        DispatcherTimer timer = new DispatcherTimer();

        public ColorEditor()
        {
            InitializeComponent();

            GradientStopCollection gsc = new GradientStopCollection();
            for (int i = 0; i < 361; i++)
            {
                gsc.Add(new GradientStop(Hsv2Rgb.GetColor(i, 100, 100, 1), (double)i / 360));
            }
            LinearGradientBrush lgb = new LinearGradientBrush(gsc);
            lgb.StartPoint = new Point(0, 0);
            lgb.EndPoint = new Point(0, 1);

            lgb.Freeze();
            gsc.Freeze();

            Rect_H.Fill = lgb;

            timer.Interval = TimeSpan.FromMilliseconds(17);
            timer.Tick += Timer_Tick;
        }

        public void SetColor(Color c)
        {
            Hsv2Rgb.ColorToHSV(c, ref H, ref S, ref V);

            SolidColorBrush brush = new SolidColorBrush(c);
            brush.Freeze();
            ColorBrush = brush;

            UpdateCursor();
        }

        bool HueEdit = false;
        bool SVEdit = false;
        Point lastPt = new Point(12344, 12344);

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                HueEdit = false;
                SVEdit = false;

                timer.Stop();
            }

            Point pt = Mouse.GetPosition(this);

            if (pt.X != lastPt.X || pt.Y != lastPt.Y)
            {
                if (HueEdit)
                {
                    H = Math.Max(0, Math.Min(360, pt.Y / ActualHeight * 360));
                }
                else if (SVEdit)
                {
                    S = Math.Min(100, Math.Max(0, (pt.X - 21) / (ActualWidth - 20) * 100));
                    V = Math.Min(100, Math.Max(0, (ActualHeight - 20 - pt.Y) / (ActualHeight - 20) * 100));
                }

                Color c = Hsv2Rgb.GetColor(H, S, V, A);
                ColorUpdated?.Invoke(this, new ColorUpdatedArgs(c));
                SolidColorBrush brush = new SolidColorBrush(c);
                brush.Freeze();
                ColorBrush = brush;

                UpdateCursor();
            }

            lastPt = pt;
        }

        private void UpdateCursor()
        {
            H = Math.Max(0, Math.Min(360, H));
            S = Math.Max(0, Math.Min(100, S));
            V = Math.Max(0, Math.Min(100, V));
            A = Math.Max(0, Math.Min(1, A));

            Cursor_H.Margin = new Thickness(5, H / 360 * ( ActualHeight - 4 ) - Cursor_H.ActualHeight / 2 + 2, 5, 0);

            Cursor_SV.Margin = new Thickness(S / 100 * (ActualWidth - 20) - Cursor_SV.ActualWidth / 2, 0, 0, V / 100 * (ActualHeight - 20) - Cursor_SV.ActualHeight / 2);

            Rect_SV.Fill = new SolidColorBrush(Hsv2Rgb.GetColor(H, 100, 100, 1));
            Rect_SV.Fill.Freeze();

            ColorPreview.Background = ColorBrush;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this);

            if(pt.X <= 30)
            {
                HueEdit = true;
            }
            else
            {
                SVEdit = true;
            }

            if (timer.IsEnabled)
            {
                timer.Stop();
            }

            timer.Start();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCursor();
        }
    }
}
