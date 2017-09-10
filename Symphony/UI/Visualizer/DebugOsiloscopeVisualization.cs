using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Symphony.Player;
using System.Windows;
using Symphony.Player.DSP;
using System.Xml;

namespace Symphony.UI
{
    public class DebugOsiloscopeVisualization : IVisualizer
    {
        private int viewerSize = 100;
        public int ViewerSize
        {
            get
            {
                return viewerSize;
            }
            set
            {
                viewerSize = value;
                q_max = (int)(sampleRate * ((double)viewerSize / 1000));
                if (q != null)
                {
                    q.Clear();
                }
            }
        }

        int sampleRate = 0;
        int q_max = 0;
        Queue<float> q = new Queue<float>();

        DirectCanvas.Brushes.Brush brush;

        public DebugOsiloscopeVisualization()
        {
            Loaded += DebugOsiloscopeVisualization_Loaded;
        }

        private void DebugOsiloscopeVisualization_Loaded(object sender, EventArgs e)
        {
            brush = Presenter.Factory.CreateSolidColorBrush(new DirectCanvas.Color4(255, 0, 0, 0));
        }

        public override void Init(DSPMaster master, int lentacy, int framems)
        {
            sampleRate = master.SampleRate;
            q_max = (int)(sampleRate * ((double)viewerSize / 1000));
        }

        public override void Render(DirectCanvas.DrawingLayer dc, VisualizerParent vp, float[] frameBuffer)
        {
            if (!On)
                return;

            if (frameBuffer != null)
            {
                foreach (float f in frameBuffer)
                {
                    q.Enqueue(f);
                    if (q.Count > q_max)
                    {
                        q.Dequeue();
                    }
                }
            }

            using (DirectCanvas.Shapes.PathGeometry path = Presenter.Factory.CreatePathGeometry())
            {
                bool firstPoint = true;
                float[] buf = q.ToArray();

                int block = (int)Math.Max(2, q.Count / vp.ActualWidth);
                if (block % 2 == 1)
                {
                    block = Math.Max(2, block - 1);
                }

                path.BeginModify(DirectCanvas.Shapes.FillMode.Alternate);
                path.BeginFigure(DirectCanvas.Shapes.FigureBegin.Filled, new DirectCanvas.Misc.PointF(-10, -10));

                for (int i = 0; i < buf.Length; i += block)
                {
                    float smp = buf[i];

                    DirectCanvas.Misc.PointF pt = new DirectCanvas.Misc.PointF((int)(vp.ActualWidth * ((double)i / buf.Length)), (int)(50 * smp + vp.ActualHeight - 80));
                    if (firstPoint)
                    {
                        firstPoint = false;
                        path.AddLine(pt);
                    }
                    else
                    {
                        path.AddLine(pt);
                    }
                }

                path.EndFigure(DirectCanvas.Shapes.FigureEnd.Open);
                path.EndModify();

                dc.DrawGeometry(brush, path, 2);
            }
        }

        public bool On { get; set; } = false;
        public override void SetOn(bool on)
        {
            On = on;
        }

        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public override void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
