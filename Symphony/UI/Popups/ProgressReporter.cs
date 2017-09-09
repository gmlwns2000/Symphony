using Symphony.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.UI
{
    public abstract class ProgressReporter
    {
        public event EventHandler<ProgressUpdatedArgs> ProgressUpdated;
        public event EventHandler<ProgressStopArgs> ProgressStopped;
        
        public abstract void Stop();
        public abstract void Start();

        public void UpdateMessage(double value, double max, string msg)
        {
            ProgressUpdated?.Invoke(this, new ProgressUpdatedArgs(value, max, msg));
        }

        public void StoppedMessage(double value, double max, string msg)
        {
            ProgressStopped?.Invoke(this, new ProgressStopArgs(value, max, msg));
        }
    }
}
