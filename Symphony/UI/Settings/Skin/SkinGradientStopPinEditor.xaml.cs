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
using System.Windows.Threading;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SkinGradientStopPinEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkinGradientStopPinEditor : UserControl
    {
        public GradientStop stop;
        FrameworkElement parent;
        bool inited;

        bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if(_selected != value)
                {
                    if (value)
                    {
                        polygon.StrokeThickness = 3;
                    }
                    else
                    {
                        polygon.StrokeThickness = 0;
                    }
                }
                _selected = value;
            }
        }

        public event EventHandler<ObjectChangedArgs<GradientStop>> GradientStopUpdated;
        public event EventHandler<ObjectChangedArgs<GradientStop>> Clicked;

        public SkinGradientStopPinEditor(GradientStop stop, FrameworkElement parent)
        {
            InitializeComponent();

            SetTarget(stop, parent);
        }

        public void SetTarget(GradientStop stop, FrameworkElement parent)
        {
            if(this.parent != null)
            {
                this.parent.SizeChanged -= Parent_SizeChanged;
            }

            this.stop = stop.Clone();
            this.parent = parent;

            parent.SizeChanged += Parent_SizeChanged;

            Update();
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        public void Update()
        {
            inited = false;
            
            double left = parent.ActualWidth * stop.Offset - ActualWidth * 0.5;
            Canvas.SetLeft(this, left);

            parent.Height = this.ActualHeight;

            polygon.Fill = new SolidColorBrush(stop.Color);
            polygon.Fill.Freeze();

            inited = true;
        }

        DispatcherTimer mouseMove;
        Point lastPt = new Point(0,12344);
        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (inited)
            {
                Clicked?.Invoke(this, new ObjectChangedArgs<GradientStop>(stop));

                lastPt = new Point(0, 12344);

                if (mouseMove == null)
                {
                    mouseMove = new DispatcherTimer();
                    mouseMove.Interval = TimeSpan.FromMilliseconds(15);
                    mouseMove.Tick += delegate (object obj, EventArgs arg)
                    {
                        if (Mouse.LeftButton == MouseButtonState.Released)
                        {
                            Clicked?.Invoke(this, new ObjectChangedArgs<GradientStop>(stop));

                            mouseMove.Stop();

                            return;
                        }

                        if (parent == null)
                            return;

                        Point pt = Mouse.GetPosition(parent);

                        if (pt.X != lastPt.X || pt.Y != lastPt.Y)
                        {
                            stop.Offset = Math.Max(0, Math.Min(1, pt.X / parent.ActualWidth));

                            GradientStopUpdated?.Invoke(this, new ObjectChangedArgs<GradientStop>(stop));

                            Update();
                        }

                        lastPt = pt;
                    };
                }

                if (!mouseMove.IsEnabled)
                    mouseMove.Start();
            }
        }
    }
}
