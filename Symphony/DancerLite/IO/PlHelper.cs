using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.DancerLite
{
    public static class PlHelper
    {
        public static void Import(string plotliteFile)
        {
            if (ZipFile.IsZipFile(plotliteFile))
            {
                using(ZipFile zip = new ZipFile(plotliteFile))
                {
                    zip.ExtractAll(PlotLiteDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
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
            DirectoryInfo di = new DirectoryInfo(PlotLiteDirectory);
            DirectoryInfo[] dis = di.GetDirectories();

            foreach(DirectoryInfo sub in dis)
            {
                try
                {
                    sub.Delete(true);
                }
                catch (Exception ex)
                {
                    Logger.Error("PL Clearcache", ex);
                }
            }
        }

        private static string _plotDirectory;
        public static string PlotLiteDirectory
        {
            get
            {
                if(_plotDirectory == null)
                {
                    _plotDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PlotLite");
                }
                if (!Directory.Exists(_plotDirectory))
                {
                    Directory.CreateDirectory(_plotDirectory);
                }
                return _plotDirectory;
            }
        }
    }
}
