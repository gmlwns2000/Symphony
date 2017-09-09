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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingLanguage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingLanguage : UserControl
    {
        bool inited = false;

        List<Util.LanguagePack> Items = new List<Util.LanguagePack>();

        public SettingLanguage()
        {
            InitializeComponent();

            LoadList();
        }

        public async void LoadList()
        {
            Task t = new Task(new Action(_LoadList));

            t.Start();

            await t;
        }

        public void _LoadList()
        {
            Items.Clear();

            DirectoryInfo di = new DirectoryInfo(Util.LanguageHelper.LibraryDirectory);
            FileInfo[] fis = di.GetFiles();
            foreach(FileInfo fi in fis)
            {
                LanguagePack pack = new LanguagePack(fi);
                if (pack.inited)
                {
                    Items.Add(pack);
                }
            }

            Dispatcher.Invoke(new Action(() => 
            {
                Cb_Languages.Items.Clear();
                Cb_Languages.ItemsSource = Items;
                Cb_Languages.Items.Refresh();

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].FileName == LanguageHelper.LanguageFileName)
                    {
                        Cb_Languages.SelectedIndex = i;

                        break;
                    }
                }
            }));

            inited = true;
        }

        private void Cb_Languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited && Cb_Languages.SelectedIndex > -1)
            {
                LanguageHelper.LoadLanguagePack(new FileInfo(System.IO.Path.Combine(LanguageHelper.LibraryDirectory, Items[Cb_Languages.SelectedIndex].FileName)));
            }
        }
    }
}
