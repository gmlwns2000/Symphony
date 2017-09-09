using Symphony.Util;
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
    /// NewPlaylist.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NewPlaylist : Window
    {
        private Storyboard PopupOff;
        private List<Symphony.Player.Playlist> plList;
        public string name = "";

        public NewPlaylist(Window Parent, List<Symphony.Player.Playlist> list ,string startname = "플레이리스트 1")
        {
            InitializeComponent();

            Owner = Parent;
            textBox.Text = startname;
            name = startname;
            plList = list;

            PopupOff = this.FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            Keyboard.Focus(textBox);
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //ok
        {
            make();
        }

        private void make()
        {
            for (int i = 0; i < plList.Count; i++)
            {
                if(plList[i].Title == textBox.Text)
                {
                    MessagePopup mp = new MessagePopup(Owner, LanguageHelper.FindText("Lang_Popup_Have_Same_Name_Of_Playlist"), LanguageHelper.FindText("Lang_Confirm"));
                    mp.ShowDialog();
                    return;
                }
            }
            name = textBox.Text;
            PopupOff.Begin();
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                make();
            }
            else if (e.Key == Key.Escape)
            {
                name = String.Empty;
                PopupOff.Begin();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)   //cancle
        {
            name = String.Empty;
            PopupOff.Begin();
        }
    }
}
