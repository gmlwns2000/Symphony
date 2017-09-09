using Symphony.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.UI.Settings
{
    public static class EffectNames
    {
        public static string Equealizer = "eq";
        public static string Limiter = "limiter";
        public static string Echo = "echo";
        public static string LuaDsp = "lua";
        public static string StereoEnhancer = "stereo";
        public static string Unknown = "unknown";

        public static LvDspItem DSPWrapperItem(DSP.DspWrapper wrap)
        {
            return new LvDspItem(LuaDsp, wrap.Name, wrap.DisplayText);
        }

        public static LvDspItem EquealizerItem
        {
            get
            {
                return new LvDspItem(Equealizer, LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_EQ_Name"), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_EQ_Tooltip"));
            }
        }

        public static LvDspItem EchoItem
        {
            get
            {
                return new LvDspItem(Echo, LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_Echo_Name"), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_Echo_Tooltip"));
            }
        }

        public static LvDspItem LimiterItem
        {
            get
            {
                return new LvDspItem(Limiter, LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_Limiter_Name"), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_Limiter_Tooltip"));
            }
        }

        public static LvDspItem StereoEnhancerItem
        {
            get
            {
                return new LvDspItem(StereoEnhancer, LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_StereoEnhancer_Name"), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_StereoEnhancer_Tooltip"));
            }
        }

        public static LvDspItem LuaDspItem
        {
            get
            {
                return new LvDspItem(LuaDsp, LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Name"), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Tooltip"));
            }
        }
    }

    public class LvDspItem
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Describe { get; set; }
        public UserControlHostWindow Editor;

        public LvDspItem(string Name, string DisplayName, string Describe)
        {
            this.Name = Name;
            this.DisplayName = DisplayName;
            this.Describe = Describe;
        }

        public void Deleting()
        {
            if (Editor == null)
            {
                return;
            }

            ((IDspEditor)Editor.Control).Deleted();
            Editor.Close();
            Editor = null;
        }
    }
}
