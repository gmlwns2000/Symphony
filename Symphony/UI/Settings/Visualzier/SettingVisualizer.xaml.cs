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
    /// SettingVisualizer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingVisualizer : UserControl
    {
        MainWindow mw;

        public SettingVisualizer(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;

            Control_Osilo.Init(mw);

            Control_VU.Init(mw);

            Control_Spec.Init(mw);

            UpdateUI();
        }

        private void UpdateUI()
        {
            Control_Osilo.UpdateUI();

            Control_VU.UpdateUI();

            Control_Spec.UpdateUI();
        }
    }
}
