using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Symphony.Player.Youtube;

namespace Symphony.Player.Youtube
{
    public class YoutubeStream : Stream, IDisposable
    {
        public static long NetworkCache = 1024 * 1024;

        public override bool CanRead
        {
            get
            {
                if (Closed)
                    return false;

                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (Closed)
                    return false;

                return true;
            }
        }

        public override bool CanWrite => false;

        private long _length = 0;
        public override long Length
        {
            get
            {
                if (CacheStream == null || Closed)
                    return 0;

                try
                {
                    return CacheStream.Length;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public override long Position
        {
            get
            {
                if (CacheStream == null || Closed)
                    return 0;

                try
                {
                    return CacheStream.Position;
                }
                catch 
                {
                    return 0;
                }
            }
            set
            {
                if (CacheStream == null || Closed)
                    return;

                BufferUntilBytes((int)value);

                lock (CacheLock)
                {
                    CacheStream.Position = value;
                }
            }
        }

        public VideoInfo VideoInfo { get; private set; }
        public string FilePath { get; private set; }
        
        private Stream SourceStream;
        private Stream CacheStream;
        private WebResponse WebResponse;

        private long bufferd = 0;

        private bool Closed = false;
        private object CacheLock = new object();

        public YoutubeStream(VideoInfo info)
        {
            VideoInfo = info;

            Open();
        }

        private void Open()
        {
            var request = (HttpWebRequest)WebRequest.Create(VideoInfo.DownloadUrl);
            
            WebResponse = request.GetResponse();

            SourceStream = WebResponse.GetResponseStream();
            SourceStream.ReadTimeout = 3000;

            _length = WebResponse.ContentLength;

            string filePath = YoutubeItem.GetFilePathFromVideoInfo(VideoInfo);
            FilePath = filePath;

            CacheStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            CacheStream.SetLength(_length);
        }

        private void BufferUntilBytes(int target)
        {
            if (CacheStream == null)
                return;

            if (target >= bufferd)
            {
                lock (CacheLock)
                {
                    int preposition = (int)CacheStream.Position;

                    CacheStream.Position = bufferd;

                    int count = target - (int)bufferd + (int)NetworkCache;

                    byte[] buffer = new byte[2048];
                    int readed = 0;

                    while (readed < count)
                    {
                        int read = SourceStream.Read(buffer, 0, buffer.Length);
                        readed += read;

                        if (read > 0)
                        {
                            CacheStream.Write(buffer, 0, read);
                        }
                        else
                        {
                            break;
                        }
                    }

                    bufferd = CacheStream.Position;
                    Logger.Log("Buffered! : " + buffer.ToString());
                    CacheStream.Position = preposition;
                }
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (CacheStream == null || Closed)
                return 0;

            Logger.Log("READ: Position: " + Position.ToString() + "    offset: " + offset.ToString() + "   count: " + count.ToString() + "    bufferd: " + bufferd);

            try
            {
                int target = (int)CacheStream.Position + count;

                BufferUntilBytes(target);

                lock (CacheLock)
                {
                    int read = CacheStream.Read(buffer, offset, count);

                    Logger.Log("READED: " + read.ToString() + "  max: " + buffer.Max().ToString());

                    return read;
                }
            }
            catch (ObjectDisposedException)
            {
                return 0;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long position = Position;

            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = offset;
                    break;
                case SeekOrigin.Current:
                    position += offset;
                    break;
                case SeekOrigin.End:
                    position = Length + offset;
                    break;
                default:
                    break;
            }

            BufferUntilBytes((int)position);

            if (CacheStream != null && !Closed)
            {
                lock (CacheLock)
                {
                    CacheStream.Seek(offset, origin);
                }
            }

            return Position;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #region Dispose 

        public new void Close()
        {
            if (!Closed)
            {
                if (CacheStream != null)
                {
                    CacheStream.Close();
                }

                if (SourceStream != null)
                {
                    SourceStream.Close();
                }

                if(WebResponse != null)
                {
                    WebResponse.Close();
                }
            }

            Closed = true;

            base.Close();
        }

        bool disposed = false;
        public new void Dispose()
        {
            if (disposed)
                return;

            if (CacheStream != null)
            {
                lock (CacheLock)
                {
                    if (!Closed)
                        CacheStream.Close();
                    CacheStream.Dispose();
                    CacheStream = null;
                }
            }

            if (SourceStream != null)
            {
                if(!Closed)
                    SourceStream.Close();
                SourceStream.Dispose();
                SourceStream = null;
            }

            if (WebResponse != null)
            {
                WebResponse.Close();
                WebResponse = null;
            }

            if (FilePath!=null &&File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch (Exception ex)
                {
                    Symphony.Logger.Error(this, ex);
                }
            }

            Closed = true;
            disposed = true;

            base.Dispose();

            GC.SuppressFinalize(this);
        }

        ~YoutubeStream()
        {
            Dispose();
        }

        #endregion Dispose
    }
}
