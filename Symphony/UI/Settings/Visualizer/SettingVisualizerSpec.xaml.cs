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
        public Util.Settings Settings = Util.Settings.Current;

        public SettingVisualizerSpec()
        {
            DataContext = Settings;

            InitializeComponent();
        }
    }
}
