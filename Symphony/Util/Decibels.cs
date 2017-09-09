using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Util
{
    public static class Decibels
    {
        public static double v2dB(double v)
        {
            return 20 * Math.Log10(v);
        }

        public static double dB2v(double d)
        {
            return Math.Pow(10, d / 20);
        }

        public static double sqrt2v(double s)
        {
            return s * s;
        }

        public static double v2sqrt(double v)
        {
            return Math.Sqrt(v);
        }
    }
}
