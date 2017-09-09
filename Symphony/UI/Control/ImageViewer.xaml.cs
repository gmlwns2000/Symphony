using Symphony.Util;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Symphony.UI
{
    /// <summary>
    /// ImageViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageViewer : UserControl, IDisposable
    {
        double _zoom = 1;
        double zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;

                if (Img_St != null)
                {
                    if (zoomAnimation == null)
                    {
                        zoomAnimation = new DispatcherTimer();
                        zoomAnimation.Interval = TimeSpan.FromMilliseconds(12);
                        zoomAnimation.Tick += ZoomAnimation_Tick;
                    }

                    if (velocityUpdater != null && velocityUpdater.IsEnabled)
                    {
                        velocityUpdater.Stop();
                    }

                    if (!zoomAnimation.IsEnabled)
                    {
                        zoomPreH = Sv.HorizontalOffset;
                        zoomPreV = Sv.VerticalOffset;
                        zoomPreHW = Sv.ScrollableWidth;
                        zoomPreVW = Sv.ScrollableHeight;

                        RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.NearestNeighbor);
                        zoomAnimation.Start();
                    }
                }
            }
        }

        double zoomPreH;
        double zoomPreV;
        double zoomPreHW;
        double zoomPreVW;

        private void ZoomAnimation_Tick(object sender, EventArgs e)
        {
            //zoom start;
            if (Math.Abs(Img_St.ScaleX - zoom) > 0.001)
            {
                Img_St.ScaleX = Img_St.ScaleX + (zoom - Img_St.ScaleX) / 4;
                Img_St.ScaleY = Img_St.ScaleX;
            }
            else
            {
                Img_St.ScaleX = zoom;
                Img_St.ScaleY = Img_St.ScaleX;

                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                zoomAnimation.Stop();
            }

            Tb_Info.Text = string.Format("{1}: {0}%", Math.Round(Img_St.ScaleX * 1000) / 10, LanguageHelper.FindText("Lang_ImageViewer_Zoom"));

            if (!Util.TextTool.StringEmpty(Title))
            {
                Tb_Info.Text += " - " + Title;
            }

            UpdateLayout();

            double zm = zoomPreH / zoomPreHW;
            if (double.IsNaN(zm) || double.IsInfinity(zm))
            {
                Sv.ScrollToHorizontalOffset(Sv.ScrollableWidth * 0.5);
                Sv.ScrollToVerticalOffset(Sv.ScrollableHeight * 0.5);
            }
            else
            {
                Sv.ScrollToHorizontalOffset(Sv.ScrollableWidth * zm);
                Sv.ScrollToVerticalOffset(Sv.ScrollableHeight * zm);
            }
        }
        public string FormatText = "";
        public string Title = "";

        DispatcherTimer zoomAnimation;
        MainWindow mw;
        public string ImagePath = "";

        public ImageViewer()
        {

        }

        public void Init(MainWindow mw, string imgSource, string title)
        {
            this.mw = mw;
            ImagePath = imgSource;
            this.Title = title;

            InitializeComponent();

            Loaded += ImageViewer_Loaded;
            SizeChanged += ImageViewer_SizeChanged;

            BitmapDecoder decoder = BitmapDecoder.Create(new Uri(ImagePath), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);
            BitmapFrame frame = decoder.Frames[0];
            int width = frame.PixelWidth;
            int height = frame.PixelHeight;
            BitmapMetadata metadata = frame.Metadata as BitmapMetadata;
            FormatText = metadata.Format.ToUpper();

            Img.Source = frame;

            frame.Freeze();

            Img.Width = width;
            Img.Height = height;
            Img.Stretch = Stretch.Fill;
        }

        DispatcherTimer resizeHandler;
        private void ImageViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.NearestNeighbor);
                if(resizeHandler == null)
                {
                    resizeHandler = new DispatcherTimer();
                    resizeHandler.Tick += delegate
                    {
                        RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);

                        resizeHandler.Stop();
                    };
                    resizeHandler.Interval = TimeSpan.FromMilliseconds(300);
                }

                if (resizeHandler.IsEnabled)
                {
                    resizeHandler.Stop();
                }
                resizeHandler.Start();
            }
        }

        private void ImageViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ZoomToFit(true);

            Tb_Info.Text = string.Format("{1}: {0}%", Math.Round(Img_St.ScaleX * 1000) / 10, LanguageHelper.FindText("Lang_ImageViewer_Zoom"));

            if (!Util.TextTool.StringEmpty(Title))
            {
                Tb_Info.Text += " - " + Title;
            }

            UpdateLayout();

            if (Img.Source != null)
            {
                Tb_Meta.Text = string.Format("{2}: {0}px {3}:{1}px", ((BitmapFrame)Img.Source).PixelWidth, ((BitmapFrame)Img.Source).PixelHeight, LanguageHelper.FindText("Lang_ImageViewer_Width"), LanguageHelper.FindText("Lang_ImageViewer_Height"));

                if (!string.IsNullOrEmpty(FormatText))
                {
                    Tb_Meta.Text += " " + LanguageHelper.FindText("Lang_ImageViewer_Format") + ": " + FormatText;
                }
            }
        }

        private void ZoomToFit(bool withOutAnimation = false)
        {
            if (Img.Source.Width > Img.Source.Height)
            {
                _zoom = Math.Min(5, (ActualWidth - 1) / ((BitmapFrame)Img.Source).PixelWidth);

                if (withOutAnimation)
                {
                    Img_St.ScaleX = _zoom;
                    Img_St.ScaleY = _zoom;
                }

                zoom = _zoom;
            }
            else
            {
                _zoom = Math.Min(5, (ActualHeight - 1) / ((BitmapFrame)Img.Source).PixelHeight);

                if (withOutAnimation)
                {
                    Img_St.ScaleX = _zoom;
                    Img_St.ScaleY = _zoom;
                }

                zoom = _zoom;
            }
        }

        double DragVelocityV;
        double DragVelocityH;
        double preSvH = 0;
        double preSvV = 0;

        DispatcherTimer velocityUpdater;
        private void DragMover_Tick(object sender, EventArgs e)
        {
            Point now = Mouse.GetPosition(Sv);

            double distH = Math.Max(0, Math.Min(Sv.ScrollableWidth, Sv.HorizontalOffset + (DragHorizontalOffset - (now.X - DragStart.X) - Sv.HorizontalOffset)));
            double distV = Math.Max(0, Math.Min(Sv.ScrollableHeight, Sv.VerticalOffset + (DragVerticalOffset - (now.Y - DragStart.Y) - Sv.VerticalOffset)));

            Sv.ScrollToHorizontalOffset(distH);
            Sv.ScrollToVerticalOffset(distV);

            DragVelocityH = (distH - preSvH);
            DragVelocityV = (distV - preSvV);
            preSvV = Sv.VerticalOffset;
            preSvH = Sv.HorizontalOffset;

            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                DragMover.Stop();

                if (velocityUpdater == null)
                {
                    velocityUpdater = new DispatcherTimer();
                    velocityUpdater.Interval = TimeSpan.FromMilliseconds(12);
                    velocityUpdater.Tick += VelocityUpdater_Tick;
                }

                if (velocityUpdater.IsEnabled)
                {
                    RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                    velocityUpdater.Stop();
                }

                DragVelocityH = Math.Max(-60, Math.Min(60, DragVelocityH));
                DragVelocityV = Math.Max(-60, Math.Min(60, DragVelocityV));

                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.NearestNeighbor);
                velocityUpdater.Start();
            }
        }

        private void VelocityUpdater_Tick(object sender, EventArgs e)
        {
            if (Math.Abs(DragVelocityH) > 0.05 || Math.Abs(DragVelocityV) > 0.05)
            {
                Sv.ScrollToHorizontalOffset(Math.Max(0, Math.Min(Sv.ScrollableWidth, Sv.HorizontalOffset + DragVelocityH)));
                Sv.ScrollToVerticalOffset(Math.Max(0, Math.Min(Sv.ScrollableHeight, Sv.VerticalOffset + DragVelocityV)));

                DragVelocityV = DragVelocityV * 0.85;
                DragVelocityH = DragVelocityH * 0.85;
            }
            else
            {
                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                velocityUpdater.Stop();
            }
        }

        public void StopAnimation()
        {
            if (zoomAnimation != null && zoomAnimation.IsEnabled)
            {
                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                zoomAnimation.Stop();
            }

            if (velocityUpdater != null && velocityUpdater.IsEnabled)
            {
                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                velocityUpdater.Stop();
            }

            if(resizeHandler != null && resizeHandler.IsEnabled)
            {
                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                resizeHandler.Stop();
            }

            if(DragMover != null && DragMover.IsEnabled)
            {
                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                DragMover.Stop();
            }
        }

        public void Dispose()
        {
            StopAnimation();

            Img = null;
        }

        DispatcherTimer DragMover;
        Point DragStart;
        double DragHorizontalOffset;
        double DragVerticalOffset;

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = e.GetPosition(this);

            if (pt.X > 0 && pt.X < ActualWidth - 14 && pt.Y > 0 && pt.Y < ActualHeight - 14)
            {
                e.Handled = true;
                mw.StopRenderingWhileClicking();

                DragStart = e.GetPosition(Sv);
                DragVerticalOffset = Sv.VerticalOffset;
                DragHorizontalOffset = Sv.HorizontalOffset;

                if (velocityUpdater != null && velocityUpdater.IsEnabled)
                {
                    RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                    velocityUpdater.Stop();
                }

                if (DragMover == null)
                {
                    DragMover = new DispatcherTimer();
                    DragMover.Interval = TimeSpan.FromMilliseconds(12);
                    DragMover.Tick += DragMover_Tick;
                }

                if (DragMover.IsEnabled)
                {
                    RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                    DragMover.Stop();
                }

                if (zoomAnimation != null && zoomAnimation.IsEnabled)
                {
                    RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.Fant);
                    zoomAnimation.Stop();
                }

                preSvV = Sv.VerticalOffset;
                preSvH = Sv.HorizontalOffset;

                RenderOptions.SetBitmapScalingMode(Img, BitmapScalingMode.NearestNeighbor);
                DragMover.Start();
            }

        }

        public void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && DragMover != null && !DragMover.IsEnabled)
            {
                double newzoom = Math.Max(0.02, Math.Min(10, zoom * (1 + (double)e.Delta / 1000)));
                if (Math.Abs(newzoom - 1) < 0.05)
                {
                    newzoom = 1;
                }
                zoom = newzoom;

                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                //side scroll
            }
        }

        private void Ct_Image_50p_Click(object sender, RoutedEventArgs e)
        {
            zoom = 0.5;
        }

        private void Ct_Image_100p_Click(object sender, RoutedEventArgs e)
        {
            zoom = 1;
        }

        private void Ct_Image_FitToScreen_Click(object sender, RoutedEventArgs e)
        {
            ZoomToFit();
        }

        private void Ct_Image_200p_Click(object sender, RoutedEventArgs e)
        {
            zoom = 2;
        }

        private void Ct_Image_Smaller_Click(object sender, RoutedEventArgs e)
        {
            zoom = zoom / 2;
        }

        private void Ct_Image_Bigger_Click(object sender, RoutedEventArgs e)
        {
            zoom = zoom * 2;
        }
    }
}
