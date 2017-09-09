using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Symphony.Player;

namespace Symphony.UI
{
    public partial class MusicLoading : Window, IDisposable
    {
        string[] FilePathes;
        Playlist Playlist;
        DispatcherTimer timer = new DispatcherTimer();
        MainWindow mw;
        public BackgroundWorker Worker = new BackgroundWorker();
        double percentage;

        public MusicLoading(MainWindow Parent, Playlist playlist, string[] FilePath)
        {
            InitializeComponent();
            Closed += MusicLoading_Closed;

            Logger.Info("start reading metadata to playlist Total: "+FilePath.Length.ToString());

            this.mw = Parent;

            Playlist = playlist;
            FilePathes = FilePath;

            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
            Worker.DoWork += Worker_DoWork;
            Worker.ProgressChanged += Worker_ProgressChanged;
            Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            timer.Start();

            Topmost = true;
            
            IntPtr window_handle = new System.Windows.Interop.WindowInteropHelper(Parent).Handle;
            Screen sc = Screen.FromHandle(window_handle);

            PresentationSource source = PresentationSource.FromVisual(Parent);

            double dpiX, dpiY;

            if (source != null)
            {
                dpiX = source.CompositionTarget.TransformToDevice.M11;
                dpiY = source.CompositionTarget.TransformToDevice.M22;
            }
            else
            {
                dpiX = 0;
                dpiY = 0;
            }

            Top = sc.WorkingArea.Top / dpiX + sc.WorkingArea.Height / dpiX - Height;
            Left = sc.WorkingArea.Left / dpiX + sc.WorkingArea.Width / dpiX - Width;
            
            Parent.Closed += Parent_Closed;

            this.Owner = Parent;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Progress.Value = (double)percentage / FilePathes.Length * 100;
            Lb_Counter.Text = percentage.ToString() + "/" + FilePathes.Length.ToString();
            Lb_FileName.Text = System.IO.Path.GetFileName(FilePathes[(int)percentage]);
        }

        private void MusicLoading_Closed(object sender, EventArgs e)
        {
            Logger.Log("Run Add Items dialog closed");
        }

        private void Parent_Closed(object sender, EventArgs e)
        {
            if (Worker.IsBusy)
            {
                Worker.CancelAsync();
            }
            timer.Stop();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            mw.SavePlaylists();
            Worker.Dispose();
            this.Close();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            percentage = e.ProgressPercentage;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for(int i=0; i < FilePathes.Length; i++)
            {
                if (!Worker.CancellationPending)
                {
                    Playlist.Add(FilePathes[i]);
                    Worker.ReportProgress(i);
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        public void Dispose()
        {
            Worker.Dispose();
            if(timer != null && timer.IsEnabled)
            {
                timer.Stop();
            }
        }
    }
}
