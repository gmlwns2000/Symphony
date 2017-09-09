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

namespace Symphony.UI
{
    /// <summary>
    /// ProgressWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProgressWindow : Window
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

        bool uploading = true;
        ProgressReporter reporter;
        Storyboard PopupOff;
        DispatcherTimer updateTimer;

        public ProgressWindow(Window Owner, ProgressReporter reporter, string title, string info)
        {
            InitializeComponent();

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(50);
            updateTimer.Tick += UpdateTimer_Tick;

            updateTimer.Start();

            this.Owner = Owner;
            this.reporter = reporter;
            reporter.ProgressStopped += ProgressStoped;
            reporter.ProgressUpdated += ProgressUpdated;

            PopupOff = FindResource("PopupOff") as Storyboard;
            PopupOff.Completed += PopupOff_Completed;

            Tb_Info.Text = info;
            Tb_Title.Text = title;
        }

        bool canupdate = true;
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            canupdate = true;
        }

        private new void Close()
        {
            PopupOff.Begin();

            updateTimer.Stop();
        }

        private void PopupOff_Completed(object sender, EventArgs e)
        {
            base.Close();
        }

        public new void ShowDialog()
        {
            base.ShowDialog();
        }

        public new void Show()
        {
            ShowDialog();
        }

        private void ProgressUpdated(object sender, Server.ProgressUpdatedArgs e)
        {
            if (canupdate)
            {
                canupdate = false;

                Dispatcher.Invoke(new Action(() =>
                {
                    Logger.Log(this, string.Format("(Updated) {0}/{1} [{2}%] - {3}", e.Value, e.Maximum, Convert.ToInt32(e.Value / e.Maximum * 100).ToString(), e.Status));
                    Bar_Prograss.Minimum = 0;
                    Bar_Prograss.Maximum = e.Maximum;
                    PrograssValue = e.Value;
                    status = e.Status;
                }));
            }
        }

        private void ProgressStoped(object sender, Server.ProgressStopArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
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
                UI.DialogMessageResult result = UI.DialogMessage.Show(this, "작업이 진행중 중입니다.\n중지 하시겟습니까?", "확인", UI.DialogMessageType.YesNo);
                if (result == UI.DialogMessageResult.Yes)
                {
                    e.Cancel = false;
                    reporter.Stop();
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

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            reporter.Start();
        }
    }
}
