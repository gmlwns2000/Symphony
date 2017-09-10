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
using System.Windows.Shapes;
using Symphony.Player;
using System.Windows.Interop;
using SlimDX;
using MMF.Matricies.Camera.CameraMotion;
using MMF.Grid;

namespace Symphony.Dancer
{
    /// <summary>
    /// ComposerRender.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ComposerRender : Window, IDisposable
    {
        public bool isEditor;
        public double position = 0;

        public int scrIndex = -1;
        public Ratio Ratio;
        public new HorizontalAlignment HorizontalAlignment;
        public new VerticalAlignment VerticalAlignment;
        
        Plot Plot;
        PlayerCore np;

        public ComposerRender(ref Plot plot, ref PlayerCore np, Window owner, bool debug, int scrIndex)
        {
            clickAble = debug;

            InitializeComponent();
            
            Owner = owner;

            this.np = np;
            np.PlayStarted += Np_PlayStarted;
            np.PlayResumed += Np_PlayResumed;
            np.PlayPaused += Np_PlayPaused;
            np.PlaySeeked += Np_PlaySeeked;

            this.Ratio = plot.Ratio;
            this.HorizontalAlignment = plot.HorizontalAlignment;
            this.VerticalAlignment = plot.VerticalAlignment;
            isEditor = debug;
            if (isEditor)
            {
                Grid_Edge.Visibility = Visibility.Visible;
            }

            SetScreen(scrIndex);

            Plot = plot;

            Loaded += ComposerRender_Loaded;
            Closed += ComposerRender_Closed;
        }

        private void Np_PlaySeeked()
        {
            if(Plot != null)
            {
                Plot.Instances.OnSeeked(np.GetPosition(TimeUnit.MilliSecond));
            }
        }

        private void Np_PlayPaused()
        {
            if (Plot != null)
            {
                Plot.Instances.OnPauseChanged(true);
            }
        }

        private void Np_PlayResumed()
        {
            if (Plot != null)
            {
                Plot.Instances.OnPauseChanged(false);
            }
        }

        private void Np_PlayStarted()
        {
            if (Plot != null)
            {
                Plot.Instances.OnPlayStarted(np.GetPosition(TimeUnit.MilliSecond));
            }
        }

        private void ComposerRender_Closed(object sender, EventArgs e)
        {
            if (Plot != null)
            {
                Plot.Instances.Dispose();
            }

            Dispose();
        }

        public void Dispose()
        {
            if (Plot != null)
            {
                Plot.Instances.Dispose();
            }
        }

        private void ComposerRender_Loaded(object sender, RoutedEventArgs e)
        {
            RenderControl.TextureContext.MatrixManager.ViewMatrixManager.CameraPosition = new Vector3(0, 50, -25);
            RenderControl.TextureContext.MatrixManager.ViewMatrixManager.CameraLookAt = new Vector3(0, 10, 0);
            RenderControl.TextureContext.CameraMotionProvider = new CameraControl(RenderControl, 45);

            if (Plot != null)
            {
                Plot.Instances.OnLoad(RenderControl, Plot.WorkingDirectory);

                if (np.isPlay)
                {
                    Plot.Instances.OnPlayStarted(np.GetPosition(TimeUnit.MilliSecond));
                }
            }
        }

        public void AddInstance(Instance inst)
        {
            inst.OnLoad(RenderControl, Plot.WorkingDirectory);

            if (np.isPlay)
            {
                inst.OnPlayStarted(np.GetPosition(TimeUnit.MilliSecond));
                inst.OnPauseChanged(np.isPaused);
            }
        }

        public void RemoveInstance(Instance inst)
        {
            RenderControl.AllowRendering = false;

            inst.Dispose();

            RenderControl.AllowRendering = true;
        }

        #region Window Control

        /// <summary>
        /// 창을 조정합니다
        /// </summary>
        /// <param name="index"></param>
        public void SetScreen(int index)
        {
            double Top, Left, Width, Height;

            scrIndex = index;
            PresentationSource source = PresentationSource.FromVisual(Owner);
            double dpiY = source.CompositionTarget.TransformToDevice.M22;

            System.Windows.Forms.Screen target;

            if (index < 0 || index > System.Windows.Forms.Screen.AllScreens.Length - 1)
            {
                target = System.Windows.Forms.Screen.PrimaryScreen;
            }
            else
            {
                target = System.Windows.Forms.Screen.AllScreens[index];
            }

            Top = target.WorkingArea.Y / dpiY;
            Left = target.WorkingArea.X / dpiY;
            Width = target.WorkingArea.Width / dpiY;
            Height = target.WorkingArea.Height / dpiY;

            if (Ratio.Width / (Width / Height) > Ratio.Height)
            {
                Height = (Width / Ratio.Width) * Ratio.Height;
                Top += (target.WorkingArea.Height / dpiY - Height) * 0.5;
            }
            else
            {
                Width = Height / Ratio.Height * Ratio.Width;
                Left += (target.WorkingArea.Width / dpiY - Width) * 0.5;
            }

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top:
                    Top = target.WorkingArea.Y / dpiY;
                    break;
                case VerticalAlignment.Center:
                    break;
                case VerticalAlignment.Bottom:
                    Top = (target.WorkingArea.Y / dpiY) + (target.WorkingArea.Height / dpiY - this.Height);
                    break;
                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    Left = target.WorkingArea.X / dpiY;
                    break;
                case HorizontalAlignment.Center:
                    break;
                case HorizontalAlignment.Right:
                    Left = (target.WorkingArea.X / dpiY) + (target.WorkingArea.Width / dpiY - this.Width);
                    break;
                default:
                    break;
            }

            this.Left = Left;
            this.Top = Top;
            this.Width = Width;
            this.Height = Height;
        }

        public void GetFocus()
        {
            bool tm = Topmost;
            Topmost = true;
            Topmost = false;
            Topmost = tm;
        }

        public bool AllowClose = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isEditor)
            {
                if (!AllowClose)
                {
                    e.Cancel = true;
                }
            }
        }

        public bool clickAble;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (!clickAble)
            {
                IntPtr hwnd = new WindowInteropHelper(this).Handle;

                UI.WindowTransclick.set(hwnd);
            }
        }

        #endregion Window Control

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isEditor)
            {
                Debug_Border.Visibility = Visibility.Visible;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isEditor)
            {
                Debug_Border.Visibility = Visibility.Hidden;
            }
        }
    }
}
