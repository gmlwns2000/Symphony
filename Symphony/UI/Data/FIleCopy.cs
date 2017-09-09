using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.UI
{
    public class FIleCopy : ProgressReporter
    {
        WebClient wc;
        string source;
        string dest;

        public FIleCopy(string source, string dest)
        {
            wc = new WebClient();

            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;

            this.source = source;
            this.dest = dest;
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            StoppedMessage(10, 10, "파일 복사 완료");
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            UpdateMessage(e.ProgressPercentage, 100, "파일 복사중... " + e.ProgressPercentage.ToString("0") + "%");
        }

        public override void Stop()
        {
            if(wc!=null && wc.IsBusy)
            {
                wc.CancelAsync();

                StoppedMessage(10, 10, "파일 복사 중지");
            }
        }

        public override void Start()
        {
            wc.DownloadFileAsync(new Uri(source), dest);
        }
    }
}
