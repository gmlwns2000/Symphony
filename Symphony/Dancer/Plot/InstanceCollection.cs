using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Dancer
{
    public class InstanceCollection : CollectionBase, IDisposable
    {
        public void OnLoad(MMF.Controls.WPF.WPFRenderControl RenderControl, string workingDirectory)
        {
            foreach(Instance i in List)
            {
                i.OnLoad(RenderControl, workingDirectory);
            }
        }

        public void OnPlayStarted(double position)
        {
            foreach(Instance i in List)
            {
                i.OnPlayStarted(position);
            }
        }

        public void OnPauseChanged(bool IsPaused)
        {
            foreach(Instance i in List)
            {
                i.OnPauseChanged(IsPaused);
            }
        }

        public void OnSeeked(double NewPosition)
        {
            foreach (Instance i in List)
            {
                i.OnSeeked(NewPosition);
            }
        }

        public void Dispose()
        {
            foreach (Instance i in List)
            {
                i.Dispose();
            }
        }

        public void Add(Instance item)
        {
            List.Add(item);
        }

        public Instance this[int index]
        {
            get
            {
                return ((Instance)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
    }
}
