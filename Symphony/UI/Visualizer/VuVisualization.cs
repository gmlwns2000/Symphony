using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Symphony.Player;
using System.Windows;
using System.Windows.Controls;
using Symphony.Player.DSP;
using DirectCanvas;
using DirectBrush = DirectCanvas.Brushes.Brush;
using Converter = DirectCanvas.Misc.Converter;

namespace Symphony.UI
{
    public class VuVisualization : IVisualizer
    {
        #region Draw

        private double _Opacity = 1;
        public double Opacity
        {
            get
            {
                return _Opacity;
            }
            set
            {
                _Opacity = value;

                if (WhiteBrush != null)
                {
                    if (WhiteBrush.IsFrozen)
                    {
                        WhiteBrush = WhiteBrush.Clone();
                    }
                    else
                    {
                        WhiteBrush.Opacity = value;
                        WhiteBrush.Freeze();
                    }
                }

                if (DarkBrush != null)
                {
                    if (DarkBrush.IsFrozen)
                    {
                        DarkBrush = DarkBrush.Clone();
                    }
                    else
                    {
                        DarkBrush.Opacity = value;
                        DarkBrush.Freeze();
                    }
                }

                UpdateDirectResource();
            }
        }

        private DirectBrush darkBrush;
        private Brush _DarkBrush;
        public Brush DarkBrush
        {
            get { return _DarkBrush; }
            set
            {
                _DarkBrush = value;
                if (_DarkBrush != null)
                {
                    if (_DarkBrush.IsFrozen)
                    {
                        _DarkBrush = _DarkBrush.Clone();
                    }
                    _DarkBrush.Opacity = Opacity;
                    if (_DarkBrush.CanFreeze)
                    {
                        _DarkBrush.Freeze();
                    }

                    UpdateDirectResource();
                }
            }
        }

        private DirectBrush whiteBrush;
        private Brush _WhiteBrush;
        public Brush WhiteBrush
        {
            get { return _WhiteBrush; }
            set
            {
                _WhiteBrush = value;
                if (_WhiteBrush != null)
                {
                    if (_WhiteBrush.IsFrozen)
                    {
                        _WhiteBrush = _WhiteBrush.Clone();
                    }
                    _WhiteBrush.Opacity = Opacity;
                    if (_WhiteBrush.CanFreeze)
                    {
                        _WhiteBrush.Freeze();
                    }

                    UpdateDirectResource();
                }
            }
        }

        #endregion Draw

        List<float> q;
        int sampleRate;
        int q_max = 0;
        double left = 0;
        double right = 0;

        Grid target;
        PlayerCore np;

        public VuVisualization(Grid Target, PlayerCore np)
        {
            this.np = np;
            target = Target;

            Loaded += VuVisualization_Loaded;
        }

        private void VuVisualization_Loaded(object sender, EventArgs e)
        {
            UpdateDirectResource();
        }

        private double _sensetive = 150;
        public double Senstive
        {
            get
            {
                return _sensetive;
            }
            set
            {
                _sensetive = value;
                q_max = (sampleRate / 500) * (int)Senstive;
                if (q != null)
                {
                    q.Clear();
                }
            }
        }

        public override void Init(DSPMaster master, int lentacy, int framems)
        {
            sampleRate = master.SampleRate;

            q_max = (sampleRate / 500) * (int)Senstive;

            q = new List<float>(q_max);
        }

        int calc_vu_frame = 0;
        int vu_avg_frame = 10;
        double pre_peek_left = 0;
        double pre_peek_right = 0;
        double actual_left = 0;
        double actual_right = 0;
        double clamp_left = 0;
        double clamp_right = 0;
        double peek_left = 0;
        double peek_right = 0;

        Point[] pointCollection = new Point[10];
        public override void Render(DirectCanvas.DrawingLayer dc, VisualizerParent vp, float[] frameBuffer)
        {
            if (frameBuffer != null && np.isPlay && !np.isPaused)
            {
                q.AddRange(frameBuffer);

                calc_vu_frame++;

                if (q.Count >= q_max)
                {
                    UpdatePeak();
                }
            }

            actual_left = Math.Max(0, Math.Min(1, actual_left + (peek_left - pre_peek_left) / vu_avg_frame));
            actual_right = Math.Max(0, Math.Min(1, actual_right + (peek_right - pre_peek_right) / vu_avg_frame));
            clamp_left = Math.Log10(actual_left * 9 + 1);
            clamp_right = Math.Log10(actual_right * 9 + 1);

            double x = 0;
            double y = 105;
            double width = target.ActualWidth;
            double height = target.ActualHeight;

            pointCollection[0] = new Point(x, y);
            pointCollection[1] = new Point(x + width, y);
            pointCollection[2] = new Point(x, y + height * (1 - clamp_left));
            pointCollection[3] = new Point(x + width, y + height * (1 - clamp_right));
            pointCollection[4] = new Point(x, y + height * (1 - 0.666 * clamp_right));
            pointCollection[5] = new Point(x + width, y + height * (1 - 0.666 * clamp_left));
            pointCollection[6] = new Point(x, y + height * (1 - 0.333 * clamp_left));
            pointCollection[7] = new Point(x + width, y + height * (1 - 0.333 * clamp_right));
            pointCollection[8] = new Point(x, y + height);
            pointCollection[9] = new Point(x + width, y + height);

            if (pointCollection != null && pointCollection.Length > 0 && Opacity != 0)
            {
                drawRect(dc, pointCollection[0], pointCollection[1], pointCollection[3], pointCollection[2], darkBrush);
                drawRect(dc, pointCollection[2], pointCollection[3], pointCollection[5], pointCollection[4], whiteBrush);
                drawRect(dc, pointCollection[4], pointCollection[5], pointCollection[7], pointCollection[6], darkBrush);
                drawRect(dc, pointCollection[6], pointCollection[7], pointCollection[9], pointCollection[8], whiteBrush);
            }
        }

        private void UpdatePeak()
        {
            for (int i = 0; i < q.Count; i++)
            {
                if (0 == i % 2)
                {
                    left = Math.Max(left, Math.Abs(q[i]));
                }
                else
                {
                    right = Math.Max(right, Math.Abs(q[i]));
                }
            }

            q.Clear();

            vu_avg_frame = calc_vu_frame;
            calc_vu_frame = 0;

            actual_left = peek_left;
            actual_right = peek_right;
            pre_peek_left = peek_left;
            pre_peek_right = peek_right;

            peek_left = left;
            peek_right = right;

            if (peek_left < pre_peek_left)
            {
                peek_left = (peek_left + pre_peek_left) * 0.5f;
            }

            if (peek_right < pre_peek_right)
            {
                peek_right = (peek_right + pre_peek_right) * 0.5f;
            }

            left = 0;
            right = 0;
        }

        private void drawRect(DirectCanvas.DrawingLayer dcx, Point p1, Point p2, Point p3, Point p4, DirectBrush brush)
        {
            if (Presenter != null && brush != null)
            {
                using (DirectCanvas.Shapes.PathGeometry path = Presenter.Factory.CreatePathGeometry()) {

                    path.BeginModify(DirectCanvas.Shapes.FillMode.Winding);
                    path.BeginFigure(DirectCanvas.Shapes.FigureBegin.Filled, Converter.ToPointF(p1));
                    path.AddLine(Converter.ToPointF(p2));
                    path.AddLine(Converter.ToPointF(p3));
                    path.AddLine(Converter.ToPointF(p4));
                    path.EndFigure(DirectCanvas.Shapes.FigureEnd.Closed);
                    path.EndModify();

                    dcx.FillGeometry(brush, path);
                }
            }
        }

        private void UpdateDirectResource()
        {
            if(Presenter != null)
            {
                if(darkBrush != null)
                {
                    darkBrush.Dispose();
                    darkBrush = null;
                }

                darkBrush = DirectCanvas.Misc.Converter.ToBrush(Presenter.Factory, _DarkBrush);

                if (whiteBrush != null)
                {
                    whiteBrush.Dispose();
                    whiteBrush = null;
                }

                whiteBrush = DirectCanvas.Misc.Converter.ToBrush(Presenter.Factory, _WhiteBrush);
            }
        }

        public override void SetOn(bool on)
        {
            throw new NotImplementedException();
        }
    }
}
