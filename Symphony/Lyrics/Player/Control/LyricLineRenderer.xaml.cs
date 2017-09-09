using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Symphony.Player;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Symphony.Lyrics
{
    /// <summary>
    /// LyricLineRenderer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricLineRenderer : UserControl, IDisposable
    {
        public static FadeInMode DefaultFadeIn;
        public static FadeOutMode DefaultFadeOut;

        public bool isClosed = false;
        public event EventHandler Closed;
        public LyricLine line;

        PlayerCore np;
        Singer singer;
        FrameworkElement content;
        DropShadowEffect Ef_DropShadow;
        double Duration;
        double FadeInDuration;
        double FadeOutDuration;
        double Zoom;

        bool Optimize = true;
        
        public LyricLineRenderer(ref PlayerCore np, Singer singer, LyricLine line, double Duration)
        {
            InitializeComponent();

            contentGrid.Children.Clear();

            this.np = np;

            this.singer = singer;
            singer.Rendering += Rendering;
            Zoom = singer.Zoom;
            Optimize = singer.Optimize;

            this.line = line;

            this.Duration = Duration;

            FadeOutDuration = Math.Max(0, Math.Min(3500, line.FadeOutLength * Duration));
            FadeInDuration = Math.Max(0, Math.Min(3500, line.FadeInLength * Duration));

            HorizontalAlignment = line.HorizontalAlignment;
            VerticalAlignment = line.VerticalAlignment;

            Translate.X = line.BoxOffset.X;
            Translate.Y = line.BoxOffset.Y;
            
            Margin = line.Margin;
            MinWidth = line.MinSize.Width;
            MinHeight = line.MinSize.Height;
            
            line.Content.RequireUpdate = true;
            line.Content.Update();

            content = line.Content.Content;

            TransformGroup group = new TransformGroup();

            ScaleTransform scale = new ScaleTransform(Zoom, Zoom, 0.5, 0.5);
            scale.Freeze();

            RotateTransform rotate = line.ContentRotation.RotationTransform;
            rotate.Freeze();

            group.Children.Add(scale);
            group.Children.Add(rotate);
            group.Freeze();

            contentGrid.LayoutTransform = group;

            if(content != null)
                contentGrid.Children.Add(content);

            Opacity = line.Opacity;

            Ef_BlurEffect.Radius = Math.Round(line.Blur.Radius);

            Ef_DropShadow = new DropShadowEffect();
            Ef_DropShadow.Opacity = line.Shadow.Opacity;
            Ef_DropShadow.Color = line.Shadow.Color;
            Ef_DropShadow.ShadowDepth = Math.Round(line.Shadow.Depth);
            Ef_DropShadow.Direction = line.Shadow.Direction;
            Ef_DropShadow.BlurRadius = Math.Round(line.Shadow.Radius);
            contentGrid.Effect = Ef_DropShadow;

            Optimizing();

            double position = np.GetPosition(TimeUnit.MilliSecond) - np.DesiredLatency;
            if (position <= line.Position + FadeInDuration)
            {
                StartFadeIn();
            }
            else
            {
                FadeIning(FadeInMode.None);
            }
        }

        private void Optimizing()
        {
            if (Optimize)
            {
                if(Ef_DropShadow == null || content == null)
                {
                    contentGrid.Effect = null;
                }
                else
                {
                    if(Ef_DropShadow.Opacity == 0)
                    {
                        contentGrid.Effect = null;
                    }
                    if(Ef_DropShadow.BlurRadius == 0)
                    {
                        contentGrid.Effect = null;
                    }
                    if(Ef_DropShadow.Color.A == 0)
                    {
                        contentGrid.Effect = null;
                    }
                }

                if (Ef_BlurEffect == null)
                {
                    grid.Effect = null;
                }
                else
                {
                    if(Ef_BlurEffect.Radius == 0)
                    {
                        grid.Effect = null;
                    }
                }

                if(Ef_AllBlur == null)
                {
                    Effect = null;
                }
                else
                {
                    FadeInMode fadein = FadeInMode.Auto;
                    if(line.FadeIn == FadeInMode.Auto)
                    {
                        fadein = DefaultFadeIn;
                    }
                    else
                    {
                        fadein = line.FadeIn;
                    }

                    FadeOutMode fadeout = FadeOutMode.Auto;
                    if(line.FadeOut == FadeOutMode.Auto)
                    {
                        fadeout = DefaultFadeOut;
                    }
                    else
                    {
                        fadeout = line.FadeOut;
                    }

                    if(fadein != FadeInMode.BlurIn && fadeout != FadeOutMode.BlurOut)
                    {
                        Effect = null;
                    }
                }

                if(line.Content is ImageContent)
                {
                    CacheMode = new BitmapCache();
                }
            }
            else
            {
                Logger.Log("LineRenderer is not optimized!");
            }
        }

        #region FadeIn

        private void StartFadeIn()
        {
            //switch fade in
            if(line.FadeIn == FadeInMode.Auto)
            {
                FadeIning(DefaultFadeIn);
            }
            else
            {
                FadeIning(line.FadeIn);
            }
        }

        Storyboard Sb_FadeIn;
        bool IsFadeIn = false;

        private void FadeIning(FadeInMode Mode)
        {
            Sb_FadeIn = new Storyboard();

            switch (Mode)
            {
                case FadeInMode.None:
                    Visibility = Visibility.Visible;
                    Sb_FadeIn = null;
                    break;
                case FadeInMode.FadeIn:
                    Sb_FadeIn = AnimationPresets.FadeIn(this, FadeInDuration, line.FadeInKeySpline);
                    break;
                case FadeInMode.BlurIn:
                    Sb_FadeIn = AnimationPresets.BlurIn(this, FadeInDuration, line.FadeInKeySpline);
                    break;
                case FadeInMode.ZoomIn:
                    Sb_FadeIn = AnimationPresets.ZoomIn_In(this, FadeInDuration, line.FadeInKeySpline);
                    break;
                case FadeInMode.ZoomOut:
                    Sb_FadeIn = AnimationPresets.ZoomOut_In(this, FadeInDuration, line.FadeInKeySpline);
                    break;
                case FadeInMode.SlideFromLeft:
                    Sb_FadeIn = AnimationPresets.SlideFromLeft(this, new Size(singer.Width, singer.Height), FadeInDuration, line.FadeInKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeInMode.SlideFromRight:
                    Sb_FadeIn = AnimationPresets.SlideFromRight(this, new Size(singer.Width, singer.Height), FadeInDuration, line.FadeInKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeInMode.SlideFromTop:
                    Sb_FadeIn = AnimationPresets.SlideFromTop(this, new Size(singer.Width, singer.Height), FadeInDuration, line.FadeInKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeInMode.SlideFromBottom:
                    Sb_FadeIn = AnimationPresets.SlideFromBottom(this, new Size(singer.Width, singer.Height), FadeInDuration, line.FadeInKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeInMode.RotateClock:
                    Sb_FadeIn = AnimationPresets.RotateCounterClock_In(this, FadeInDuration, line.FadeInKeySpline);
                    break;
                case FadeInMode.RotateCounterClock:
                    Sb_FadeIn = AnimationPresets.RotateCounterClock_In(this, FadeInDuration, line.FadeInKeySpline);
                    break;
                default:
                    throw new NotImplementedException("Unknown Fadein Animation Preset. " + Mode.ToString());
            }

            IsFadeIn = true;

            if (Sb_FadeIn != null)
            {
                Sb_FadeIn.Completed += Sb_FadeIn_Completed;

                Sb_FadeIn.Freeze();

                Sb_FadeIn.Begin();
            }
            else
            {
                Sb_FadeIn_Completed(this, null);
            }

            Visibility = Visibility.Visible;
        }

        private void Sb_FadeIn_Completed(object sender, EventArgs e)
        {
            IsFadeIn = false;
        }

        #endregion FadeIn

        #region FadeOut

        private void StartFadeOut()
        {
            IsFadeOut = true;

            if (Sb_FadeIn != null && IsFadeIn)
            {
                Sb_FadeIn.Stop();
            }

            if (line.FadeOut == FadeOutMode.Auto)
            {
                FadeOuting(DefaultFadeOut);
            }
            else
            {
                FadeOuting(line.FadeOut);
            }
        }

        Storyboard Sb_FadeOut;
        bool IsFadeOut = false;

        private void FadeOuting(FadeOutMode Mode)
        {
            Sb_FadeOut = new Storyboard();

            switch (Mode)
            {
                case FadeOutMode.None:
                    Sb_FadeOut = null;
                    break;
                case FadeOutMode.FadeOut:
                    Sb_FadeOut = AnimationPresets.FadeOut(this, FadeOutDuration, line.FadeOutKeySpline);
                    break;
                case FadeOutMode.BlurOut:
                    if (Effect != null)
                    {
                        Sb_FadeOut = AnimationPresets.BlurOut(this, FadeOutDuration, line.FadeOutKeySpline);
                    }
                    else
                    {
                        Sb_FadeOut_Completed(this, null);
                        return;
                    }
                    break;
                case FadeOutMode.ZoomIn:
                    Sb_FadeOut = AnimationPresets.ZoomIn_Out(this, FadeInDuration, line.FadeOutKeySpline);
                    break;
                case FadeOutMode.ZoomOut:
                    Sb_FadeOut = AnimationPresets.ZoomOut_Out(this, FadeInDuration, line.FadeOutKeySpline);
                    break;
                case FadeOutMode.SlideToLeft:
                    Sb_FadeOut = AnimationPresets.SlideToLeft(this, new Size(singer.Width, singer.Height), FadeOutDuration, line.FadeOutKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeOutMode.SlideToRight:
                    Sb_FadeOut = AnimationPresets.SlideToRight(this, new Size(singer.Width, singer.Height), FadeOutDuration, line.FadeOutKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeOutMode.SlideToTop:
                    Sb_FadeOut = AnimationPresets.SlideToTop(this, new Size(singer.Width, singer.Height), FadeOutDuration, line.FadeOutKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeOutMode.SlideToBottom:
                    Sb_FadeOut = AnimationPresets.SlideToBottom(this, new Size(singer.Width, singer.Height), FadeOutDuration, line.FadeOutKeySpline, line.BoxOffset.X, line.BoxOffset.Y);
                    break;
                case FadeOutMode.RotateClock:
                    Sb_FadeOut = AnimationPresets.RotateClock_Out(this, FadeOutDuration, line.FadeOutKeySpline);
                    break;
                case FadeOutMode.RotateCounterClock:
                    Sb_FadeOut = AnimationPresets.RotateCounterClock_Out(this, FadeOutDuration, line.FadeOutKeySpline);
                    break;
                default:
                    throw new NotImplementedException("Unknown Fadeout Animation Preset. " + Mode.ToString());
            }

            if (Sb_FadeIn != null)
            {
                Sb_FadeIn.Stop();
            }

            if (Sb_FadeOut != null)
            {
                Sb_FadeOut.Completed += Sb_FadeOut_Completed;

                Sb_FadeOut.Freeze();

                Sb_FadeOut.Begin();
            }
            else
            {
                Sb_FadeOut_Completed(this, null);
            }
        }

        private void Sb_FadeOut_Completed(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            isClosed = true;
            Closed?.Invoke(this, null);
        }

        #endregion FadeOut

        #region Rendering
        
        private void Rendering(object sender, EventArgs e)
        {
            if(!IsFadeOut)
            {
                if (singer.Position > line.Position + Duration - FadeOutDuration)
                {
                    StartFadeOut();
                }
                else if (singer.Position < line.Position || singer.Position > line.Position + Duration)
                {
                    Visibility = Visibility.Hidden;
                    isClosed = true;
                    Closed?.Invoke(this, null);
                }
            }
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if(singer != null)
                singer.Rendering -= Rendering;
        }

        #endregion Rendering
    }
}
