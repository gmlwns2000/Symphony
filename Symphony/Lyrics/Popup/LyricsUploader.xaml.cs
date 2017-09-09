using Symphony.Util;
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

namespace Symphony.Lyrics
{
    public partial class LyricsUploader : Window, IDisposable
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

                if(barAnimator == null)
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
            Tb_Status.Text = status + " " + (Math.Round(Bar_Prograss.Value / Bar_Prograss.Maximum * 1000)/10).ToString("0.0")+"%";

            if(Math.Abs(Bar_Prograss.Value - PrograssValue) < 0.005)
            {
                barAnimator.Stop();
            }
        }

        bool uploading = true;
        string filePath;
        Server.MusicMetadata metadata;
        Server.LyricUploader uploader;
        Storyboard PopupOff;

        public LyricsUploader(Window Owner, string lyricFile, Server.MusicMetadata metadata)
        {
            InitializeComponent();

            UI.ShadowWindow sw = new UI.ShadowWindow(this, null, 12, 1, false);

            this.Owner = Owner;

            filePath = lyricFile;
            this.metadata = metadata;

            uploader = new Server.LyricUploader(this);
            uploader.ProgressStoped += Uploader_ProgressStoped;
            uploader.ProgressUpdated += Uploader_ProgressUpdated;

            Logger.Log(this, "upload start : " + System.IO.Path.GetFileNameWithoutExtension(filePath));

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            Tb_Info.Text = "";

            if (!Util.TextTool.StringEmpty(metadata.Title))
            {
                Tb_Info.Text = metadata.Title;
            }
            else
            {
                Tb_Info.Text = metadata.FileName;
            }

            if (!Util.TextTool.StringEmpty(metadata.Artist))
            {
                Tb_Info.Text = metadata.Artist + " - " + Tb_Info.Text;
            }

            if (!Util.TextTool.StringEmpty(metadata.Album))
            {
                Tb_Info.Text += " (" + metadata.Album + ")";
            }
        }

        private new void Close()
        {
            PopupOff.Begin();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        public new void ShowDialog()
        {
            uploader.Upload(filePath, metadata);

            base.ShowDialog();
        }

        public new void Show()
        {
            ShowDialog();
        }

        private void Uploader_ProgressUpdated(object sender, Server.ProgressUpdatedArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Logger.Log(this, string.Format("(Updated) {0}/{1} [{2}%] - {3}", e.Value, e.Maximum, Convert.ToInt32(e.Value / e.Maximum * 100).ToString(), e.Status));
                Bar_Prograss.Minimum = 0;
                Bar_Prograss.Maximum = e.Maximum;
                PrograssValue = e.Value;
                status = e.Status;
            }));
        }

        private void Uploader_ProgressStoped(object sender, Server.ProgressStopArgs e)
        {
            Dispatcher.Invoke(new Action(()=>
            {
                Logger.Log(this, string.Format("(Stopped) {0}/{1} [{2}%] - {3}", e.Value, e.Maximum, Convert.ToInt32(e.Value / e.Maximum * 100).ToString(), e.Status));
                Bar_Prograss.Minimum = 0;
                Bar_Prograss.Maximum = e.Maximum;
                PrograssValue = e.Value;
                status = e.Status;
                uploading = false;
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (uploading)
            {
                UI.DialogMessageResult result = UI.DialogMessage.Show(this, LanguageHelper.FindText("Lang_Lyric_Uploader_Confirm_Close"), LanguageHelper.FindText("Lang_Confirm"), UI.DialogMessageType.YesNo);
                if (result == UI.DialogMessageResult.Yes)
                {
                    e.Cancel = false;
                    uploader.Stop();
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

        public void Dispose()
        {
            if (uploader != null)
            {
                uploader.Dispose();
            }
        }
    }
}
