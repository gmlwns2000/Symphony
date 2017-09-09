using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Symphony.Player;
using Symphony.Util;
using Symphony.Player.DSP;

namespace Symphony.DSP
{
    public class DSPChainLoaderV1 : IDSPChainLoader
    {
        public const string EqName = "EQ";
        public const string LimiterName = "Limiter";
        public const string EchoName = "Echo";
        public const string DspWrapperName = "DspWrapper";
        public const string StereoEnhancerName = "StereoEnhancer";

        PlayerCore np;
        public DSPChainLoaderV1(ref PlayerCore np)
        {
            this.np = np;
        }

        public List<DSPBase> Load(XmlReader reader)
        {
            List<DSPBase> DSPs = new List<DSPBase>();
            bool success = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if(reader.Name == "DSP")
                        {
                            success = true;
                        }
                        break;
                    case XmlNodeType.Element:
                        string name = reader.Name;
                        switch (name)
                        {
                            case EqName:
                                Equalizer eq = ReadEQ(reader);
                                if(eq == null)
                                {
                                    return null;
                                }
                                else
                                {
                                    DSPs.Add(eq);
                                }
                                break;
                            case LimiterName:
                                Limitter limter = ReadLimiter(reader);
                                if(limter == null)
                                {
                                    return null;
                                }
                                else
                                {
                                    DSPs.Add(limter);
                                }
                                break;
                            case EchoName:
                                Echo echo = ReadEcho(reader);
                                if(echo == null)
                                {
                                    return null;
                                }
                                else
                                {
                                    DSPs.Add(echo);
                                }
                                break;
                            case DspWrapperName:
                                DspWrapper wrap = ReadDspWrapper(reader);
                                if(wrap == null)
                                {
                                    return null;
                                }
                                else
                                {
                                    DSPs.Add(wrap);
                                }
                                break;
                            case StereoEnhancerName:
                                StereoEnhancer en = ReadStereoEnhancer(reader);
                                DSPs.Add(en);
                                break;
                            default:
                                Logger.Log("Unknown DSP!");
                                break;
                        }
                        break;
                }
            }

            if(DSPs.Count <= 0 || !success)
            {
                return null;
            }

            return DSPs;
        }

        public StereoEnhancer ReadStereoEnhancer(XmlReader reader)
        {
            StereoEnhancer en = new StereoEnhancer();

            en.opacity = (float)Convert.ToDouble(reader.GetAttribute("Opacity"));
            en.PreAmp = (float)Convert.ToDouble(reader.GetAttribute("PreAmp"));
            en.Factor = (float)Convert.ToDouble(reader.GetAttribute("Factor"));
            en.on = XmlHelper.String2Bool(reader.GetAttribute("Use"));
            
            return en;
        }

        public Equalizer ReadEQ(XmlReader reader)
        {
            double opacity = Convert.ToDouble(reader.GetAttribute("Opacity"));
            double preamp = Convert.ToDouble(reader.GetAttribute("PreAmp"));
            bool on = XmlHelper.String2Bool(reader.GetAttribute("Use"));
            List<EqBand> bands = new List<EqBand>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if(reader.Name.ToLower() == "band")
                        {
                            string strBandwidth = reader.GetAttribute("Bandwidth");
                            string strGain = reader.GetAttribute("Gain");
                            string strFreq = reader.GetAttribute("Frequency");
                            bands.Add(new EqBand()
                            {
                                Bandwidth = (float)Convert.ToDouble(strBandwidth),
                                Gain = (float)Convert.ToDouble(strGain),
                                Frequency = (float)Convert.ToDouble(strFreq)
                            });
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if(reader.Name == EqName)
                        {
                            return new Equalizer(ref np, bands.ToArray(), (float)preamp, on, (float)opacity);
                        }
                        break;
                    default:
                        break;
                }
            }

            return null;
        }

        public Echo ReadEcho(XmlReader reader)
        {
            float echoFactor = (float)Convert.ToDouble(reader.GetAttribute("Factor"));
            int echoLength = Convert.ToInt32(reader.GetAttribute("Length"));
            bool echoon = XmlHelper.String2Bool(reader.GetAttribute("Use"));

            return new Echo(echoLength, echoFactor) { on = echoon };
        }

        public Limitter ReadLimiter(XmlReader reader)
        {
            float strength = (float)Convert.ToDouble(reader.GetAttribute("Strength"));
            float limit = (float)Convert.ToDouble(reader.GetAttribute("Limit"));
            bool limOn = XmlHelper.String2Bool(reader.GetAttribute("Use"));

            return new Limitter(limit, strength) { on = limOn };
        }

        public DspWrapper ReadDspWrapper(XmlReader reader)
        {
            string fileName = reader.GetAttribute("FileName");
            bool dspOn = XmlHelper.String2Bool(reader.GetAttribute("Use"));

            return new DspWrapper(fileName) { on = dspOn };
        }
    }
}
