using Symphony.Server;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingAccount : UserControl
    {
        Storyboard InfoOn;
        Storyboard InfoOff;
        Storyboard RegisterOn;
        Storyboard RegisterOff;

        public SettingAccount()
        {
            InitializeComponent();

            InfoOn = FindResource("InfoOn") as Storyboard;
            InfoOff = FindResource("InfoOff") as Storyboard;
            RegisterOn = FindResource("RegisterOn") as Storyboard;
            RegisterOff = FindResource("RegisterOff") as Storyboard;
            
            Session.LoginChanged += Session_LoginChanged;
            this.Loaded += SettingAccount_Loaded;

            UpdateUI();
        }

        private void SettingAccount_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closed += SettingAccount_Closed;
        }

        private void Session_LoginChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(UpdateUI));
        }

        private void SettingAccount_Closed(object sender, EventArgs e)
        {
            Session.LoginChanged -= Session_LoginChanged;
        }

        bool preState = !Session.IsLogined;
        private void UpdateUI()
        {
            bool loginState = Session.IsLogined;
            if (loginState)
            {
                Tb_ID.Text = Session.UserID;
                Tb_Email.Text = Session.UserEmail;
                Tb_ID_Hint.Text = LanguageHelper.FindText("Lang_Setting_General_Account_Logout");

                if (preState != loginState)
                {
                    InfoOn.Begin();
                    RegisterOff.Begin();
                }

                Grid_EditAccount.IsEnabled = true;
                Grid_NewAccount.IsEnabled = false;
            }
            else
            {
                Tb_ID.Text = LanguageHelper.FindText("Lang_Setting_General_Account_PleaseLogin");
                Tb_ID_Hint.Text = LanguageHelper.FindText("Lang_Setting_General_Account_Login");

                if (preState != loginState)
                {
                    InfoOff.Begin();
                    RegisterOn.Begin();
                }

                Grid_EditAccount.IsEnabled = false;
                Grid_NewAccount.IsEnabled = true;
            }

            preState = loginState;
        }

        private void Bt_Unregister_Click(object sender, RoutedEventArgs e)
        {
            Unregister();
        }

        private async void Unregister()
        {
            DialogMessageResult result = DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_General_Account_Unregister_Confirm"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

            if(result == DialogMessageResult.Yes)
            {
                DialogMessageResult confirm = DialogMessage.Show(Window.GetWindow(this), string.Format(LanguageHelper.FindText("Lang_Setting_General_Account_Unregister_Goodbye"), Session.UserID), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.OkayCanel);
                
                if(confirm == DialogMessageResult.Okay)
                {
                    QueryResult qr = await Session.UnregisterAsync();

                    if (!qr.Success)
                    {
                        DialogMessage.Show(Window.GetWindow(this), qr.Message, LanguageHelper.FindText("Lang_Error"), DialogMessageType.Okay);
                    }
                }
            }

            UpdateUI();
        }

        private void Bt_EditAccountInfo_Click(object sender, RoutedEventArgs e)
        {
            EditAccountWindow eaw = new EditAccountWindow(Window.GetWindow(this));
            eaw.ShowDialog();

            UpdateUI();
        }

        private void Bt_NewAccount_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow(Window.GetWindow(this));

            rw.Show();

            UpdateUI();
        }

        private void Elipse_LoginInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool handled = false;

            if (Session.IsLogined)
            {
                DialogMessageResult result = DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_General_Account_Logout_Confirm"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

                if (result == DialogMessageResult.Yes)
                {
                    Session.Logout();
                }
                else
                {
                    handled = true;
                }
            }
            else
            {
                LoginWindow lw = new LoginWindow(Window.GetWindow(this));
                lw.ShowDialog();
            }

            if (!handled)
                UpdateUI();
        }
    }
}
