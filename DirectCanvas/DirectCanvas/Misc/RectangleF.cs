using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DirectCanvas.Misc
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RectangleF
    {
        [FieldOffset(0)]
        public float X;
        [FieldOffset(4)]
        public float Y;
        [FieldOffset(8)]
        public float Width;
        [FieldOffset(12)]
        public float Height;

        [FieldOffset(0)]
        internal System.Drawing.RectangleF InternalRectangleF;

        private static RectangleF m_empty;

        public float Left
        {
            get
            {
                return X;
            }
        }
        public float Right
        {
            get
            {
                return (X + Width);
            }
        }
        public float Top
        {
            get
            {
                return Y;
            }
        }
        public float Bottom
        {
            get
            {
                return (Y + Height);
            }
        }

        public PointF Location
        {
            get
            {
                return new PointF(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public PointF Center
        {
            get
            {
                return new PointF(X + (Width / 2), Y + (Height / 2));
            }
        }
        public static RectangleF Empty
        {
            get
            {
                return m_empty;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return ((((Width == 0) && (Height == 0)) && (X == 0)) && (Y == 0));
            }
        }

        public RectangleF(float x, float y, float width, float height)
        {
            InternalRectangleF = new System.Drawing.RectangleF(x,y, width, height);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(double x_double, double y_double, double width_double, double height_double)
        {
            float x, y, width, height;
            x = (float)x_double;
            y = (float)y_double;
            width = (float)width_double;
            height = (float)height_double;

            InternalRectangleF = new System.Drawing.RectangleF(x, y, width, height);
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleF(PointF point, SizeF size)
        {
            InternalRectangleF = new System.Drawing.RectangleF(point.X, point.Y, size.Width, size.Height);
            X = point.X;
            Y = point.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public RectangleF(System.Windows.Point point, System.Windows.Size size) : this(Converter.ToPointF(point), Converter.ToSizeF(size))
        {

        }

        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return ((((a.X == b.X) && (a.Y == b.Y)) && (a.Width == b.Width)) && (a.Height == b.Height));
        }

        public static bool operator !=(RectangleF a, RectangleF b)
        {
            if (((a.X == b.X) && (a.Y == b.Y)) && (a.Width == b.Width))
            {
                return (a.Height != b.Height);
            }
            return true;
        }

        static RectangleF()
        {
            m_empty = new RectangleF();
        }

        public bool Equals(RectangleF other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y) && other.Width.Equals(Width) && other.Height.Equals(Height) && other.InternalRectangleF.Equals(InternalRectangleF);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (RectangleF)) return false;
            return Equals((RectangleF) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result*397) ^ Y.GetHashCode();
                result = (result*397) ^ Width.GetHashCode();
                result = (result*397) ^ Height.GetHashCode();
                result = (result*397) ^ InternalRectangleF.GetHashCode();
                return result;
            }
        }
    }
}
