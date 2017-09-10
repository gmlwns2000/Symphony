using Symphony.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;

namespace Symphony
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
#if DEBUG
        private const string MutexName = "Symphony";
        public const string PipeName = "Symphony.Pipe";
#else
        private const string MutexName = "Symphony";
        public const string PipeName = "Symphony.Pipe";
#endif
        private Mutex _mutex;
        bool createdNew;

        public App()
        {
            Util.Settings.Load();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            List<string> files = new List<string>();

            for (int i = 0; i != e.Args.Length; i++)
            {
                if (e.Args[i].StartsWith("-") || e.Args[i].StartsWith("/"))
                {
                    //Arguments
                    Logger.Error("Unknown Argument On " + i.ToString() + "Index \"" + e.Args[i] + "\"");
                }
                else
                {
                    //Open Files
                    files.Add(e.Args[i]);
                    Logger.Log("File Queue Add: " + e.Args[i]);
                }
            }

            bool needOpen = false;
            bool addNew = false;
            List<string> filesToOpen = new List<string>();
            foreach(string path in files)
            {
                Logger.Log("Consider File: " + path);

                if (path.ToLower().EndsWith("lyric"))
                {
                    try
                    {
                        Logger.Log("Try Open Lyric : " + path);

                        Lyrics.LyricHelper.Import(path);

                        DialogMessage msg = new DialogMessage(null, "가사를 불러왔습니다");
                        msg.Topmost = true;
                        msg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        msg.ShowDialog();
                    }
                    catch ( Exception ex )
                    {
                        Logger.Error("Errored: " + ex.ToString());

                        DialogMessage msg = new DialogMessage(null, "가사 불러오기 도중 오류가 발생했습니다");
                        msg.Topmost = true;
                        msg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        msg.ShowDialog();
                    }
                }
                else if (path.ToLower().EndsWith("plot"))
                {
                    try
                    {
                        Logger.Log("Try Open Dance : " + path);

                        Dancer.PlotHelper.Import(path);

                        DialogMessage msg = new DialogMessage(null, "춤을 불러왔습니다");
                        msg.Topmost = true;
                        msg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        msg.ShowDialog();
                    }
                    catch(Exception ex)
                    {
                        Logger.Error("Errored: " + ex.ToString());

                        DialogMessage msg = new DialogMessage(null, "춤 불러오기 도중 오류가 발생했습니다");
                        msg.Topmost = true;
                        msg.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        msg.ShowDialog();
                    }
                }
                else if (Symphony.Player.PlayerCore.IsSupport(path))
                {
                    Logger.Log("Playlist Add : " + path);

                    needOpen = true;

                    filesToOpen.Add(path);
                }
            }

            if (needOpen)
            {
                _mutex = new Mutex(true, MutexName, out createdNew);
                Console.WriteLine(createdNew);

                if (!createdNew)
                {
                    //SEND
                    Logger.Log("Need To Send");

                    try
                    {
                        NamedPipeClientStream c = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                        if (c.CanTimeout)
                            c.WriteTimeout = 10000;
                        c.Connect();

                        Util.StreamString ss = new Util.StreamString(c);

                        string save = "";

                        for(int i=0; i<filesToOpen.Count; i++)
                        {
                            if (i != 0)
                                save += "\n";
                            save += filesToOpen[i];
                        }

                        ss.WriteString(save);

                        c.Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    needOpen = false;

                    Environment.Exit(0);
                }
                else
                {
                    try
                    {
                        //ADD
                        Logger.Log("Need To ADD");

                        DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Playlists");
                        if (!di.Exists)
                        {
                            di.Create();
                        }
                    
                        string filePath = Path.Combine(di.FullName, "Default Playlist.m3u8");
                    
                        string save = "";
                        if (File.Exists(filePath))
                        {
                            save = File.ReadAllText(filePath);
                        }

                        for (int i = 0; i < filesToOpen.Count; i++)
                        {
                            save += "\n";

                            save += filesToOpen[i];
                        }

                        File.WriteAllText(filePath, save);

                        addNew = true;
                    }
                    catch(Exception ex)
                    {
                        Logger.Error("ERROR ON OPEN FILES VIA ARGS. \n" + ex.ToString());
                    }
                }
            }

            if (!needOpen && files.Count != 0)
            {
                Environment.Exit(0);
            }
            else
            {
                if(_mutex == null)
                {
                    _mutex = new Mutex(true, MutexName, out createdNew);
                    Console.WriteLine(createdNew);
                }

                Stopwatch sw = new Stopwatch();
                sw.Start();

                SplashWindow splash = null;
                Thread thread = new Thread(() =>
                {
                    splash = new SplashWindow(Symphony.MainWindow.versionText);

                    splash.Show();

                    splash.Closed += (sender2, e2) => splash.Dispatcher.InvokeShutdown();

                    System.Windows.Threading.Dispatcher.Run();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                while (splash == null)
                {
                    Thread.Sleep(1);
                }

                while (!splash.showed)
                {
                    Thread.Sleep(1);
                }

                MainWindow mw = new MainWindow(splash);

                MainWindow = mw;

                mw.Show();

                if (addNew)
                {
                    splash.Update("파일 열기");

                    mw.PipePlayJustAdded();
                }

                TimeSpan ts = sw.Elapsed;
                sw.Stop();
                string timeText = ts.TotalSeconds.ToString("0.00");

                splash.Update(string.Format("완료 ({0} 초)", timeText));

                splash.Close();
            }
        }
    }
}
