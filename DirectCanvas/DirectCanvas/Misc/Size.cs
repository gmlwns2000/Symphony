using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DirectCanvas.Misc
{
     [StructLayout(LayoutKind.Explicit)]
    public struct Size
    {
        public Size(int width, int height)
        {
            InternalSize = new System.Drawing.Size(width, height);
            Width = width;
            Height = height;
        }

        [FieldOffset(0)]
        public int Width;
        [FieldOffset(4)]
        public int Height;

        [FieldOffset(0)]
        internal System.Drawing.Size InternalSize;

        public static implicit operator Size(System.Drawing.Size gdiSize)
        {
           return new Size(gdiSize.Width, gdiSize.Height);
        }
    }
}
