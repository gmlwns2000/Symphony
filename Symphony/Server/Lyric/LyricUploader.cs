using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Lyrics;
using System.Threading;
using System.Windows;
using Symphony.Util;

namespace Symphony.Server
{
    public class LyricUploader : IDisposable
    {
        public static string DatabaseName = "sym_lyrics";
        public bool IsWorking = false;
        public event EventHandler<ProgressUpdatedArgs> ProgressUpdated;
        public event EventHandler<ProgressStopArgs> ProgressStoped;
        public Window Parent;

        private string lyricFile;
        private MusicMetadata metadata;
        private Task uploadTask;
        private CancellationTokenSource cts { get; set; }
        private CancellationToken cancelpadding { get; set; }
        private LyricWeb lyricWeb;

        public LyricUploader(Window Parent)
        {
            cts = new CancellationTokenSource();
            cancelpadding = cts.Token;
            lyricWeb = new LyricWeb();
            lyricWeb.ProgressUpdated += LyricWeb_ProgressUpdated;
            this.Parent = Parent;
        }

        private void LyricWeb_ProgressUpdated(object sender, ProgressUpdatedArgs e)
        {
            updateMsg(8*(e.Value/e.Maximum)+2, 10, e.Status);
        }

        public void Upload(string lyricFile, MusicMetadata metadata)
        {
            if (!IsWorking)
            {
                updateMsg(0, 10, LanguageHelper.FindText("Lang_Lyric_Uploader_Start_Upload"));

                IsWorking = true;

                this.lyricFile = lyricFile;
                this.metadata = metadata;

                uploadTask = new Task(new Action(Upload));

                uploadTask.Start();

                IsWorking = false;
            }
            else
            {
                throw new ArgumentNullException("업로더가 이미 실행중입니다");
            }
        }

        private void Upload()
        {
            if (!Session.IsLogined)
            {
                if (Parent == null)
                {
                    if (ProgressStoped != null)
                        ProgressStoped(this, new ProgressStopArgs(10, 10, LanguageHelper.FindText("Lang_Lyric_Uploader_Fail_To_Login")));

                    return;
                }
                else
                {
                    updateMsg(0.5, 10, LanguageHelper.FindText("Lang_Lyric_Uploader_Check_Login"));

                    Parent.Dispatcher.Invoke(new Action(() => 
                    {
                        LoginWindow lw = new LoginWindow(Parent);
                        lw.ShowDialog();
                    }));

                    if (!Session.IsLogined)
                    {
                        if (ProgressStoped != null)
                            ProgressStoped(this, new ProgressStopArgs(10, 10, LanguageHelper.FindText("Lang_Lyric_Uploader_Login_Fail")));

                        return;
                    }
                }
            }

            updateMsg(1, 10, string.Format(LanguageHelper.FindText("Lang_Lyric_Uploader_Welcome_Message"), Session.UserID));

            QueryResult result = SongSearcher.SearchAndRegister(metadata, 0, 5);

            if (result.Success)
            {
                updateMsg(1.5, 10, LanguageHelper.FindText("Lang_Lyric_Uploader_Search_Finish"));

                Song target = (Song)result.Tag;

                if (target != null)
                {
                    Logger.Log(string.Format("ID:{0}  Title:{1}  Filename:{2}  Time:{3}", target.Index, target.metadata.Title, target.metadata.FileName, target.CreationTime.ToString()));
                    
                    QueryResult resultLyric = lyricWeb.Register(target, lyricFile, cancelpadding);

                    if (ProgressStoped != null)
                        ProgressStoped(this, new ProgressStopArgs(10, 10, resultLyric.Message));
                    return;
                }
                else
                {
                    if (ProgressStoped != null)
                        ProgressStoped(this, new ProgressStopArgs(10, 10, LanguageHelper.FindText("Lang_Lyric_Uploader_Fail_To_Search")));
                    return;
                }
            }
            else
            {
                if (ProgressStoped != null)
                    ProgressStoped(this, new ProgressStopArgs(10, 10, LanguageHelper.FindText("Lang_Lyric_Uploader_DB_Error")));
                return;
            }
        }

        public void Stop()
        {
            if (uploadTask != null && IsWorking)
            {
                cts.Cancel();

                Thread.Sleep(25);
            }
        }

        private void updateMsg(double value, double maximum, string text)
        {
            cancelpadding.ThrowIfCancellationRequested();

            if (ProgressUpdated != null)
                ProgressUpdated(this, new ProgressUpdatedArgs(value, maximum, text));
        }

        public void Dispose()
        {
            Stop();
            
            lyricWeb.Dispose();

            cts.Dispose();

            ProgressStoped = null;
            
            ProgressUpdated = null;

            uploadTask.Dispose();
        }
    }
}
