using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPlayer
{
    public struct PlaylistLoaderArgs
    {
        public List<string> list;
        public int id;
        public int cores;

        public PlaylistLoaderArgs(List<string> list, int id, int cores)
        {
            this.list = list;
            this.id = id;
            this.cores = cores;
        }
    }
}
