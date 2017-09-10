using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Symphony.Player;
using System.Windows;
using Symphony.Player.DSP;
using DirectBrush = DirectCanvas.Brushes.Brush;
using DirectPen = DirectCanvas.Brushes.Pen;
using DirectCanvas.Shapes;
using System.Xml;
using Symphony.Util;

namespace Symphony.UI
{
    public class OsiloscopeVisualization : IVisualizer
    {
        private AudioGridProvider GridProvider = new AudioGridProvider();
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

        private double _view = 50;
        public double View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;

                q_max = (int)((View / 1000) * sampleRate);
                if (q != null)
                {
                    q.Clear();
                }
            }
        }

        private double _strength = 0.7f;
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

        private double _height = 0.33f;
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
        private Brush _fillBrush;
        public Brush FillBrush
        {
            get
            {
                return _fillBrush;
            }
            set
            {
                _fillBrush = value;

                if (_fillBrush != null)
                {
                    if (_fillBrush.IsFrozen)
                    {
                        _fillBrush = _fillBrush.Clone();
                    }

                    if (_fillBrush.CanFreeze)
                    {
                        _fillBrush.Freeze();
                    }

                    UpdateDirectResource();
                }
            }
        }

        private double _opacity = 1;
        public double Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;

                if (FillBrush != null)
                {
                    if (FillBrush.IsFrozen)
                    {
                        FillBrush = FillBrush.Clone();
                    }
                    else
                    {
                        FillBrush.Freeze();
                    }

                    UpdateDirectResource();
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

                if(_fillBrush != null)
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

        #region XML Serialization

        private const string XMLRootNode = "Oscilloscope";

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(XMLRootNode);

            writer.WriteAttributeString("GridTextHorizontalAlignement", Enum.GetName(typeof(HorizontalAlignment), GridTextHorizontalAlignment));
            writer.WriteAttributeString("RenderType", Enum.GetName(typeof(BarRenderTypes), RenderType));

            XmlHelper.WriteBoolAttribute(writer, "RenderGrid", RenderGrid);
            XmlHelper.WriteBoolAttribute(writer, "UseInvert", UseInvert);

            writer.WriteAttributeString("View",         View.ToString());
            writer.WriteAttributeString("Strength",     Strength.ToString());
            writer.WriteAttributeString("Height",       Height.ToString());
            writer.WriteAttributeString("Opacity",      Opacity.ToString());
            writer.WriteAttributeString("Width",        Width.ToString());
            writer.WriteAttributeString("Dash",         Dash.ToString());
            writer.WriteAttributeString("View",         View.ToString());
            writer.WriteAttributeString("Top",          Top.ToString());
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            try
            {
                GridTextHorizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), reader.GetAttribute("GridTextHorizontalAlignement"));
                RenderType                  = (BarRenderTypes)Enum.Parse(typeof(BarRenderTypes), reader.GetAttribute("RenderType"));


            }
            catch (Exception)
            {
            }

        }


        #endregion

        Queue<float> q = new Queue<float>();
        int q_max = 0;
        int sampleRate = 0;

        object brushLock = new object();

        public OsiloscopeVisualization()
        {
            RenderGrid = false;

            GridProvider = new AudioGridProvider();

            GridProvider.GridTextHorizontalAlignment = HorizontalAlignment.Left;

            Loaded += OsiloscopeVisualization_Loaded;
        }

        private void OsiloscopeVisualization_Loaded(object sender, EventArgs e)
        {
            UpdateDirectResource();

            GridProvider.InitRender(Presenter);
        }

        public override void Init(DSPMaster master, int lentacy, double framems)
        {
            sampleRate = master.SampleRate;

            q_max = (int)((View / 1000) * sampleRate );
            Queue<float> q = new Queue<float>();
        }

        double[] rects;
        List<double> bufRect = new List<double>();
        public override void Render(DirectCanvas.DrawingLayer dc, VisualizerParent vp, float[] frameBuffer)
        {
            double[] rt = null;
            double pointWidth = _dash + _width;

            Brush FillBrush = this.FillBrush;

            if (frameBuffer != null)
            {
                int ch = 0;
                float left = 0;
                foreach (float f in frameBuffer)
                {
                    //LEFT
                    if (ch % 2 == 0)
                    {
                        left = f;
                    }
                    else
                    {
                        q.Enqueue((f + left) * 0.5f);

                        if (q.Count > q_max)
                        {
                            q.Dequeue();
                        }
                    }
                    ch++;
                }

                int barcount = (int)Math.Ceiling((vp.ActualWidth / 2) / pointWidth) + 1;
                if (Math.Round(pointWidth) != 0)
                {
                    float[] q_buf = q.ToArray();
                    for (int i = barcount - 1; i >= 0; i--)
                    {
                        double f;
                        if (UseInvert)
                        {
                            f = Math.Abs(q_buf[Math.Min(q_buf.Length - 1, (q_buf.Length - 1) * (barcount - 1 - i) / (barcount - 1))]) * (vp.ActualHeight * Height);
                        }
                        else
                        {
                            f = Math.Abs(q_buf[Math.Min(q_buf.Length - 1, (q_buf.Length - 1) * i / (barcount - 1))]) * (vp.ActualHeight * Height);
                        }

                        if (bufRect.Count - 1 < barcount - i - 1)
                        {
                            bufRect.Add(f);
                        }
                        else
                        {
                            if (Math.Abs(f) > Math.Abs(bufRect[barcount - i - 1]) || Strength >= 1)
                            {
                                bufRect[barcount - i - 1] = f;
                            }
                            else
                            {
                                bufRect[barcount - i - 1] = f * Strength + bufRect[barcount - i - 1] * (1 - Strength);
                            }
                        }
                    }

                    rt = bufRect.ToArray();
                }
            }

            if (_opacity != 0)
            {
                if (rt != null)
                {
                    rects = rt;
                }
                else if (rects != null)
                {
                    rects = nullRect(rects);
                }
            }

            if (rects != null && _opacity != 0 && pointWidth != 0 && Presenter != null && rects.Length < 5000)
            {
                double parentWidth = vp.ActualWidth;
                double x_origin = Math.Round(vp.ActualWidth / 2);
                double y_origin = Math.Round(vp.ActualHeight * Top);

                int count = Math.Min(rects.Length, (int)Math.Ceiling(x_origin / pointWidth + 1));
                int startX = (int)x_origin - (int)pointWidth * count;

                if (count > 0)
                {
                    lock (brushLock)
                    {
                        switch (RenderType)
                        {
                            case BarRenderTypes.Dots:
                                for (int i = 0; x_origin + pointWidth * i < parentWidth + _width && i < rects.Length; i++)
                                {
                                    double height = rects[i];
                                    double radius = _width / 2;
                                    dc.DrawEllipse(directBrush, null, new Ellipse(new Point(x_origin + (i * pointWidth), y_origin - height), radius, radius));
                                    dc.DrawEllipse(directBrush, null, new Ellipse(new Point(x_origin - ((i + 1) * pointWidth), y_origin - height), radius, radius));
                                }
                                break;
                            case BarRenderTypes.Filled:
                                using (DirectCanvas.Shapes.PathGeometry geo = Presenter.Factory.CreatePathGeometry())
                                {
                                    geo.BeginModify(FillMode.Alternate);
                                    geo.BeginFigure(FigureBegin.Filled, new DirectCanvas.Misc.PointF(startX, y_origin));

                                    for (int i = count - 1; i >= 0; i--)
                                    {
                                        double height = rects[i];
                                        geo.AddLine(new DirectCanvas.Misc.PointF(x_origin - (i * pointWidth), y_origin - height));
                                    }

                                    for (int i = 0; i < count; i++)
                                    {
                                        double height = rects[i];
                                        geo.AddLine(new DirectCanvas.Misc.PointF(x_origin + (i * pointWidth), y_origin - height));
                                    }
                                    geo.AddLine(new DirectCanvas.Misc.PointF(parentWidth, y_origin));

                                    geo.EndFigure(FigureEnd.Closed);
                                    geo.EndModify();

                                    dc.FillGeometry(directBrush, geo);
                                }
                                break;
                            case BarRenderTypes.Line:
                                using (DirectCanvas.Shapes.PathGeometry geo_ = Presenter.Factory.CreatePathGeometry())
                                {
                                    geo_.BeginModify(FillMode.Winding);
                                    bool started = false;

                                    for (int i = count - 1; i >= 0; i--)
                                    {
                                        double height = rects[i];
                                        DirectCanvas.Misc.PointF pt = new DirectCanvas.Misc.PointF(x_origin - (i * pointWidth), y_origin - height);
                                        if (started)
                                        {
                                            geo_.AddLine(pt);
                                        }
                                        else
                                        {
                                            pt.X = 0;
                                            geo_.BeginFigure(FigureBegin.Hollow, pt);
                                            started = true;
                                        }
                                    }

                                    for (int i = 0; i < count; i++)
                                    {
                                        double height = rects[i];
                                        DirectCanvas.Misc.PointF pt = new DirectCanvas.Misc.PointF(x_origin + (i * pointWidth), y_origin - height);
                                        if (count == i + 1)
                                            pt.X = (float)parentWidth;
                                        geo_.AddLine(pt);
                                    }

                                    geo_.EndFigure(FigureEnd.Open);
                                    geo_.EndModify();

                                    dc.DrawGeometry(directBrush, geo_, (float)_width);
                                }
                                break;
                            case BarRenderTypes.Rectangle:
                                for (int i = 0; x_origin + pointWidth * i < vp.ActualWidth + Width && i < rects.Length; i++)
                                {
                                    double height = rects[i];
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
                    GridProvider.RenderRect = new Rect(new Point(0, y_origin - Math.Max(0, vp.ActualHeight * _height)), new Size(vp.ActualWidth, Math.Abs(vp.ActualHeight * _height)));

                    if (Height < 0)
                    {
                        GridProvider.MaxV = 0;
                        GridProvider.MinV = 1;
                    }
                    else
                    {
                        GridProvider.MaxV = 1;
                        GridProvider.MinV = 0;
                    }

                    GridProvider.GridRender(dc, vp);
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
            if (Presenter == null)
                return;

            if(_fillBrush != null)
            {
                lock (brushLock)
                {
                    if (directBrush != null)
                    {
                        directBrush.Dispose();
                        directBrush = null;
                    }

                    directBrush = DirectCanvas.Misc.Converter.ToBrush(Presenter.Factory, _fillBrush);
                    directBrush.Opacity = (float)(_fillBrush.Opacity * Opacity);

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
