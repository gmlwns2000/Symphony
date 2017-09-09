using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Symphony.Player
{
    public class TagImage : IDisposable
    {
        public static long CRCLimit = 262144;

        private static long _cachingLimit = 1073741824; // 1GB
        public static long CachingLimit
        {
            get
            {
                return _cachingLimit;
            }
            set
            {
                _cachingLimit = value;
            }
        }

        private FileStream _stream;
        public FileStream Stream
        {
            get { return _stream; }
            set
            {
                _stream = value;
            }
        }

        public string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            private set
            {
                if(_filePath != value)
                {
                    _filePath = value;

                    if(_stream != null)
                    {
                        _stream.Close();
                        _stream.Dispose();
                        _stream = null;
                    }

                    _stream = File.OpenRead(_filePath);
                }
            }
        }
        
        public byte[] RawData
        {
            get
            {
                byte[] _rawData;

                try
                {
                    if (FilePath != "" && File.Exists(FilePath))
                    {
                        _rawData = File.ReadAllBytes(FilePath);

                        return _rawData;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public static DirectoryInfo CacheFolder
        {
            get
            {
                return new DirectoryInfo(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), Pathes.AlbumArtCacheFolder));
            }
        }

        private static void CheckClearCache()
        {
            DirectoryInfo di = CacheFolder;

            if (di.Exists)
            {
                FileInfo[] files = di.GetFiles();
                long size = 0;

                foreach(FileInfo fi in files)
                {
                    try
                    {
                        size += fi.Length;
                    }
                    catch
                    {

                    }
                }

            }
        }

        public static void ClearCache()
        {
            Console.WriteLine("Start Clear Cache");

            DirectoryInfo di = CacheFolder;

            if (di.Exists)
            {
                FileInfo[] files = di.GetFiles();
                foreach(FileInfo fi in files)
                {
                    try
                    {
                        fi.Delete();
                        Console.WriteLine("Removed: " + fi.FullName);
                    }
                    catch
                    {

                    }
                }

                Console.WriteLine("Finish Clear Cache");
            }
        }

        private static void MoveToCache(TagImage tag, string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            try
            {
                tag.FilePath = filename;
            }
            catch
            {
                tag.FilePath = "";
            }
        }
        
        private static void MoveToCache(TagImage tag, byte[] Buffer)
        {
            DirectoryInfo di = CacheFolder;
            if (!di.Exists)
            {
                try
                {
                    di.Create();
                }
                catch
                {

                }
            }

            byte[] buf = new byte[Math.Min(Buffer.Length, CRCLimit)];
            Array.Copy(Buffer, Buffer.Length - buf.Length, buf, 0, buf.Length);

            string crc = Crc32.GetBufferCRC(buf);

            string path = Path.Combine(di.FullName, crc + ".taa");

            try
            {
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, Buffer);

                    Console.WriteLine("Caching: " + crc);
                }

                tag.FilePath = path;
            }
            catch
            {
                tag.FilePath = "";
            }
        }

        public TagImage(string FilePath)
        {
            MoveToCache(this, FilePath);
        }

        public TagImage(byte[] Buffer)
        {
            MoveToCache(this, Buffer);
        }

        ~TagImage()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
