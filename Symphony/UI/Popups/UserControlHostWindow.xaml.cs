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
    /// UserControlHostWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControlHostWindow : Window
    {
        public UserControl Control
        {
            get
            {
                return (UserControl)userControlContainer.Children[0];
            }
            set
            {
                userControlContainer.Children.Clear();
                userControlContainer.Children.Add(value);
            }
        }
        Storyboard PopupOff;

        ShadowWindow shadow;

        public UserControlHostWindow(Window Parent, MainWindow mw, string Title, UserControl UserControl, bool useResizer = false)
        {
            InitializeComponent();

            shadow = new ShadowWindow(this, mw, 12, 1, useResizer);

            if (useResizer)
            {
                SizeToContent = SizeToContent.Manual;
                MinWidth = 300;
                MinHeight = 200;
            }

            Owner = Parent;

            Control = UserControl;

            if (Util.TextTool.StringEmpty(Title))
            {
                Title = "";
            }

            Tb_Title.Text = Title;
            this.Title = Title;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        public new void Close()
        {
            PopupOff.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount >= 2)
            {
                if(WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
            }
            else
            {
                DragMove();
            }
        }
    }
}
