using Symphony.Util;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        private Storyboard PopupOff;
        private Storyboard LoginOn;
        private Storyboard LoginOff;

        private bool inited = false;
        private bool _loginning = false;
        private bool loginning
        {
            get
            {
                return _loginning;
            }
            set
            {
                _loginning = value;

                if (value)
                {
                    LoginOff.Stop();
                    LoginOn.Begin();
                }
                else
                {
                    LoginOn.Stop();
                    LoginOff.Begin();
                }
            }
        }

        public LoginWindow(Window parent)
        {
            InitializeComponent();
            Owner = parent;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
            LoginOn = FindResource("LoginOn") as Storyboard;
            LoginOff = FindResource("LoginOff") as Storyboard;

            LoadLoginData();

            Closed += LoginWindow_Closed;

            inited = true;
        }

        Task<QueryResult> loginTask;
        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            if(loginTask!= null)
            {
                loginTask.Wait(10);
                loginTask.Dispose();
                loginTask = null;
            }
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        private void LoadLoginData()
        {
            DirectoryInfo settingFolder = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings"));
            FileInfo settingFile = new FileInfo(System.IO.Path.Combine(settingFolder.FullName, "LoginData.dat"));

            if (!settingFolder.Exists)
            {
                settingFolder.Create();
            }

            string[] db;

            if (settingFile.Exists)
            {
                db = File.ReadAllLines(settingFile.FullName);

                foreach(string line in db)
                {
                    if (line.ToLower().StartsWith("auto_login="))
                    {
                        string[] spl = line.Split("=".ToCharArray(), 2);
                        if (!Util.TextTool.StringEmpty(spl[1]))
                        {
                            if(spl[1].ToLower() == "true")
                            {
                                checkBox.IsChecked = true;
                            }
                            else
                            {
                                checkBox.IsChecked = false;
                            }
                        }
                        else
                        {
                            checkBox.IsChecked = false;
                        }
                    }
                    else if (line.ToLower().StartsWith("uid="))
                    {
                        string[] spl = line.Split("=".ToCharArray(), 2);

                        if (!Util.TextTool.StringEmpty(spl[1]))
                        {
                            Tb_ID.Text = spl[1];
                        }
                        else
                        {
                            Tb_ID.Text = "";
                        }
                    }
                    else if (line.ToLower().StartsWith("password="))
                    {
                        string[] spl = line.Split("=".ToCharArray(), 2);

                        if (!Util.TextTool.StringEmpty(spl[1]))
                        {
                            try
                            {
                                Tb_PASS.Password = Util.StringCipher.Decrypt(spl[1], "symphony-Project@2016_");
                            }
                            catch
                            {
                                Tb_PASS.Password = "";
                            }
                        }
                        else
                        {
                            Tb_PASS.Password = "";
                        }
                    }
                }
            }
            else
            {
                string init = "auto_login=false\nuid=\npassword=";

                File.WriteAllText(settingFile.FullName, init);
            }
        }

        private void SaveLoginData()
        {
            DirectoryInfo settingFolder = new DirectoryInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings"));
            FileInfo settingFile = new FileInfo(System.IO.Path.Combine(settingFolder.FullName, "LoginData.dat"));

            if (!settingFolder.Exists)
            {
                settingFolder.Create();
            }

            string db = "";

            if ((bool)checkBox.IsChecked)
            {
                db += "auto_login=true";
            }
            else
            {
                db += "auto_login=false";
            }

            if ((bool)checkBox.IsChecked)
            {
                db += string.Format("\nuid={0}\npassword={1}", Tb_ID.Text, Util.StringCipher.Encrypt(Tb_PASS.Password, "symphony-Project@2016_"));
            }

            File.WriteAllText(settingFile.FullName, db);
        }

        private async void Login()
        {
            if (loginning)
            {
                return;
            }

            loginning = true;

            loginTask = Session.LoginAsync(Tb_ID.Text, Tb_PASS.Password);
            QueryResult r = await loginTask;
            SaveLoginData();

            if (r.Success)
            {
                Close();
            }
            else
            {
                UI.DialogMessage.Show(this, r.Message, LanguageHelper.FindText("Lang_Error"), UI.DialogMessageType.Okay);

                Tb_PASS.Password = "";
                loginning = false;
            }
        }

        private new void Close()
        {
            PopupOff.Begin();
        }

        private void Bt_Submit_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Bt_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                e.Handled = true;
                Login();
            }
        }

        private void Tb_PASS_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(Tb_PASS.Password == "")
            {
                Tb_PASS_Hint.Visibility = Visibility.Visible;
            }
            else
            {
                Tb_PASS_Hint.Visibility = Visibility.Hidden;
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

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Bt_Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow rw = new RegisterWindow(this);
            rw.ShowDialog();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                return;
            }

            UI.DialogMessageResult r = UI.DialogMessage.Show(this, LanguageHelper.FindText("Lang_Server_Login_Auto_Login_Warning"), LanguageHelper.FindText("Lang_Confirm"), UI.DialogMessageType.OkayCanel);

            if(r != UI.DialogMessageResult.Okay)
            {
                checkBox.IsChecked = false;
            }
        }
    }
}
