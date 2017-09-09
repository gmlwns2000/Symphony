using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Server
{
    public class ProgressUpdatedArgs : EventArgs
    {
        public double Value;
        public double Maximum;
        public string Status;

        public ProgressUpdatedArgs(double value, double max, string status)
        {
            Value = value;
            Maximum = max;
            Status = status;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}[{3}%] - {2}", Value, Maximum, Status, Convert.ToInt32(Value/Maximum*100).ToString());
        }
    }
}
