using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using DirectCanvas.Misc;

namespace DirectCanvas.Shapes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Ellipse
    {
        public Ellipse(PointF center, float radiusX, float radiusY)
        {
            InternalEllipse = new SlimDX.Direct2D.Ellipse();

            Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public Ellipse(System.Windows.Point center, double radiusX, double radiusY) : this(Misc.Converter.ToPointF(center), (float)radiusX, (float)radiusY)
        {

        }

        public RectangleF GetBounds()
        {
            var bounds = new RectangleF(Center.X - (RadiusX),
                                        Center.Y - (RadiusY),
                                        RadiusX * 2, RadiusY * 2);

            return bounds;
        }

        [FieldOffset(0)]
        public PointF Center;
        [FieldOffset(8)]
        public float RadiusX;
        [FieldOffset(12)]
        public float RadiusY;

        [FieldOffset(0)]
        internal SlimDX.Direct2D.Ellipse InternalEllipse;
    }
}
