using Ionic.Zip;
using Symphony.Dancer;
using Symphony.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Lyrics
{
    public static class LyricHelper
    {
        public static string LyricDirectory
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lyrics"));

                if (!di.Exists)
                {
                    di.Create();
                }

                return di.FullName;
            }
        }

        public readonly static string ResoureceDirectoryName = "Resources";

        public static LyricSearchResult SearchLocal(MusicMetadata meta)
        {
            bool localExist = false;
            string localPath = "";

            DirectoryInfo di = new DirectoryInfo(LyricDirectory);

            DirectoryInfo[] contents = di.GetDirectories();

            if (contents.Length > 0)
            {
                List<MusicMetadata> metas = new List<MusicMetadata>();
                for (int i = 0; i < contents.Length; i++)
                {
                    DirectoryInfo item = contents[i];
                    MusicMetadata mt = new MusicMetadata(Path.Combine(item.FullName, "lyric.xml"));
                    metas.Add(mt);
                }

                int searchResult = ScoredSearcher.Search(metas, meta);

                if (searchResult > -1)
                {
                    localExist = true;
                    localPath = contents[searchResult].FullName;
                }
            }

            if (localExist)
            {
                return new LyricSearchResult(LyricExistState.LocalExist, localPath);
            }
            else
            {
                return new LyricSearchResult(LyricExistState.None, localPath);
            }
        }

        public static LyricSearchResult SearchWeb(MusicMetadata meta, Song song, LyricSearchResult LocalResult)
        {
            //check server
            bool serverExist = false;
            RegisteredLyric registeredLyric = null;

            if (song != null)
            {
                QueryResult webResult = LyricWeb.Search(song);
                if (webResult.Success && webResult.Tag != null && ((RegisteredLyric)webResult.Tag).RawData != null)
                {
                    serverExist = true;
                    registeredLyric = (RegisteredLyric)webResult.Tag;
                }
            }

            //end checks erver

            //check local
            bool localExist = false;
            string localPath = "";
            if (LocalResult != null)
            {
                if(LocalResult.Exist == LyricExistState.LocalExist)
                {
                    localExist = true;
                    localPath = LocalResult.LocalPath;
                }
            }

            //end check local

            //return
            if (serverExist)
            {
                if (localExist)
                {
                    return new LyricSearchResult(LyricExistState.GlobalExist, localPath, registeredLyric);
                }
                else
                {
                    return new LyricSearchResult(LyricExistState.WebExitst, localPath, registeredLyric);
                }
            }
            else
            {
                if (localExist)
                {
                    return new LyricSearchResult(LyricExistState.LocalExist, localPath, registeredLyric);
                }
                else
                {
                    return new LyricSearchResult(LyricExistState.None, localPath, registeredLyric);
                }
            }
        }

        public static DirectoryInfo Import(string FilePath)
        {
            string distFile = Path.Combine(LyricDirectory, "lyric.tmp");

            if (File.Exists(distFile))
            {
                File.Delete(distFile);
            }

            try {
                File.Copy(FilePath, distFile);

                string folderName = "";

                using (ZipFile zip = ZipFile.Read(distFile))
                {
                    foreach (ZipEntry entry in zip)
                    {
                        Logger.Log(entry.FileName);

                        if (entry.FileName.EndsWith("lyric.xml"))
                        {
                            folderName = Path.GetDirectoryName(entry.FileName);

                            Logger.Log("folder name: {0}", folderName);

                            if (Directory.Exists(Path.Combine(LyricDirectory, folderName)))
                            {
                                DirectoryInfo di = new DirectoryInfo(Path.Combine(LyricDirectory, folderName));

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

                        entry.Extract(LyricDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                }

                File.Delete(distFile);

                string outDirectory = Path.Combine(LyricDirectory, folderName);

                Logger.Log(outDirectory);

                return new DirectoryInfo(outDirectory);
            }
            catch (Exception e)
            {
                Logger.Error("LyricHelper.Import", e);

                return null;
            }
        }

        public async static Task ClearCacheAsync()
        {
            Task t = new Task(new Action(ClearCache));

            t.Start();

            await t;
        }

        public static void ClearCache()
        {
            DirectoryInfo di = new DirectoryInfo(LyricDirectory);

            DirectoryInfo[] dis = di.GetDirectories();

            FileInfo[] fis = di.GetFiles();

            foreach(FileInfo file in fis)
            {
                file.Delete();
            }

            foreach(DirectoryInfo directory in dis)
            {
                directory.Delete(true);
            }
        }

        public static void SetWrokingDirectory(Lyric Lyric)
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(LyricDirectory, PlotHelper.PlotName(Lyric.Metadata)));
            if (!di.Exists)
            {
                di.Create();
            }
            Lyric.WorkingDirectory = di.FullName;

            Logger.Log("Lyric WorkingDirectory " + Lyric.WorkingDirectory);
        }
    }
}
