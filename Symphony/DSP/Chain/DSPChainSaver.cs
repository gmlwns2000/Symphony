using Symphony.Player.DSP;
using Symphony.Player;
using Symphony.Server;
using Symphony.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Symphony.DSP
{
    public static class DSPChainSaver
    {
        public static void Save(string xmlPath, List<DSPBase> DSPs)
        {
            using(XmlWriter writer = XmlWriter.Create(xmlPath, new XmlWriterSettings { OmitXmlDeclaration = false, Indent = true, IndentChars = "\t" }))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("DSP");

                XmlHelper.WriteString(writer, "Version", "1");

                //write order
                writer.WriteStartElement("DSPs");
                for (int i=0; i<DSPs.Count; i++)
                {
                    DSPBase dsp = DSPs[i];
                    if (dsp is Equalizer)
                    {
                        SaveEQ(writer, (Equalizer) dsp);
                    }
                    else if (dsp is Echo)
                    {
                        SaveEcho(writer, (Echo)dsp);
                    }
                    else if (dsp is Limitter)
                    {
                        SaveLimiter(writer, (Limitter)dsp);
                    }
                    else if (dsp is DspWrapper)
                    {
                        SaveDspWrapper(writer, (DspWrapper)dsp);
                    }
                    else if(dsp is StereoEnhancer)
                    {
                        SaveStereoEnhancer(writer, (StereoEnhancer)dsp);
                    }
                    else
                    {
                        Logger.Error("Unknown DSP");
                    }
                }
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.WriteEndDocument();

                writer.Close();
            }

            return;
        }

        private static void SaveStereoEnhancer(XmlWriter wr, StereoEnhancer en)
        {
            wr.WriteStartElement("StereoEnhancer");

            XmlHelper.WriteBoolAttribute(wr, "Use", en.on);
            wr.WriteAttributeString("Factor", en.Factor.ToString("0.000"));
            wr.WriteAttributeString("Opacity", en.opacity.ToString("0.000"));
            wr.WriteAttributeString("PreAmp", en.PreAmp.ToString("0.000"));

            wr.WriteEndElement();
        }

        private static void SaveDspWrapper(XmlWriter wr, DspWrapper wrap)
        {
            wr.WriteStartElement("DspWrapper");

            XmlHelper.WriteBoolAttribute(wr, "Use", wrap.on);
            wr.WriteAttributeString("FileName", wrap.LuaFileName);

            wr.WriteEndElement();
        }

        private static void SaveEQ(XmlWriter wr, Equalizer eq)
        {
            wr.WriteStartElement("EQ");

            XmlHelper.WriteBoolAttribute(wr, "Use", eq.on);
            wr.WriteAttributeString("Opacity", eq.opacity.ToString("0.000"));
            wr.WriteAttributeString("PreAmp", eq.preAmp.ToString("0.000"));

            foreach (EqBand band in eq.bands)
            {
                wr.WriteStartElement("Band");

                wr.WriteAttributeString("Bandwidth", band.Bandwidth.ToString("0.000"));
                wr.WriteAttributeString("Frequency", band.Frequency.ToString("0.0"));
                wr.WriteAttributeString("Gain", band.Gain.ToString("0.000"));

                wr.WriteEndElement();
            }

            wr.WriteEndElement();
        }

        private static void SaveEcho(XmlWriter wr, Echo echo)
        {
            wr.WriteStartElement("Echo");

            XmlHelper.WriteBoolAttribute(wr, "Use", echo.on);
            wr.WriteAttributeString("Factor", echo.EchoFactor.ToString());
            wr.WriteAttributeString("Length", echo.EchoLength.ToString());

            wr.WriteEndElement();
        }

        private static void SaveLimiter(XmlWriter wr, Limitter limiter)
        {
            wr.WriteStartElement("Limiter");

            XmlHelper.WriteBoolAttribute(wr, "Use", limiter.on);
            wr.WriteAttributeString("Limit", limiter.limit.ToString());
            wr.WriteAttributeString("Strength", limiter.strength.ToString());

            wr.WriteEndElement();
        }
    }
}
