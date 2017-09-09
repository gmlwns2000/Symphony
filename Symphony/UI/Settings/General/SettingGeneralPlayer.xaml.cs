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
    /// SettingGeneralPlayer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingGeneralPlayer : UserControl
    {
        MainWindow mw;
        public SettingGeneralPlayer(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;

            Control_AlbumArt.Init(mw);

            Control_PlayerControl.Init(mw);
        }
    }
}
