using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.DancerLite
{
    public class PlLoaderV1
    {
        public bool Load(XmlReader reader, PlotLite pl)
        {
            while (reader.Read())
            {
                if(reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "PMXPath":
                            pl.PMXPath = reader.ReadElementContentAsString();
                            break;
                        case "VMDPath":
                            pl.VMDPath = reader.ReadElementContentAsString();
                            break;
                    }
                }
                else if(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Data")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
