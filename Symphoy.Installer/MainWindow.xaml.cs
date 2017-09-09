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
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using System.Net;
using Ionic.Zip;
using System.Diagnostics;
using System.Threading;
using System.Security.AccessControl;
using System.Windows.Threading;

namespace Symphoy.Installer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        string InstallDirecotory;
        string ServerFile = "http://symphony.esy.es/install/3.1/lastest.zip";
        string LocalFile;
        string TempFolder;

        Stopwatch sw;
        Storyboard ProgressOn;
        WebClient wc;
        Brush warnBursh;
        bool animating = false;
        bool installing = false;

        int percentDownloadComp = 60;
        int percentUnzipComp = 65;

        public MainWindow()
        {
            InitializeComponent();

            ProgressOn = FindResource("ProgressOn") as Storyboard;
            ProgressOn.Completed += ProgressOn_Completed;

            InstallDirecotory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            Lb_EditLocation.ToolTip = InstallDirecotory;

            Topmost = true;
            Topmost = false;

            sw = new Stopwatch();
            sw.Start();

            warnBursh = new SolidColorBrush(Color.FromArgb(255, 255, 65, 110));

            Closed += MainWindow_Closed;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (installing && Pb_Progress.Value != 100)
            {
                MessageBoxResult r = System.Windows.MessageBox.Show("설치가 진행중입니다. 종료하시겠습니까?", "경고", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.Yes)
                {
                    e.Cancel = false;

                    if(wc != null && wc.IsBusy)
                    {
                        wc.CancelAsync();
                    }

                    if(installThread != null && installThread.IsAlive)
                    {
                        installThread.Abort();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            sw.Stop();

            if (wc != null)
            {
                wc.CancelAsync();
            }

            if(TempFolder!= null)
            {
                if (Directory.Exists(TempFolder))
                {
                    try
                    {
                        RemoveDirectory(TempFolder);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private void RemoveDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            if (di.Exists)
            {
                FileInfo[] fis = di.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    log("Remove File: " + fi.FullName);

                    try
                    {
                        fi.Delete();
                    }
                    catch
                    {
                        log("Using... Skipped");
                    }
                }

                DirectoryInfo[] dis = di.GetDirectories();
                foreach (DirectoryInfo dir in dis)
                {
                    RemoveDirectory(dir.FullName);
                }

                try
                {
                    di.Delete(true);
                }
                catch
                {
                    log("Using: " + di.FullName);
                }
            }
        }

        private void ProgressOn_Completed(object sender, EventArgs e)
        {
            animating = false;

            //install start

            installing = true;

            log("start install");
            log("install path : " + InstallDirecotory);

            wc = new WebClient();
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;

            StartDownload();
        }

        private void StartDownload()
        {
            TempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Symphony_Install");
            DirectoryInfo tempdi = new DirectoryInfo(TempFolder);
            try
            {
                if (!tempdi.Exists)
                {
                    tempdi.Create();
                    Console.WriteLine("Temp Created: " + TempFolder);
                }
                else
                {
                    tempdi.Delete(true);

                    tempdi.Create();

                    Console.WriteLine("Temp Re-Created: " + TempFolder);
                }
            }
            catch
            {
                UpdateMsg("다운로드 실패", 100, "인터넷 연결을 확인해주세요");
                installing = false;
                Dispatcher.Invoke(new Action(() => { Pb_Progress.Foreground = warnBursh; }));

                return;
            }

            UpdateMsg("다운로드 시작", 0, "잠시후 다운로드가 시작됩니다");

            LocalFile = Path.Combine(tempdi.FullName, "install.zip");

            pre_time = sw.ElapsedMilliseconds;
            wc.DownloadFileAsync(new Uri(ServerFile), LocalFile);
        }

        private void UpdateMsg(string msg, double value, string ToolTip = null)
        {
            Dispatcher.BeginInvoke(new Action(() => 
            {
                Lb_Progress.Text = msg;
                Lb_Progress.ToolTip = ToolTip;
                Pb_Progress.Value = value;
            }));
        }

        double pre_time = 0;
        double pre_byte = 0;
        double avg_speed = 0;
        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double percent = e.BytesReceived / (double)e.TotalBytesToReceive * 100;
            string strpercent = percent.ToString("0.0");

            double speedKB = avg_speed;
            if (sw.ElapsedMilliseconds - pre_time > 1000)
            {
                speedKB = (e.BytesReceived - pre_byte) / (sw.ElapsedMilliseconds - pre_time);
                avg_speed = speedKB;
                speedKB = Math.Max(0, speedKB);

                pre_time = sw.ElapsedMilliseconds;
                pre_byte = e.BytesReceived;
            }

            string speedText;
            if (speedKB >= 1000)
            {
                speedText = (speedKB / 1000).ToString("0.0") + "MB/s";
            }
            else
            {
                speedText = speedKB.ToString("0") + "KB/s";
            }

            int sec = (int)((e.TotalBytesToReceive-e.BytesReceived) / 1000 / speedKB);
            string tooltip = "";
            if(sec > 3600)
            {
                tooltip = Math.Min((sec / 3600), 999).ToString() + "시간 " + ((sec % 3600) / 60).ToString() + "분 " + (sec % 60) + "초 남음";
            }
            else if(sec > 60)
            {
                tooltip = (sec / 60).ToString() + "분 " + (sec % 60).ToString() + "초 남음";
            }
            else
            {
                tooltip = Math.Max(sec, 0).ToString() + "초 남음";
            }

            UpdateMsg(string.Format("다운로드 중 {0}% - {1}", strpercent, speedText), percent / 100 * percentDownloadComp, tooltip);
        }

        int retry = 0;
        int maxRetry = 5;

        Thread installThread;
        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                installThread = new Thread(new ThreadStart(ExtractAndInstall));
                installThread.IsBackground = true;
                installThread.Start();
            }
            else
            {
                UpdateMsg("다운로드 실패", 100, "다운로드 중 오류가 발생했습니다");
                Dispatcher.Invoke(new Action(() => { Pb_Progress.Foreground = warnBursh; }));
            }
        }

        private void ExtractAndInstall()
        {
            UpdateMsg("설치 준비중", percentDownloadComp);
            //download end gogo next step
            if (ZipFile.IsZipFile(LocalFile))
            {
                using (ZipFile zip = new ZipFile(LocalFile, Encoding.UTF8))
                {
                    zip.ExtractProgress += Zip_ExtractProgress;

                    UpdateMsg("압축 해제 시작", percentDownloadComp);

                    zip.ExtractAll(TempFolder, ExtractExistingFileAction.OverwriteSilently);

                    zip.Dispose();

                    UpdateMsg("설치 정보 읽는중", percentDownloadComp + percentUnzipComp);

                    FileInfo fi = new FileInfo(Path.Combine(TempFolder, "install.sis"));

                    if (fi.Exists)
                    {
                        try
                        {
                            SIS.SIS sis = new SIS.SIS(fi.FullName, TempFolder);

                            sis.Updated += Sis_Updated;

                            sis.Install(Path.Combine(InstallDirecotory,"Symphony"));

                            string programFile = Path.Combine(InstallDirecotory, "Symphony", "Symphony.exe");
                            if (File.Exists(programFile))
                            {
                                UpdateMsg("설치 성공!", 100);

                                Thread.Sleep(2500);

                                Process.Start("explorer", programFile);

                                Dispatcher.Invoke(new Action(()=> { Close(); }));
                            }
                            else
                            {
                                UpdateMsg("설치 실패...", 100, "설치 중 오류가 발생했습니다");
                                Dispatcher.Invoke(new Action(() => { Pb_Progress.Foreground = warnBursh; }));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            UpdateMsg("설치 정보 읽기 실패", 100);
                            Dispatcher.Invoke(new Action(()=> { Pb_Progress.Foreground = warnBursh; }));
                        }
                    }
                    else
                    {
                        UpdateMsg("설치 파일 오류", 100);
                        Dispatcher.Invoke(new Action(() => { Pb_Progress.Foreground = warnBursh; }));
                    }
                }
            }
            else
            {
                if (retry < maxRetry)
                {
                    retry++;

                    UpdateMsg("다운로드 오류, 재시도 " + retry.ToString(), 0, "3초뒤 다운로드가 재시도됩니다");

                    Thread.Sleep(3000);

                    StartDownload();
                }
                else
                {
                    UpdateMsg("다운로드 실패", 100, "재시도 한계를 넘어 다운로드가 중지되었습니다\n\n인터넷 연결을 확인 해주세요");
                    Dispatcher.Invoke(new Action(() => { Pb_Progress.Foreground = warnBursh; }));

                    return;
                }
            }
        }

        private void Sis_Updated(object sender, SIS.ProgressUpdated e)
        {
            double percent = (e.Value - e.Minimum) / (e.Maximum - e.Minimum) * 100;
            string strpercent = percent.ToString("0.0");

            UpdateMsg(string.Format("{0} {1}%", e.Message, strpercent), percentUnzipComp + percent / 100 * (100 - percentUnzipComp), "백그라운드에서 인스톨러가 작업중입니다. 잠시만 기달려주세요");
        }

        private void Zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            switch (e.EventType)
            {
                case ZipProgressEventType.Extracting_BeforeExtractEntry:
                    Console.WriteLine(e.CurrentEntry.FileName);

                    double percent = e.EntriesExtracted / e.EntriesTotal * 100;
                    string textpercent = percent.ToString("0.0");

                    UpdateMsg(string.Format("압축 해제중 {0}%", textpercent), percentDownloadComp + percent / 100 * (percentUnzipComp - percentDownloadComp));
                    break;
                case ZipProgressEventType.Extracting_AfterExtractEntry:
                    break;
                default:
                    break;
            }
        }

        private void log(string msg)
        {
            Console.WriteLine(msg);
        }

        private void Lb_EditLocation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!animating)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "설치할 폴더를 선택하세요\n\n수정권한이있는 폴더를 선택하셔야 합니다";
                fbd.SelectedPath = InstallDirecotory;

                if (System.Windows.Forms.DialogResult.OK == fbd.ShowDialog())
                {
                    InstallDirecotory = fbd.SelectedPath;
                }

                Lb_EditLocation.ToolTip = InstallDirecotory;
            }
        }

        public static bool HasWritePermissionOnDir(string path)
        {
            AuthorizationRuleCollection collection = Directory.GetAccessControl(path).GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (FileSystemAccessRule rule in collection)
            {
                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    return true;
                }
            }
            return false;
        }

        private void Bt_Install_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!animating)
            {
                ProgressOn.Begin();
            }
        }
    }
}
