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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingVisualGeneral.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingVisualGeneral : UserControl
    {
        bool inited = false;
        MainWindow mw;

        public SettingVisualGeneral(MainWindow mw)
        {
            InitializeComponent();

            this.mw = mw;

            UpdateUI();

            inited = true;
        }

        private void UpdateUI()
        {
            inited = false;

            Cb_General_UseFooterInfo.IsChecked = mw.UseFooterInfoText;
            Cb_General_UseImageAnimation.IsChecked = mw.UseImageAnimation;
            Cb_General_SavePlayerMode.IsChecked = mw.SaveWindowMode;
            Sld_General_GUIUpdateFPS.Value = 1000 / mw.GUIUpdate;

            Cb_Singer_DragMove.IsChecked = mw.SingerCanDragmove;
            Cb_Singer_ResetPosition.IsChecked = mw.SingerResetPosition;
            Cb_Singer_Use.IsChecked = mw.SingerShow;
            Sld_Singer_Opacity.Value = mw.SingerOpacity * 100;
            Sld_Singer_Zoom.Value = mw.SingerZoom * 100;
            Cbb_Singer_FadeIn.SelectedIndex = (int)mw.SingerDefaultFadeInMode;
            Cbb_Singer_FadeOut.SelectedIndex = (int)mw.SingerDefaultFadeOutMode;
            switch (mw.SingerHorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Cbb_Singer_HorizontalAlignment.SelectedIndex = 0;
                    break;
                case HorizontalAlignment.Center:
                    Cbb_Singer_HorizontalAlignment.SelectedIndex = 1;
                    break;
                case HorizontalAlignment.Right:
                    Cbb_Singer_HorizontalAlignment.SelectedIndex = 2;
                    break;
                default:
                    break;
            }
            switch (mw.SingerVerticalAlignment)
            {
                case VerticalAlignment.Top:
                    Cbb_Singer_VerticalAlignment.SelectedIndex = 0;
                    break;
                case VerticalAlignment.Center:
                    Cbb_Singer_VerticalAlignment.SelectedIndex = 1;
                    break;
                case VerticalAlignment.Bottom:
                    Cbb_Singer_VerticalAlignment.SelectedIndex = 2;
                    break;
                default:
                    break;
            }
            Cb_Singer_WindowMode.IsChecked = mw.SingerWindowMode;

            Cb_Composer_Topmost.IsChecked = mw.ComposerTopmost;
            Cb_Composer_Use.IsChecked = mw.ComposerUse;
            Cb_Composer_WindowMode.IsChecked = mw.ComposerWindowMode;
            Sld_Composer_Opacity.Value = mw.ComposerOpacity * 100;

            inited = true;
        }

        #region General

        private void Cb_General_UseImageAnimation_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.UseImageAnimation = true;
            }
        }

        private void Cb_General_UseImageAnimation_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.UseImageAnimation = false;
            }
        }

        private void Cb_General_UseFooterInfo_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.UseFooterInfoText = true;
            }
        }

        private void Cb_General_UseFooterInfo_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.UseFooterInfoText = false;
            }
        }

        private void Sld_General_GUIUpdateFPS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.GUIUpdate = (int)Math.Max(1,1000 / e.NewValue);
            }
        }

        private void Cb_General_SavePlayerMode_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SaveWindowMode = true;
            }
        }

        private void Cb_General_SavePlayerMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SaveWindowMode = false;
            }
        }

        #endregion General

        #region Singer

        private void Cb_Singer_Use_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerShow = true;
            }
        }

        private void Cb_Singer_Use_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerShow = false;
            }
        }

        private void Cb_Singer_ResetPosition_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerResetPosition = true;
            }
        }

        private void Cb_Singer_ResetPosition_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerResetPosition = false;
            }
        }

        private void Cb_Singer_DragMove_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerCanDragmove = true;
            }
        }

        private void Cb_Singer_DragMove_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerCanDragmove = false;
            }
        }

        private void Sld_Singer_Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.SingerZoom = e.NewValue / 100;
            }
        }

        private void Sld_Singer_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.SingerOpacity = e.NewValue / 100;
            }
        }

        private void Cbb_Singer_FadeIn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                mw.SingerDefaultFadeInMode = (Lyrics.FadeInMode)Cbb_Singer_FadeIn.SelectedIndex;
            }
        }

        private void Cbb_Singer_FadeOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                mw.SingerDefaultFadeOutMode = (Lyrics.FadeOutMode)Cbb_Singer_FadeOut.SelectedIndex;
            }
        }

        private void Cbb_Singer_HorizontalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Singer_HorizontalAlignment.SelectedIndex)
                {
                    case 0:
                        mw.SingerHorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case 1:
                        mw.SingerHorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case 2:
                        mw.SingerHorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    default:
                        break;
                }
            }
        }

        private void Cbb_Singer_VerticalAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (inited)
            {
                switch (Cbb_Singer_VerticalAlignment.SelectedIndex)
                {
                    case 0:
                        mw.SingerVerticalAlignment = VerticalAlignment.Top;
                        break;
                    case 1:
                        mw.SingerVerticalAlignment = VerticalAlignment.Center;
                        break;
                    case 2:
                        mw.SingerVerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    default:
                        break;
                }
            }
        }

        private void Cb_Singer_WindowMode_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerWindowMode = true;
            }
        }

        private void Cb_Singer_WindowMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.SingerWindowMode = false;
            }
        }

        #endregion Singer

        #region Composer

        private void Cb_Composer_Use_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.ComposerUse = true;
            }
        }

        private void Cb_Composer_Use_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.ComposerUse = false;
            }
        }

        private void Cb_Composer_Topmost_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.ComposerTopmost = true;
            }
        }

        private void Cb_Composer_Topmost_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.ComposerTopmost = false;
            }
        }

        private void Sld_Composer_Opacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (inited)
            {
                mw.ComposerOpacity = e.NewValue / 100;
            }
        }

        private void Cb_Composer_WindowMode_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.ComposerWindowMode = true;
            }
        }

        private void Cb_Composer_WindowMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                mw.ComposerWindowMode = false;
            }
        }

        #endregion Composer
    }
}
