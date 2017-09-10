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
    /// SettingVisualizerSpec.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingVisualizerSpec : UserControl
    {
        Brush borderBrush;
        Brush warnBrush;

        public SettingVisualizerSpec()
        {
            InitializeComponent();

            borderBrush = Tb_Spec_Dash.BorderBrush;

            warnBrush = new SolidColorBrush(Color.FromArgb(180, 255, 30, 50));
            warnBrush.Freeze();
        }

        MainWindow mw;
        bool inited = false;
        public void Init(MainWindow mw)
        {
            this.mw = mw;
        }

        public void UpdateUI()
        {
            inited = false;

            Sld_Spec_Width.Value = mw.Spec_Width;
            Sld_Spec_Dash.Value = mw.Spec_Dash;
            Sld_Spec_Top.Value = mw.SpecTop * 100;
            Sld_Spec_Height.Value = mw.SpecHeight * 100;
            Sld_Spec_MaxFreq.Value = mw.SpecMaxFreq;
            Sld_Spec_MinFreq.Value = mw.SpecMinFreq;
            Sld_Spec_Opacity.Value = mw.SpecOpacity * 100;
            Sld_Spec_Strength.Value = mw.SpecStrength;

            Cb_Spec_Invert.IsChecked = mw.SpecInvert;
            Cb_Spec_UseLogScale.IsChecked = mw.SpecUseLogScale;
            Cb_Spec_UseResampler.IsChecked = mw.SpecUseResampler;
            Cb_Spec_GridShow.IsChecked = mw.SpecGridShow;

            switch (mw.Spec_ScalingMode)
            {
                case Symphony.Player.DSP.CSCore.ScalingStrategy.Linear:
                    Cbb_Spec_ScalingMode.SelectedIndex = 0;
                    break;
                case Symphony.Player.DSP.CSCore.ScalingStrategy.Sqrt:
                    Cbb_Spec_ScalingMode.SelectedIndex = 1;
                    break;
                case Symphony.Player.DSP.CSCore.ScalingStrategy.Decibel:
                    Cbb_Spec_ScalingMode.SelectedIndex = 2;
                    break;
            }
            switch (mw.SpecRenderType)
            {
                case BarRenderTypes.Dots:
                    Cbb_Spec_RenderType.SelectedIndex = 0;
                    break;
                case BarRenderTypes.Line:
                    Cbb_Spec_RenderType.SelectedIndex = 1;
                    break;
                case BarRenderTypes.Rectangle:
                    Cbb_Spec_RenderType.SelectedIndex = 2;
                    break;
                case BarRenderTypes.Filled:
                    Cbb_Spec_RenderType.SelectedIndex = 3;
                    break;
            }
            switch (mw.SpecGridTextHorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Cbb_Spec_GridTextHorizontalAlignment.SelectedIndex = 0;
                    break;
                case HorizontalAlignment.Center:
                    Cbb_Spec_GridTextHorizontalAlignment.SelectedIndex = 1;
                    break;
                case HorizontalAlignment.Right:
                    Cbb_Spec_GridTextHorizontalAlignment.SelectedIndex = 2;
                    break;
            }
            switch (mw.SpecResampleMode)
            {
                case Symphony.Player.ResamplingMode.Linear:
                    Cbb_Spec_ResamplingMode.SelectedIndex = 0;
                    break;
                case Symphony.Player.ResamplingMode.HalfSine:
                    Cbb_Spec_ResamplingMode.SelectedIndex = 1;
                    break;
                case Symphony.Player.ResamplingMode.FullSine:
                    Cbb_Spec_ResamplingMode.SelectedIndex = 2;
                    break;
            }

            UpdateTextbox();

            inited = true;
        }

        private void UpdateTextbox()
        {
            Tb_Spec_Dash.Text = mw.Spec_Dash.ToString("0.0");
            Tb_Spec_Dash.BorderBrush = borderBrush;
            Tb_Spec_Width.Text = mw.Spec_Width.ToString("0.0");
            Tb_Spec_Width.BorderBrush = borderBrush;
        }

        private void Sld_Spec_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.SpecOpacity = e.NewValue / 100;
            }
        }

        private void Sld_Spec_Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.SpecHeight = e.NewValue / 100;
            }
        }

        private void Sld_Spec_Top_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.SpecTop = e.NewValue / 100;
            }
        }

        private void Sld_Spec_Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //
            if (inited && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                mw.Spec_Width = e.NewValue;
                UpdateTextbox();
            }
        }

        DispatcherTimer timerWidth;
        private void Tb_Spec_Width_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                if(timerWidth == null)
                {
                    timerWidth = new DispatcherTimer();
                    timerWidth.Interval = TimeSpan.FromMilliseconds(120);
                    timerWidth.Tick += TimerWidth_Tick;
                }

                if (timerWidth.IsEnabled)
                    timerWidth.Stop();

                timerWidth.Start();
            }
        }

        private void TimerWidth_Tick(object sender, EventArgs e)
        {
            try
            {
                double d = Math.Max(0.001, Convert.ToDouble(Tb_Spec_Width.Text));

                mw.Spec_Width = d;

                Sld_Spec_Width.Value = d;

                Tb_Spec_Width.BorderBrush = borderBrush;
            }
            catch
            {
                Tb_Spec_Width.BorderBrush = warnBrush;
            }

            timerWidth.Stop();
        }

        private void Sld_Spec_Dash_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //
            if (inited && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                mw.Spec_Dash = e.NewValue;
                UpdateTextbox();
            }
        }

        DispatcherTimer timerDash;
        private void Tb_Spec_Dash_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                if (timerDash == null)
                {
                    timerDash = new DispatcherTimer();
                    timerDash.Interval = TimeSpan.FromMilliseconds(120);
                    timerDash.Tick += TimerDash_Tick; ;
                }

                if (timerDash.IsEnabled)
                    timerDash.Stop();

                timerDash.Start();
            }
        }

        private void TimerDash_Tick(object sender, EventArgs e)
        {
            try
            {
                double d = Convert.ToDouble(Tb_Spec_Dash.Text);

                mw.Spec_Dash = d;

                Sld_Spec_Dash.Value = d;

                Tb_Spec_Dash.BorderBrush = borderBrush;
            }
            catch
            {
                Tb_Spec_Dash.BorderBrush = warnBrush;
            }

            timerDash.Stop();
        }

        private void Cb_Spec_Invert_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SpecInvert = true;
            }
        }

        private void Cb_Spec_Invert_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SpecInvert = false;
            }
        }

        private void Sld_Spec_MinFreq_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if((int)e.NewValue >= mw.SpecMaxFreq)
                {
                    e.Handled = true;
                    Sld_Spec_MinFreq.Value = mw.SpecMaxFreq - 2;
                }
                else
                {
                    mw.SpecMinFreq = (int)e.NewValue;
                }
            }
        }

        private void Sld_Spec_MaxFreq_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if((int)e.NewValue <= mw.SpecMinFreq)
                {
                    e.Handled = true;
                    Sld_Spec_MaxFreq.Value = mw.SpecMinFreq + 2;
                }
                else
                {
                    mw.SpecMaxFreq = (int)e.NewValue;
                }
            }
        }

        private void Cb_Spec_UseLogScale_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SpecUseLogScale = true;
            }
        }

        private void Cb_Spec_UseLogScale_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SpecUseLogScale = false;
            }
        }

        private void Cb_Spec_UseResampler_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SpecUseResampler = true;
            }
        }

        private void Cb_Spec_UseResampler_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SpecUseResampler = false;
            }
        }

        private void Cbb_Spec_ScalingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Spec_ScalingMode.SelectedIndex)
                {
                    case 0:
                        mw.Spec_ScalingMode = Symphony.Player.DSP.CSCore.ScalingStrategy.Linear;
                        break;
                    case 1:
                        mw.Spec_ScalingMode = Symphony.Player.DSP.CSCore.ScalingStrategy.Sqrt;
                        break;
                    case 2:
                        mw.Spec_ScalingMode = Symphony.Player.DSP.CSCore.ScalingStrategy.Decibel;
                        break;
                }
            }
        }

        private void Cbb_Spec_RenderType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Spec_RenderType.SelectedIndex)
                {
                    case 0:
                        mw.SpecRenderType = BarRenderTypes.Dots;
                        break;
                    case 1:
                        mw.SpecRenderType = BarRenderTypes.Line;
                        break;
                    case 2:
                        mw.SpecRenderType = BarRenderTypes.Rectangle;
                        break;
                    case 3:
                        mw.SpecRenderType = BarRenderTypes.Filled;
                        break;
                }
            }
        }

        private void Sld_Spec_Strength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.SpecStrength = e.NewValue;
            }
        }

        private void Cb_Spec_GridShow_Checked(object sender, RoutedEventArgs e)
        {
            if(inited)
            {
                mw.SpecGridShow = true;
            }
        }

        private void Cb_Spec_GridShow_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SpecGridShow = false;
            }
        }

        private void Cbb_Spec_GridTextHorizontalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Spec_GridTextHorizontalAlignment.SelectedIndex)
                {
                    case 0:
                        mw.SpecGridTextHorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case 1:
                        mw.SpecGridTextHorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case 2:
                        mw.SpecGridTextHorizontalAlignment = HorizontalAlignment.Right;
                        break;
                }
            }
        }

        private void Cbb_Spec_ResamplingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Spec_ResamplingMode.SelectedIndex)
                {
                    case 0:
                        mw.SpecResampleMode = Symphony.Player.ResamplingMode.Linear;
                        break;
                    case 1:
                        mw.SpecResampleMode = Symphony.Player.ResamplingMode.HalfSine;
                        break;
                    case 2:
                        mw.SpecResampleMode = Symphony.Player.ResamplingMode.FullSine;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
