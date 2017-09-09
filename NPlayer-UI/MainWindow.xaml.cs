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
using NPlayer;
using NPlayer_UI;
using Microsoft.Win32;
using System.IO;

namespace NPlayer_UI
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public nPlayerCore np;
        public nPlayerEcho echo;
        public nPlayerEQ eq;
        public nPlayerLimiter limt;

        private Playlist plwindow;
        private delegate void updatedele();
        private bool updated = false;
        private bool updateGra = false;
        private System.Timers.Timer timer = new System.Timers.Timer();
        private System.Timers.Timer frameUpdater = new System.Timers.Timer();
        private System.Timers.Timer calcFrame = new System.Timers.Timer();
        private OpenFileDialog ofd = new OpenFileDialog();
        private string musicFilePath = null;
        private int graphFps = 0;

        public MainWindow()
        {
            InitializeComponent();

            np = new nPlayerCore(true);
            eq = new nPlayerEQ(ref np);
            echo = new nPlayerEcho(10000, 0.3f);
            limt = new nPlayerLimiter();

            calcFrame.Elapsed += CalcFrame_Elapsed;
            calcFrame.Interval = 1000;
            calcFrame.Start();

            np.DSPs.Add(eq);
            np.DSPs.Add(echo);
            np.DSPs.Add(limt);
            np.UpdateDSP();

            timer.Elapsed += timer_Elapsed;
            timer.Interval = 250;
            timer.Start();

            frameUpdater.Elapsed += FrameUpdater_Elapsed;
            frameUpdater.Interval = 33;
            frameUpdater.Start();

            ofd.Filter = "mp3 Files|*.mp3|wave Files|*.wav|Aiff File|*.aiff|All Files|*.*";
            barVol.Value = np.Volume * 100;
            np.PlayStopped += stopped;
            np.PlayStarted += started;
        }

        private void CalcFrame_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            graphFps = calcsample;
            calcsample = 0;
        }

        private void FrameUpdater_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateGra = true;
        }

        private void stopped()
        {
            updated = true;
            barTime.Value = 0;
        }

        #region update ui
        private void started()
        {
            np.DSPMaster.SampleComplete += DSPMaster_SampleComplete;
            UpdateMeta();
        }

        private float left = 0;
        private float right = 0;
        private int calcsample = 0;
        Queue<float> q = new Queue<float>(2000);

        private void DSPMaster_SampleComplete(object sender, SampleEventArgs e)
        {
            if (left < e.Left)
            {
                left = e.Left;
            }
            if (right < e.Right)
            {
                right = e.Right;
            }
            q.Enqueue((e.Right + e.Left) * 0.5f);
            if (q.Count >= 2000)
            {
                q.Dequeue();
            }
            if (updateGra)
            {
                calcsample++;
                updateGra = false;
                updateGraphe();
                left = 0;
                right = 0;
            }
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            update();
        }

        private void update()
        {
            if (this.Dispatcher.CheckAccess())
            {
                lbBitSec.Content = np.BitPerSecond.ToString();
                lbSampleRate.Content = np.SampleRate.ToString();
                lbBps.Content = np.BitsPerSample.ToString();
                lbFilePath.Content = musicFilePath;
                lbFileName.Content = np.FileName;
                lbFilePath.Content = np.FilePath;
                lbPlay.Content = np.isPlay.ToString();
                lbPos.Content = np.GetPosition("c") + " / " + np.GetLength("c");
                lbVol.Content = np.Volume.ToString();
                if (np.isPlay)
                {
                    updated = true;
                    barTime.Maximum = (int)np.GetLength(TIMEUNIT.MSEC);
                    barTime.Value = (int)np.GetPosition(TIMEUNIT.MSEC);
                }
            }
            else
            {
                this.Dispatcher.Invoke(new updatedele(update));
                return;
            }
        }

        private void UpdateMeta()
        {
            lbTagAlbum.Content = np.CurrentMusic.Tag.Album;
            lbTagArtist.Content = np.CurrentMusic.Tag.Artist;
            lbTagBPM.Content = np.CurrentMusic.Tag.BPM;
            lbTagGanre.Content = np.CurrentMusic.Tag.Genre;
            lbTagTitle.Content = np.CurrentMusic.Tag.Title;
            lbTagTrack.Content = np.CurrentMusic.Tag.Track;
            lbTagYear.Content = np.CurrentMusic.Tag.Year;
            richTextBox1.Text = np.CurrentMusic.Tag.Lyrics;
            AlbumArt1.Source = null;
            try
            {
                if ((np.CurrentMusic.Tag != null) && (np.CurrentMusic.Tag.Pictures[0] != null))
                {
                    MemoryStream ms = new MemoryStream(np.CurrentMusic.Tag.Pictures[0]);
                    ms.Position = 0;
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                    AlbumArt1.Source = bi;
                }
            }
            catch { Console.WriteLine("Update Meta catched"); }
        }

        private void updateGraphe()
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (np.isPlay)
                {
                    calcsample++;

                    rightRect.Height = rightRect.Height * 0.2 + Math.Abs(Math.Min(1, right)) * canvas.ActualHeight * 0.8;
                    leftRect.Height = leftRect.Height * 0.2 + Math.Abs(Math.Min(1, left)) * canvas.ActualHeight * 0.8;

                    lbFps.Content = "FPS: " + graphFps.ToString();

                    try
                    {
                        float[] qlist = q.ToArray();
                        polyline1.Points.Clear();

                        int index = 0;
                        double bin = canvas.ActualWidth / (qlist.Length);
                        for (int i = 0; i < qlist.Length; i += (int)Math.Ceiling((double)qlist.Length / ((int)canvas.ActualWidth)))
                        {
                            Point pt = new Point(bin * i, canvas.ActualHeight * 0.5f + (Math.Max(-1, Math.Min(qlist[i], 1)) * canvas.ActualHeight * 0.5f));
                            if (index >= polyline1.Points.Count)
                            {
                                polyline1.Points.Add(pt);
                            }
                            else
                            {
                                polyline1.Points[index] = pt;
                            }
                            index++;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                this.Dispatcher.Invoke(new updatedele(updateGraphe));
                return;
            }
        }
        #endregion update ui

        #region playback region
        private void Button_Click(object sender, RoutedEventArgs e) //open
        {
            ofd.ShowDialog();
            musicFilePath = ofd.FileName;
            openlb.Content = musicFilePath;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)   //play
        {
            if (np.isPlay)
            {
                if (np.isPaused)
                {
                    np.Resume();
                }
                else
                {
                    np.Stop();

                    np.Play(musicFilePath);
                }
            }
            else
            {
                np.Play(musicFilePath);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)   //pause
        {
            np.Pause();
        }

        private void stop_Click(object sender, RoutedEventArgs e)   //stop
        {
            np.Stop();
        }

        private void barVol_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)   //volume
        {
            np.SetVolume((float)barVol.Value / 100);
        }

        private void barTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)  //time
        {
            if (!updated)
            {
                np.SetPosition((int)barTime.Value);
            }
            else
            {
                updated = false;
            }
        }

        private void barTime_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)     //time1
        {
            updated = false;
        }

        private void Window_Closed(object sender, EventArgs e)      //close 
        {
            np.Stop();
            System.Windows.Application.Current.Shutdown();
        }

        private void previous_Click(object sender, RoutedEventArgs e)   //previous
        {
            np.Previous();
        }

        private void next_Click(object sender, RoutedEventArgs e)       //next
        {
            np.Next();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)       //exit   
        {
            np.Stop();
            this.Close();
        }
        #endregion playback region

        #region playlist control

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (plwindow != null)
            {
                plwindow.Close();
                plwindow = null;
            }
            else {
                plwindow = new Playlist(ref np);
                plwindow.updatePos((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);
                plwindow.Show();
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (plwindow != null)
            {
                plwindow.updatePos((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (plwindow != null)
            {
                plwindow.updatePos((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if ((plwindow != null) && (this.WindowState == WindowState.Maximized))
            {
            }
        }
        #endregion playlist control

        #region EQ
        private void rest_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 18; i++)
            {
                ChangeEQ(i, 0);
            }
            eq3.Value = 0;
            eq4.Value = 0;
            eq5.Value = 0;
            eq6.Value = 0;
            eq7.Value = 0;
            eq8.Value = 0;
            eq9.Value = 0;
            eq10.Value = 0;
            eq11.Value = 0;
            eq12.Value = 0;
            eq13.Value = 0;
            eq14.Value = 0;
            eq15.Value = 0;
            eq16.Value = 0;
            eq17.Value = 0;
            eq18.Value = 0;
            eq1.Value = 0;
            eq2.Value = 0;
        }

        private void eq1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(0, (float)eq1.Value);
        }

        private void eq2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(1, (float)eq2.Value);
        }

        private void eq3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(2, (float)eq3.Value);
        }

        private void eq4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(3, (float)eq4.Value);
        }

        private void eq5_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(4, (float)eq5.Value);
        }

        private void eq6_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(5, (float)eq6.Value);
        }

        private void eq7_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(6, (float)eq7.Value);
        }

        private void eq8_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(7, (float)eq8.Value);
        }

        private void eq9_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(8, (float)eq9.Value);
        }

        private void eq10_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(9, (float)eq10.Value);
        }

        private void eq11_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(10, (float)eq11.Value);
        }

        private void eq12_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(11, (float)eq12.Value);
        }

        private void eq13_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(12, (float)eq13.Value);
        }

        private void eq14_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(13, (float)eq14.Value);
        }

        private void eq15_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(14, (float)eq15.Value);
        }

        private void eq16_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(15, (float)eq16.Value);
        }

        private void eq17_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(16, (float)eq17.Value);
        }

        private void eq18_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEQ(17, (float)eq18.Value);
        }

        private void eqAmp_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeAmp(eqAmp.Value);
        }

        private void eqOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeEqOpacity(eqOpacity.Value);
        }

        private void ChangeEQ(int id, float gain)
        {
            eq.ChangeEQ(id, (float)gain * 0.35f);
            np.DSPs[0] = eq;
        }

        private void ChangeAmp(double value)
        {
            if (np != null)
            {
                eq.ChangeAmp((float)value);
                np.DSPs[0] = eq;
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            SetEqOn((bool)checkBox.IsChecked);
        }

        private void ChangeEqOpacity(double value)
        {
            if (np != null)
            {
                eq.SetOpacity((float)eqOpacity.Value);
                np.DSPs[0] = eq;
            }
        }

        private void SetEqOn(bool on)
        {
            if (np != null)
            {
                eq.SetStatus((bool)checkBox.IsChecked);
                np.DSPs[0] = eq;
            }
        }
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetEqOn((bool)checkBox.IsChecked);
        }

        #endregion EQ

        #region Echo
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (echo != null)
            {
                echo = new nPlayerEcho((int)sdEchoLegnth.Value, (float)sdEchoFactor.Value);
                np.DSPs[1] = echo;
            }
        }

        private void sdEchoLegnth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (echo != null)
            {
                echo = new nPlayerEcho((int)sdEchoLegnth.Value, (float)sdEchoFactor.Value);
                np.DSPs[1] = echo;
            }
        }

        private void btEchoReset_Click(object sender, RoutedEventArgs e)
        {
            if (echo != null)
            {
                echo = new nPlayerEcho(10000, 0.3f);
                sdEchoFactor.Value = 10000;
                sdEchoLegnth.Value = 0.3f;
                cbLimitOn.IsChecked = true;
                np.DSPs[1] = echo;
                np.UpdateDSP();
            }
        }
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (echo != null)
            {
                echo.SetStatus((bool)sdEchoOn.IsChecked);
                np.DSPs[1] = echo;
            }
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if (echo != null)
            {
                echo.SetStatus((bool)sdEchoOn.IsChecked);
                np.DSPs[1] = echo;
            }
        }

        #endregion Echo

        #region Limiter

        private void sdLimitLimit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sdLimitLimit != null && sdLimitStrength != null)
            {
                LimiterChange(sdLimitLimit.Value, sdLimitStrength.Value);
            }
        }
        private void sdLimitStrength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sdLimitLimit != null && sdLimitStrength != null)
            {
                LimiterChange(sdLimitLimit.Value, sdLimitStrength.Value);
            }
        }
        private void btLimitReset_Click(object sender, RoutedEventArgs e)
        {
            LimiterReset();
        }

        private void cbLimitOn_Checked(object sender, RoutedEventArgs e)
        {
            LimiterOnOff((bool)cbLimitOn.IsChecked);
        }

        private void cbLimitOn_Unchecked(object sender, RoutedEventArgs e)
        {
            LimiterOnOff((bool)cbLimitOn.IsChecked);
        }

        private void LimiterChange(double Limit, double Strength)
        {
            if (np != null)
            {
                limt.SetParamater((float)Limit, (float)Strength);
                np.DSPs[2] = limt;
            }
        }

        private void LimiterOnOff(bool on)
        {
            if (np != null)
            {
                limt.SetStatus(on);
                np.DSPs[2] = limt;
            }
        }

        private void LimiterReset()
        {
            if (np != null)
            {
                this.limt = new nPlayerLimiter();
                np.DSPs[2] = limt;
                np.UpdateDSP();
            }
        }
        #endregion Limiter
    }
}
