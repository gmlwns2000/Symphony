using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Lyrics
{
    public class IContentUpdatedArgs : EventArgs
    {
        public IContent Content { get; set; }

        public IContentUpdatedArgs(IContent content)
        {
            Content = content;
        }
    }
}
