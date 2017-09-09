using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Symphony.Lyrics
{
    public class ValueKeyframe
    {
        public double Value { get; set; }
        public ValueUnits Unit { get; set; }

        public double Time { get; set; }
        public ValueUnits TimeUnit { get; set; }

        public AnimationKeySpline KeySpline { get; set; }

        public string Target { get; set; }

        public ValueKeyframe(string target, double value, double time, AnimationKeySpline ks, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent)
        {
            Target = target;
            Value = value;
            Time = time;
            Unit = unit;
            TimeUnit = timeunit;
            KeySpline = ks;
        }

        public ValueKeyframe(string target, double value, double time, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent)
        {
            Target = target;
            Unit = unit;
            Value = value;
            Time = time;
            TimeUnit = timeunit;
            KeySpline = new AnimationKeySpline(0, 0, 0, 1);
        }

        public ValueKeyframe(string target, double value, double time, AnimationKeySpline ks)
        {
            Target = target;
            Value = value;
            Time = time;
            Unit = ValueUnits.Value;
            TimeUnit = ValueUnits.Percent;
            KeySpline = ks;
        }

        public ValueKeyframe()
        {
            Value = 0;
            Time = 0;
            Unit = ValueUnits.Value;
            TimeUnit = ValueUnits.Percent;
            Target = "";
            KeySpline = new AnimationKeySpline(0, 0, 0, 1);
        }
    }

    public enum ValueUnits
    {
        Value = 0,
        Percent = 1
    }
}
