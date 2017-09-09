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
    /// SettingEqualizer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingEqualizer : UserControl
    {
        private NPlayer.nPlayerEQ eq;
        private NPlayer.nPlayerCore np;
        private DispatcherTimer timer = new DispatcherTimer();
        private double min = -10;
        private double max = 10;
        private bool inited = false;

        public SettingEqualizer()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!inited)
            {
                return;
            }
            eq.preAmp = (float)Sld_Amp.Value;
            eq.opacity = (float)Sld_Opacity.Value;
            np.DSPs[0] = eq;
            np.UpdateDSP();
            timer.Stop();
        }

        public void initEQ(ref NPlayer.nPlayerCore np, NPlayer.nPlayerEQ eq)
        {
            this.np = np;
            this.eq = eq;
            updateUI();
        }

        private void updateUI()
        {
            Stack_Bands.Children.Clear();
            for (int i = 0; i < eq.bands.Length; i++)
            {
                SettingEqualizerBand eqBand = new SettingEqualizerBand();
                eqBand.init_Band(i, eq.bands[i], min, max);
                eqBand.BandUpdated += EqBand_BandUpdated;
                Stack_Bands.Children.Add(eqBand);
            }
            Sld_Amp.Value = eq.preAmp;
            Sld_Amp.ToolTip = ((int)(Sld_Amp.Value * 100)).ToString() + "%";
            Sld_Opacity.Value = eq.opacity;
            Sld_Opacity.ToolTip = ((int)(Sld_Opacity.Value * 100)).ToString() + "%";
            Chk_On.IsChecked = eq.on;
            inited = true;
        }

        private void EqBand_BandUpdated(object sender, EQBandUpdateArgs e)
        {
            eq.bands[e.Index] = e.Band;
            eq.UpdateEqBands();
            np.DSPs[0] = eq;
        }

        private void Sld_Amp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                timer.Start();
            }
            else
            {
                timer.Start();
            }
            Sld_Amp.ToolTip = ((int)(Sld_Amp.Value * 100)).ToString() + "%";
        }

        private void Sld_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
                timer.Start();
            }
            else
            {
                timer.Start();
            }
            Sld_Opacity.ToolTip = ((int)(Sld_Opacity.Value * 100)).ToString() + "%";
        }

        private void Chk_On_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }
            eq.SetStatus(false);
            np.DSPs[0] = eq;
        }

        private void Chk_On_Checked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }
            eq.SetStatus(true);
            np.DSPs[0] = eq;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < eq.bands.Length; i++)
            {
                eq.bands[i].Gain = 0;
                if (eq.bands[i].Frequency < 500)
                {
                    eq.bands[i].Bandwidth = 0.35f;
                }
                else
                {
                    eq.bands[i].Bandwidth = 0.75f;
                }
            }
            eq.opacity = 1;
            eq.preAmp = 1;
            eq.on = true;
            eq.UpdateEqBands();
            np.DSPs[0] = eq;

            updateUI();
        }

        
    }
}
