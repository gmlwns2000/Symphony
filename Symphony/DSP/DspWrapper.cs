using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;
using System.Windows.Forms;
using System.Diagnostics;
using NLua;
using Symphony.Util;
using Symphony.Player.DSP;

namespace Symphony.DSP
{
    public class DspWrapper : DSPBase, IDisposable
    {
        bool inited = false;
        
        Lua lua;
        LuaFunction LuaInit;
        LuaFunction LuaApply;
        LuaFunction LuaArray;

        DspAPI api;
        DSPCalcPoint dcp;

        public bool OnDsp;
        public bool AfterDsp;
        public string ApiVersion;
        public string Version;
        public string ReleaseDate;
        public string LuaFileName;
        public string script = "";
        private string _name = "";
        public new string Name
        {
            get
            {
                if (_name != null && _name != "")
                {
                    return _name;
                }
                else
                {
                    return LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_EmptyPlugin");
                }
            }
            set
            {
                _name = value;
            }
        }

        public string DisplayText
        {
            get
            {
                string text = "";

                if (!TextTool.StringEmpty(Name))
                {
                    text += string.Format("{0}: {1}", LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_PluginName"), Name);
                }

                if (!TextTool.StringEmpty(Describe))
                {
                    text += string.Format("\n{0}: {1}", LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_Describe"), Describe);
                }

                if(!TextTool.StringEmpty(Author))
                {
                    text += string.Format("\n{0}: {1}", LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_Author"), Author);
                }

                if (!TextTool.StringEmpty(ReleaseDate))
                {
                    text += string.Format("\n{0}: {1}", LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_ReleaseDate"), ReleaseDate);
                }

                if (!TextTool.StringEmpty(Version))
                {
                    text += string.Format("\n{0}: {1}", LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_Version"), Version);
                }

                if (!TextTool.StringEmpty(ApiVersion))
                {
                    text += string.Format("\n{0}: {1}", LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_ApiVersion"), ApiVersion);
                }

                text += string.Format("\n{0}: ", LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_AvailableCheck"));
                if (inited && DspHelper.IsAvailableApiVersion(ApiVersion))
                {
                    text += LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_AvailableCheck_Okay");
                }
                else
                {
                    text += LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_Display_AvailableCheck_Disabled");
                }

                return text.Trim('\n').Trim();
            }
        }

        public DspWrapper(string Name)
        {
            LuaFileName = Name;
            Logger.Log(this, "Create New DSP wrap " + LuaFileName);

            Load();
        }

        public void Load()
        {
            inited = false;

            try
            {
                Logger.Log(this, "Load Start");
                DspLog.Log(this, "Load Start");

                //disposing
                Dispose();

                //new loading
                api = new DspAPI(LuaFileName);

                string sr = DspHelper.Search(LuaFileName);
                if(sr != null)
                {
                    StreamReader reader = new StreamReader(sr);

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.ToUpper().StartsWith("--#ON_DSP="))
                        {
                            string result = line.Split('=')[1].ToLower();
                            if (result != null)
                            {
                                if (result == "true")
                                {
                                    OnDsp = true;
                                }
                                else
                                {
                                    OnDsp = false;
                                }
                            }
                        }
                        else if (line.ToUpper().StartsWith("--#AFTER_DSP="))
                        {
                            string result = line.Split('=')[1].ToLower();
                            if (result != null)
                            {
                                if (result == "true")
                                {
                                    AfterDsp = true;
                                }
                                else
                                {
                                    AfterDsp = false;
                                }
                            }
                        }
                        else if (line.ToUpper().StartsWith("--#TITLE="))
                        {
                            Name = line.Split('=')[1];
                        }
                        else if (line.ToUpper().StartsWith("--#AUTHOR="))
                        {
                            Author = line.Split('=')[1];
                        }
                        else if (line.ToUpper().StartsWith("--#DESCRIBE="))
                        {
                            Describe = line.Split('=')[1];
                        }
                        else if (line.ToUpper().StartsWith("--#API_VERSION="))
                        {
                            ApiVersion = line.Split('=')[1];
                        }
                        else if (line.ToUpper().StartsWith("--#RELEASE_DATE="))
                        {
                            ReleaseDate = line.Split('=')[1];
                        }
                        else if (line.ToUpper().StartsWith("--#VERSION="))
                        {
                            Version = line.Split('=')[1];
                        }

                        script += "\n" + line;
                    }

                    //make functions

                    lua = new Lua();
                    lua.LoadCLRPackage();
                    lua.DebugHook += Lua_DebugHook;
                    lua.HookException += Lua_HookException;
                    lua["api"] = api;

                    Logger.Log(this, "Lua Load File");

                    lua.DoString(script);

                    LuaInit = lua["Init"] as LuaFunction;

                    if (OnDsp)
                    {
                        LuaApply = lua["Apply"] as LuaFunction;
                        Logger.Log(this, "Lua Apply Function Defined");
                    }

                    if (AfterDsp)
                    {
                        LuaArray = lua["ArrayApply"] as LuaFunction;
                        Logger.Log(this, "Lua Array Function Defined");
                    }

                    if (!OnDsp && !AfterDsp)
                    {
                        dcp = DSPCalcPoint.None;
                    }
                    else if(OnDsp && AfterDsp)
                    {
                        dcp = DSPCalcPoint.Everytime;
                    }
                    else if(OnDsp && !AfterDsp)
                    {
                        dcp = DSPCalcPoint.OnDSP;
                    }
                    else if(!OnDsp && AfterDsp)
                    {
                        dcp = DSPCalcPoint.AfterDSP;
                    }

                    if (!DspHelper.IsAvailableApiVersion(ApiVersion))
                    {
                        inited = false;
                    }

                    Logger.Log(this, "Load Finished");
                    DspLog.Log(this, "Load Finished");
                    inited = true;
                }
                else
                {
                    inited = false;
                    return;
                }
            }
            catch(Exception e)
            {
                inited = false;
                OnDsp = false;
                AfterDsp = false;

                Logger.Error(this, e);
                DspLog.Error(this, e.Message);

                dcp = DSPCalcPoint.None;
                if (lua != null)
                {
                    lua.Close();
                    lua.Dispose();
                    lua = null;
                }

                if(LuaApply != null)
                {
                    LuaApply.Dispose();
                    LuaApply = null;
                }

                if(LuaArray != null)
                {
                    LuaArray.Dispose();
                    LuaArray = null;
                }

                if(LuaInit != null)
                {
                    LuaInit.Dispose();
                    LuaInit = null;
                }
            }
        }

        private void Lua_HookException(object sender, NLua.Event.HookExceptionEventArgs e)
        {
            if (e != null && e.Exception != null)
            {
                DspLog.Error(this, e.Exception.ToString());
                Logger.Error(this, e.Exception);
            }
        }

        private void Lua_DebugHook(object sender, NLua.Event.DebugHookEventArgs e)
        {
            if(e!=null)
            {
                string log = string.Format("==== Info =====\nEvent Number: {0}, Current Line: {1}, Name: {2}({3})\nCode: {4}",
                    e.LuaDebug.eventCode,
                    e.LuaDebug.currentline,
                    e.LuaDebug.name, e.LuaDebug.namewhat,
                    e.LuaDebug.shortsrc);
                Logger.Log(this, log);
                DspLog.Log(this, log);
            }
        }

        public override void Init(DSPMaster master)
        {
            DSPmaster = master;

            Logger.Log(this, "Begin Init");
            DspLog.Log(this, "Begin Init");

            if (master == null)
                return;

            if (!inited)
            {
                Load();
            }

            try
            {
                if (inited && LuaInit != null)
                {
                    LuaInit.Call(master.Channel, master.SampleRate);
                }
            }
            catch(Exception ex)
            {
                Logger.Error(this, ex);
                DspLog.Error(this, ex.Message);
            }

        }

        public override void SetStatus(bool on)
        {
            this.on = on;
        }

        public override void SetOpacity(float opacity)
        {
            this.opacity = opacity;
        }

        public override DSPCalcPoint GetCalcPoint()
        {
            return dcp;
        }

        public override float[] ArrayApply(float[] buffer, int offset, int count)
        {
            if (on && AfterDsp && inited && LuaArray != null)
            {
                object obj = LuaArray.Call(buffer, offset, count);

                //float[] buf = ((float[]).First());

                return buffer;
            }
            else
            {
                return buffer;
            }
        }

        public override float Apply(int channel, float sample, int index, int count)
        {
            if (on && OnDsp && inited && LuaApply != null)
            {
                return (float)((double)LuaApply.Call(channel, sample, index, count).First());
            }
            else
            {
                return sample;
            }
        }

        public void Dispose()
        {
            if (lua != null)
            {
                lua.Close();
                lua.Dispose();
                lua = null;
            }

            if (LuaApply != null)
            {
                LuaApply.Dispose();
                LuaApply = null;
            }

            if (LuaArray != null)
            {
                LuaArray.Dispose();
                LuaArray = null;
            }

            if (LuaInit != null)
            {
                LuaInit.Dispose();
                LuaInit = null;
            }
        }
    }
}
