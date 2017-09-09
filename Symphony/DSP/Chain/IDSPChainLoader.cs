using Symphony.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.DSP
{
    public interface IDSPChainLoader
    {
        List<Symphony.Player.DSP.DSPBase> Load(XmlReader reader);
    }
}
