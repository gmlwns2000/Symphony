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
using Symphony.Player;
using System.Windows.Threading;
using Symphony.Player.DSP;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingStereoEnhancer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingStereoEnhancer : UserControl, IDspEditor
    {
        DispatcherTimer timer;
        StereoEnhancer eff;
        bool inited = false;

        public SettingStereoEnhancer(StereoEnhancer eff)
        {
            this.eff = eff;
            
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;

            inited = true;

            UpdateUI();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (inited)
            {
                Updated.Invoke(this, new DspUpdatedArgs(eff));
            }
            timer.Stop();
        }

        void UpdateUI()
        {
            if(!inited)
            {
                return;
            }

            inited = false;

            Sld_Left_Factor.Value = Sld_Left_Factor.Maximum - eff.Factor;
            Sld_Right_Factor.Value = eff.Factor;
            Sld_Opacity.Value = eff.opacity;
            Sld_PreAmp.Value = eff.PreAmp;
            Cb_Use.IsChecked = eff.on;

            inited = true;
        }

        public void Deleted()
        {
            inited = false;

            eff = null;

            Updated = null;

            timer.Stop();

            timer = null;
        }

        public event EventHandler<DspUpdatedArgs> Updated;

        private void Sld_Left_Factor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                eff.Factor = (float)(Sld_Left_Factor.Maximum - Sld_Left_Factor.Value);

                inited = false;
                Sld_Right_Factor.Value = eff.Factor;
                inited = true;

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();
            }
        }

        private void Sld_Right_Factor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                eff.Factor = (float)Sld_Right_Factor.Value;

                inited = false;
                Sld_Left_Factor.Value = Sld_Left_Factor.Maximum - eff.Factor;
                inited = true;

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();
            }
        }

        private void Cb_Use_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                eff.SetStatus(true);

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();
            }
        }

        private void Cb_Use_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                eff.SetStatus(false);

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();
            }
        }

        private void Sld_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                eff.SetOpacity((float)Sld_Opacity.Value);

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();
            }
        }

        private void Sld_PreAmp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                eff.PreAmp = (float)Sld_PreAmp.Value;

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();
            }
        }

        private void Bt_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                int sid = eff.SID;
                eff = new StereoEnhancer();
                eff.SID = sid;

                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                timer.Start();

                UpdateUI();
            }
        }
    }
}
