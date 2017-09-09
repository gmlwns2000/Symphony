using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Lyrics
{
    public interface IContentEditor
    {
        event EventHandler<IContentUpdatedArgs> ContentUpdated;

        void Init(IContent content);
    }
}
