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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Symphony.UI
{
    /// <summary>
    /// SplashWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplashWindow : Window
    {
        Storyboard Sb_Close;
        public bool showed = false;

        public SplashWindow(string version)
        {
            InitializeComponent();

            Tb_VersionText.Text = version;

            Sb_Close = (Storyboard)FindResource("Close");
            Sb_Close.Completed += Sb_Close_Completed;

            Activated += SplashWindow_Activated;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            WindowInteropHelper helper = new WindowInteropHelper(this);

            WindowTransclick.set(helper.Handle);
        }

        private void SplashWindow_Activated(object sender, EventArgs e)
        {
            showed = true;
        }

        private void Sb_Close_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        public void Update(string text)
        {
            Logger.Log(this, text);
            Dispatcher.Invoke(new Action(() => 
            {
                Tb_Console.Text = text;
            }));
        }

        public new void Close()
        {
            Dispatcher.Invoke(new Action(() => 
            {
                Sb_Close.Begin();
            }));
        }
    }
}
