using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.DSP
{
    public static class DspHelper
    {
        public static string API_VERSION = "1";

        public static string[] AvailableApiVersions = new string[]
        {
            "1"
        };

        public static DirectoryInfo Library
        {
            get
            {
                return new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DSP"));
            }
        }

        /// <summary>
        /// return copied path
        /// </summary>
        /// <param name="luaFile">.lua file to import</param>
        /// <returns></returns>
        public static string Import(string luaFile)
        {
            if (!Library.Exists)
            {
                Library.Create();
            }

            string dist = Path.Combine(Library.FullName, Path.GetFileName(luaFile));

            File.Copy(luaFile, dist, true);

            return dist;
        }

        /// <summary>
        /// search file in libaray.
        /// if not exists? NULL!
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static string Search(string FileName)
        {
            FileInfo[] fis = Library.GetFiles("*.lua");
            foreach(FileInfo fi in fis)
            {
                if(Path.GetFileName(fi.FullName) == FileName)
                {
                    return fi.FullName;
                }
            }

            return null;
        }

        public static string[] SearchArray(string FileName)
        {
            FileInfo[] fis = Library.GetFiles("*.lua");
            List<string> list = new List<string>();

            foreach (FileInfo fi in fis)
            {
                if (Path.GetFileName(fi.FullName) == FileName)
                {
                    list.Add(fi.FullName);
                }
            }

            return list.ToArray();
        }

        public static bool IsAvailableApiVersion(string versionText)
        {
            if (versionText == null)
                return false;

            for(int i=0; i<AvailableApiVersions.Length; i++)
            {
                if(AvailableApiVersions[i].ToLower() == versionText)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
