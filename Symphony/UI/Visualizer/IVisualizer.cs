using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Symphony.UI
{
    public abstract class IVisualizer
    {
        public bool IsInited { get; private set; } = false;

        public DirectCanvas.Presenter Presenter { get; set; }

        public event EventHandler Loaded;
        
        public abstract void Render(DirectCanvas.DrawingLayer dc, VisualizerParent vp, float[] frameBuffer);

        public virtual void InitRender(DirectCanvas.Presenter presenter)
        {
            Presenter = presenter;

            IsInited = true;

            Loaded?.Invoke(this, null);
        }

        public abstract void Init(Player.DSP.DSPMaster master, int lentacy, int framems);

        public abstract void SetOn(bool on);
    }
}
