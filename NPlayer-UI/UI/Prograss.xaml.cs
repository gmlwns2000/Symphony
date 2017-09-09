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
using System.Windows.Shapes;

namespace NPlayer_UI
{
    /// <summary>
    /// prograss.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Prograss : Window
    {
        public Prograss()
        {
            InitializeComponent();
        }
        public void update(string label1, string label2, int max, int value)
        {
            this.label1.Content = label1;
            this.label2.Content = label2;
            this.prograssBar.Maximum = max;
            this.prograssBar.Value = value;
        }
    }
}
