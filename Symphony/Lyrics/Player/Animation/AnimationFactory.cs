using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Symphony.Lyrics
{
    public class AnimationFactory
    {
        public ValueKeyframeCollection Keyframes { get; set; } = new ValueKeyframeCollection();
        public static int MaxFrameRate { get; set; } = 60;
        public static int MinFrameRate { get; set; } = 24;

        public void Add(ValueKeyframe keyframe)
        {
            Keyframes.Add(keyframe);
        }

        public Storyboard ToStoryboard(DependencyObject targetObj, double duration)
        {
            if (Keyframes.Count > 0)
            {
                Duration du = new Duration(TimeSpan.FromMilliseconds(duration));

                Storyboard sb_main = new Storyboard();
                sb_main.Duration = du;
                sb_main.FillBehavior = FillBehavior.HoldEnd;

                Dictionary<string, DoubleAnimationUsingKeyFrames> dic_ani = new Dictionary<string, DoubleAnimationUsingKeyFrames>();

                List<string> targets = Keyframes.GetTargets();
                bool HasScaleOrRotation = false;
                bool HasBlur = false;
                bool HasSlide = false;
                foreach (string target in targets)
                {
                    if(target == AnimationTargets.RotationAngle || target == AnimationTargets.ScaleX || target == AnimationTargets.ScaleY)
                    {
                        HasScaleOrRotation = true;
                    }
                    else if(target == AnimationTargets.Blur)
                    {
                        HasBlur = true;
                    }
                    else if(target == AnimationTargets.TranslateX || target == AnimationTargets.TranslateY)
                    {
                        HasSlide = true;
                    }

                    DoubleAnimationUsingKeyFrames ani = new DoubleAnimationUsingKeyFrames();

                    ani.Duration = du;

                    Storyboard.SetTarget(ani, targetObj);
                    Storyboard.SetTargetProperty(ani, new PropertyPath(target));

                    dic_ani.Add(target, ani);
                }

                //dynamic framerate control
                double FrameRate = MinFrameRate;
                if (HasScaleOrRotation)
                {
                    FrameRate = MinFrameRate + (MaxFrameRate - MinFrameRate) * 0.15;
                }
                if (HasBlur)
                {
                    FrameRate = MinFrameRate + (MaxFrameRate - MinFrameRate) * 0.3;
                }
                if (HasSlide)
                {
                    FrameRate = MaxFrameRate;
                }

                //create keyframe
                foreach (ValueKeyframe v in Keyframes)
                {
                    string keyTarget = v.Target;
                    DoubleAnimationUsingKeyFrames ani = dic_ani[keyTarget];

                    AnimationKeySpline ks = v.KeySpline;
                    DoubleKeyFrame kf;
                    if (ks == null || ( ks.ControlPoint1.X == 0 && ks.ControlPoint1.Y == 0 && ks.ControlPoint2.X == 1 && ks.ControlPoint2.Y == 1 )) 
                    {
                        kf = new LinearDoubleKeyFrame();
                    }
                    else
                    {
                        kf = new SplineDoubleKeyFrame() { KeySpline = ks.KeySpline };
                    }

                    if(v.Unit == ValueUnits.Value)
                    {
                        kf.Value = v.Value;
                    }
                    else
                    {
                        throw new NotImplementedException("UserAnimation Creation Error: ValueUnits.Percent is not supported with ValueKeyframe.Value");
                    }

                    if(v.TimeUnit == ValueUnits.Percent)
                    {
                        kf.KeyTime = KeyTime.FromPercent(v.Time);
                    }
                    else
                    {
                        double time = v.Time;

                        kf.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(v.Time));

                        if (duration > time)
                        {
                            double total = ani.Duration.TimeSpan.TotalMilliseconds;
                            if (total < time || total == duration)
                            {
                                ani.Duration = new Duration(TimeSpan.FromMilliseconds(time));
                                ani.RepeatBehavior = new RepeatBehavior(sb_main.Duration.TimeSpan);
                            }
                        }
                    }

                    ani.KeyFrames.Add(kf);
                }

                //complete
                foreach(DoubleAnimationUsingKeyFrames ani in dic_ani.Values)
                {
                    foreach(DoubleKeyFrame key in ani.KeyFrames)
                    {
                        key.Freeze();
                    }

                    ani.Freeze();
                    sb_main.Children.Add(ani);
                }
                
                Timeline.SetDesiredFrameRate(sb_main, (int)Math.Round(FrameRate));

                return sb_main;
            }
            else
            {
                return null;
            }
        }
    }
}