using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Lyrics
{
    public static class AnimationTargets
    {
        public static string Opacity = "Opacity";

        public static string Blur = "Effect.Radius";

        public static string ScaleX = "RenderTransform.Children[0].ScaleX";

        public static string ScaleY = "RenderTransform.Children[0].ScaleY";

        public static string TranslateX = "RenderTransform.Children[3].X";

        public static string TranslateY = "RenderTransform.Children[3].Y";

        public static string RotationAngle = "RenderTransform.Children[2].Angle";
    }
}
