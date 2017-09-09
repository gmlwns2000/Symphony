using Ionic.Zip;
using Microsoft.Win32;
using Symphony.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingSkin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingSkin : UserControl
    {
        bool inited = false;
        MainWindow mw;
        List<string> items = new List<string>();
        DispatcherTimer timer = new DispatcherTimer();

        public SettingSkin(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;

            UpdateList();

            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();
            Loaded += SettingSkin_Loaded;

            inited = true;
        }

        private void SettingSkin_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Closed += SettingSkin_Closed;
        }

        private void SettingSkin_Closed(object sender, EventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void UpdateList()
        {
            items.Clear();

            DirectoryInfo di_setting = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings"));

            DirectoryInfo[] dis = di_setting.GetDirectories();

            foreach(DirectoryInfo di in dis)
            {
                if(File.Exists(Path.Combine(di.FullName, "skin.xaml")))
                {
                    items.Add(di.Name);
                }
            }

            Lst_Data.ItemsSource = items;
            Lst_Data.Items.Refresh();

            for(int i =0; i < items.Count; i++)
            {
                if (items[i] == mw.CurrentTheme)
                {
                    Lst_Data.SelectedIndex = i;
                    break;
                }
            }

            Lb_CurrentTheme.Text = LanguageHelper.FindText("Lang_Setting_Video_Skin_CurrentSkin") + " " + mw.CurrentTheme;
        }

        private void Lst_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (inited)
            {
                SkinEditor se = new SkinEditor(mw, items[Lst_Data.SelectedIndex]);
            }
        }

        OpenFileDialog ofd;

        private void Bt_Open_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                if(ofd == null)
                {
                    ofd = new OpenFileDialog();
                    ofd.Title = LanguageHelper.FindText("Lang_Skin_Open");
                    ofd.Filter = LanguageHelper.FindText("Lang_SkinFile") + "|*.symskin";
                    ofd.FileOk += Ofd_FileOk;
                }

                ofd.ShowDialog(Window.GetWindow(this));
            }
        }

        private void Ofd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    using (ZipFile zip = new ZipFile(ofd.FileName))
                    {
                        zip.ExtractAll(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings"), ExtractExistingFileAction.OverwriteSilently);

                        UpdateList();
                    }
                }
                catch (Exception exc)
                {
                    Logger.Error(this, exc);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_OpenErrorMsg") + "\n" + exc.ToString());
                }
            }
        }

        private void Bt_New_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                DialogText dt = new DialogText(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_MakeNewSkinMsg"), LanguageHelper.FindText("Lang_Setting_Video_Skin_DefaultName") + " " + (items.Count+1).ToString(), true, LanguageHelper.FindText("Lang_Make"), LanguageHelper.FindText("Lang_Cancel"));
                DialogTextResult result = dt.Show();
                if (result.Okay)
                {
                    string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", result.Text);

                    if (Directory.Exists(targetPath))
                    {
                        DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_AlreadyExist"));
                    }
                    else
                    {
                        if(ThemeHelper.CreateTheme(result.Text) != null)
                        {
                            mw.CurrentTheme = result.Text;

                            ThemeHelper.LoadTheme(new DirectoryInfo(targetPath), mw);
                        }
                    }
                }

                UpdateList();
            }
        }

        private void Bt_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (inited && Lst_Data.SelectedIndex >= 0)
            {
                if(ThemeHelper.SkinEditor != null && ThemeHelper.SkinEditor.helper.ThemeName == items[Lst_Data.SelectedIndex])
                {
                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_DeleteEdittingSkin"));

                    return;
                }

                DialogMessageResult result = DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_Delete_Confirm"), LanguageHelper.FindText("Lang_Setting_Video_Skin_Delete_Confirm"), DialogMessageType.YesNo);
                if(result == DialogMessageResult.Yes)
                {
                    DialogMessageResult reresult = DialogMessage.Show(Window.GetWindow(this), string.Format(LanguageHelper.FindText("Lang_Setting_Video_Skin_Delete_Msg"), items[Lst_Data.SelectedIndex]), LanguageHelper.FindText("Lang_Setting_Video_Skin_Delete_Confirm"), DialogMessageType.OkayCanel);
                    if(reresult == DialogMessageResult.Okay)
                    {
                        RemoveTheme(items[Lst_Data.SelectedIndex], 0, "");

                        UpdateList();

                        if(items.Count <= 0)
                        {
                            ThemeHelper.CreateTheme("Default Theme");
                        }

                        ThemeHelper.LoadTheme(new DirectoryInfo(Path.Combine(ThemeHelper.LibraryFolder, "Default Theme")), mw);

                        mw.CurrentTheme = "Default Theme";

                        UpdateList();
                    }
                }
            }
        }

        int MaxRetry = 5;

        private void RemoveTheme(string target, int retry, string exception)
        {
            if(retry > MaxRetry)
            {
                DialogMessage.Show(null, exception, LanguageHelper.FindText("Lang_Error"), DialogMessageType.Okay);

                return;
            }

            try
            {
                string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", target);

                if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                }
            }
            catch(Exception e)
            {
                Logger.Error(this, e);

                RemoveTheme(target, retry + 1, e.ToString());
            }
        }

        private void Lst_Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited && ThemeHelper.SkinEditor == null)
            {
                if (Lst_Data.SelectedIndex >= 0)
                {
                    DirectoryInfo di_target = new DirectoryInfo(Path.Combine(ThemeHelper.LibraryFolder, items[Lst_Data.SelectedIndex]));

                    ResourceDictionary dic = ThemeHelper.LoadTheme(di_target, mw);

                    if (dic != null)
                    {
                        mw.CurrentTheme = items[Lst_Data.SelectedIndex];
                        
                        Lb_CurrentTheme.Text = LanguageHelper.FindText("Lang_Setting_Video_Skin_CurrentSkin") + " " + mw.CurrentTheme;
                    }
                }
                else
                {
                    if (items.Count > 0)
                    {
                        Lst_Data.SelectedIndex = 0;
                    }
                    else
                    {
                        Lb_CurrentTheme.Text = LanguageHelper.FindText("Lang_Setting_Video_Skin_NothingSelected");
                    }
                }
            }
        }

        SaveFileDialog sfd;

        private void Bt_Export_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                if(Lst_Data.SelectedIndex > -1 && Lst_Data.SelectedIndex < items.Count)
                {
                    if(sfd == null)
                    {
                        sfd = new SaveFileDialog();
                        sfd.Title = LanguageHelper.FindText("Lang_Skin_Save");
                        sfd.DefaultExt = "symskin";
                        sfd.Filter = LanguageHelper.FindText("Lang_SkinFile") + "|*.symskin";
                        sfd.FileOk += Sfd_FileOk;
                    }
                    sfd.ShowDialog(Window.GetWindow(this));
                }
            }
        }

        private void Sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    if (File.Exists(sfd.FileName))
                    {
                        File.Delete(sfd.FileName);
                    }
                }
                catch
                {
                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_OtherPrgmUsing"));
                    return;
                }

                try
                {
                    ZipFile zip = new ZipFile(Encoding.UTF8);

                    zip.AddDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", items[Lst_Data.SelectedIndex]), items[Lst_Data.SelectedIndex]);

                    zip.Save(sfd.FileName);
                }
                catch (Exception exc)
                {
                    Logger.Error(this, exc);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_SaveErrorMsg") + "\n"+exc.ToString());
                }
            }
        }

        private void Bt_Rename_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                if (ThemeHelper.SkinEditor != null && ThemeHelper.SkinEditor.helper.ThemeName == items[Lst_Data.SelectedIndex])
                {
                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_EditEdittingSkin"));

                    return;
                }

                DialogTextResult result = new DialogText(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_EditName"), items[Lst_Data.SelectedIndex], true).Show();
                string skin = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", items[Lst_Data.SelectedIndex]);

                if (result.Okay && Directory.Exists(skin) && File.Exists(Path.Combine(skin,"skin.xaml")))
                {
                    string dist = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", result.Text);

                    if (Directory.Exists(dist))
                    {
                        DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_AlreadyExist"));
                    }
                    else
                    {
                        try
                        {
                            Directory.Move(skin, dist);
                            mw.CurrentTheme = result.Text;
                            UpdateList();
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine(exc.ToString());
                            DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Video_Skin_RenameErrorMsg") + exc.ToString());
                        }
                    }
                }
            }
        }
    }
}
