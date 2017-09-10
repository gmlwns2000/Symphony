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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Symphony.Player;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Diagnostics;

namespace Symphony.UI
{
    /// <summary>
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {
        private Storyboard PopupOff;

        private Settings.SettingMainPage mainPage;
        private Settings.SettingOpenSource openSource;
        private Settings.SettingSoundEffect soundEffect;
        private Settings.SettingSkin skinSetting;
        private Settings.SettingVisualGeneral visualGeneral;
        private Settings.SettingVisualizer visualizer;
        private Settings.SettingAccount account;
        private Settings.SettingSoundGeneral soundGeneral;
        private Settings.SettingGeneralPlayer generalPlayer;

        MainWindow mw;

        ShadowWindow shadow;

        public SettingWindow(MainWindow Parent, ref PlayerCore np)
        {
            InitializeComponent();

            mw = Parent;
            mw.SetRenderUI(false);

            shadow = new ShadowWindow(this, mw);
            
            DataContext = this;

            mainPage = new Settings.SettingMainPage();
            mainPage.Width = double.NaN;
            mainPage.Height = double.NaN;

            openSource = new Settings.SettingOpenSource();
            openSource.Width = double.NaN;
            openSource.Height = double.NaN;

            soundEffect = new Settings.SettingSoundEffect();
            soundEffect.init_Effects(ref np, mw);
            soundEffect.Width = double.NaN;
            soundEffect.Height = double.NaN;

            skinSetting = new Settings.SettingSkin(mw);
            skinSetting.Width = double.NaN;
            skinSetting.Height = double.NaN;

            visualGeneral = new Settings.SettingVisualGeneral(mw);
            visualGeneral.Width = double.NaN;
            visualGeneral.Height = double.NaN;

            visualizer = new Settings.SettingVisualizer();
            visualizer.Width = double.NaN;
            visualizer.Height = double.NaN;

            account = new Settings.SettingAccount();
            account.Width = double.NaN;
            account.Height = double.NaN;

            soundGeneral = new Settings.SettingSoundGeneral();
            soundGeneral.Width = double.NaN;
            soundGeneral.Height = double.NaN;

            generalPlayer = new Settings.SettingGeneralPlayer();
            generalPlayer.Width = double.NaN;
            generalPlayer.Height = double.NaN;

            Parent.Closed += Parent_Closed;
            
            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
        }

        #region Window Control

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(Grid_Background);
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(Grid_Background);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mw.SetRenderUI(false);
            PopupOff.Begin();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            mw.SetRenderUI(true);
            Close();
        }

        private void Parent_Closed(object sender, EventArgs e)
        {
            Close();
        }
        
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mw.StopRenderingWhileClicking();
            DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
        #endregion Window Control

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = treeView.SelectedItem as TreeViewItem;
            string pageName = item.Tag as string;
            if(pageName!=String.Empty && !String.IsNullOrEmpty(pageName))
            {
                userControlContainer.Children.Clear();
                switch (pageName)
                {
                    case "mainPage":
                        userControlContainer.Children.Add(mainPage);
                        break;
                    case "openSource":
                        userControlContainer.Children.Add(openSource);
                        break;
                    case "soundEffect":
                        userControlContainer.Children.Add(soundEffect);
                        break;
                    case "uiVisualGeneral":
                        userControlContainer.Children.Add(visualGeneral);
                        break;
                    case "uiVisualizer":
                        userControlContainer.Children.Add(visualizer);
                        break;
                    case "uiSkin":
                        userControlContainer.Children.Add(skinSetting);
                        break;
                    case "generalAccount":
                        userControlContainer.Children.Add(account);
                        break;
                    case "soundGeneral":
                        userControlContainer.Children.Add(soundGeneral);
                        break;
                    case "generalPlayer":
                        userControlContainer.Children.Add(generalPlayer);
                        break;
                    default:
                        Debug.WriteLine(pageName);
                        break;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainPage = null;
            openSource = null;
            soundEffect = null;
            GC.SuppressFinalize(this);
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            mw.SetRenderUI(true);
        }
    }
}
