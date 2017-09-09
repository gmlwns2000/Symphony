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
    /// SettingVisualizerVU.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingVisualizerVU : UserControl
    {
        public SettingVisualizerVU()
        {
            InitializeComponent();
        }

        MainWindow mw;
        bool inited = false;
        public void Init(MainWindow mw)
        {
            this.mw = mw;

            UpdateUI();
        }

        public void UpdateUI()
        {
            inited = false;

            Sld_Vu_Opacity.Value = mw.VU_Opacity * 100;
            Sld_Vu_Senstive.Value = mw.VU_Senstive;

            inited = true;
        }

        private void Sld_Vu_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.VU_Opacity = e.NewValue / 100;
            }
        }

        private void Sld_Vu_Senstive_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.VU_Senstive = e.NewValue;
            }
        }
    }
}
