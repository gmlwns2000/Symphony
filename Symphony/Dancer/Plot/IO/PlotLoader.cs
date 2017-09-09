using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.Dancer
{
    public static class PlotLoader
    {
        /// <summary>
        /// XML플롯을 엽니다. PLOT플롯을 열기 위해서는 PlotHelper.Import()로 저정소로 불러들인 뒤 사용해 주십시오.
        /// </summary>
        /// <param name="PlotFile"></param>
        public static bool Load(string workingDirectory, out Plot plot)
        {
            string PlotFile = Path.Combine(workingDirectory, "plot.xml");
            Logger.Log(PlotFile);

            plot = new Plot();

            using (XmlReader reader = XmlReader.Create(PlotFile))
            {
                //openXml

                reader.ReadStartElement("Plot");

                reader.ReadStartElement("Version");

                plot.Version = reader.ReadContentAsString();
                Logger.Log(string.Format("plot version: {0}", plot.Version));

                //read version
                switch (plot.Version)
                {
                    case "1":
                        PlotLoaderV1 v1 = new PlotLoaderV1();
                        plot = v1.Do(reader);
                        break;
                    default:
                        return false;
                }

                reader.Close();
            }

            return true;
        }
    }
}
