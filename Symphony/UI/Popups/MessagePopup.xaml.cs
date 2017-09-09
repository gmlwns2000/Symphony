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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Symphony.UI
{
    /// <summary>
    /// MessagePopup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessagePopup : Window
    {
        Storyboard PopupOff;
        public MessagePopup(Window owner, string msg, string button)
        {
            InitializeComponent();

            Title = msg;
            lb_msg.Text = msg;

            Bt_Ok.Content = button;

            Owner = owner;

            PopupOff = this.FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PopupOff.Begin();
        }
    }
}
