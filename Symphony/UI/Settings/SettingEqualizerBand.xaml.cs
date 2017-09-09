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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingEqualizerBand.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingEqualizerBand : UserControl
    {
        private NPlayer.nPlayerEQBand band;
        private int index;
        private DispatcherTimer timer;

        public event EventHandler<EQBandUpdateArgs> BandUpdated;

        public SettingEqualizerBand()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Sld_Power.Value > 0)
            {
                band.Bandwidth = (float)Sld_Power.Value;
            }
            else
            {
                band.Bandwidth = 0.0001f;
            }

            band.Gain = (float)Sld_Gain.Value;

            BandUpdated(this, new EQBandUpdateArgs(index, band));

            timer.Stop();
        }

        public void init_Band(int index, NPlayer.nPlayerEQBand band, double min, double max)
        {
            this.band = band;

            Sld_Gain.Value = band.Gain;
            Sld_Gain.ToolTip = (((int)(Sld_Gain.Value*100))/100.0f).ToString() + "dB";
            Sld_Gain.Maximum = max;
            Sld_Gain.Minimum = min;

            if(band.Frequency > 1000)
            {
                Lb_Freq.Text = (band.Frequency / 1000).ToString() + "kHz";
            }
            else
            {
                Lb_Freq.Text = band.Frequency.ToString() + "Hz";
            }

            Sld_Power.Value = band.Bandwidth;
            Sld_Power.ToolTip = ((int)(Sld_Power.Value * 100)).ToString() + "%";

            this.index = index;
        }

        private void Sld_Power_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
                timer.Start();
            }
            Sld_Power.ToolTip = ((int)(Sld_Power.Value * 100)).ToString() + "%";
        }

        private void Sld_Gain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
                timer.Start();
            }
            Sld_Gain.ToolTip = (((int)(Sld_Gain.Value*100))/100.0f).ToString() + "dB";
        }
    }
    public class EQBandUpdateArgs : EventArgs
    {
        public NPlayer.nPlayerEQBand Band;
        public int Index;

        public EQBandUpdateArgs(int index, NPlayer.nPlayerEQBand band)
        {
            Index = index;
            Band = band;
        }
    }
}
