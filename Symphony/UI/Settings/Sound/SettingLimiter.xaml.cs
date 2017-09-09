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
    /// SettingLimiter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingLimiter : IDspEditor
    {
        private Limitter limiter;
        private bool inited = false;
        private DispatcherTimer timer = new DispatcherTimer();

        public event EventHandler<DspUpdatedArgs> Updated;

        public SettingLimiter(Limitter limiter)
        {
            InitializeComponent();

            this.limiter = limiter;

            updateUi();

            inited = true;

            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (inited)
            {
                int id = limiter.SID;
                limiter = new Limitter((float)Sld_Limit.Value, (float)Sld_Smooth.Value);
                limiter.SID = id;
                if (Updated != null)
                {
                    Updated(this, new DspUpdatedArgs(limiter));
                }
                updateUi();
            }
            timer.Stop();
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
            int id = limiter.SID;
            limiter = new Limitter(1.0f,0.05f);
            limiter.SID = id;
            if (Updated != null)
            {
                Updated(this, new DspUpdatedArgs(limiter));
            }
            updateUi();
        }

        private void Chk_On_Checked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }
            limiter.SetStatus(true);

            if (Updated != null)
            {
                Updated(this, new DspUpdatedArgs(limiter));
            }
        }

        private void Chk_On_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }
            limiter.SetStatus(false);

            if (Updated != null)
            {
                Updated(this, new DspUpdatedArgs(limiter));
            }
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

        public void Deleted()
        {
            timer.Stop();
            inited = false;
            limiter = null;
        }
    }
}
