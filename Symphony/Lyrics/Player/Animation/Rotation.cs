using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Symphony.Lyrics
{
    public class Rotation
    {
        public double Angle { get; set; } = 0;

        public double X { get; set; } = 0.5;

        public double Y { get; set; } = 0.5;

        public Rotation()
        {

        }

        public Rotation(double Angle)
        {
            this.Angle = Angle;
        }

        public Rotation(double Angle, double X, double Y)
        {
            this.Angle = Angle;
            this.X = X;
            this.Y = Y;
        }

        public RotateTransform RotationTransform
        {
            get
            {
                return GetRotationTransform(this);
            }
        }
        
        private static RotateTransform GetRotationTransform(Rotation r)
        {
            RotateTransform rotate = new RotateTransform(r.Angle, r.X, r.Y);
            rotate.Freeze();

            return rotate;
        }
    }
}
