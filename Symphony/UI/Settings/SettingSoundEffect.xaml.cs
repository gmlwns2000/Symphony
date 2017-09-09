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
using NPlayer;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingUiVisualizer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingSoundEffect : UserControl
    {
        private nPlayerCore np;

        public SettingSoundEffect()
        {
            InitializeComponent();
        }

        public void init_Effects(ref nPlayerCore np)
        {
            this.np = np;
            settingEq.initEQ(ref this.np, np.DSPs[0] as nPlayerEQ);
            settingEcho.initEcho(ref this.np, np.DSPs[1] as nPlayerEcho);
            settingLimiter.initLimiter(ref this.np, np.DSPs[np.DSPs.Count - 1] as nPlayerLimiter);
        }
    }
}
