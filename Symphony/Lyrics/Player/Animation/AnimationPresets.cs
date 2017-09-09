using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Symphony.Lyrics
{
    public static class AnimationPresets
    {
        public static Storyboard RotateClock_In(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Rotate(-30, 0, 0.33, 1, targetObj, duration, ks);
        }

        public static Storyboard RotateClock_Out(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Rotate(0, 30, 1, 0.33, targetObj, duration, ks);
        }

        public static Storyboard RotateCounterClock_In(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Rotate(30, 0, 0.33, 1, targetObj, duration, ks);
        }

        public static Storyboard RotateCounterClock_Out(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Rotate(0, -30, 1, 0.33, targetObj, duration, ks);
        }

        private static Storyboard Rotate(double startAngle, double endAngle, double startOpacity, double endOpacity, DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            AnimationFactory animation = new AnimationFactory();

            animation.Add(new OpacityKeyframe(startOpacity, 0, new AnimationKeySpline()));
            animation.Add(new RotateAngleKeyframe(startAngle, 0, new AnimationKeySpline()));

            animation.Add(new OpacityKeyframe(endOpacity, 1, new AnimationKeySpline()));
            animation.Add(new RotateAngleKeyframe(endAngle, 1, ks));

            return animation.ToStoryboard(targetObj, duration);
        }

        public static Storyboard FadeIn(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Opacity(0, 1, targetObj, duration, ks);
        }

        public static Storyboard FadeOut(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Opacity(1, 0, targetObj, duration, ks);
        }

        private static Storyboard Opacity(double opacityStart, double opacityEnd, DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            AnimationFactory animation = new AnimationFactory();

            animation.Add(new OpacityKeyframe(opacityStart, 0, new AnimationKeySpline()));
            animation.Add(new OpacityKeyframe(opacityEnd, 1, ks));

            return animation.ToStoryboard(targetObj, duration);
        }

        public static Storyboard BlurIn(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Blur(12, 0, targetObj, duration, ks);
        }

        public static Storyboard BlurOut(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Blur(0, 12, targetObj, duration, ks);
        }

        private static Storyboard Blur(double blruStart, double blurEnd, DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            AnimationFactory animation = new AnimationFactory();

            animation.Add(new ValueKeyframe(AnimationTargets.Blur, blruStart, 0, new AnimationKeySpline()));
            animation.Add(new ValueKeyframe(AnimationTargets.Blur, blurEnd, 1, ks));

            return animation.ToStoryboard(targetObj, duration);
        }

        public static Storyboard SlideToLeft(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(0, -targetSize.Width, 0, 0, targetObj, duration, ks, baseX, baseY);
        }

        public static Storyboard SlideToRight(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(0, targetSize.Width, 0, 0, targetObj, duration, ks, baseX, baseY);
        }

        public static Storyboard SlideToTop(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(0, 0, 0, -targetSize.Height, targetObj, duration, ks, baseX, baseY);
        }

        public static Storyboard SlideToBottom(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(0, 0, 0, targetSize.Height, targetObj, duration, ks, baseX, baseY);
        }

        public static Storyboard SlideFromLeft(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(-targetSize.Width, 0, 0, 0, targetObj, duration, ks, baseX, baseY);
        }

        public static Storyboard SlideFromRight(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(targetSize.Width, 0, 0, 0, targetObj, duration, ks, baseX, baseY);
        }

        public static Storyboard SlideFromTop(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(0, 0, -targetSize.Height, 0, targetObj, duration, ks, baseX, baseY);
        }

        public static Storyboard SlideFromBottom(DependencyObject targetObj, Size targetSize, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            return Slide(0, 0, targetSize.Height, 0, targetObj, duration, ks, baseX, baseY);
        }

        private static Storyboard Slide(double startX, double endX, double startY, double endY, DependencyObject targetObj, double duration, AnimationKeySpline ks, double baseX, double baseY)
        {
            AnimationFactory animation = new AnimationFactory();
            
            if(startX != endX)
            {
                animation.Add(new TranslateXKeyframe(baseX + startX, 0, new AnimationKeySpline()));
                animation.Add(new TranslateXKeyframe(baseX + endX, 1, ks));
            }

            if(startY != endY)
            {
                animation.Add(new TranslateYKeyframe(baseY + startY, 0, new AnimationKeySpline()));
                animation.Add(new TranslateYKeyframe(baseY + endY, 1, ks));
            }

            return animation.ToStoryboard(targetObj, duration);
        }

        public static Storyboard ZoomIn_In(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Zoom(0, 1, 0.66, 1, targetObj, duration, ks);
        }

        public static Storyboard ZoomIn_Out(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Zoom(1, 2, 1, 0, targetObj, duration, ks);
        }

        public static Storyboard ZoomOut_In(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Zoom(2, 1, 0.33, 1, targetObj, duration, ks);
        }

        public static Storyboard ZoomOut_Out(DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            return Zoom(1, 0, 1, 0, targetObj, duration, ks);
        }

        private static Storyboard Zoom(double startZoom, double endZoom, double startOpacity, double endOpacity, DependencyObject targetObj, double duration, AnimationKeySpline ks)
        {
            AnimationFactory animation = new AnimationFactory();

            animation.Add(new ScaleXKeyframe(startZoom, 0, new AnimationKeySpline()));
            animation.Add(new ScaleYKeyframe(startZoom, 0, new AnimationKeySpline()));
            animation.Add(new OpacityKeyframe(startOpacity, 0, new AnimationKeySpline()));

            animation.Add(new ScaleXKeyframe(endZoom, 1, ks));
            animation.Add(new ScaleYKeyframe(endZoom, 1, ks));
            animation.Add(new OpacityKeyframe(endOpacity, 1, new AnimationKeySpline()));

            return animation.ToStoryboard(targetObj, duration);
        }
    }
}
