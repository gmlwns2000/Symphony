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
using DirectCanvas.Scenes;
using DirectCanvas.Misc;
using System.Windows.Threading;

namespace DirectCanvas
{
    /// <summary>
    /// DirectCanvasControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DirectCanvasControl : UserControl, IDisposable
    {
        public WPFPresenter Presenter { get; private set; }

        public DirectCanvasFactory Factory { get; set; }

        public bool IsRendering
        {
            get
            {
                return Presenter.IsRenderingActive;
            }
        }

        public bool IsAllowRender
        {
            get
            {
                return Presenter.IsAllowRender;
            }
            set
            {
                Presenter.IsAllowRender = value;
            }
        }

        private Scene _scene;
        public Scene Scene
        {
            get
            {
                return _scene;
            }
            set
            {
                _scene = value;

                Presenter.CurrentScene = _scene;
            }
        }

        DispatcherTimer resizeTimer;

        public DirectCanvasControl()
        {
            InitializeComponent();

            Loaded += DirectCanvasControl_Loaded;
        }

        private void DirectCanvasControl_Loaded(object sender, RoutedEventArgs e)
        {
            Factory = new DirectCanvasFactory();

            Presenter = new WPFPresenter(Factory, ActualWidth, ActualHeight, wpfImage);

            Presenter.CurrentScene = _scene;
            Presenter.StartRendering();
            Presenter.TargetFPS = 60;
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            var pos = e.GetPosition(this);
            var point = new PointF((float)pos.X, (float)pos.Y);

            _scene.SetInputState(0, InputStatus.Down, point);
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            var pos = e.GetPosition(this);
            var point = new PointF((float)pos.X, (float)pos.Y);

            _scene.SetInputState(0, InputStatus.Up, point);
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this);
            var point = new PointF((float)pos.X, (float)pos.Y);
            var status = InputStatus.Up;

            if (e.LeftButton == MouseButtonState.Pressed)
                status = InputStatus.Down;

            _scene.SetInputState(0, status, point);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Presenter == null)
                return;

            if (resizeTimer == null)
            {
                resizeTimer = new DispatcherTimer();

                resizeTimer.Tick += delegate
                {
                    UpdateLayout();

                    Presenter.SetSize(ActualWidth, ActualHeight);

                    resizeTimer.Stop();
                };

                resizeTimer.Interval = TimeSpan.FromMilliseconds(200);
            }

            resizeTimer.Start();
        }

        public void Dispose()
        {
            if (Presenter != null)
            {
                Presenter.Dispose();
                Presenter = null;
            }

            if(Factory != null)
            {
                Factory.Dispose();
                Factory = null;
            }
        }
    }
}
