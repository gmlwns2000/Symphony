using Symphony.Player.DSP.CSCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DirectBrush = DirectCanvas.Brushes.Brush;
using DirectPen = DirectCanvas.Brushes.Pen;

namespace Symphony.UI
{
    public class AudioGridProvider
    {
        public ScalingStrategy GridScaling { get; set; } = ScalingStrategy.Linear;

        public double MinDb { get; set; }
        public double MaxDb { get; set; }

        public double MinV { get; set; }
        public double MaxV { get; set; }

        public double MinSqrt { get; set; }
        public double MaxSqrt { get; set; }

        public double Interval { get; set; } = 32;

        public Rect RenderRect { get; set; }

        private DirectBrush directBrush;
        private Brush _Foreground;
        public Brush GridForeground
        {
            get
            {
                return _Foreground;
            }
            set
            {
                _Foreground = value;

                UpdateDirectResource();
            }
        }

        private DirectPen directPen;
        private Pen _stroke;
        public Pen GridStroke
        {
            get
            {
                return _stroke;
            }
            set
            {
                _stroke = value;

                UpdateDirectResource();
            }
        }

        public HorizontalAlignment GridTextHorizontalAlignment { get; set; } = HorizontalAlignment.Right;

        private DirectCanvas.Presenter Presenter;
        private FontFamily GridFontFamily;
        private Typeface GridFont;
        private object brushLock = new object();

        public AudioGridProvider()
        {
            RenderRect = new Rect();

            GridFontFamily = new FontFamily("NanumBarunGothic");

            GridFont = new Typeface(GridFontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            GridForeground = Brushes.White;

            GridStroke = new Pen(Brushes.White, 1);
        }

        public void InitRender(DirectCanvas.Presenter presenter)
        {
            Presenter = presenter;

            UpdateDirectResource();
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

                    directBrush = DirectCanvas.Misc.Converter.ToBrush(Presenter.Factory, GridForeground);

                    if (directPen != null)
                    {
                        directPen.Dispose();
                        directPen = null;
                    }

                    directPen = new DirectPen(DirectCanvas.Misc.Converter.ToBrush(Presenter.Factory, GridStroke.Brush), GridStroke.Thickness);
                }
            }
        }

        public void GridRender(DirectCanvas.DrawingLayer dc, VisualizerParent parent)
        {
            if (RenderRect.Height < Interval)
                return;
            if (directBrush == null && directPen == null)
                return;

            Rect rt = RenderRect;
            double min;
            double max;
            switch (GridScaling)
            {
                case ScalingStrategy.Decibel:
                    min = MinDb;
                    max = MaxDb;
                    break;
                case ScalingStrategy.Linear:
                    min = MinV;
                    max = MaxV;
                    break;
                case ScalingStrategy.Sqrt:
                    min = MinSqrt;
                    max = MaxSqrt;
                    break;
                default:
                    throw new NotImplementedException();
            }
            double line_interval = Interval;

            int line_count = (int)(rt.Height / line_interval);

            lock (brushLock)
            {
                for (int i = 0; i < line_count + 1; i++)
                {
                    double y = rt.Y + rt.Height / line_count * i;

                    if (y > 0 && y < parent.ActualHeight + 12)
                    {
                        double dB;

                        switch (GridScaling)
                        {
                            case ScalingStrategy.Decibel:
                                dB = DecibelGetDecibel(i, line_count, min, max);
                                break;
                            case ScalingStrategy.Linear:
                                dB = LinearGetDecibel(i, line_count, min, max);
                                break;
                            case ScalingStrategy.Sqrt:
                                dB = SqrtGetDecibel(i, line_count, min, max);
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        dc.DrawLine(directPen,
                            new DirectCanvas.Misc.PointF(Math.Round(rt.X), Math.Round(y) + directPen.Thickness * 0.5),
                            new DirectCanvas.Misc.PointF(Math.Round(rt.X + rt.Width), Math.Round(y) + directPen.Thickness * 0.5));

                        FormattedText text = new FormattedText(dB.ToString("0.00") + "dB", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, GridFont, 12, GridForeground);

                        Point text_pt;
                        switch (GridTextHorizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                text_pt = new Point(12, Math.Round(y - text.Height));
                                break;
                            case HorizontalAlignment.Center:
                            case HorizontalAlignment.Stretch:
                                text_pt = new Point(Math.Round(rt.X + rt.Width * 0.5 - text.Width * 0.5), Math.Round(y - text.Height));
                                break;
                            case HorizontalAlignment.Right:
                                text_pt = new Point(Math.Round(rt.X + rt.Width - text.Width - 12), Math.Round(y - text.Height));
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        //dc.DrawText(text, text_pt);
                    }
                }
            }
        }

        private double LinearGetDecibel(int index, int count, double min, double max)
        {
            return Util.Decibels.v2dB(max - ((max - min) / count * index));
        }

        private double SqrtGetDecibel(int index, int count, double min, double max)
        {
            return Util.Decibels.v2dB(Util.Decibels.sqrt2v(max - ((max - min) / count * index)));
        }

        private double DecibelGetDecibel(int index, int count, double min, double max)
        {
            return max - ((max - min) / count * index);
        }
    }
}
