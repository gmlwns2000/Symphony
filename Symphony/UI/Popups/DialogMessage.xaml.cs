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
    public enum DialogMessageType
    {
        Okay = -1,
        OkayCanel = 0,
        YesNo = 1
    }

    public enum DialogMessageResult
    {
        Yes = 0,
        No = 1,
        Okay = 0,
        Cancel = 1
    }

    /// <summary>
    /// DialogMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DialogMessage : Window
    {
        Storyboard PopupOff;
        public bool okay = true;

        public DialogMessage(Window owner, string text)
        {
            InitializeComponent();

            Owner = owner;

            Lb_Text.Text = text;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
        }

        public static void Show(Window owner, string text)
        {
            new DialogMessage(owner, text).Show();
        }

        public static DialogMessageResult Show(Window owner, string text, string title, DialogMessageType type)
        {
            DialogMessage wnd;

            if(owner == null && Application.Current!=null && Application.Current.MainWindow != null)
            {
                wnd = new DialogMessage(Application.Current.MainWindow, text);
            }
            else
            {
                wnd = new DialogMessage(owner, text);
            }

            wnd.Title = title;
            wnd.ShowInTaskbar = false;

            switch (type)
            {
                case DialogMessageType.Okay:
                    wnd.Bt_Okay.Visibility = Visibility.Visible;
                    wnd.Bt_Okay.Content = LanguageHelper.FindText("Lang_Okay");
                    wnd.Bt_Cancel.Visibility = Visibility.Collapsed;
                    break;
                case DialogMessageType.OkayCanel:
                    wnd.Bt_Okay.Visibility = Visibility.Visible;
                    wnd.Bt_Okay.Content = LanguageHelper.FindText("Lang_Okay");
                    wnd.Bt_Cancel.Visibility = Visibility.Visible;
                    wnd.Bt_Cancel.Content = LanguageHelper.FindText("Lang_Cancel");
                    break;
                case DialogMessageType.YesNo:
                    wnd.Bt_Cancel.Visibility = Visibility.Visible;
                    wnd.Bt_Cancel.Content = LanguageHelper.FindText("Lang_No");
                    wnd.Bt_Okay.Visibility = Visibility.Visible;
                    wnd.Bt_Okay.Content = LanguageHelper.FindText("Lang_Yes");
                    break;
                default:
                    break;
            }

            wnd.Show();

            switch (type)
            {
                case DialogMessageType.Okay:
                    return DialogMessageResult.Okay;
                case DialogMessageType.OkayCanel:
                    if (wnd.okay)
                    {
                        return DialogMessageResult.Okay;
                    }
                    else
                    {
                        return DialogMessageResult.Cancel;
                    }
                case DialogMessageType.YesNo:
                    if (wnd.okay)
                    {
                        return DialogMessageResult.Yes;
                    }
                    else
                    {
                        return DialogMessageResult.No;
                    }
                default:
                    break;
            }

            return DialogMessageResult.Cancel;
        }

        public new void ShowDialog()
        {
            Show();
        }

        public new void Show()
        {
            base.ShowDialog();
        }

        public void Show(bool showdialog = true)
        {
            if(showdialog)
                base.ShowDialog();
            else
                base.Show();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            Close();
        }

        private void Bt_Okay_Click(object sender, RoutedEventArgs e)
        {
            PopupOff.Begin();
        }

        private void Bt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            okay = false;

            PopupOff.Begin();
        }

        private void Lb_Text_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

            if(e.ClickCount >= 3)
            {
                Clipboard.SetText(Lb_Text.Text);
            }
        }
    }
}
