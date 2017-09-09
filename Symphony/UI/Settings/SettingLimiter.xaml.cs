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
    /// SettingLimiter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingLimiter : UserControl
    {
        private NPlayer.nPlayerCore np;
        private NPlayer.nPlayerLimiter limiter;
        private bool inited = false;
        private DispatcherTimer timer = new DispatcherTimer();

        public SettingLimiter()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (inited)
            {
                limiter = new NPlayer.nPlayerLimiter((float)Sld_Limit.Value, (float)Sld_Smooth.Value);
                np.DSPs[np.DSPs.Count - 1] = limiter;
                np.UpdateDSP();
                updateUi();
            }
            timer.Stop();
        }

        public void initLimiter(ref NPlayer.nPlayerCore np, NPlayer.nPlayerLimiter limiter)
        {
            this.np = np;
            this.limiter = limiter;

            updateUi();

            inited = true;
        }

        private void updateUi()
        {
            Sld_Limit.Value = limiter.limit;
            Sld_Limit.ToolTip = ((int)(limiter.limit * 100)).ToString() + "%";
            Sld_Smooth.Value = limiter.strength;
            Chk_On.IsChecked = limiter.on;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            limiter = new NPlayer.nPlayerLimiter(1.0f,0.05f);
            np.DSPs[np.DSPs.Count - 1] = limiter;
            updateUi();
        }

        private void Chk_On_Checked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }
            limiter.SetStatus(true);
            np.DSPs[np.DSPs.Count - 1] = limiter;
        }

        private void Chk_On_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }
            limiter.SetStatus(false);
            np.DSPs[np.DSPs.Count - 1] = limiter;
        }

        private void Sld_Limit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        private void Sld_Smooth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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
