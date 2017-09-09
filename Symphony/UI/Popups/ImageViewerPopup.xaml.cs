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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Symphony.UI
{
    /// <summary>
    /// ImageViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewerPopup : Window, IDisposable
    {
        Storyboard PopupOff;
        Storyboard MouseMoveOn;
        Storyboard MouseMoveOff;
        
        MainWindow mw;

        List<string> Files = new List<string>();
        string groupTitle;
        int FileIndex = 0;
        ImageViewer currentViewer;

        ShadowWindow shadow;

        public ImageViewerPopup(MainWindow mw, Window Parent, List<string> imgSources, int index, string title)
        {
            InitializeComponent();

            shadow = new ShadowWindow(this, mw, 20, 0.28, true);
            shadow.Show();

            this.mw = mw;
            Owner = Parent;
            
            groupTitle = title;
            Files = imgSources;
            FileIndex = index;
            UpdateControl();

            currentViewer = new ImageViewer();

            Grid_Background.Children.Add(currentViewer);

            currentViewer.Init(mw, Files[FileIndex], title);

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;
            MouseMoveOn = (Storyboard)FindResource("MouseMoveOn");
            MouseMoveOff = (Storyboard)FindResource("MouseMoveOff");

            mw.SetRenderUI(false);
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            mw.SetRenderUI(true);

            if (currentViewer != null)
            {
                currentViewer.StopAnimation();
            }

            base.Close();
        }

        private new void Close()
        {
            mw.SetRenderUI(false);

            PopupOff.Begin();
        }

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mw.StopRenderingWhileClicking();
            DragMove();
        }

        private void Grid_Background_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(currentViewer != null)
            {
                currentViewer.UserControl_PreviewMouseWheel(sender, e);
            }
        }

        bool controlOn = false;
        DispatcherTimer controlOffTimer;
        private void wd_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (grid != null)
            {
                if (!controlOn)
                {
                    MouseMoveOff.Stop();

                    MouseMoveOn.Begin();
                    
                    controlOn = true;

                    if(controlOffTimer == null)
                    {
                        controlOffTimer = new DispatcherTimer();
                        controlOffTimer.Interval = TimeSpan.FromSeconds(2);
                        controlOffTimer.Tick += delegate
                        {
                            MouseMoveOn.Stop();

                            MouseMoveOff.Begin();

                            controlOn = false;

                            controlOffTimer.Stop();
                        };
                    }

                    if (controlOffTimer.IsEnabled)
                    {
                        controlOffTimer.Stop();
                    }
                    controlOffTimer.Start();
                }
            }
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            mw.SetRenderUI(true);
        }

        SaveFileDialog sfd;
        private void Menu_File_Save_Click(object sender, RoutedEventArgs e)
        {
            if(sfd == null && currentViewer != null)
            {
                sfd = new SaveFileDialog();
                sfd.Title = LanguageHelper.FindText("Lang_Save");
                sfd.Filter = Util.IO.SupportedImageFilter;
                sfd.DefaultExt = "." + currentViewer.FormatText.ToLower();
                sfd.FileOk += Sfd_FileOk;
            }
            sfd.ShowDialog(this);
        }

        private void Sfd_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                try
                {
                    File.Copy(currentViewer.ImagePath, sfd.FileName);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);
                    
                    DialogMessage.Show(this, LanguageHelper.FindText("Lang_ImageViewer_FileSavingError"), LanguageHelper.FindText("Lang_Error"), DialogMessageType.Okay);
                }
            }
        }

        private void Menu_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Dispose()
        {
            PopupOff.Remove();

            if (currentViewer != null)
            {
                currentViewer.Dispose();
            }
        }

        #region SlideShow

        int count = 0;
        int _storyboardRunning = 0;
        int storyboardRunning
        {
            get
            {
                return _storyboardRunning;
            }
            set
            {
                _storyboardRunning = value;
                if(value == 0)
                {
                    mw.SetRenderUI(true);
                }
                else
                {
                    mw.SetRenderUI(false);
                }
            }
        }

        double Fade_Duration = 240;
        private void FadeOut(double from, double to)
        {
            Storyboard storyFadeOut = new Storyboard();

            ImageViewer v = currentViewer;

            string nameOut = "viewer" + count.ToString();
            RegisterName(nameOut, v);
            count++;

            TransformGroup groupOut = new TransformGroup();
            TranslateTransform translate = new TranslateTransform(from,0);
            groupOut.Children.Add(translate);
            currentViewer.RenderTransform = groupOut;

            DoubleAnimation fadeOut = new DoubleAnimation();
            fadeOut.Duration = new Duration(TimeSpan.FromMilliseconds(Fade_Duration));
            fadeOut.From = 1;
            fadeOut.To = 0;

            DoubleAnimationUsingKeyFrames moveRight = new DoubleAnimationUsingKeyFrames();
            moveRight.Duration = new Duration(TimeSpan.FromMilliseconds(Fade_Duration));
            moveRight.KeyFrames.Add(new SplineDoubleKeyFrame(from));
            moveRight.KeyFrames.Add(new SplineDoubleKeyFrame(to, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(Fade_Duration)), new KeySpline(0, 0, 0, 1)));

            storyFadeOut.Children.Add(moveRight);
            storyFadeOut.Children.Add(fadeOut);

            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));
            Storyboard.SetTargetName(fadeOut, nameOut);
            Storyboard.SetTargetProperty(moveRight, new PropertyPath("RenderTransform.Children[0].X"));
            Storyboard.SetTargetName(moveRight, nameOut);

            Timeline.SetDesiredFrameRate(storyFadeOut, 60);
            storyFadeOut.Completed += delegate
            {
                v.Dispose();
                Grid_Background.Children.Remove(v);
                UnregisterName(nameOut);
                v = null;

                storyboardRunning--;
            };

            storyFadeOut.Begin(this);
            storyboardRunning++;
        }

        private void FadeIn(double from, double to)
        {
            ImageViewer vv = new ImageViewer();
            currentViewer = vv;

            Grid_Background.Children.Add(vv);
            vv.Init(mw, Files[FileIndex], groupTitle);

            Storyboard storyFadeIn = new Storyboard();

            TransformGroup groupIn = new TransformGroup();
            TranslateTransform translateIn = new TranslateTransform(from, 0);
            groupIn.Children.Add(translateIn);
            vv.RenderTransform = groupIn;

            DoubleAnimationUsingKeyFrames moveRightIn = new DoubleAnimationUsingKeyFrames();
            moveRightIn.Duration = new Duration(TimeSpan.FromMilliseconds(Fade_Duration));
            moveRightIn.KeyFrames.Add(new SplineDoubleKeyFrame(from));
            moveRightIn.KeyFrames.Add(new SplineDoubleKeyFrame(to, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(Fade_Duration)), new KeySpline(0, 0, 0, 1)));

            DoubleAnimation fadeIn = new DoubleAnimation();
            fadeIn.Duration = new Duration(TimeSpan.FromMilliseconds(Fade_Duration));
            fadeIn.From = 0;
            fadeIn.To = 1;

            storyFadeIn.Children.Add(moveRightIn);
            storyFadeIn.Children.Add(fadeIn);

            Storyboard.SetTargetProperty(moveRightIn, new PropertyPath("RenderTransform.Children[0].X"));
            Storyboard.SetTarget(moveRightIn, vv);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(fadeIn, vv);

            Timeline.SetDesiredFrameRate(storyFadeIn, 60);
            storyFadeIn.Completed += delegate
            {
                if (vv.RenderTransform == groupIn)
                {
                    vv.RenderTransform = null;
                }
                storyboardRunning--;
            };

            storyFadeIn.Begin();
            storyboardRunning++;
        }

        private void Bt_Next_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(FileIndex < Files.Count - 1)
            {
                double length = Math.Min(Grid_Background.ActualWidth / 2, 320);

                FadeOut(0, -length);

                FileIndex++;
                UpdateControl();

                FadeIn(length, 0);
            }
        }

        private void Bt_Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (FileIndex > 0)
            {
                double length = Math.Min(Grid_Background.ActualWidth / 2, 320);

                FadeOut(0, length);

                FileIndex--;
                UpdateControl();

                FadeIn(-length, 0);
            }
        }

        private void UpdateControl()
        {
            Tb_Index.Text = (FileIndex + 1).ToString() + "/" + Files.Count.ToString();

            if(FileIndex == 0)
            {
                Bt_Back.Visibility = Visibility.Hidden;
            }
            else
            {
                Bt_Back.Visibility = Visibility.Visible;
            }

            if(FileIndex == Files.Count - 1)
            {
                Bt_Next.Visibility = Visibility.Hidden;
            }
            else
            {
                Bt_Next.Visibility = Visibility.Visible;
            }
        }

        #endregion SlideShow
    }
}