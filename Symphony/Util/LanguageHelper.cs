using Symphony.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace Symphony.Util
{
    public class LanguagePack
    {
        public string Title { get; set; }
        public string Tooltip { get; set; }
        public string FileName { get; set; }

        public bool inited = false;

        public LanguagePack(FileInfo fi)
        {
            if (!fi.Exists && !fi.FullName.ToLower().EndsWith(".xaml"))
            {
                return;
            }

            try
            {
                ParserContext pc = new ParserContext();
                pc.BaseUri = new Uri(fi.FullName);

                using (Stream s = File.Open(fi.FullName, FileMode.Open, FileAccess.Read))
                {
                    ResourceDictionary dic = XamlReader.Load(s, pc) as ResourceDictionary;
                    dic.BeginInit();
                    dic.EndInit();

                    FileName = fi.Name;
                    Title = (string)dic["Lang_Language_Title"];
                    Tooltip = (string)dic["Lang_Language_Tooltip"];

                    Logger.Log(Title + " / " + Tooltip + " / " + FileName);

                    s.Close();
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(new Action(()=> 
                {
                    string prefix = "";
                    if(ex is XamlParseException)
                    {
                        XamlParseException xe = (XamlParseException)ex;
                        prefix = fi.Name + " 줄: " + xe.LineNumber.ToString() + " 위치: " + xe.LinePosition.ToString() ;
                    }
                    else
                    {
                        prefix = fi.Name;
                    }

                    DialogMessage.Show(null, prefix + "\n언어팩을 읽는중에 오류가 발생했습니다.", "에러", DialogMessageType.Okay);
                }));

                Logger.Error(this, ex);

                return;
            }

            inited = true;
        }
    }

    public static class LanguageHelper
    {
        public static CultureInfo DefaultOSLanguage = CultureInfo.CurrentCulture;

        public static string DefaultLanguagePath = DefaultOSLanguage.EnglishName == "English"
            ? "English.xaml"
            : "Korean.xaml";

        public static event EventHandler LangaugeChanged;

        public static string LanguageTitle = "";
        public static string LanguageTooltip = "";
        public static string LanguageFileName = "Korean.xaml";

        public static string _libraryDirectory;
        public static string LibraryDirectory
        {
            get
            {
                if(_libraryDirectory == null)
                {
                    _libraryDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Language");
                }

                return _libraryDirectory;
            }
        }

        public static ResourceDictionary Dictionary;

        public static string FindText(string Key)
        {
            if (Dictionary == null)
            {
                Logger.Error("Language Pack is not loaded: " + Key);
                return Key;
            }
            else
            {
                if (Dictionary.Contains(Key))
                {
                    return (string)Dictionary[Key];
                }
                else
                {
                    Logger.Error("Language Value Not Founded. : " + Key);
                    return Key;
                }
            }
        }

        private static void AdjustResources(ref ResourceDictionary dic)
        {
            string[] keys = new string[dic.Count];
            dic.Keys.CopyTo(keys, 0);

            foreach (string key in keys)
            {
                object obj = dic[key];
                if(obj is string)
                {
                    string str = (string)obj;
                    str = str.Replace("\\n", "\n");
                    dic[key] = str;
                }
            }
        }

        private static void EndLoad()
        {
            Player.PlayerCore.Lang_FileOpenError = FindText("Lang_Player_FileOpenError");
            Player.PlayerCore.Lang_UnSupportedFormat = FindText("Lang_Player_UnSupportedFormat");
            Player.PlayerCore.Lang_AACFiles = FindText("Lang_AACFile");
            Player.PlayerCore.Lang_AC3Files = FindText("Lang_AC3File");
            Player.PlayerCore.Lang_AIFFFiles = FindText("Lang_AiffFile");
            Player.PlayerCore.Lang_AllFiles = FindText("Lang_AllFileFormat");
            Player.PlayerCore.Lang_DDPFiles = FindText("Lang_DDPFile");
            Player.PlayerCore.Lang_FLACFiles = FindText("Lang_FlacFile");
            Player.PlayerCore.Lang_M4AFiles = FindText("Lang_M4AFile");
            Player.PlayerCore.Lang_MP3Files = FindText("Lang_MP3File");
            Player.PlayerCore.Lang_SupportFiles = FindText("Lang_SupportMusicFormat");
            Player.PlayerCore.Lang_WAVFiles = FindText("Lang_WaveFile");
        }

        public static void LoadDefaultPack()
        {
            ResourceDictionary dic = new ResourceDictionary();
            dic.BeginInit();
            dic.Source = new Uri("/Symphony;component/Language/Korean.xaml", UriKind.RelativeOrAbsolute);
            dic.EndInit();

            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.NewLineChars = "\n";
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                DirectoryInfo di = new DirectoryInfo(LibraryDirectory);
                if (!di.Exists)
                {
                    di.Create();
                }

                string skinFile = Path.Combine(di.FullName, "Korean.xaml");

                using (XmlWriter writer = XmlWriter.Create(skinFile, settings))
                {
                    XamlDesignerSerializationManager designer = new XamlDesignerSerializationManager(writer);
                    designer.XamlWriterMode = XamlWriterMode.Expression;

                    XamlWriter.Save(dic, writer);

                    writer.Close();
                }
            }
            catch
            {
                return;
            }

            AdjustResources(ref dic);

            Application.Current.Resources.MergedDictionaries[1] = dic;
            Dictionary = dic;

            LanguageFileName = "Korean.xaml";
            LanguageTitle = (string)dic["Lang_Language_Title"];
            LanguageTooltip = (string)dic["Lang_Language_Tooltip"];

            EndLoad();

            Logger.Log(LanguageTitle + " / " + LanguageTooltip + " / " + LanguageFileName);

            LangaugeChanged?.Invoke(null, null);
        }

        public static void LoadLanguagePack(FileInfo fi)
        {
            if (!fi.Exists)
            {
                return;
            }

            ParserContext pc = new ParserContext();
            pc.BaseUri = new Uri(fi.FullName);

            using (Stream s = File.Open(fi.FullName, FileMode.Open, FileAccess.Read))
            {
                ResourceDictionary dic = XamlReader.Load(s, pc) as ResourceDictionary;
                dic.BeginInit();
                dic.EndInit();

                AdjustResources(ref dic);

                LanguageFileName = fi.Name;
                LanguageTitle = (string)dic["Lang_Language_Title"];
                LanguageTooltip = (string)dic["Lang_Language_Tooltip"];

                Logger.Log(LanguageTitle + " / " + LanguageTooltip + " / " + LanguageFileName);

                Application.Current.Resources.MergedDictionaries[1] = dic;
                Dictionary = dic;

                s.Close();
            }

            EndLoad();

            LangaugeChanged?.Invoke(null, null);
        }
    }
}
