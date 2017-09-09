using System.Runtime.InteropServices;
using DirectCanvas.Misc;
using SlimDX.Direct2D;

namespace DirectCanvas.Shapes
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RoundedRectangleF
    {
        [FieldOffset(0)]
        public float Left;
        [FieldOffset(4)]
        public float Top;
        [FieldOffset(8)]
        public float Right;
        [FieldOffset(12)]
        public float Bottom;
        [FieldOffset(16)]
        public float RadiusX;
        [FieldOffset(20)]
        public float RadiusY;

        [FieldOffset(0)] 
        internal RoundedRectangle InternalRoundedRectangle;

        public RoundedRectangleF(float x, float y, float width, float height, float radiusX, float radiusY)
        {
            InternalRoundedRectangle = new RoundedRectangle();
            Left = x;
            Top = y;
            Right = Left + width;
            Bottom = Top + height;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        public RoundedRectangleF(float x, float y, float width, float height, SizeF radius)
        {
            InternalRoundedRectangle = new RoundedRectangle();
            Left = x;
            Top = y;
            Right = Left + width;
            Bottom = Top + height;
            RadiusX = radius.Width;
            RadiusY = radius.Height;
        }

        public RoundedRectangleF(float x, float y, SizeF size, SizeF radius)
        {
            InternalRoundedRectangle = new RoundedRectangle();
            Left = x;
            Top = y;
            Right = Left + size.Width;
            Bottom = Top + size.Height;
            RadiusX = radius.Width;
            RadiusY = radius.Height;
        }

        public float Width
        {
            get { return Right - Left; }
            set { Right = value + Left; }
        }

        public float Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public PointF Location
        {
            get
            {
                return new PointF(Left, Top);
            }
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public PointF Center
        {
            get
            {
                return new PointF(Left + (Width / 2), Top + (Height / 2));
            }
        }
        

        internal RectangleF GetBounds()
        {
            var bounds = new RectangleF();

            bounds.X = Left;
            bounds.Y = Top;
            bounds.Width = Right - Left;
            bounds.Height = Bottom - Top;

            return bounds;
        }
    }
}
