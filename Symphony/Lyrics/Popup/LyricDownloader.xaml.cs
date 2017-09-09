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
using System.Windows.Shapes;
using System.Windows.Threading;
using Symphony.Server;
using System.Threading;
using Symphony.Util;

namespace Symphony.Lyrics
{
    /// <summary>
    /// LyricDownloader.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricDownloader : Window
    {
        double _pv = 0;
        double PrograssValue
        {
            get
            {
                return _pv;
            }
            set
            {
                _pv = value;

                if (barAnimator == null)
                {
                    barAnimator = new DispatcherTimer();
                    barAnimator.Tick += BarAnimator_Tick;
                    barAnimator.Interval = TimeSpan.FromMilliseconds(1);
                }

                if (!barAnimator.IsEnabled)
                {
                    barAnimator.Start();
                }
            }
        }
        string status = "";

        DispatcherTimer barAnimator;

        private void BarAnimator_Tick(object sender, EventArgs e)
        {
            Bar_Prograss.Value = Bar_Prograss.Value + (PrograssValue - Bar_Prograss.Value) * 0.28;
            Tb_Status.Text = status + " " + (Math.Round(Bar_Prograss.Value / Bar_Prograss.Maximum * 1000) / 10).ToString("0.0") + "%";

            if (Math.Abs(Bar_Prograss.Value - PrograssValue) < 0.005)
            {
                barAnimator.Stop();
            }
        }

        bool downloading = true;
        Storyboard PopupOff;
        Server.LyricDownloader downloader;
        Thread downThread;
        RegisteredLyric rlyric;

        public LyricDownloader(Window Parent, RegisteredLyric rlyric)
        {
            InitializeComponent();

            UI.ShadowWindow sw = new UI.ShadowWindow(this, null, 12, 1, false);

            this.rlyric = rlyric;

            downloader = new Server.LyricDownloader();
            downloader.ProgressStopped += Downloader_ProgressStopped;
            downloader.ProgressUpdated += Downloader_ProgressUpdated;

            Owner = Parent;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            Tb_Info.Text = "";

            if (!Util.TextTool.StringEmpty(rlyric.Song.metadata.Title))
            {
                Tb_Info.Text = rlyric.Song.metadata.Title;
            }
            else
            {
                Tb_Info.Text = rlyric.Song.metadata.FileName;
            }

            if (!Util.TextTool.StringEmpty(rlyric.Song.metadata.Artist))
            {
                Tb_Info.Text = rlyric.Song.metadata.Artist + " - " + Tb_Info.Text;
            }

            if (!Util.TextTool.StringEmpty(rlyric.Song.metadata.Album))
            {
                Tb_Info.Text += " (" + rlyric.Song.metadata.Album + ")";
            }
        }

        private void Downloader_ProgressUpdated(object sender, ProgressUpdatedArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Logger.Log(string.Format("(Updated) {0}/{1} [{2}%] - {3}", e.Value, e.Maximum, Convert.ToInt32(e.Value / e.Maximum * 100).ToString(), e.Status));
                Bar_Prograss.Minimum = 0;
                Bar_Prograss.Maximum = e.Maximum;
                PrograssValue = e.Value;
                status = e.Status;
            }));
        }

        private void Downloader_ProgressStopped(object sender, ProgressStopArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Logger.Log(string.Format("(Stopped) {0}/{1} [{2}%] - {3}", e.Value, e.Maximum, Convert.ToInt32(e.Value / e.Maximum * 100).ToString(), e.Status));
                Bar_Prograss.Minimum = 0;
                Bar_Prograss.Maximum = e.Maximum;
                PrograssValue = e.Value;
                status = e.Status;
                downloading = false;
            }));
        }

        private new void Close()
        {
            PopupOff.Begin();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        private void DownloadStart()
        {
            downThread = new Thread(new ThreadStart(() => { downloader.Download(rlyric); }));
            downThread.Start();
        }

        private void DownloadStop()
        {
            downThread.Abort();
            downThread = null;
        }

        public new void ShowDialog()
        {
            DownloadStart();

            base.ShowDialog();
        }

        public new void Show()
        {
            ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (downloading)
            {
                UI.DialogMessageResult result = UI.DialogMessage.Show(this, LanguageHelper.FindText("Lang_Lyric_Downloader_Confirm_Close"), LanguageHelper.FindText("Lang_Okay"), UI.DialogMessageType.YesNo);
                if (result == UI.DialogMessageResult.Yes)
                {
                    e.Cancel = false;
                    DownloadStop();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (barAnimator.IsEnabled)
            {
                barAnimator.Stop();
            }
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Bt_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
