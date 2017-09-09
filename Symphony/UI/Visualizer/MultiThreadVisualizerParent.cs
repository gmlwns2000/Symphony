using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using DirectCanvas;
using System.Threading.Tasks;

namespace Symphony.UI
{
    public class MultiThreadVisualizerParent : VisualizerParent
    {
        private ImageSource RenderedSource;

        private DirectCanvasFactory factory;
        private WPFPresenter presenter;
        private DispatcherTimer resizeTimer;

        private bool _allowRender = true;
        public new bool AllowRender
        {
            get
            {
                return _allowRender;
            }
            set
            {
                _allowRender = value;

                UpdateRenderState();
            }
        }

        public MultiThreadVisualizerParent() : base()
        {
            UseMotionBlur = false;

            Loaded += MultiThreadVisualizerParent_Loaded;
            SizeChanged += MultiThreadVisualizerParent_SizeChanged;
        }

        private void UpdateRenderState()
        {
            if (presenter != null)
            {
                if (mw != null)
                {
                    presenter.IsAllowRender = _allowRender && mw.isFrameUpadteAllowed;
                }
                else
                {
                    presenter.IsAllowRender = _allowRender;
                }
            }
        }

        private void MultiThreadVisualizerParent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (presenter != null)
            {
                if (resizeTimer == null)
                {
                    resizeTimer = new DispatcherTimer();
                    resizeTimer.Interval = TimeSpan.FromMilliseconds(10);
                    resizeTimer.Tick += delegate
                    {
                        

                        resizeTimer.Stop();
                    };
                }

                resizeTimer.Start();

                presenter.Width = (int)e.NewSize.Width;
                presenter.Height = (int)e.NewSize.Height;
            }
        }

        private void MultiThreadVisualizerParent_Loaded(object sender, RoutedEventArgs e)
        {
            factory = new DirectCanvasFactory();

            presenter = new WPFPresenter(factory, ActualWidth, ActualHeight);
            presenter.FrameUpdated += Presenter_FrameUpdated;
            presenter.Rendering += Presenter_Rendering;
            presenter.VSync = false;

            foreach (IVisualizer v in Visualizers)
            {
                if (!v.IsInited)
                {
                    v.InitRender(presenter);
                }
            }

            presenter.StartRendering();

            mw.UpdateAllowChanged += Mw_UpdateAllowChanged;

            UpdateRenderState();
        }

        private void Mw_UpdateAllowChanged(object sender, bool e)
        {
            UpdateRenderState();
        }

        private void Presenter_Rendering(object sender, EventArgs e)
        {
            if (!inited)
                return;

            presenter.Clear(new Color4(0,0,0,0));

            presenter.BeginDraw();

            float[] buffer = GetFrameBuffer();

            foreach(IVisualizer v in Visualizers)
            {
                v.Render(presenter, this, buffer);
            }

            presenter.EndDraw();
        }

        private void Presenter_FrameUpdated(object sender, EventArgs e)
        {
            ImageSource source = presenter.ImageSource;
            if (Dispatcher.CheckAccess())
            {
                UpdateFrame(source);
            }
            else
            {
                Dispatcher.Invoke(delegate
                {
                    UpdateFrame(source);
                });
            }
        }

        private void UpdateFrame(ImageSource source)
        {
            if (source != null && AllowRender)
            {
                DrawingVisual visual = new DrawingVisual();
                visual.CacheMode = new BitmapCache();

                using (System.Windows.Media.DrawingContext dc = visual.RenderOpen())
                {
                    dc.DrawImage(source, new Rect(new System.Windows.Size(source.Width, source.Height)));

                    dc.Close();
                }

                PushVisual(visual);
            }
        }

        public ImageSource GetBitmapSource()
        {
            ImageSource source = RenderedSource;

            RenderedSource = null;

            return source;
        }

        public new void Update()
        {
            if (inited)
            {
                framems = mw.GUIUpdate;
                if (presenter != null)
                    presenter.TargetFPS = 1000 / framems;
            }
        }
    }
}
