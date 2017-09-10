using Symphony.Util;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Symphony.UI
{
    public static class VisualizerChainSerializer
    {
        public const int VersionMajor = 1;
        public const int VersionMinor = 0;

        public static bool IsCompatible(int major, int minor)
        {
            // Version compatibilty settings here
            return VersionMajor == major;
        }

        public const string RootNodeName = "Visualizer";
        public const string ChainNodeName = "Visualizers";


        public static void Save(string xmlPath, List<IVisualizer> visualizers)
        {
            using (XmlWriter writer = XmlWriter.Create(xmlPath, new XmlWriterSettings { OmitXmlDeclaration = false, Indent = true, IndentChars = "\t" }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(RootNodeName);

                XmlHelper.WriteString(writer, "Version", VersionMajor + "." + VersionMinor);

                writer.WriteStartElement(ChainNodeName);

                foreach(var element in visualizers)
                    element.WriteXml(writer);

                writer.WriteEndElement(); // ChainNodeName
                writer.WriteEndElement(); // RootNodeName

                writer.WriteEndDocument();
                writer.Close();
            }
        }
    }
}
