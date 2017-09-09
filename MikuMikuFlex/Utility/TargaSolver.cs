using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MMF.Utility
{
    public static class TargaSolver
    {
        public static Stream LoadTargaImage(string filePath,ImageFormat rootFormat=null)
        {
            Bitmap tgaFile = null;
            if (rootFormat == null) rootFormat = ImageFormat.Png;
            try
            {
                tgaFile = Paloma.TargaImage.LoadTargaImage(filePath);
            }
            catch (Exception)
            {
                return File.OpenRead(filePath);
            }
            MemoryStream ms=new MemoryStream();
            tgaFile.Save(ms,rootFormat);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
