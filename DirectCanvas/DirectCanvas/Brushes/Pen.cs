using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectCanvas.Brushes
{
    public class Pen : IDisposable
    {
        public Brush Brush { get; set; }

        public float Thickness { get; set; }

        public Pen(Brush brush, int thickness)
        {
            Brush = brush;

            Thickness = thickness;
        }

        public Pen(Brush brush, double thickness)
        {
            Brush = brush;

            Thickness = (float)thickness;
        }

        public void Dispose()
        {
            if(Brush != null)
            {
                Brush.Dispose();

                Brush = null;
            }
        }
    }
}
