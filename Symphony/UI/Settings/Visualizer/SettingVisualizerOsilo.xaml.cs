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

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingVisualizerOsilo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingVisualizerOsilo : UserControl
    {
        Brush borderBrush;
        Brush warnBrush;

        public SettingVisualizerOsilo()
        {
            InitializeComponent();

            borderBrush = Tb_Osilo_Dash.BorderBrush;
            warnBrush = new SolidColorBrush(Color.FromArgb(180, 255, 30, 50));
            warnBrush.Freeze();
        }

        MainWindow mw;
        bool inited = false;
        public void Init(MainWindow mw)
        {
            this.mw = mw;
            inited = true;
        }

        public void UpdateUI()
        {
            if (!inited)
            {
                return;
            }

            inited = false;

            Sld_Osilo_Dash.Value = mw.OsiloDash;
            Sld_Osilo_Width.Value = mw.OsiloWidth;
            Sld_Osilo_Height.Value = mw.OsiloHeight * 100;
            Sld_Osilo_Opacity.Value = mw.OsiloOpacity * 100;
            Sld_Osilo_Strength.Value = mw.OsiloStrength * 100;
            Sld_Osilo_Top.Value = mw.OsiloTop * 100;
            Sld_Osilo_View.Value = mw.OsiloView;

            Tb_Osilo_Dash.Text = mw.OsiloDash.ToString("0.0");
            Tb_Osilo_Width.Text = mw.OsiloWidth.ToString("0.0");

            Cb_Osilo_Invert.IsChecked = mw.OsiloUseInvert;
            Cb_Osilo_GridShow.IsChecked = mw.OsiloGridShow;

            switch (mw.OsiloRenderType)
            {
                case BarRenderTypes.Dots:
                    Cbb_Osilo_RenderType.SelectedIndex = 0;
                    break;
                case BarRenderTypes.Line:
                    Cbb_Osilo_RenderType.SelectedIndex = 1;
                    break;
                case BarRenderTypes.Rectangle:
                    Cbb_Osilo_RenderType.SelectedIndex = 2;
                    break;
                case BarRenderTypes.Filled:
                    Cbb_Osilo_RenderType.SelectedIndex = 3;
                    break;
            }

            switch (mw.OsiloGridTextHorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Cbb_Osilo_GridTextHorizontalAlignment.SelectedIndex = 0;
                    break;
                case HorizontalAlignment.Center:
                    Cbb_Osilo_GridTextHorizontalAlignment.SelectedIndex = 1;
                    break;
                case HorizontalAlignment.Right:
                    Cbb_Osilo_GridTextHorizontalAlignment.SelectedIndex = 2;
                    break;
            }

            inited = true;
        }

        private void Sld_Osilo_View_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.OsiloView = (float)e.NewValue;
            }
        }

        private void Sld_Osilo_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.OsiloHeight = (float)e.NewValue / 100;
            }
        }

        private void Sld_Osilo_Top_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.OsiloTop = e.NewValue / 100;
            }
        }

        private void Sld_Osilo_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                mw.OsiloWidth = e.NewValue;
                Tb_Osilo_Width.Text = e.NewValue.ToString("0.0");
            }
        }

        DispatcherTimer timerOsiloWidth;

        private void Tb_Osilo_Width_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                if (timerOsiloWidth == null)
                {
                    timerOsiloWidth = new DispatcherTimer();
                    timerOsiloWidth.Interval = TimeSpan.FromMilliseconds(300);
                    timerOsiloWidth.Tick += TimerOsiloWidth_Tick;
                }
                if (timerOsiloWidth.IsEnabled)
                {
                    timerOsiloWidth.Stop();
                }
                timerOsiloWidth.Start();
            }
        }

        private void TimerOsiloWidth_Tick(object sender, EventArgs e)
        {
            try
            {
                double width = Math.Max(0.001, Convert.ToDouble(Tb_Osilo_Width.Text));

                mw.OsiloWidth =  width;

                Tb_Osilo_Width.BorderBrush = borderBrush;
                Sld_Osilo_Width.Value = mw.OsiloWidth;
            }
            catch
            {
                Tb_Osilo_Width.BorderBrush = warnBrush;
            }

            timerOsiloWidth.Stop();
        }

        private void Sld_Osilo_Dash_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                mw.OsiloDash = e.NewValue;
                Tb_Osilo_Dash.Text = e.NewValue.ToString("0.0");
            }
        }

        DispatcherTimer timerOsiloDash;

        private void Tb_Osilo_Dash_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                if (timerOsiloDash == null)
                {
                    timerOsiloDash = new DispatcherTimer();
                    timerOsiloDash.Interval = TimeSpan.FromMilliseconds(300);
                    timerOsiloDash.Tick += TimerOsiloDash_Tick; ;
                }
                if (timerOsiloDash.IsEnabled)
                {
                    timerOsiloDash.Stop();
                }
                timerOsiloDash.Start();
            }
        }

        private void TimerOsiloDash_Tick(object sender, EventArgs e)
        {
            try
            {
                double dash = Convert.ToDouble(Tb_Osilo_Dash.Text);

                mw.OsiloDash = dash;

                Tb_Osilo_Dash.BorderBrush = borderBrush;
                Sld_Osilo_Dash.Value = mw.OsiloDash;
            }
            catch
            {
                Tb_Osilo_Dash.BorderBrush = warnBrush;
            }

            timerOsiloDash.Stop();
        }

        private void Sld_Osilo_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.OsiloOpacity = e.NewValue / 100;
            }
        }

        private void Sld_Osilo_Strength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.OsiloStrength = (float)e.NewValue / 100.0f;
            }
        }

        private void Cbb_Osilo_RenderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Osilo_RenderType.SelectedIndex)
                {
                    case 0:
                        mw.OsiloRenderType = BarRenderTypes.Dots;
                        break;
                    case 1:
                        mw.OsiloRenderType = BarRenderTypes.Line;
                        break;
                    case 2:
                        mw.OsiloRenderType = BarRenderTypes.Rectangle;
                        break;
                    case 3:
                        mw.OsiloRenderType = BarRenderTypes.Filled;
                        break;
                }
            }
        }

        private void Cb_Osilo_Invert_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.OsiloUseInvert = true;
            }
        }

        private void Cb_Osilo_Invert_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.OsiloUseInvert = false;
            }
        }

        private void Cbb_Osilo_GridTextHorizontalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Osilo_GridTextHorizontalAlignment.SelectedIndex)
                {
                    case 0:
                        mw.OsiloGridTextHorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case 1:
                        mw.OsiloGridTextHorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case 2:
                        mw.OsiloGridTextHorizontalAlignment = HorizontalAlignment.Right;
                        break;
                }
            }
        }

        private void Cb_Osilo_GridShow_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.OsiloGridShow = true;
            }
        }

        private void Cb_Osilo_GridShow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.OsiloGridShow = false;
            }
        }
    }
}
