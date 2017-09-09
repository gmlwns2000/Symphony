using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Util
{
    public class IO
    {
        public static string SupportedImageFilter
        {
            get
            {
                return string.Format("{0}|*.jpeg;*.jpg;*.png;*.tiff;*.bmp;*.gif|{1}|*.jpeg;*.jpg|{2}|*.png|{3}|*.*", 
                    LanguageHelper.FindText("Lang_SupportImageFormat"), LanguageHelper.FindText("Lang_JPEGFile"), LanguageHelper.FindText("Lang_PNGFile"), LanguageHelper.FindText("Lang_AllFileFormat"));
            }
        }

        public static void DirectoryCopy(string sourceDi, string destDi, bool overWrite = true)
        {
            Logger.Log("Copy Directory: " + sourceDi + " => " + destDi);

            if (!Directory.Exists(destDi))
            {
                Directory.CreateDirectory(destDi);
            }

            foreach (string file in Directory.GetFiles(sourceDi))
            {
                string dest = Path.Combine(destDi, Path.GetFileName(file));
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        File.Copy(file, dest, overWrite);
                        i = 100;
                        break;
                    }
                    catch(Exception e)
                    {
                        Logger.Error("Directory copy", e);
                    }
                }
            }

            foreach (string folder in Directory.GetDirectories(sourceDi))
            {
                string dest = Path.Combine(destDi, Path.GetFileName(folder));
                DirectoryCopy(folder, dest, overWrite);
            }
        }
    }
}
