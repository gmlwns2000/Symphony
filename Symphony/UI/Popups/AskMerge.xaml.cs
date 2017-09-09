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
    /// AskMerge.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AskMerge : Window
    {
        public MergeMode result = MergeMode.Skip;
        private Storyboard PopupOff;

        public AskMerge(Window wd)
        {
            InitializeComponent();

            Owner = wd;

            PopupOff = this.FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed; ;
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //merge
        {
            result = MergeMode.Merge;
            PopupOff.Begin();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)   //change
        {
            result = MergeMode.Change;
            PopupOff.Begin();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)   //skip
        {
            result = MergeMode.Skip;
            PopupOff.Begin();
        }
    }
    public enum MergeMode
    {
        Merge = 2,
        Change = 1,
        Skip = 0
    }
}
