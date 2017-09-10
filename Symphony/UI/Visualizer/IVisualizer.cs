using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Symphony.UI
{
    public abstract class IVisualizer : IXmlSerializable
    {
        public bool IsInited { get; private set; } = false;
        public event EventHandler Loaded;

        public DirectCanvas.Presenter Presenter { get; set; }


        public abstract void Init(Player.DSP.DSPMaster master, int lentacy, double framems);

        public abstract void Render(DirectCanvas.DrawingLayer dc, VisualizerParent vp, float[] frameBuffer);
        public abstract void SetOn(bool on);

        public virtual void InitRender(DirectCanvas.Presenter presenter)
        {
            Presenter = presenter;
            IsInited = true;

            Loaded?.Invoke(this, null);
        }

        // XML serialization
        public abstract void ReadXml(XmlReader reader);
        public abstract void WriteXml(XmlWriter writer);

        public XmlSchema GetSchema()
        {
            return null;
        }
    }
}
