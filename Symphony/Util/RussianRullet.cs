using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Symphony.Util
{
    public static class RussianRullet
    {
        public static Random NewRandom
        {
            get
            {
                return new Random((int)DateTime.Now.TimeOfDay.Ticks);
            }
        }

        private static Random _random;
        public static Random Random
        {
            get
            {
                if(_random == null)
                {
                    _random = NewRandom;
                }

                return _random;
            }
        }

        public static Color RandomColor()
        {
            Random rand = Random;

            return Color.FromRgb((byte)Math.Round((double)rand.Next(0, 255)), (byte)Math.Round((double)rand.Next(0, 255)), (byte)Math.Round((double)rand.Next(0, 255)));
        }
    }
}
