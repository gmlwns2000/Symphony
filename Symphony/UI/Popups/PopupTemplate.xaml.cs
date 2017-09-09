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
    /// PopupTemplate.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopupTemplate : Window
    {
        private Storyboard PopupOff;
        public PopupTemplate(Window Parent)
        {
            InitializeComponent();

            Owner = Parent;

            PopupOff = this.FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
