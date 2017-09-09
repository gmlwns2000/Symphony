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
    /// SettingEcho.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingEcho : UserControl
    {
        private NPlayer.nPlayerEcho echo;
        private NPlayer.nPlayerCore np;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool inited = false;

        public SettingEcho()
        {
            InitializeComponent();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (inited)
            {
                echo = new NPlayer.nPlayerEcho((int)Sld_Length.Value, (float)Sld_Factor.Value);
                echo.SetStatus((bool)Chk_On.IsChecked);
                np.DSPs[1] = echo;
                updateUi();
            }
            timer.Stop();
        }

        public void initEcho(ref NPlayer.nPlayerCore np, NPlayer.nPlayerEcho echo)
        {
            this.echo = echo;
            this.np = np;

            updateUi();

            inited = true;
        }

        private void updateUi()
        {
            Sld_Factor.Value = echo.EchoFactor;
            Sld_Factor.ToolTip = ((int)(echo.EchoFactor * 100)).ToString() + "%";
            Sld_Length.Value = echo.EchoLength;
            Sld_Length.ToolTip = echo.EchoLength.ToString();
            Chk_On.IsChecked = echo.on;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            inited = false;
            echo = new NPlayer.nPlayerEcho(10000, 0.3f);
            echo.SetStatus(false);
            np.DSPs[1] = echo;
            updateUi();
            inited = true;
        }

        private void Chk_On_Checked(object sender, RoutedEventArgs e)
        {
            echo.SetStatus(true);
            np.DSPs[1] = echo;
        }

        private void Chk_On_Unchecked(object sender, RoutedEventArgs e)
        {
            echo.SetStatus(false);
            np.DSPs[1] = echo;
        }

        private void Sld_Length_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    timer.Start();
                }
                else
                {
                    timer.Start();
                }
            }
        }

        private void Sld_Factor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    timer.Start();
                }
                else
                {
                    timer.Start();
                }
            }
        }
    }
}
