using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Symphony.Lyrics
{
    public class AnimationKeySpline
    {
        public Point ControlPoint1 { get; set; }
        public Point ControlPoint2 { get; set; }
        
        public AnimationKeySpline(double x1, double y1, double x2, double y2)
        {
            ControlPoint1 = new Point(x1, y1);
            ControlPoint2 = new Point(x2, y2);
        }

        public AnimationKeySpline(Point ControlPoint1, Point ControlPoint2)
        {
            this.ControlPoint1 = ControlPoint1;
            this.ControlPoint2 = ControlPoint2;
        }

        public AnimationKeySpline(KeySpline ks)
        {
            ControlPoint1 = new Point(ks.ControlPoint1.X, ks.ControlPoint1.Y);
            ControlPoint2 = new Point(ks.ControlPoint2.X, ks.ControlPoint2.Y);
        }

        public AnimationKeySpline()
        {
            ControlPoint1 = new Point();
            ControlPoint2 = new Point(1,1);
        }

        public KeySpline ToKeySpline()
        {
            return new KeySpline(ControlPoint1, ControlPoint2);
        }

        public KeySpline KeySpline
        {
            get
            {
                return ToKeySpline();
            }
        }
    }
}
