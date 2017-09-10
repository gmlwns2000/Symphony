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
using System.Windows.Threading;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingAlbumArtLocalSearch.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingAlbumArt : UserControl
    {
        public Util.Settings Settings = Util.Settings.Current;

        public SettingAlbumArt()
        {
            DataContext = Settings;

            InitializeComponent();
            
            ListOn = FindResource("ListOn") as Storyboard;
            ListOff = FindResource("ListOff") as Storyboard;
        }
        
        Storyboard ListOn;
        Storyboard ListOff;
        bool inited = false;
        public void Init()
        {
            UpdateUI();
        }

        bool showlist = false;

        public void UpdateUI()
        {
            inited = false;

            string text = "";
            foreach (string s in Settings.PlayerAlbumArtSearchPathes)
            {
                text += "\n" + s;
            }
            text = text.Trim('\n');
            textBox.Text = text;

            Cb_Use.IsChecked = Settings.PlayerUseSearchLocalAlbumArt;

            inited = true;
        }

        DispatcherTimer timerText;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (inited)
            {
                if(timerText == null)
                {
                    timerText = new DispatcherTimer();
                    timerText.Interval = TimeSpan.FromMilliseconds(1000);
                    timerText.Tick += TimerText_Tick;
                }

                if (timerText.IsEnabled)
                    timerText.Stop();

                timerText.Start();
            }
        }

        private void TimerText_Tick(object sender, EventArgs e)
        {
            if (inited)
            {
                string[] split = textBox.Text.Replace("\r", string.Empty).Split('\n');
                List<string> ret = new List<string>();

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = split[i].Trim();
                    if (!Util.TextTool.StringEmpty(split[i]))
                    {
                        ret.Add(split[i]);
                    }
                }

                Settings.PlayerAlbumArtSearchPathes = ret.ToArray();
            }

            timerText.Stop();
        }

        private void Bt_Show_Click(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                showlist = !showlist;
                
                UpdateUI();

                if (showlist)
                {
                    Bt_Show.Content = Util.LanguageHelper.FindText("Lang_Setting_General_Player_AlbumArt_LocalSearchPath_Hide");

                    ListOn.Begin();
                }
                else
                {
                    Bt_Show.Content = Util.LanguageHelper.FindText("Lang_Setting_General_Player_AlbumArt_LocalSearchPath_More");

                    ListOff.Begin();
                }
            }
        }
    }
}
