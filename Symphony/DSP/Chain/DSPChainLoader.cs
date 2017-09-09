using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;
using System.Xml;

namespace Symphony.DSP
{
    public class DSPChainLoader
    {
        public static List<Player.DSP.DSPBase> Load(string xmlFile, ref PlayerCore np)
        {
            using (XmlReader reader = XmlReader.Create(xmlFile))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if(reader.Name.ToLower() == "version")
                            {
                                string version = reader.ReadElementContentAsString();
                                switch (version)
                                {
                                    case "1":
                                        DSPChainLoaderV1 v1 = new DSPChainLoaderV1(ref np);
                                        return v1.Load(reader);
                                    default:
                                        break;
                                }
                            }
                            break;
                    }
                }

                reader.Close();
            }

            return null;
        }
    }
}
