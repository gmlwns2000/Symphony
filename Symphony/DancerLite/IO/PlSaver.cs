using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.DancerLite
{
    public static class PlSaver
    {
        public static void Export(PlotLite pl, string plotliteFile)
        {
            using (ZipFile zip = new ZipFile(Encoding.UTF8))
            {
                zip.AddDirectory(pl.WorkingDirectory, Path.GetFileName(pl.WorkingDirectory));

                zip.Save(plotliteFile);
            }
        }

        public static void Save(PlotLite pl)
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            setting.IndentChars = "\t";

            using(XmlWriter writer = XmlWriter.Create(Path.Combine(pl.WorkingDirectory, "pl.xml"), setting))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("PlotLite");

                Util.XmlHelper.WriteString(writer, "Version", "1");

                writer.WriteStartElement("Metadata");

                Util.XmlHelper.WriteString(writer, "Title", pl.Metadata.Title);
                Util.XmlHelper.WriteString(writer, "Album", pl.Metadata.Album);
                Util.XmlHelper.WriteString(writer, "Artist", pl.Metadata.Artist);
                Util.XmlHelper.WriteString(writer, "FileName", pl.Metadata.FileName);
                Util.XmlHelper.WriteString(writer, "Author", pl.Metadata.Author);

                writer.WriteEndElement();

                writer.WriteStartElement("Data");

                Util.XmlHelper.WriteString(writer, "PMXPath", pl.PMXPath);
                Util.XmlHelper.WriteString(writer, "VMDPath", pl.VMDPath);

                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteEndDocument();

                writer.Close();
            }
        }
    }
}
