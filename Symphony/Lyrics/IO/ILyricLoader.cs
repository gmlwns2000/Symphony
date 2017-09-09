using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.Lyrics
{
    interface ILyricLoader
    {
        Lyric Do(XmlReader reader);
    }
}
