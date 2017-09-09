using Symphony.UI.Settings;
using Symphony.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Symphony.UI
{
    public class ThemeHelper : ResourcesParent
    {
        #region static field
        
        public static string LibraryFolder
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings");
            }
        }

        public static SkinEditor SkinEditor;

        public static ResourceDictionary CreateTheme(string themeName)
        {
            ResourceDictionary myResourceDictionary;

            try
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(LibraryFolder, themeName));

                if (!di.Exists)
                {
                    di.Create();
                }

                myResourceDictionary = new ResourceDictionary();
                myResourceDictionary.BeginInit();
                myResourceDictionary.Source = new Uri("/Symphony;component/UI/Style/DefaultTheme.xaml", UriKind.RelativeOrAbsolute);
                myResourceDictionary.EndInit();

                Application.Current.Resources.MergedDictionaries[0] = myResourceDictionary;

                SaveTheme(new DirectoryInfo(Path.Combine(LibraryFolder, themeName)));
            }
            catch (Exception e)
            {
                Logger.Error("ThemeHelper", e);

                return null;
            }

            return myResourceDictionary;
        }

        public static void SaveTheme(DirectoryInfo di)
        {
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                string skinFile = Path.Combine(di.FullName, "skin.xaml");

                using (XmlWriter writer = XmlWriter.Create(skinFile, settings))
                {
                    XamlDesignerSerializationManager designer = new XamlDesignerSerializationManager(writer);
                    designer.XamlWriterMode = XamlWriterMode.Expression;

                    XamlWriter.Save(Application.Current.Resources.MergedDictionaries[0], writer);

                    writer.Close();

                    XmlDocument doc = new XmlDocument();

                    doc.Load(skinFile);

                    FixNode(doc, doc);

                    doc.Save(skinFile);
                }
            }
            catch (Exception e)
            {
                Logger.Error("ThemeHelper", e);
            }
        }

        public static ResourceDictionary LoadTheme(DirectoryInfo di, MainWindow mw)
        {
            ResourceDictionary myResourceDictionary;

            if (!di.Exists || !File.Exists(Path.Combine(di.FullName, "skin.xaml")))
            {
                myResourceDictionary = CreateTheme(mw.CurrentTheme);
            }
            else
            {
                string file = Path.Combine(di.FullName, "skin.xaml");

                try
                {
                    XmlDocument doc = new XmlDocument();

                    doc.Load(file);
                    FixNode(doc, doc);
                    doc.Save(file);

                    ParserContext pc = new ParserContext();
                    pc.BaseUri = new Uri(file);

                    using (Stream s = File.Open(file, FileMode.Open, FileAccess.Read))
                    {
                        myResourceDictionary = XamlReader.Load(s, pc) as ResourceDictionary;
                        myResourceDictionary.BeginInit();
                        myResourceDictionary.EndInit();

                        Application.Current.Resources.MergedDictionaries[0] = myResourceDictionary;

                        s.Close();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("ThemeHelper", e);

                    myResourceDictionary = CreateTheme(mw.CurrentTheme);
                }
            }

            ApplyResource(mw);

            return myResourceDictionary;
        }

        public static void ApplyResource(MainWindow mw)
        {
            try
            {
                mw.WaveformForeground = (Color)mw.FindResource("Theme_Waveform_Foreground");
                mw.WaveformHighlight = (Color)mw.FindResource("Theme_Waveform_Highlight");
            }
            catch
            {
                mw.WaveformHighlight = Color.FromRgb(255, 255, 255);
                mw.WaveformForeground = Color.FromRgb(76, 215, 255);
            }

            mw.Brush_VU_Dark = (Brush)mw.FindResource("Theme_VU_DarkBrush");
            mw.Brush_VU_White = (Brush)mw.FindResource("Theme_VU_LightBrush");

            mw.OsiloFill = (Brush)mw.FindResource("Theme_Osilo_Color");
            mw.SpecFill = (Brush)mw.FindResource("Theme_Spectrum_Color");
        }

        public static bool IsExist(string ThemeName)
        {
            return new DirectoryInfo(Path.Combine(LibraryFolder, ThemeName)).Exists;
        }

        public static void FixNode(XmlDocument doc, XmlNode node)
        {
            if (node != null && node.Attributes != null)
            {
                int i = 0;
                while (i < node.Attributes.Count)
                {
                    if (node.Attributes[i].Name.ToLower() == "baseuri")
                    {
                        Logger.Log("ThemeHelper", "BaseURI Removed: " + node.Name);

                        node.Attributes.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                if(node.Name.ToLower() == "bitmapimage")
                {
                    bool cachemodeExist = false;
                    foreach(XmlAttribute attribute in node.Attributes)
                    {
                        if(attribute.Name.ToLower() == "cacheoption")
                        {
                            attribute.Value = "OnLoad";

                            cachemodeExist = true;
                        }
                    }

                    if (!cachemodeExist)
                    {
                        XmlAttribute attr = doc.CreateAttribute("CacheOption");
                        attr.Value = "OnLoad";

                        node.Attributes.SetNamedItem(attr);
                    }
                }
            }

            if (node.HasChildNodes && node.ChildNodes != null)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    FixNode(doc, childNode);
                }
            }
        }

        public static string GetName(string Key)
        {
            return LanguageHelper.FindText("Lang_ThemeHelper_" + Key);
        }

        #endregion static region

        public ResourceDictionary Dictionary { get; set; }

        public string ResourcesFolderName { get; private set; } = "Resources";

        private DirectoryInfo _directoryInfo;
        public DirectoryInfo DirectoryInfo
        {
            get
            {
                return _directoryInfo;
            }
            set
            {
                _directoryInfo = value;

                ThemeName = Path.GetFileName(_directoryInfo.FullName);

                DirectoryInfo resDi = new DirectoryInfo(Path.Combine(value.FullName, ResourcesFolderName));
                if (!resDi.Exists)
                {
                    resDi.Create();
                }

                ResourceDirectory = resDi.FullName;
            }
        }

        public string ThemeName { get; private set; }

        public event EventHandler Updated;

        MainWindow mw;

        public ThemeHelper(DirectoryInfo di, MainWindow mw)
        {
            this.mw = mw;

            GcWhenImport = false;
            
            DirectoryInfo = di;

            Dictionary = LoadTheme(di, mw);
        }

        public ThemeHelper(string ThemeName, MainWindow mw)
        {
            this.mw = mw;

            GcWhenImport = false;

            DirectoryInfo = new DirectoryInfo(Path.Combine(LibraryFolder, ThemeName));

            Dictionary = LoadTheme(DirectoryInfo, mw);
        }

        private new void ResourceGarbageCollection()
        {
            base.ResourceGarbageCollection();
        }

        public new void Dispose()
        {
            ResourceClose();

            Resources.Clear();

            Dictionary = LoadTheme(DirectoryInfo, mw);

            if (Dictionary == null)
                return;

            foreach(DictionaryEntry obj in Dictionary)
            {
                if(obj.Value is ImageBrush)
                {
                    ImageBrush brush = (ImageBrush)obj.Value;
                    if(brush.ImageSource != null)
                    {
                        BitmapImage img = (BitmapImage)brush.ImageSource;

                        Resource r = new Resource(this);

                        r.Open(Path.GetFileName(img.UriSource.OriginalString));
                    }
                }
            }

            ResourceGarbageCollection();

            Resources.Clear();

            Dictionary = null;
        }

        public void Update()
        {
            mw.CurrentTheme = ThemeName;

            Application.Current.Resources.MergedDictionaries[0] = Dictionary;

            ApplyResource(mw);

            Updated?.Invoke(this, null);
        }

        public void Save()
        {
            mw.CurrentTheme = ThemeName;

            SaveTheme(DirectoryInfo);
        }
    }
}
