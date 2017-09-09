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

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingSoundGeneral.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingSoundGeneral : UserControl
    {
        MainWindow mw;
        bool inited = false;

        public SettingSoundGeneral(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;

            UpdateUI();
        }

        public void UpdateUI()
        {
            inited = false;

            Sld_Audio_Buffer.Value = mw.AudioDesiredLantency;
            Sld_Audio_Volume.Value = mw.AudioVolume / 100;
            Cb_Audio_Effect_Limit.IsChecked = mw.AudioUseDspLimit;

            int limit = mw.AudioDspLimitSampleRate;
            int ind = -1;
            for(int i=0; i<Cb_Audio_Effect_Limit_Samplerate.Items.Count; i++)
            {
                if(((string)((ComboBoxItem)Cb_Audio_Effect_Limit_Samplerate.Items[i]).Content).Replace(",","") == limit.ToString())
                {
                    ind = i;
                    break;
                }
            }

            if(ind != -1)
            {
                Cb_Audio_Effect_Limit_Samplerate.SelectedIndex = ind;
            }
            else
            {
                Cb_Audio_Effect_Limit_Samplerate.Items.Add(new ComboBoxItem() { Content = mw.AudioDspLimitSampleRate.ToString("0,000") });
                Cb_Audio_Effect_Limit_Samplerate.SelectedIndex = Cb_Audio_Effect_Limit_Samplerate.Items.Count - 1;
            }

            inited = true;
        }

        private void Sld_Audio_Buffer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.AudioDesiredLantency = (int)e.NewValue;
            }
        }

        private void Sld_Audio_Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.AudioVolume = e.NewValue * 100;
            }
        }

        private void Cb_Audio_Effect_Limit_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.AudioUseDspLimit = true;
            }
        }

        private void Cb_Audio_Effect_Limit_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.AudioUseDspLimit = false;
            }
        }

        private void Cb_Audio_Effect_Limit_Samplerate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                mw.AudioDspLimitSampleRate = (int)Convert.ToDouble(((string)((ComboBoxItem)Cb_Audio_Effect_Limit_Samplerate.Items[Cb_Audio_Effect_Limit_Samplerate.SelectedIndex]).Content).Replace(",", ""));
            }
        }
    }
}
