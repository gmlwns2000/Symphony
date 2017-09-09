using Microsoft.Win32;
using Symphony.Server;
using Symphony.Util;
using System;
using System.Collections.Generic;
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

namespace Symphony.DancerLite
{
    /// <summary>
    /// DanceLiteEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DanceLiteEditor : UserControl
    {
        MusicMetadata meta;
        PlotLite pl;

        public DanceLiteEditor(MusicMetadata meta)
        {
            InitializeComponent();

            this.meta = meta;

            pl = new PlotLite(meta);

            UpdateText();
        }

        public void UpdateText()
        {
            if(Util.TextTool.StringEmpty(pl.PMXPath))
            {
                Tb_PMX_Path.Text = LanguageHelper.FindText("Lang_PlotLite_Editor_PleaseLoadPMX");
            }
            else
            {
                Tb_PMX_Path.Text = pl.PMXPath;
            }

            if (Util.TextTool.StringEmpty(pl.VMDPath))
            {
                Tb_VMD_Path.Text = LanguageHelper.FindText("Lang_PlotLite_Editor_PleaseLoadVMD");
            }
            else
            {
                Tb_VMD_Path.Text = pl.VMDPath;
            }
        }

        public DanceLiteEditor(string xmlFile)
        {
            InitializeComponent();

            pl = new PlotLite(xmlFile);

            UpdateText();
        }

        private void Menu_Close_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }

        SaveFileDialog sfd;
        private void Menu_Save_Click(object sender, RoutedEventArgs e)
        {
            if(sfd == null)
            {
                sfd = new SaveFileDialog();
                sfd.Title = LanguageHelper.FindText("Lang_Plot_Save");
                sfd.DefaultExt = ".plotlite";
                sfd.Filter = string.Format("{0}|*.plotlite|{1}|*.*", LanguageHelper.FindText("Lang_PlotFile"), LanguageHelper.FindText("Lang_AllFileFormat"));
            }

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    PlSaver.Save(pl);
                    PlSaver.Export(pl, sfd.FileName);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);
                    UI.DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_FileSaveError"));
                }
            }
        }

        private void Bt_PMX_Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = LanguageHelper.FindText("Lang_File_Open");
            ofd.Filter = string.Format("{0}|*.pmx|{1}|*.*", LanguageHelper.FindText("Lang_PMXFile"), LanguageHelper.FindText("Lang_AllFileFormat"));

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    pl.PMXLoad(ofd.FileName);
                    PlSaver.Save(pl);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);
                    UI.DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_FileLoadError"));
                }
            }
            UpdateText();
        }

        private void Bt_VMD_Add_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = LanguageHelper.FindText("Lang_File_Open");
            ofd.Filter = string.Format("{0}|*.vmd|{1}|*.*", LanguageHelper.FindText("Lang_VMDFile"), LanguageHelper.FindText("Lang_AllFileFormat"));

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    pl.VMDLoad(ofd.FileName);
                    PlSaver.Save(pl);
                }
                catch (Exception ex)
                {
                    Logger.Error(this, ex);
                    UI.DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_FileLoadError"));
                }
            }
            UpdateText();
        }
    }
}
