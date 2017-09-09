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
    /// SettingPlayerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingPlayerControl : UserControl
    {
        bool inited = false;
        MainWindow mw;

        public SettingPlayerControl()
        {
            InitializeComponent();
        }

        public void Init(MainWindow mw)
        {
            this.mw = mw;

            Update();
        }

        public void Update()
        {
            inited = false;

            Cb_Mini_Control.IsChecked = mw.PlayerMiniControlShow;
            Cb_Mini_Control_Topmost.IsChecked = mw.PlayerMiniControlTopmost;
            Cb_Mini_Control_Position_Save.IsChecked = mw.PlayerMiniControlSavePosition;

            inited = true;
        }

        private void Cb_Mini_Control_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.PlayerMiniControlShow = true;
            }
        }

        private void Cb_Mini_Control_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.PlayerMiniControlShow = false;
            }
        }

        private void Cb_Mini_Control_Topmost_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.PlayerMiniControlTopmost = true;
            }
        }

        private void Cb_Mini_Control_Topmost_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.PlayerMiniControlTopmost = false;
            }
        }

        private void Cb_Mini_Control_Position_Save_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.PlayerMiniControlSavePosition = true;
            }
        }

        private void Cb_Mini_Control_Position_Save_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.PlayerMiniControlSavePosition = false;
            }
        }
    }
}
