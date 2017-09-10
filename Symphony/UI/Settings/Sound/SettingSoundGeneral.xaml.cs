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
        public Util.Settings Settings = Util.Settings.Current;

        public SettingSoundGeneral(MainWindow mw)
        {
            InitializeComponent();

            DataContext = Settings;
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
