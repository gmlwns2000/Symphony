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
    public class DialogTextResult
    {
        public readonly string Text;
        public readonly bool Okay;

        public DialogTextResult(string Text, bool Okay)
        {
            this.Okay = Okay;
            this.Text = Text;
        }
    }

    public partial class DialogText : Window
    {
        private bool okay = false;
        private bool died = false;
        private bool checkFileName = false;
        private Storyboard PopupOff;

        public DialogText(Window owner, string Title, string HintText, bool chkFileName = false, string OkayText=null, string CancelText=null)
        {
            InitializeComponent();

            this.Owner = owner;

            checkFileName = chkFileName;

            Lb_Text.Text = Title;
            Tb_Input.Text = HintText;

            if (CancelText == null)
                Bt_Cancel.Content = LanguageHelper.FindText("Lang_Cancel");
            else
                Bt_Cancel.Content = CancelText;

            if (OkayText == null)
                Bt_Okay.Content = LanguageHelper.FindText("Lang_Okay");
            else
                Bt_Okay.Content = OkayText;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            Close();
        }

        public new DialogTextResult ShowDialog()
        {
            return Show();
        }

        public new DialogTextResult Show()
        {   
            base.ShowDialog();

            return new DialogTextResult(Tb_Input.Text, okay);
        }

        private void Bt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (!died)
            {
                okay = false;
                died = true;
                PopupOff.Begin();
            }
        }

        private void Bt_Okay_Click(object sender, RoutedEventArgs e)
        {
            if (!died)
            {
                if (!checkFileName)
                {
                    okay = true;
                    died = true;
                    PopupOff.Begin();
                }
                else
                {
                    if(string.IsNullOrEmpty(Tb_Input.Text) || string.IsNullOrWhiteSpace(Tb_Input.Text))
                    {
                        new DialogMessage(this, "이름을 입력해주십시오.").Show();
                    }
                    else if (Tb_Input.Text.Contains("\\") || Tb_Input.Text.Contains(":") || Tb_Input.Text.Contains("|") || Tb_Input.Text.Contains("*") || Tb_Input.Text.Contains("?") || Tb_Input.Text.Contains("\"") || Tb_Input.Text.Contains("<") || Tb_Input.Text.Contains(">"))
                    {
                        new DialogMessage(this, "이름은 \\, :, |, *, ?, /, <, >, \" 을 포함해선 안됩니다.").Show();
                    }
                    else
                    {
                        okay = true;
                        died = true;
                        PopupOff.Begin();
                    }
                }
            }
        }
    }
}
