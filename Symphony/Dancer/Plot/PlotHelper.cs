using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;
using System.IO;
using Ionic.Zip;
using Symphony.Server;
using System.Diagnostics;

namespace Symphony.Dancer
{
    public static class PlotHelper
    {
        public static string TextUnknown = "알 수 없음";
        public static string PlotServer = "127.0.0.1";
        public static string PlotServerPort = "3306";


        public static PlotSearchResult Search(MusicMetadata mt, Song song)
        {
            //check server
            bool serverExist = false;
            bool internetOk = false;
            string webPath = "";
            //end checks erver

            //check local
            bool localExist = false;
            string localPath = "";

            DirectoryInfo di = new DirectoryInfo(PlotDirectory);

            DirectoryInfo[] contents = di.GetDirectories();

            if (contents.Length > 0)
            {
                double[] score = new double[contents.Length];

                for (int i = 0; i < score.Length; i++) score[i] = 0;

                List<MusicMetadata> metas = new List<MusicMetadata>();
                for (int i = 0; i < contents.Length; i++)
                {
                    DirectoryInfo item = contents[i];
                    MusicMetadata meta = new MusicMetadata(Path.Combine(item.FullName, "plot.xml"));
                    metas.Add(meta);
                }

                int searchResult = ScoredSearcher.Search(metas, mt);

                if (searchResult > -1)
                {
                    localExist = true;
                    localPath = contents[searchResult].FullName;
                }
            }

            //end check local

            //return
            if (internetOk)
            {
                if (serverExist && localExist)
                {
                    return new PlotSearchResult(PlotExistState.GlobalExist, webPath, localPath);
                }
                else if(serverExist && !localExist)
                {
                    return new PlotSearchResult(PlotExistState.WebExitst, webPath, localPath);
                }
                else if(!serverExist && localExist)
                {
                    return new PlotSearchResult(PlotExistState.LocalExist, webPath, localPath);
                }
                else
                {
                    throw new InvalidOperationException("Wait! Where is here dud? Here is Dancer.PlotHelper.Search[77]");
                }
            }
            else
            {
                if (localExist)
                {
                    return new PlotSearchResult(PlotExistState.UnsureLocal, webPath, localPath);
                }
                else
                {
                    return new PlotSearchResult(PlotExistState.None, webPath, localPath);
                }
            }
        }

        public static string PlotDirectory
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo( Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plots") );

                if (!di.Exists)
                {
                    di.Create();
                }

                return di.FullName;
            }
        }

        /// <summary>
        /// 경로로 내보냅니다.
        /// </summary>
        public static void Export(string PlotFile, string FilePath, string WorkingDirectory)
        {

        }

        /// <summary>
        /// 저장소로 불러옵니다. 예외 처리 되지 않음.
        /// </summary>
        public static DirectoryInfo Import(string PlotFile)
        {
            string distFile = Path.Combine(PlotDirectory, "plot.tmp");

            if (File.Exists(distFile))
            {
                File.Delete(distFile);
            }

            File.Copy(PlotFile, distFile);

            string folderName = "";

            using (ZipFile zip = ZipFile.Read(distFile))
            {
                foreach(ZipEntry entry in zip)
                {
                    Debug.WriteLine(entry.FileName);

                    if (entry.FileName.EndsWith("plot.xml"))
                    {
                        folderName = Path.GetDirectoryName(entry.FileName);

                        Debug.WriteLine("folder name: {0}",folderName);

                        if (Directory.Exists(Path.Combine(PlotDirectory, folderName)))
                        {
                            DirectoryInfo di = new DirectoryInfo(Path.Combine(PlotDirectory, folderName));

                            FileInfo[] files = di.GetFiles();
                            DirectoryInfo[] directories = di.GetDirectories();

                            foreach (FileInfo fi in files)
                            {
                                fi.Delete();
                            }

                            foreach (DirectoryInfo dii in directories)
                            {
                                dii.Delete(true);
                            }
                        }
                    }

                    entry.Extract(PlotDirectory,ExtractExistingFileAction.OverwriteSilently);
                }
            }

            File.Delete(distFile);

            string outDirectory = Path.Combine(PlotDirectory, folderName);

            Debug.WriteLine(outDirectory);

            return new DirectoryInfo(outDirectory);
        }

        public static string PlotName(MusicMetadata Metadata)
        {
            return PlotName(Metadata.Title, Metadata.Artist, Metadata.Album, Metadata.FileName);
        }

        public static string PlotName(string title, string artist, string album, string fileName)
        {
            string tt;
            if (title != null)
            {
                tt = Util.TextTool.FileNameFix(title.Trim());
            }
            else
            {
                tt = fileName;
            }

            string at;
            if (artist != null)
            {
                at = Util.TextTool.FileNameFix(artist.Trim());
            }
            else
            {
                at = TextUnknown;
            }

            string al;
            if (album != null)
            {
                al = Util.TextTool.FileNameFix(album.Trim());
            }
            else
            {
                al = TextUnknown;
            }
            
            if (Util.TextTool.StringEmpty(tt))
            {
                tt = fileName;
            }

            if (Util.TextTool.StringEmpty(at))
            {
                at = TextUnknown;
            }

            if (Util.TextTool.StringEmpty(al))
            {
                al = TextUnknown;
            }

            return string.Format("{0},{1},{2}",tt,at,al);
        }

        public async static Task ClearCacheAsync()
        {
            Task t = new Task(new Action(ClearCache));
            t.Start();
            await t;
        }

        public static void ClearCache()
        {
            DirectoryInfo di = new DirectoryInfo(PlotDirectory);

            DirectoryInfo[] directories = di.GetDirectories();

            FileInfo[] files = di.GetFiles();

            foreach(FileInfo fi in files)
            {
                fi.Delete();
            }

            foreach(DirectoryInfo dii in directories)
            {
                dii.Delete(true);
            }
        }
    }
}
