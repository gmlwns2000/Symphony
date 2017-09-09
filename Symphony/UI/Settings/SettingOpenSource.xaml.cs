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
    /// SettingOpenSource.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingOpenSource : UserControl
    {
        public SettingOpenSource()
        {
            InitializeComponent();

            FileInfo fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory+"License");

            if (fi.Exists)
            {
                licenseLabel.Text = File.ReadAllText(fi.FullName);
            }
            else
            {
                licenseLabel.Text = "라이센스 파일을 찾을 수 없습니다.";
            }
        }
    }
}
