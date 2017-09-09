using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectCanvas
{
    public class DrawCalledArgs
    {
        public DrawingLayer Source { get; set; }

        public DrawCalledArgs(DrawingLayer Source)
        {
            this.Source = Source;
        }
    }
}
