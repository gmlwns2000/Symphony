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
    /// RegisterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RegisterWindow : Window
    {
        Storyboard PopupOff;
        Storyboard LoadingOn;
        Storyboard LoadingOff;

        public RegisterWindow(Window Parent)
        {
            InitializeComponent();

            Owner = Parent;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            LoadingOn = FindResource("LoadingOn") as Storyboard;
            LoadingOff = FindResource("LoadingOff") as Storyboard;
        }

        public new void Show()
        {
            ShowDialog();
        }

        public new void ShowDialog()
        {
            base.ShowDialog();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        private void Tb_Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(Tb_Email.Text == "")
            {
                Tb_Email_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_Email_Hint.Visibility = Visibility.Hidden;
            }
        }

        private void Tb_ID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Tb_ID.Text == "")
            {
                Tb_ID_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_ID_Hint.Visibility = Visibility.Hidden;
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

        private void Tb_PASS_Chks_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Tb_PASS_Chks.Password == "")
            {
                Tb_PASS_Chk_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_PASS_Chk_Hint.Visibility = Visibility.Hidden;
            }
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Bt_Register_Click(object sender, RoutedEventArgs e)
        {
            Register();
        }

        private void Bt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        bool _registering = false;
        bool registering
        {
            get
            {
                return _registering;
            }
            set
            {
                _registering = value;

                if (value)
                {
                    Grid_Form.IsEnabled = false;

                    if(LoadingOff != null && LoadingOn != null)
                    {
                        LoadingOff.Stop();
                        LoadingOn.Begin();
                    }
                }
                else
                {
                    Grid_Form.IsEnabled = true;

                    if (LoadingOff != null && LoadingOn != null)
                    {
                        LoadingOn.Stop();
                        LoadingOff.Begin();
                    }
                }
            }
        }

        private async void Register()
        {
            if (registering)
                return;

            bool success = false;
            
            registering = true;

            QueryResult result = await Session.RegisterAsync(Tb_ID.Text, Tb_PASS.Password, Tb_Email.Text);

            if (result.Success)
            {
                //성공
                UI.DialogMessage.Show(this, result.Message, LanguageHelper.FindText("Lang_Server_Register_Welcome"), UI.DialogMessageType.Okay);

                success = true;

                Close();

                return;
            }
            else
            {
                //실패
                UI.DialogMessage.Show(this, result.Message, LanguageHelper.FindText("Lang_Error"), UI.DialogMessageType.Okay);
            }

            registering = false;

            if (!success)
            {
                Tb_PASS.Password = "";
                Tb_PASS_Chks.Password = "";
            }
        }

        private new void Close()
        {
            PopupOff.Begin();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                e.Handled = true;
                Register();
            }
        }
    }
}
