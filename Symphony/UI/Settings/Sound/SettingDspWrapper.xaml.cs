using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Reflection;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using Symphony.DSP;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit.Document;
using Symphony.Util;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingDspWrapper.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingDspWrapper : UserControl, IDspEditor
    {
        public DspWrapper wrap;
        IHighlightingDefinition customHighlighting;
        string LuaName = "";

        public SettingDspWrapper(DSP.DspWrapper DspWrapper)
        {
            try
            {
                using (Stream fs = Assembly.GetExecutingAssembly().GetManifestResourceStream("Symphony.UI.Settings.Sound.LuaHighlight.xshd"))
                {
                    using (XmlReader reader = new XmlTextReader(fs))
                    {
                        customHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.
                            HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.Error("syntax errored " + ex.ToString());
            }

            InitializeComponent();

            wrap = DspWrapper;
            if(wrap.LuaFileName == "")
            {
                Menu_File_New_Click(null, null);
            }
            else
            {
                textEditor.Load(System.IO.Path.Combine(DspHelper.Library.FullName, wrap.LuaFileName));
                textEditor.SyntaxHighlighting = customHighlighting;
            }
            LuaName = wrap.LuaFileName;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(1.5);
            foldingUpdateTimer.Tick += FoldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();

            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.DefaultIndentationStrategy();
            foldingStrategy = new LuaFunctionFolding();
            if (foldingManager == null)
                foldingManager = FoldingManager.Install(textEditor.TextArea);

            textEditor.Options.CutCopyWholeLine = true;
            textEditor.Options.HighlightCurrentLine = true;

            FoldingUpdateTimer_Tick(null, null);

            updateText();
        }

        CompletionWindow completionWindow;

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "\t" || e.Text == "\r" || e.Text == "\n" || e.Text == " ")
            {
                if (completionWindow != null)
                {
                    return;
                }

                // open code completion after the user has pressed dot:
                completionWindow = new CompletionWindow(textEditor.TextArea);
                completionWindow.WindowStyle = WindowStyle.None;
                completionWindow.AllowsTransparency = true;
                completionWindow.Resources = this.Resources;
                completionWindow.SizeToContent = SizeToContent.Width;

                // provide AvalonEdit with the data:
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                GeneralFunctions(data);

                completionWindow.Show();

                completionWindow.Closed += delegate {
                    completionWindow = null;
                };
            }
        }

        private void GeneralFunctions(IList<ICompletionData> data)
        {
            data.Add(new LuaCompletionData("function", ""));
            data.Add(new LuaCompletionData("while", ""));
            data.Add(new LuaCompletionData("if", ""));
            data.Add(new LuaCompletionData("do", ""));
            data.Add(new LuaCompletionData("then", ""));
            data.Add(new LuaCompletionData("repeat", ""));
            data.Add(new LuaCompletionData("until", ""));
            data.Add(new LuaCompletionData("end", ""));
            data.Add(new LuaCompletionData("return", ""));
            data.Add(new LuaCompletionData("false", ""));
            data.Add(new LuaCompletionData("true", ""));
            data.Add(new LuaCompletionData("function\nend", ""));
            data.Add(new LuaCompletionData("if then\nend", ""));

            //math.
            data.Add(new LuaCompletionData("math.floor(n)", ""));
            data.Add(new LuaCompletionData("math.max(n)", ""));
            data.Add(new LuaCompletionData("math.min(n)", ""));
            data.Add(new LuaCompletionData("math.random()", ""));
            data.Add(new LuaCompletionData("math.randomseed( seed )", ""));
            data.Add(new LuaCompletionData("math.abs(n)", ""));
            data.Add(new LuaCompletionData("math.ceil(n)", ""));
            data.Add(new LuaCompletionData("math.log(n)", ""));
            data.Add(new LuaCompletionData("math.pow(n)", ""));
            data.Add(new LuaCompletionData("math.frexp(n)", ""));
            data.Add(new LuaCompletionData("math.acos(n)", ""));
            data.Add(new LuaCompletionData("math.cos(n)", ""));
            data.Add(new LuaCompletionData("math.log10(n)", ""));
            data.Add(new LuaCompletionData("math.rad(n)", ""));
            data.Add(new LuaCompletionData("math.ldexp(n)", ""));
            data.Add(new LuaCompletionData("math.asin(n)", ""));
            data.Add(new LuaCompletionData("math.deg(n)", ""));
            data.Add(new LuaCompletionData("math.sin(n)", ""));
            data.Add(new LuaCompletionData("math.atan(n)", ""));
            data.Add(new LuaCompletionData("math.exp(n)", ""));
            data.Add(new LuaCompletionData("math.sqrt(n)", ""));
            data.Add(new LuaCompletionData("math.atan2(n)", ""));
            data.Add(new LuaCompletionData("math.mod(n)", ""));
            data.Add(new LuaCompletionData("math.tan(n)", ""));

            //general
            data.Add(new LuaCompletionData("assert(v [, message])", ""));
            data.Add(new LuaCompletionData("collectgarbage([limit])", ""));
            data.Add(new LuaCompletionData("dofile(filename)", ""));
            data.Add(new LuaCompletionData("error(message [, level])", ""));
            data.Add(new LuaCompletionData("_G", ""));
            data.Add(new LuaCompletionData("getfenv(f)", ""));
            data.Add(new LuaCompletionData("getmetatable(object)", ""));
            data.Add(new LuaCompletionData("gcinfo()", ""));
            data.Add(new LuaCompletionData("ipairs(t)", ""));
            data.Add(new LuaCompletionData("loadfile(filename)", ""));
            data.Add(new LuaCompletionData("loadlib(libname, funcname)", ""));
            data.Add(new LuaCompletionData("loadstring(string [, chunkname])", ""));
            data.Add(new LuaCompletionData("next(table [, index])", ""));
            data.Add(new LuaCompletionData("pairs(t)", ""));
            data.Add(new LuaCompletionData("pcall(f, arg1, arg2, ...)", ""));
            data.Add(new LuaCompletionData("print(e1, e2, ...)", ""));
            data.Add(new LuaCompletionData("rawequal(v1, v2)", ""));
            data.Add(new LuaCompletionData("rawget(table, index)", ""));
            data.Add(new LuaCompletionData("rawset(table, index, value)", ""));
            data.Add(new LuaCompletionData("require(packagename)", ""));
            data.Add(new LuaCompletionData("setfenv(f, table)", ""));
            data.Add(new LuaCompletionData("setmetatable(table, metatable)", ""));
            data.Add(new LuaCompletionData("type(v)", ""));
            data.Add(new LuaCompletionData("unpack(list)", ""));
            data.Add(new LuaCompletionData("_VERSION", ""));
            data.Add(new LuaCompletionData("xpcall(f, err)", ""));

            //string
            data.Add(new LuaCompletionData("tonumber(s)", ""));
            data.Add(new LuaCompletionData("tostring(n)", ""));
            data.Add(new LuaCompletionData("string.char(n1, n2, ...)", ""));
            data.Add(new LuaCompletionData("string.byte(s [, i])", ""));
            data.Add(new LuaCompletionData("string.char(i1, i2, ...)", ""));
            data.Add(new LuaCompletionData("string.dump(function)", ""));
            data.Add(new LuaCompletionData("string.find(s, pattern [, init [, plain]])", ""));
            data.Add(new LuaCompletionData("string.len(s)", ""));
            data.Add(new LuaCompletionData("string.lower(s)", ""));
            data.Add(new LuaCompletionData("string.rep(s, n)", ""));
            data.Add(new LuaCompletionData("string.sub(s, i [, j])", ""));
            data.Add(new LuaCompletionData("string.upper(s)", ""));
            data.Add(new LuaCompletionData("string.format(formatstring, e1, e2, ...)", ""));
            data.Add(new LuaCompletionData("string.gfind(s, pat)", ""));
            data.Add(new LuaCompletionData("string.gsub(s, pat, repl [, n])", ""));

            //table
            data.Add(new LuaCompletionData("table.concat(table [, sep [, i [, j]]])", ""));
            data.Add(new LuaCompletionData("table.foreach(table, f)", ""));
            data.Add(new LuaCompletionData("table.foreachi(table, f)", ""));
            data.Add(new LuaCompletionData("table.getn(table)", ""));
            data.Add(new LuaCompletionData("table.sort(table [, comp])", ""));
            data.Add(new LuaCompletionData("table.insert(table, [pos,] value)", ""));
            data.Add(new LuaCompletionData("table.remove(table [, pos])", ""));
            data.Add(new LuaCompletionData("table.setn(table, n)", ""));

            //io
            data.Add(new LuaCompletionData("io.close([file])", ""));
            data.Add(new LuaCompletionData("io.flush()", ""));
            data.Add(new LuaCompletionData("io.input([file])", ""));
            data.Add(new LuaCompletionData("io.lines([filename])", ""));
            data.Add(new LuaCompletionData("io.open(filename [, mode])", ""));
            data.Add(new LuaCompletionData("io.output([file])", ""));
            data.Add(new LuaCompletionData("io.read(format1, ...)", ""));
            data.Add(new LuaCompletionData("io.tmpfile()", ""));
            data.Add(new LuaCompletionData("io.type(obj)", ""));
            data.Add(new LuaCompletionData("io.write(value1, ...)", ""));

            //os
            data.Add(new LuaCompletionData("os.clock()", ""));
            data.Add(new LuaCompletionData("os.date([format [, time]])", ""));
            data.Add(new LuaCompletionData("os.difftime(t2, t1)", ""));
            data.Add(new LuaCompletionData("os.execute(command)", ""));
            data.Add(new LuaCompletionData("os.exit([code])", ""));
            data.Add(new LuaCompletionData("os.getenv(varname)", ""));
            data.Add(new LuaCompletionData("os.remove(filename)", ""));
            data.Add(new LuaCompletionData("os.rename(oldname, newname)", ""));
            data.Add(new LuaCompletionData("os.setlocale(locale [, category])", ""));
            data.Add(new LuaCompletionData("os.time([table])", ""));
            data.Add(new LuaCompletionData("os.tmpname()", ""));

            //debug
            data.Add(new LuaCompletionData("debug.debug()", ""));
            data.Add(new LuaCompletionData("debug.gethook()", ""));
            data.Add(new LuaCompletionData("debug.getinfo(function [, what])", ""));
            data.Add(new LuaCompletionData("debug.getlocal(level, local)", ""));
            data.Add(new LuaCompletionData("debug.getupvalue(func, up)", ""));
            data.Add(new LuaCompletionData("debug.setlocal(level, local, value)", ""));
            data.Add(new LuaCompletionData("debug.setupvalue(func, up, value)", ""));
            data.Add(new LuaCompletionData("debug.sethook(hook, mask [, count])", ""));
            data.Add(new LuaCompletionData("debug.traceback([message])", ""));
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (e.Text[0] == '\t' || e.Text[0] == '\r' || e.Text[0] == '\n' || e.Text[0] == ' ')
                {
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // do not set e.Handled=true - we still want to insert the character that was typed
        }

        FoldingManager foldingManager;
        LuaFunctionFolding foldingStrategy;
        
        private void FoldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (foldingStrategy != null)
            {
                foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            }
        }

        public event EventHandler<DspUpdatedArgs> Updated;

        #region 파일

        private void Menu_File_New_Click(object sender, RoutedEventArgs e)
        {
            using(Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Symphony.UI.Settings.Sound.LuaDspTemplete.lua"))
            {
                textEditor.Load(s);
                textEditor.SyntaxHighlighting = customHighlighting;
            }
        }

        private void Menu_File_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = LanguageHelper.FindText("Lang_Lua_Open");
            ofd.Filter = string.Format("{0}|*.lua|{1}|*.*", LanguageHelper.FindText("Lang_LuaFile"), LanguageHelper.FindText("Lang_AllFileFormat"));

            if ((bool)ofd.ShowDialog(Window.GetWindow(this)))
            {
                try
                {
                    textEditor.Load(ofd.FileName);
                    textEditor.SyntaxHighlighting = customHighlighting;

                    LuaName = System.IO.Path.GetFileName(ofd.FileName);

                    UpdateDSP(ofd.FileName);
                }
                catch (System.IO.IOException ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_FileOpenError"));
                }
            }
        }

        SaveFileDialog sfd;

        private void Menu_File_Save_Click(object sender, RoutedEventArgs e)
        {
            if(sfd == null)
            {
                sfd = new SaveFileDialog();
                sfd.Title = LanguageHelper.FindText("Lang_Lua_Save");
                sfd.Filter = string.Format("{0}|*.lua|{1}|*.*", LanguageHelper.FindText("Lang_LuaFile"), LanguageHelper.FindText("Lang_AllFileFormat"));
                sfd.DefaultExt = ".lua";
            }

            if (Util.TextTool.StringEmpty(sfd.FileName) && (bool)sfd.ShowDialog(Window.GetWindow(this)))
            {
                try
                {
                    textEditor.Save(sfd.FileName);

                    LuaName = System.IO.Path.GetFileName(sfd.FileName);

                    UpdateDSP(sfd.FileName);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_FileSaveError"));
                }
            }
            else if(!Util.TextTool.StringEmpty(sfd.FileName))
            {
                try
                {
                    textEditor.Save(sfd.FileName);

                    UpdateDSP(sfd.FileName);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_FileSaveError"));
                }
            }
        }

        private void Menu_File_Save_Other_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = LanguageHelper.FindText("Lang_SaveOther");
            sfd.Filter = string.Format("{0}|*.lua|{1}|*.*", LanguageHelper.FindText("Lang_LuaFile"), LanguageHelper.FindText("Lang_AllFileFormat"));
            sfd.DefaultExt = ".lua";

            if ((bool)sfd.ShowDialog())
            {
                try
                {
                    textEditor.Save(sfd.FileName);
                    LuaName = System.IO.Path.GetFileName(sfd.FileName);

                    string[] result = DspHelper.SearchArray(LuaName);

                    if (result.Length > 0)
                    {
                        DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_NameError"));
                    }
                    else
                    {
                        UpdateDSP(sfd.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UserDSP_FileSaveError"));
                }
            }
        }

        #endregion 파일

        private void UpdateDSP(string filePath)
        {
            DspHelper.Import(filePath);

            int id = wrap.SID;
            wrap = new DspWrapper(LuaName);
            wrap.SID = id;

            Updated?.Invoke(this, new DspUpdatedArgs(wrap));
        }

        private void Menu_File_Close_Click(object sender, RoutedEventArgs e)
        {
            Window w = Window.GetWindow(this);
            if (w != null)
            {
                w.Close();
            }
        }

        private void Menu_Edit_Undo_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Undo();
        }

        private void Menu_Edit_Redo_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Redo();
        }

        private void Menu_Edit_Copy_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Copy();
        }

        private void Menu_Edit_Paste_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Paste();
        }

        private void Menu_Edit_Cut_Click(object sender, RoutedEventArgs e)
        {
            textEditor.Cut();
        }

        private void Menu_Edit_AutoReturn_Click(object sender, RoutedEventArgs e)
        {
            textEditor.WordWrap = !textEditor.WordWrap;
            updateText();
        }

        private void Menu_Edit_ShowLine_Click(object sender, RoutedEventArgs e)
        {
            textEditor.ShowLineNumbers = !textEditor.ShowLineNumbers;
            updateText();
        }

        private void updateText()
        {
            if (textEditor.WordWrap)
            {
                Menu_Edit_AutoReturn.Header = LanguageHelper.FindText("Lang_AutoNewLine");
            }

            if (textEditor.ShowLineNumbers)
            {
                Menu_Edit_ShowLine.Header = LanguageHelper.FindText("Lang_ShowLineNumber");
            }
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Menu_File_Save_Click(this, null);
                e.Handled = true;
            }
        }

        private void textEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        public void Deleted()
        {
            wrap = null;
        }
    }
}
