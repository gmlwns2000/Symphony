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

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SkinEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinEditor : Window
    {
        MainWindow mw;
        Storyboard PopupOff;
        ShadowWindow shadow;
        public ThemeHelper helper;

        bool edited = false;

        public SkinEditor(MainWindow mw, string ThemeName)
        {
            shadow = new ShadowWindow(this, mw, 12, 1, true, false);

            InitializeComponent();

            Loaded += delegate (object sender, RoutedEventArgs arg)
            {
                helper = new ThemeHelper(ThemeName, mw);
                
                Tb_Title.Text = LanguageHelper.FindText("Lang_SkinEditor_Title") + " - " + ThemeName;

                Title = Tb_Title.Text;

                Closed += SkinEditor_Closed;
                helper.Updated += Helper_Updated;

                if(helper.Dictionary == null)
                {
                    Close();

                    return;
                }

                string[] keys = helper.Dictionary.Keys.Cast<string>().OrderBy(s => s).ToArray();
                
                foreach (string key in keys)
                {
                    SkinValueEditor editor = new SkinValueEditor(key, helper);

                    ListViewItem item = new ListViewItem();

                    item.Content = editor;
                    item.CacheMode = new BitmapCache();

                    lstData.Items.Add(item);
                }
            };

            Closed += delegate (object sender, EventArgs arg)
            {
                if(ThemeHelper.SkinEditor != null && ThemeHelper.SkinEditor == this)
                {
                    ThemeHelper.SkinEditor = null;
                }
            };

            StateChanged += delegate (object sender, EventArgs arg)
            {
                if(WindowState == WindowState.Maximized)
                {
                    Grid_Background.Margin = ShadowWindow.MaximizeMargin;
                }
                else
                {
                    Grid_Background.Margin = new Thickness();
                }
            };

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            this.mw = mw;
            mw.Closed += Mw_Closed;

            Show();
        }

        private void SkinEditor_Closed(object sender, EventArgs e)
        {
            helper.Updated -= Helper_Updated;
        }

        private void Helper_Updated(object sender, EventArgs e)
        {
            if (!edited)
            {
                Tb_Title.Text += "*";

                Title = Tb_Title.Text;
            }
            edited = true;
        }

        public new void Show()
        {
            if(ThemeHelper.SkinEditor != null)
            {
                if (ThemeHelper.SkinEditor.Close())
                {
                    if (ThemeHelper.SkinEditor != null)
                    {
                        ThemeHelper.SkinEditor.Closed += delegate (object obj, EventArgs arg)
                        {
                            ThemeHelper.SkinEditor = this;
                        };
                    }
                    else
                    {
                        ThemeHelper.SkinEditor = this;
                    }

                    base.Show();
                }
                else
                {
                    shadow.Close();
                }
            }
            else
            {
                ThemeHelper.SkinEditor = this;

                base.Show();
            }
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            shadow.parentopen = false;
            base.Close();
        }

        private void Mw_Closed(object sender, EventArgs e)
        {
            shadow.parentopen = false;
            base.Close();
        }
        
        public new bool Close()
        {
            if (edited)
            {
                DialogMessageResult r = DialogMessage.Show(this, LanguageHelper.FindText("Lang_SkinEditor_CloseConfirm"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.YesNo);

                if (r != DialogMessageResult.Yes)
                {
                    return false;
                }
            }

            helper.Dispose();

            PopupOff.Begin();

            shadow.Close();

            return true;
        }

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount > 1)
            {
                if(WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else
                {
                    WindowState = WindowState.Maximized;
                }
            }
            else
            {
                shadow.DragMove();
            }
        }

        private void Menu_Save_Click(object sender, RoutedEventArgs e)
        {
            helper.Save();

            Tb_Title.Text = LanguageHelper.FindText("Lang_SkinEditor_Title") + " - " + helper.ThemeName;

            Title = Tb_Title.Text;

            edited = false;
        }

        private void Menu_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
