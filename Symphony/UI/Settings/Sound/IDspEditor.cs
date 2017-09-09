using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Symphony.UI.Settings
{
    public interface IDspEditor
    {
        void Deleted();

        event EventHandler<DspUpdatedArgs> Updated;
    }

    public class DspUpdatedArgs : EventArgs
    {
        public Symphony.Player.DSP.DSPBase DSP;

        public DspUpdatedArgs(Symphony.Player.DSP.DSPBase dsp)
        {
            DSP = dsp;
        }
    }
}
