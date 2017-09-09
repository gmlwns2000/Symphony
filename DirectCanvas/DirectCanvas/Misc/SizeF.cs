using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DirectCanvas.Misc
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SizeF
    {
        public SizeF(float width, float height)
        {
            InternalSize = new System.Drawing.SizeF(width, height);
            Width = width;
            Height = height;
        }

        public SizeF(double width, double height)
        {
            InternalSize = new System.Drawing.SizeF((float)width, (float)height);
            Width = (float)width;
            Height = (float)height;
        }

        [FieldOffset(0)]
        public float Width;
        [FieldOffset(4)]
        public float Height;

        [FieldOffset(0)]
        internal System.Drawing.SizeF InternalSize;

        public static implicit operator SizeF(System.Drawing.SizeF gdiSize)
        {
            return new SizeF(gdiSize.Width, gdiSize.Height);
        }
    }
}
