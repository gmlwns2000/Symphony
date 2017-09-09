using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Server
{
    public class ProgressStopArgs : EventArgs
    {
        public string Status;
        public double Value;
        public double Maximum;

        public ProgressStopArgs(double value, double maximum, string status)
        {
            Status = status;
            Value = value;
            Maximum = maximum;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}[{2}] - Stopped: {3}", Value, Maximum, Convert.ToInt32(Value/Maximum*100).ToString(), Status);
        }
    }
}
