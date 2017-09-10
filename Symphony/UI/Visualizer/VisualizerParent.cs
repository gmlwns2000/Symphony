using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Symphony.Player;
using Symphony.Player.DSP;

namespace Symphony.UI
{
    public class VisualizerParent : FrameworkElement
    {
        public Util.Settings Settings = Util.Settings.Current;
        public List<IVisualizer> Visualizers = new List<IVisualizer>();
        public int framems;
        public bool AllowRender { get; set; }
        public bool UseMotionBlur = false;

        protected override Visual GetVisualChild(int index)
        {
            return canvas[index];
        }

        protected override int VisualChildrenCount => canvas.Count;

        protected MainWindow mw;
        protected bool inited = false;

        VisualCollection canvas;
        Queue<float> buffer;
        int sampleRate;
        int lentacy;
        int channel;
        object buffer_lock = new object();
        int buffer_thresold = 0;
        int buffer_frame = 0;
        int buffer_lentancy = 0;

        public VisualizerParent()
        {
            canvas = new VisualCollection(this);
            AllowRender = true;
        }

        public void Init(MainWindow mw)
        {
            this.mw = mw;
            inited = true;
        }

        public void InitSample(int lentacy, int framems, DSPMaster master)
        {
            sampleRate = master.SampleRate;
            this.lentacy = lentacy;
            framems = Settings.GUIUpdate;
            channel = master.Channel;

            // sampleRate * channel * 3 frame
            buffer_lentancy = (int)(((double)lentacy / 1000) * master.Channel * sampleRate);
            buffer_frame = (int)(((double)framems / 1000) * master.Channel * sampleRate * 1.33);
            buffer_thresold = Math.Max(buffer_lentancy + buffer_frame, buffer_frame * 3);
            buffer = new Queue<float>();

            foreach (IVisualizer v in Visualizers)
            {
                v.Init(master, lentacy, framems);
            }
        }

        public void Sample(object sender, SampleEndEventArgs e)
        {
            lock (buffer_lock)
            {
                for (int i = 0; i < e.Count; i++)
                {
                    if (buffer.Count == buffer_thresold)
                    {
                        buffer.Dequeue();
                    }
                    buffer.Enqueue(e.Buffer[i]);
                }
            }
        }

        public float[] GetFrameBuffer()
        {
            float[] buf;

            lock (buffer_lock)
            {
                if (buffer != null && buffer.Count > 0)
                {
                    if (buffer.Count >= buffer_thresold)
                    {
                        buf = buffer.ToArray();
                        buffer.Clear();
                    }
                    else
                    {
                        buf = new float[Math.Min(buffer.Count, buffer_frame)];
                        for (int i = 0; i < buf.Length; i++)
                        {
                            buf[i] = buffer.Dequeue();
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            return buf;
        }

        public void Update()
        {
            framems = Settings.GUIUpdate;
            if (inited)
            {
                DrawingVisual visual = new DrawingVisual();
                if (UseMotionBlur)
                    visual.CacheMode = new BitmapCache();

                using (DrawingContext dc = visual.RenderOpen())
                {
                    float[] buf = GetFrameBuffer();

                    dc.Close();
                }

                PushVisual(visual);
            }
        }

        public void PushVisual(Visual visual)
        {
            if (UseMotionBlur)
            {
                foreach (DrawingVisual v in canvas)
                {
                    v.Opacity = v.Opacity - 0.25;
                }
                if (canvas.Count == 4)
                {
                    canvas.RemoveAt(0);
                }
            }
            else
            {
                canvas.Clear();
            }
            canvas.Add(visual);
        }
    }
}
