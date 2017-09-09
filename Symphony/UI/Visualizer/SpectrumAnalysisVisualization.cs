using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Symphony.Player;
using Symphony.Player.DSP;
using Symphony.Player.DSP.CSCore;
using DirectCanvas;
using DirectBrush = DirectCanvas.Brushes.Brush;
using DirectPen = DirectCanvas.Brushes.Pen;

namespace Symphony.UI
{
    public class SpectrumAnalysisVisualization : IVisualizer
    {
        private AudioGridProvider GridProvider;
        public HorizontalAlignment GridTextHorizontalAlignment
        {
            get
            {
                return GridProvider.GridTextHorizontalAlignment;
            }
            set
            {
                GridProvider.GridTextHorizontalAlignment = value;
            }
        }

        public BarRenderTypes RenderType { get; set; } = BarRenderTypes.Rectangle;

        public bool RenderGrid { get; set; } = true;

        public bool UseInvert { get; set; } = false;

        private double _height = 0.38;
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        private DirectPen directPen;
        private DirectBrush directBrush;
        private Brush _Brush;
        public Brush Brush
        {
            get
            {
                return _Brush;
            }
            set
            {
                _Brush = value;

                if (_Brush != null)
                {
                    if (_Brush.IsFrozen)
                    {
                        _Brush = _Brush.Clone();
                    }

                    _Brush.Opacity = _Opacity;

                    if (_Brush.CanFreeze)
                    {
                        _Brush.Freeze();
                    }

                    UpdateDirectResource();
                }
            }
        }

        private double _Opacity = 0.528;
        public double Opacity
        {
            get
            {
                return _Opacity;
            }
            set
            {
                _Opacity = value;

                if (Brush != null)
                {
                    if (Brush.IsFrozen)
                    {
                        Brush = Brush;
                    }
                    else
                    {
                        Brush.Opacity = value;
                        Brush.Freeze();
                    }
                }
            }
        }

        private double _width = 12;
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                if(_Brush != null)
                {
                    UpdateDirectResource();
                }
            }
        }

        private double _dash = -1;
        public double Dash
        {
            get
            {
                return _dash;
            }
            set
            {
                _dash = value;
            }
        }

        private double _strength = 0.75;
        public double Strength
        {
            get
            {
                return _strength;
            }
            set
            {
                _strength = value;
            }
        }

        private double _top = 0.52;
        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                _top = value;
            }
        }

        public SpectrumAnalysis SpectrumAnalysis;
        
        private PlayerCore np;
        private double[] bars;

        private object brushLock = new object();

        public SpectrumAnalysisVisualization(PlayerCore np)
        {
            this.np = np;

            GridProvider = new AudioGridProvider();

            RenderGrid = false;

            GridProvider.GridTextHorizontalAlignment = HorizontalAlignment.Right;

            Loaded += SpectrumAnalysisVisualization_Loaded;
        }

        private void SpectrumAnalysisVisualization_Loaded(object sender, EventArgs e)
        {
            UpdateDirectResource();

            GridProvider.InitRender(Presenter);
        }

        public override void Init(DSPMaster master, int lentacy, int framems)
        {
            SpectrumAnalysis = new SpectrumAnalysis();

            SpectrumAnalysis.Init(master);
        }

        public override void Render(DrawingLayer dc, VisualizerParent VisualParent, float[] frameBuffer)
        {
            double _width = this._width;
            //calc
            double[] barBuffer;
            
            if (SpectrumAnalysis !=null && frameBuffer != null && Dash + Width != 0 && Width != 0 && frameBuffer != null && SpectrumAnalysis != null && !np.isPaused && np.isPlay)
            {
                int barCount = (int)Math.Ceiling(Math.Abs((VisualParent.ActualWidth * 0.5) / (Dash + Width)) + 1);

                SpectrumBase.SpectrumPointData[] datas = SpectrumAnalysis.GetSpectrum(frameBuffer, barCount, Dash);

                if (datas == null)
                {
                    barBuffer = null;
                }
                else
                {
                    double[] ret = new double[datas.Length + 1];

                    if (UseInvert)
                    {
                        for (int i = 1; i < datas.Length + 1; i++)
                        {
                            ret[ret.Length - i - 1] = datas[i - 1].Value * Height * VisualParent.ActualHeight;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < datas.Length; i++)
                        {
                            ret[i] = datas[i].Value * Height * VisualParent.ActualHeight;
                        }
                    }

                    ret[datas.Length] = ret[datas.Length - 1];

                    if (bars != null && _strength != 1)
                    {
                        for (int i = 0; i < Math.Min(ret.Length, bars.Length); i++)
                        {
                            if (Math.Abs(ret[i]) < Math.Abs(bars[i]))
                            {
                                ret[i] = ret[i] * _strength + bars[i] * (1 - _strength);
                            }
                        }
                    }

                    barBuffer = ret;
                }
            }
            else
            {
                barBuffer = null;
            }

            //null
            if (Opacity != 0)
            {
                if (barBuffer != null && barBuffer.Length > 0)
                {
                    bars = barBuffer;
                }
                else if (bars != null)
                {
                    bars = nullRect(bars);
                }
            }

            //render
            if (Presenter != null && bars != null && Opacity != 0 && Math.Round(Width) + Math.Round(Dash) != 0)
            {
                double parentWidth = VisualParent.ActualWidth;
                double x_origin = Math.Round(VisualParent.ActualWidth / 2);
                double y_origin = Math.Round(VisualParent.ActualHeight * Top);
                double pointWidth = Dash + Width;

                int count = Math.Min(bars.Length, (int)Math.Ceiling(x_origin / pointWidth + 1));
                int startX = (int)x_origin - (int)pointWidth * count;

                if (count > 0)
                {
                    lock (brushLock)
                    {
                        switch (RenderType)
                        {
                            case BarRenderTypes.Dots:
                                for (int i = 0; x_origin + pointWidth * i < parentWidth + Width && i < bars.Length; i++)
                                {
                                    double height = bars[i];
                                    double radius = Width / 2;
                                    dc.DrawEllipse(directBrush, null, new DirectCanvas.Shapes.Ellipse(new Point(x_origin + (i * pointWidth), y_origin - height), radius, radius));
                                    dc.DrawEllipse(directBrush, null, new DirectCanvas.Shapes.Ellipse(new Point(x_origin - ((i + 1) * pointWidth), y_origin - height), radius, radius));
                                }
                                break;
                            case BarRenderTypes.Filled:
                                using (DirectCanvas.Shapes.PathGeometry geo = Presenter.Factory.CreatePathGeometry())
                                {
                                    geo.BeginModify(DirectCanvas.Shapes.FillMode.Alternate);
                                    geo.BeginFigure(DirectCanvas.Shapes.FigureBegin.Filled, new DirectCanvas.Misc.PointF(startX, y_origin));

                                    for (int i = count - 1; i >= 0; i--)
                                    {
                                        double height = bars[i];
                                        geo.AddLine(new DirectCanvas.Misc.PointF(x_origin - (i * pointWidth), y_origin - height));
                                    }

                                    for (int i = 0; i < count; i++)
                                    {
                                        double height = bars[i];
                                        geo.AddLine(new DirectCanvas.Misc.PointF(x_origin + (i * pointWidth), y_origin - height));
                                    }
                                    geo.AddLine(new DirectCanvas.Misc.PointF(parentWidth, y_origin));
                                    geo.EndFigure(DirectCanvas.Shapes.FigureEnd.Closed);
                                    geo.EndModify();

                                    dc.FillGeometry(directBrush, geo);
                                }
                                break;
                            case BarRenderTypes.Line:
                                using (DirectCanvas.Shapes.PathGeometry geo_ = Presenter.Factory.CreatePathGeometry())
                                {
                                    geo_.BeginModify(DirectCanvas.Shapes.FillMode.Winding);
                                    bool started = false;

                                    for (int i = count - 1; i >= 0; i--)
                                    {
                                        double height = bars[i];
                                        DirectCanvas.Misc.PointF pt = new DirectCanvas.Misc.PointF(x_origin - (i * pointWidth), y_origin - height);
                                        if (started)
                                        {
                                            geo_.AddLine(pt);
                                        }
                                        else
                                        {
                                            pt.X = 0;
                                            geo_.BeginFigure(DirectCanvas.Shapes.FigureBegin.Hollow, pt);
                                            started = true;
                                        }
                                    }

                                    for (int i = 0; i < count; i++)
                                    {
                                        double height = bars[i];
                                        DirectCanvas.Misc.PointF pt = new DirectCanvas.Misc.PointF(x_origin + (i * pointWidth), y_origin - height);
                                        if (count == i + 1)
                                            pt.X = (float)parentWidth;
                                        geo_.AddLine(pt);
                                    }

                                    geo_.EndFigure(DirectCanvas.Shapes.FigureEnd.Open);
                                    geo_.EndModify();

                                    dc.DrawGeometry(directBrush, geo_, (float)_width);
                                }
                                break;
                            case BarRenderTypes.Rectangle:
                                for (int i = 0; x_origin + pointWidth * i < parentWidth + Width && i < bars.Length; i++)
                                {
                                    double height = bars[i];
                                    Size size = new Size(Width, Math.Abs(height));
                                    dc.DrawRectangle(directBrush, null, new DirectCanvas.Misc.RectangleF(new Point(x_origin + (i * pointWidth), y_origin - Math.Max(0, height)), size));
                                    dc.DrawRectangle(directBrush, null, new DirectCanvas.Misc.RectangleF(new Point(x_origin - ((i + 1) * pointWidth), y_origin - Math.Max(0, height)), size));
                                }
                                break;
                        }
                    }
                }

                if (RenderGrid)
                {
                    GridProvider.GridScaling = SpectrumAnalysis.ScalingStrategy;

                    if (Height > 0)
                    {
                        switch (GridProvider.GridScaling)
                        {
                            case ScalingStrategy.Decibel:
                                GridProvider.MinDb = SpectrumBase.MinDbValue;
                                GridProvider.MaxDb = SpectrumBase.MaxDbValue;
                                break;
                            case ScalingStrategy.Linear:
                                GridProvider.MinV = 0;
                                GridProvider.MaxV = 1;
                                break;
                            case ScalingStrategy.Sqrt:
                                GridProvider.MinSqrt = 0;
                                GridProvider.MaxSqrt = 1;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        switch (GridProvider.GridScaling)
                        {
                            case ScalingStrategy.Decibel:
                                GridProvider.MinDb = SpectrumBase.MaxDbValue;
                                GridProvider.MaxDb = SpectrumBase.MinDbValue;
                                break;
                            case ScalingStrategy.Linear:
                                GridProvider.MinV = 1;
                                GridProvider.MaxV = 0;
                                break;
                            case ScalingStrategy.Sqrt:
                                GridProvider.MinSqrt = 1;
                                GridProvider.MaxSqrt = 0;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }

                    switch (GridProvider.GridScaling)
                    {
                        case ScalingStrategy.Decibel:
                            GridProvider.RenderRect = new Rect(new System.Windows.Point(0, y_origin - Math.Max(0, VisualParent.ActualHeight * Height)), new System.Windows.Size(VisualParent.ActualWidth, Math.Abs(VisualParent.ActualHeight * Height)));
                            break;
                        case ScalingStrategy.Linear:
                            double l_height = VisualParent.ActualHeight * Height * SpectrumBase.ScaleFactorLinear;
                            GridProvider.RenderRect = new Rect(new System.Windows.Point(0, y_origin - Math.Max(0, l_height)), new System.Windows.Size(VisualParent.ActualWidth, Math.Abs(l_height)));
                            break;
                        case ScalingStrategy.Sqrt:
                            double s_height = VisualParent.ActualHeight * Height * SpectrumBase.ScaleFactorSqr;
                            GridProvider.RenderRect = new Rect(new System.Windows.Point(0, y_origin - Math.Max(0, s_height)), new System.Windows.Size(VisualParent.ActualWidth, Math.Abs(s_height)));
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    GridProvider.GridRender(dc, VisualParent);
                }
            }
        }

        private double[] nullRect(double[] rt)
        {
            for (int i = 0; i < rt.Length; i++)
            {
                if (rt[i] > 0)
                {
                    rt[i] = Math.Max(0, rt[i] - Math.Min(32, rt[i] - Math.Min(15, rt[i] * 0.85) * 0.75));
                }
                else
                {
                    rt[i] = Math.Min(0, rt[i] - Math.Max(-32, rt[i] - Math.Max(-15, rt[i] * 0.85) * 0.75));
                }
            }

            return rt;
        }

        private void UpdateDirectResource()
        {
            lock (brushLock)
            {
                if (Presenter != null)
                {
                    if (directBrush != null)
                    {
                        directBrush.Dispose();
                        directBrush = null;
                    }

                    directBrush = DirectCanvas.Misc.Converter.ToBrush(Presenter.Factory, Brush);

                    if (directPen != null)
                    {
                        directPen.Dispose();
                        directPen = null;
                    }

                    directPen = new DirectPen(directBrush, _width);
                }
            }
        }

        public override void SetOn(bool on)
        {
            throw new NotImplementedException();
        }
    }
}
