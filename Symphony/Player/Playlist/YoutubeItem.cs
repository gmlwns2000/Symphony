using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Symphony.Player.Youtube;
using System.Threading;

namespace Symphony.Player
{
    public class YoutubeItem : PlaylistItem
    {
        bool _isAvailable = false;
        public override bool IsAvailable
        {
            get
            {
                return _isAvailable;
            }
        }

        public string Uri { get; private set; }

        public VideoInfo VideoInfo { get; private set; }

        public YoutubeItem(string uriPath)
        {
            Uri = uriPath;

            FilePath = uriPath;

            //LANGSUP
            FileName = "유튜브에서 가져오는중... " + uriPath;

            LazyInit(uriPath);
        }
        
        private async void LazyInit(string uriPath)
        {
            Task t = Util.LimitedTaskScheduler.Factory.StartNew(delegate { DownloadInfo(uriPath, 0); });

            await t;

            t.Dispose();
        }

        private void DownloadInfo(string uriPath, int retry)
        {
            if (uriPath == null || uriPath == string.Empty || retry >= 3)
            {
                FilePath = null;

                //LANGSUP
                FileName = "다운로드에 실패했습니다";

                InvokeUpdated(this, this);

                _isAvailable = false;

                return;
            }

            try
            {
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(Uri, false);

                VideoInfo video = videoInfos
                    .OrderByDescending(info => info.AudioBitrate)
                    .First(info => info.VideoType == VideoType.Mp4 && info.Resolution <= 720);
                int bitrate = video.AudioBitrate;

                video = videoInfos.OrderBy(info => info.Resolution)
                    .First(info => info.VideoType == VideoType.Mp4 && info.AudioBitrate == bitrate && (info.AudioType == AudioType.Aac || info.AudioType == AudioType.Mp3));

                if (video.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(video);
                }

                VideoInfo = video;

                FileName = video.GetFileName();

                FilePath = GetFilePathFromVideoInfo(video);

                InvokeUpdated(this, this);

                Tag = new Tags(VideoInfo);
                Tag.TagUpdated += delegate
                {
                    InvokeUpdated(this, this);
                };

                _isAvailable = true;
            }
            catch (Exception ex)
            {
                Logger.Error(this, ex.ToString());

                _isAvailable = false;

                FileName = "다운로드에 실패함, 재시도 " + (retry + 1).ToString();

                InvokeUpdated(this, this);

                Thread.Sleep(500);

                DownloadInfo(uriPath, retry + 1);
            }
        }

        public static string GetFilePathFromVideoInfo(VideoInfo info)
        {
            return Path.Combine(Pathes.YoutubeVideoFolder, info.GetFileName());
        }
        
        public override Stream GetStream()
        {
            return new YoutubeStream(VideoInfo);
        }
    }
}
