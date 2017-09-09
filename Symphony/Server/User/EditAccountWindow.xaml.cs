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

namespace Symphony.Server
{
    /// <summary>
    /// EditAccountWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditAccountWindow : Window
    {
        Storyboard PopupOff;
        Storyboard LoadingOn;
        Storyboard LoadingOff;

        public EditAccountWindow(Window Parent)
        {
            InitializeComponent();

            Owner = Parent;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
            LoadingOn = FindResource("LoadingOn") as Storyboard;
            LoadingOff = FindResource("LoadingOff") as Storyboard;

            Loaded += EditAccountWindow_Loaded;
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        private void EditAccountWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Session.IsLogined)
            {
                UpdateUI();
            }
            else
            {
                Close();
            }
        }

        private bool _sbm = false;
        private bool submitting
        {
            get
            {
                return _sbm;
            }
            set
            {
                _sbm = value;

                if (value)
                {
                    Grid_Form.IsEnabled = false;

                    if (LoadingOff != null && LoadingOn != null)
                    {
                        LoadingOff.Stop();
                        LoadingOn.Begin();
                    }
                }
                else
                {
                    Tb_PASS.Password = "";
                    Tb_PASS_Chk.Password = "";
                    Tb_PASS_NOW.Password = "";

                    Grid_Form.IsEnabled = true;

                    if (LoadingOff != null && LoadingOn != null)
                    {
                        LoadingOn.Stop();
                        LoadingOff.Begin();
                    }
                }
            }
        }

        private async void Submit()
        {
            if (submitting)
            {
                return;
            }

            if (!Session.IsLogined)
            {
                UI.DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Server_EditAccount_Login_Require"));

                return;
            }

            if ((bool)Cb_ChangePassword.IsChecked)
            {
                if(Tb_PASS_Chk.Password != Tb_PASS.Password)
                {
                    UI.DialogMessage.Show(this, LanguageHelper.FindText("Lang_Server_EditAccount_Check_New_Password"), LanguageHelper.FindText("Lang_Error"), UI.DialogMessageType.Okay);

                    return;
                }
            }

            submitting = true;

            //로그인 정보 확인
            QueryResult result = await Session.UserExistAsync(Tb_ID.Text, Tb_PASS_NOW.Password);

            if (result.Success)
            {
                string newPwd = "";
                if ((bool)Cb_ChangePassword.IsChecked)
                {
                    newPwd = Tb_PASS.Password;
                }

                QueryResult r = await Session.EditAccountAsync(Tb_Email.Text, newPwd);

                if (!r.Success)
                {
                    UI.DialogMessage.Show(this, r.Message, LanguageHelper.FindText("Lang_Error"), UI.DialogMessageType.Okay);

                    submitting = false;
                    return;
                }

                //성공
                UpdateUI();

                UI.DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Server_EditAccount_Success"));

                Close();
            }
            else
            {
                UI.DialogMessage.Show(Window.GetWindow(this), result.Message, LanguageHelper.FindText("Lang_Error"), UI.DialogMessageType.Okay);
            }

            submitting = false;
        }

        private void UpdateUI()
        {
            Tb_ID.Text = Session.UserID;
            Tb_Email.Text = Session.UserEmail;
        }

        private new void Close()
        {
            PopupOff.Begin();
        }

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void wd_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                e.Handled = true;
                Submit();
            }
        }

        private void Bt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Bt_Submit_Click(object sender, RoutedEventArgs e)
        {
            Submit();
        }

        private void Tb_PASS_NOW_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(Tb_PASS_NOW.Password == "")
            {
                Tb_PASS_NOW_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_PASS_NOW_Hint.Visibility = Visibility.Hidden;
            }
        }

        private void Tb_PASS_Chk_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Tb_PASS_Chk.Password == "")
            {
                Tb_PASS_Chk_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_PASS_Chk_Hint.Visibility = Visibility.Hidden;
            }
        }

        private void Tb_PASS_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Tb_PASS.Password == "")
            {
                Tb_PASS_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_PASS_Hint.Visibility = Visibility.Hidden;
            }
        }

        private void Tb_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Tb_Email.Text == "")
            {
                Tb_Email_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_Email_Hint.Visibility = Visibility.Hidden;
            }
        }
    }
}
