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
using Symphony.Player.DSP;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingEqualizer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingEqualizer : IDspEditor
    {
        private Equalizer eq;
        private DispatcherTimer timer;
        private double min = -10;
        private double max = 10;
        private bool inited = false;

        public event EventHandler<DspUpdatedArgs> Updated;

        public SettingEqualizer(Equalizer eq)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;

            InitializeComponent();

            this.eq = eq;
            updateUI();

            inited = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!inited)
            {
                return;
            }
            eq.preAmp = (float)Sld_Amp.Value;
            eq.opacity = (float)Sld_Opacity.Value;

            if(Updated != null)
            {
                Updated(this, new DspUpdatedArgs(eq));
            }

            timer.Stop();
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
            if (!inited)
                return;
            eq.bands[e.Index] = e.Band;
            eq.UpdateEqBands();
            if(Updated != null)
            {
                Updated(this, new DspUpdatedArgs(eq));
            }
        }

        private void Sld_Amp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!inited)
                return;

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
            if (!inited)
                return;
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
            
            if(Updated != null)
            {
                Updated(this, new DspUpdatedArgs(eq));
            }
        }

        private void Chk_On_Checked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }
            eq.SetStatus(true);

            if (Updated != null)
            {
                Updated(this, new DspUpdatedArgs(eq));
            }
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

            if (Updated != null)
            {
                Updated(this, new DspUpdatedArgs(eq));
            }

            updateUI();
        }

        public void Deleted()
        {
            inited = false;
            timer.Stop();
            eq = null;
        }
    }
}
