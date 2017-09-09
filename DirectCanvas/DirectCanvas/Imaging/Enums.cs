using System;

namespace DirectCanvas.Imaging
{
    public enum ImageFormat
    {
        Bitmap,
        Png,
        Jpeg,
        Gif,
        Tiff,
        Wmp
    }

    [Flags]
    public enum ImageLock
    {
        Read,
        Write,
        ReadWrite
    }
}