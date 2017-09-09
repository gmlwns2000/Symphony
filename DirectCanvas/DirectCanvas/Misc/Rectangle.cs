using System.Runtime.InteropServices;

namespace DirectCanvas.Misc
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        private static Rectangle m_empty;
        public int Left
        {
            get
            {
                return X;
            }
        }
        public int Right
        {
            get
            {
                return (X + Width);
            }
        }
        public int Top
        {
            get
            {
                return Y;
            }
        }
        public int Bottom
        {
            get
            {
                return (Y + Height);
            }
        }
        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public Point Center
        {
            get
            {
                return new Point(X + (Width / 2), Y + (Height / 2));
            }
        }
        public static Rectangle Empty
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
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public System.Drawing.Rectangle GetInternalRect()
        {
            return new System.Drawing.Rectangle(X, Y, Width, Height);
        }

        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return ((((a.X == b.X) && (a.Y == b.Y)) && (a.Width == b.Width)) && (a.Height == b.Height));
        }

        public static bool operator !=(Rectangle a, Rectangle b)
        {
            if (((a.X == b.X) && (a.Y == b.Y)) && (a.Width == b.Width))
            {
                return (a.Height != b.Height);
            }
            return true;
        }

        static Rectangle()
        {
            m_empty = new Rectangle();
        }

        public bool Equals(Rectangle other)
        {
            return other.X == X && other.Y == Y && other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (Rectangle)) return false;
            return Equals((Rectangle) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X;
                result = (result*397) ^ Y;
                result = (result*397) ^ Width;
                result = (result*397) ^ Height;
                return result;
            }
        }
    }
}
