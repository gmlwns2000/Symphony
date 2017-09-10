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

using Symphony.Player.DSP;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingEcho.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingEcho : IDspEditor
    {
        private Echo echo;
        private bool inited = false;
        private DispatcherTimer timer;

        public event EventHandler<DspUpdatedArgs> Updated;

        public SettingEcho(Echo echo)
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);

            this.echo = echo;

            UpdateUi();

            inited = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (inited)
            {
                int id = echo.SID;
                echo = new Echo((int)Sld_Length.Value, (float)Sld_Factor.Value);
                echo.SetStatus((bool)Chk_On.IsChecked);
                echo.SID = id;
                Updated?.Invoke(this, new DspUpdatedArgs(echo));
                UpdateUi();
            }
            timer.Stop();
        }

        private void UpdateUi()
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
            int id = echo.SID;
            echo = new Echo(10000, 0.3f);
            echo.SID = id;
            echo.SetStatus(false);
            Updated?.Invoke(this, new DspUpdatedArgs(echo));
            UpdateUi();
            inited = true;
        }

        private void Chk_On_Checked(object sender, RoutedEventArgs e)
        {
            echo.SetStatus(true);
            Updated?.Invoke(this, new DspUpdatedArgs(echo));
        }

        private void Chk_On_Unchecked(object sender, RoutedEventArgs e)
        {
            echo.SetStatus(false);
            Updated?.Invoke(this, new DspUpdatedArgs(echo));
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

        public void Deleted()
        {
            timer.Stop();
            Updated = null;
            inited = false;
            echo = null;
        }
    }
}
