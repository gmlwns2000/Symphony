using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Symphony.Lyrics
{
    public class OpacityKeyframe : ValueKeyframe
    {
        public OpacityKeyframe(double opacity, double time, AnimationKeySpline ks, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent) 
            : base(AnimationTargets.Opacity, opacity, time, ks, unit, timeunit)
        {

        }
    }

    public class ScaleXKeyframe : ValueKeyframe
    {
        public ScaleXKeyframe(double scalex, double time, AnimationKeySpline ks, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent)
            : base(AnimationTargets.ScaleX, scalex, time, ks, unit, timeunit)
        {

        }
    }

    public class ScaleYKeyframe : ValueKeyframe
    {
        public ScaleYKeyframe(double opacity, double time, AnimationKeySpline ks, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent)
            : base(AnimationTargets.ScaleY, opacity, time, ks, unit, timeunit)
        {

        }
    }

    public class TranslateXKeyframe : ValueKeyframe
    {
        public TranslateXKeyframe(double transx, double time, AnimationKeySpline ks, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent)
            : base(AnimationTargets.TranslateX, transx, time, ks, unit, timeunit)
        {

        }
    }

    public class TranslateYKeyframe : ValueKeyframe
    {
        public TranslateYKeyframe(double opacity, double time, AnimationKeySpline ks, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent)
            : base(AnimationTargets.TranslateY, opacity, time, ks, unit, timeunit)
        {

        }
    }

    public class RotateAngleKeyframe : ValueKeyframe
    {
        public RotateAngleKeyframe(double angle, double time, AnimationKeySpline ks, ValueUnits unit = ValueUnits.Value, ValueUnits timeunit = ValueUnits.Percent)
            : base(AnimationTargets.RotationAngle, angle, time, ks, unit, timeunit)
        {

        }
    }
}
