using Ionic.Zip;
using Symphony.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Server
{
    public class LyricDownloader
    {
        public event EventHandler<ProgressStopArgs> ProgressStopped;
        public event EventHandler<ProgressUpdatedArgs> ProgressUpdated;

        public LyricDownloader()
        {

        }

        public void TmpClear()
        {
            try
            {
                string working_directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Symphony", "LyricDownload");
                DirectoryInfo di = new DirectoryInfo(working_directory);
                if (di.Exists)
                {
                    try
                    {
                        di.Delete(true);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(this, e);
                    }
                }

                string lyricFoler = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lyrics");

                DirectoryInfo ld = new DirectoryInfo(lyricFoler);

                FileInfo[] fis = ld.GetFiles();

                foreach(FileInfo fi in fis)
                {
                    if (fi.Extension.Trim('.') == "tmp")
                    {
                        string tmp = Path.GetFileNameWithoutExtension(fi.FullName);

                        string uncomp = Path.Combine(lyricFoler, tmp);

                        if (Directory.Exists(uncomp))
                        {
                            Directory.Delete(uncomp, true);
                        }

                        fi.Delete();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(this, e);
            }
        }

        public QueryResult Download(RegisteredLyric rLyric)
        {
            try
            {
                UpdateMsg(0, 10, LanguageHelper.FindText("Lang_Lyric_Downloader_Ready_For_Download"));

                TmpClear();

                string tmpFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Symphony", "LyricDownload");

                string tmpDown = Path.Combine(tmpFolder, rLyric.RawData.Index.ToString() + ".lyric");

                DirectoryInfo di = new DirectoryInfo(tmpFolder);
                if (!di.Exists)
                {
                    di.Create();
                }

                UpdateMsg(1.5, 10, LanguageHelper.FindText("Lang_Lyric_Downloader_Download_Started"));

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(new Uri(rLyric.WebPath), tmpDown);

                    Logger.Log(rLyric.WebPath + " Download Completed");
                }

                UpdateMsg(4, 10, LanguageHelper.FindText("Lang_Lyric_Downloader_Download_Completed"));

                if (File.Exists(tmpDown) && ZipFile.IsZipFile(tmpDown))
                {
                    using (ZipFile zip = new ZipFile(tmpDown))
                    {
                        UpdateMsg(4.5, 10, LanguageHelper.FindText("Lang_Lyric_Downloader_Ready_For_Decompress"));

                        if (Directory.Exists(zip.TempFileFolder))
                        {
                            Directory.Delete(zip.TempFileFolder, true);
                        }

                        string[] entries = zip.EntryFileNames.ToArray();

                        if (entries != null && entries.Length > 0)
                        {
                            string[] spl = entries[0].Split('/');
                            if (spl.Length > 1)
                            {
                                string master = spl[0];

                                FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lyrics", master + ".tmp"));
                                FileStream fs = null;
                                if (!fi.Exists)
                                {
                                    fs = fi.Create();
                                }
                                else
                                {
                                    fs = fi.Open(FileMode.Open);
                                }

                                DirectoryInfo lyric = Lyrics.LyricHelper.Import(tmpDown);

                                UpdateMsg(9, 10, LanguageHelper.FindText("Lang_Lyric_Downloader_Decompress_Completed"));

                                fs.Close();
                                fs.Dispose();
                                fs = null;

                                fi.Delete();
                                
                                TmpClear();
                                string download_comp = LanguageHelper.FindText("Lang_Lyric_Downloader_Download_Completed");
                                StopMsg(10, 10, download_comp);
                                return new QueryResult(null, download_comp, true, lyric.FullName);
                            }
                            else
                            {
                                TmpClear();
                                string server_error = LanguageHelper.FindText("Lang_Lyric_Downloader_Server_File_Type_Error");
                                StopMsg(10, 10, server_error);
                                return new QueryResult(null, server_error, false);
                            }
                        }
                        else
                        {
                            TmpClear();
                            string server_err = LanguageHelper.FindText("Lang_Lyric_Downloader_Server_File_Error");
                            StopMsg(10, 10, server_err);
                            return new QueryResult(null, server_err, false);
                        }
                    }
                }
                else
                {
                    TmpClear();
                    string download_err = LanguageHelper.FindText("Lang_Lyric_Downloader_Downloaded_File_Error");
                    StopMsg(10, 10, download_err);
                    return new QueryResult(null, download_err, false);
                }
            }
            catch (Exception e)
            {
                TmpClear();
                StopMsg(10, 10, LanguageHelper.FindText("Lang_Lyric_Downloader_Download_Error"));
                return ExceptionText.PrintAndResult("LyricDownloader", e);
            }
        }

        public void UpdateMsg(double value, double max, string exception)
        {
            ProgressUpdated?.Invoke(this, new ProgressUpdatedArgs(value, max, exception));
        }

        public void StopMsg(double value, double max, string exception)
        {
            ProgressStopped?.Invoke(this, new ProgressStopArgs(value, max, exception));
        }
    }
}
