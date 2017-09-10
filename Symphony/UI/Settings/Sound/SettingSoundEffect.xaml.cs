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
using Symphony.Player;
using Symphony.Player.DSP;
using Microsoft.Win32;
using Symphony.DSP;
using Symphony.Util;

namespace Symphony.UI.Settings
{
    /// <summary>
    /// SettingUiVisualizer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingSoundEffect : UserControl
    {
        private PlayerCore np;
        bool inited = false;
        MainWindow mw;

        public SettingSoundEffect()
        {
            InitializeComponent();
        }

        public void init_Effects(ref PlayerCore np, MainWindow mw)
        {
            this.np = np;
            this.mw = mw;

            libList.Clear();

            libList.Add(EffectNames.EchoItem);
            libList.Add(EffectNames.EquealizerItem);
            libList.Add(EffectNames.LimiterItem);
            libList.Add(EffectNames.StereoEnhancerItem);
            libList.Add(EffectNames.LuaDspItem);

            Cb_DspUse.IsChecked = np.UseDspProcessing;

            UpdateDSPList();

            inited = true;
        }

        List<LvDspItem> chainList = new List<LvDspItem>();
        List<LvDspItem> libList = new List<LvDspItem>();

        private void UpdateDSPList()
        {
            foreach(LvDspItem item in chainList)
            {
                item.Deleting();
            }

            chainList.Clear();

            if (np!=null && np.DSPs != null)
            {
                for(int i=0; i<np.DSPs.Count; i++)
                {
                    if (np.DSPs[i] is Echo)
                    {
                        chainList.Add(EffectNames.EchoItem);
                    }
                    else if (np.DSPs[i] is Equalizer)
                    {
                        chainList.Add(EffectNames.EquealizerItem);
                    }
                    else if (np.DSPs[i] is Limitter)
                    {
                        chainList.Add(EffectNames.LimiterItem);
                    }
                    else if (np.DSPs[i] is DspWrapper)
                    {
                        chainList.Add(new LvDspItem(EffectNames.LuaDsp, ((DspWrapper)np.DSPs[i]).Name, ((DspWrapper)np.DSPs[i]).DisplayText));
                    }
                    else if(np.DSPs[i] is StereoEnhancer)
                    {
                        chainList.Add(EffectNames.StereoEnhancerItem);
                    }
                    else
                    {
                        chainList.Add(new LvDspItem(EffectNames.Unknown, LanguageHelper.FindText("Lang_Unknown"), ""));
                    }
                }
            }

            Lst_Chain.ItemsSource = chainList;
            Lst_Library.ItemsSource = libList;

            Lst_Chain.Items.Refresh();
            Lst_Library.Items.Refresh();
        }

        private void Bt_Reset_Click(object sender, RoutedEventArgs e)
        {
            DialogMessageResult r = DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_ResetConfrim"), LanguageHelper.FindText("Lang_Confirm"), DialogMessageType.OkayCanel);

            if(r == DialogMessageResult.Okay)
            {
                np.DSPs.Clear();

                np.UpdateDSP();

                UpdateDSPList();
            }
        }

        private void Bt_Preset_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = LanguageHelper.FindText("Lang_DspChain_Open");

            string presetFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Presets");
            ofd.CustomPlaces.Add(new FileDialogCustomPlace(presetFolder));
            ofd.InitialDirectory = presetFolder;

            ofd.Filter = string.Format("{0}|*.DSPs|{1}|*.*", LanguageHelper.FindText("Lang_DSPChainFile"), LanguageHelper.FindText("Lang_AllFileFormat"));

            if ((bool)ofd.ShowDialog())
            {
                try
                {
                    List<DSPBase> dsplist = DSPChainLoader.Load(ofd.FileName, ref np);

                    if (dsplist != null)
                    {
                        np.DSPs = dsplist;
                        np.UpdateDSP();
                        UpdateDSPList();
                    }
                    else
                    {
                        DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_FileBroken"));
                    }
                }
                catch ( Exception ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_FileLoadError"));
                }
            }
        }

        private void Bt_Preset_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = LanguageHelper.FindText("Lang_DspChain_Save");

            sfd.Filter = string.Format("{0}|*.DSPs|{1}|*.*", LanguageHelper.FindText("Lang_DSPChainFile"), LanguageHelper.FindText("Lang_AllFileFormat"));

            if ((bool)sfd.ShowDialog())
            {
                try
                {
                    DSPChainSaver.Save(sfd.FileName, np.DSPs);
                }
                catch ( Exception ex)
                {
                    Logger.Error(this, ex);

                    DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_FileSaveError"));
                }
            }
        }

        private void Lst_ItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lastClickIsChain)
            {
                if(np != null && np.DSPs != null && Lst_Chain.SelectedIndex > -1 && Lst_Chain.SelectedIndex < np.DSPs.Count)
                {
                    string n = chainList[Lst_Chain.SelectedIndex].Name;
                    int index = Lst_Chain.SelectedIndex;

                    if(n == EffectNames.Echo)
                    {
                        SettingEcho echo = new SettingEcho(np.DSPs[index] as Echo);
                        echo.Updated += delegate (object s, DspUpdatedArgs ev)
                        {
                            np.SetDSP(ev.DSP);
                            np.UpdateDSP();
                        };
                        UserControlHostWindow host = new UserControlHostWindow(Window.GetWindow(this), mw, EffectNames.EchoItem.DisplayName, echo);
                        host.Show();

                        chainList[index].Editor = host;
                    }
                    else if(n == EffectNames.Equealizer)
                    {
                        SettingEqualizer eq = new SettingEqualizer(np.DSPs[index] as Equalizer);
                        eq.Updated += delegate (object s, DspUpdatedArgs ev)
                        {
                            np.SetDSP(ev.DSP);
                            np.UpdateDSP();
                        };
                        UserControlHostWindow host = new UserControlHostWindow(Window.GetWindow(this), mw, EffectNames.EquealizerItem.DisplayName, eq);
                        host.Show();
                        
                        chainList[index].Editor = host;
                    }
                    else if(n == EffectNames.Limiter)
                    {
                        SettingLimiter lim = new SettingLimiter(np.DSPs[index] as Limitter);
                        lim.Updated += delegate (object s, DspUpdatedArgs ev)
                        {
                            np.SetDSP(ev.DSP);
                            np.UpdateDSP();
                        };
                        UserControlHostWindow host = new UserControlHostWindow(Window.GetWindow(this), mw, EffectNames.LimiterItem.DisplayName, lim);
                        host.Show();

                        chainList[index].Editor = host;
                    }
                    else if(n == EffectNames.LuaDsp)
                    {
                        SettingDspWrapper wrap = new SettingDspWrapper(np.DSPs[index] as DspWrapper);
                        wrap.Updated += delegate (object s, DspUpdatedArgs ev)
                        {
                            bool paused = false;
                            if (np.isPlay && !np.isPaused)
                            {
                                paused = true;
                                np.Pause(true);
                            }

                            np.SetDSP(ev.DSP);
                            np.UpdateDSP();

                            if(paused)
                                np.Pause(false);

                            foreach(LvDspItem item in chainList)
                            {
                                if(item.Editor != null && item.Editor.Control is SettingDspWrapper && item.Editor.Control == s)
                                {
                                    item.DisplayName = ((DspWrapper)ev.DSP).Name;
                                    item.Describe = ((DspWrapper)ev.DSP).DisplayText;

                                    Lst_Chain.Items.Refresh();
                                    break;
                                }
                            }
                        };
                        UserControlHostWindow host = new UserControlHostWindow(Window.GetWindow(this), mw, EffectNames.LuaDspItem.DisplayName, wrap, true);

                        host.Width = 620;
                        host.Height = 400;
                        host.Show();

                        chainList[index].Editor = host;
                    }
                    else if(n == EffectNames.StereoEnhancer)
                    {
                        SettingStereoEnhancer se = new SettingStereoEnhancer(np.DSPs[index] as StereoEnhancer);
                        se.Updated += delegate (object s, DspUpdatedArgs arg)
                        {
                            np.SetDSP(arg.DSP);
                            np.UpdateDSP();
                        };
                        UserControlHostWindow host = new UserControlHostWindow(Window.GetWindow(this), mw, EffectNames.StereoEnhancerItem.DisplayName, se, false);

                        host.Show();
                        
                        chainList[index].Editor = host;
                    }
                    else if(n == EffectNames.Unknown)
                    {
                        DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UnknownDSP"));
                    }
                    else
                    {
                        DialogMessage.Show(Window.GetWindow(this), LanguageHelper.FindText("Lang_Setting_Audio_SoundEffect_UnknownPlugin"));
                    }
                }
            }
            else
            {
                UI.DialogMessage.Show(Window.GetWindow(this), libList[Lst_Library.SelectedIndex].DisplayName + "\n\n" + LanguageHelper.FindText("Lang_Describe") + ": " + libList[Lst_Library.SelectedIndex].Describe, LanguageHelper.FindText("Lang_Information"), DialogMessageType.Okay);
            }
        }

        private void Bt_Chain_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (np != null && np.DSPs != null)
            {
                if (Lst_Chain.SelectedIndex > -1 && Lst_Chain.SelectedIndex < np.DSPs.Count)
                {
                    int ind = Lst_Chain.SelectedIndex;

                    chainList[ind].Deleting();
                    chainList.RemoveAt(ind);

                    Lst_Chain.Items.Refresh();

                    np.DSPs.RemoveAt(ind);
                    np.UpdateDSP();

                    Lst_Chain.SelectedIndex = ind;
                }
            }
        }

        private void Bt_Chain_Add_Click(object sender, RoutedEventArgs e)
        {
            if (np != null && np.DSPs != null)
            {
                if (Lst_Library.SelectedIndex > -1 && Lst_Library.SelectedIndex < libList.Count)
                {
                    LvDspItem item = Lst_Library.SelectedItem as LvDspItem;
                    if (item != null)
                    {
                        if (item.Name == EffectNames.Echo)
                        {
                            np.DSPs.Add(new Echo());
                            chainList.Add(EffectNames.EchoItem);
                        }
                        else if(item.Name == EffectNames.Equealizer)
                        {
                            np.DSPs.Add(new Equalizer(ref np));
                            chainList.Add(EffectNames.EquealizerItem);
                        }
                        else if(item.Name == EffectNames.Limiter)
                        {
                            np.DSPs.Add(new Limitter(0.95f, 0.05f));
                            chainList.Add(EffectNames.LimiterItem);
                        }
                        else if(item.Name == EffectNames.LuaDsp)
                        {
                            np.DSPs.Add(new DspWrapper(""));
                            chainList.Add(EffectNames.DSPWrapperItem((DspWrapper)np.DSPs[np.DSPs.Count-1]));
                        }
                        else if(item.Name == EffectNames.StereoEnhancer)
                        {
                            np.DSPs.Add(new StereoEnhancer());
                            chainList.Add(EffectNames.StereoEnhancerItem);
                        }
                    }
                    np.UpdateDSP();

                    Lst_Library.SelectedIndex = -1;

                    Lst_Chain.Items.Refresh();
                }
            }
        }

        bool lastClickIsChain = false;
        private void Lst_Chain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastClickIsChain = true;
        }

        private void Lst_Library_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastClickIsChain = false;
        }

        private void Bt_Chain_Up_Click(object sender, RoutedEventArgs e)
        {
            if(Lst_Chain.SelectedIndex > 0 && Lst_Chain.SelectedIndex < chainList.Count)
            {
                int i = Lst_Chain.SelectedIndex;

                DSPBase d = np.DSPs[i];
                LvDspItem it = chainList[i];

                np.DSPs[i] = np.DSPs[i - 1];
                chainList[i] = chainList[i - 1];
                np.DSPs[i - 1] = d;
                chainList[i - 1] = it;
                np.UpdateDSP();
                Lst_Chain.Items.Refresh();

                Lst_Chain.SelectedIndex = i - 1;
            }
        }

        private void Bt_Chain_Down_Click(object sender, RoutedEventArgs e)
        {
            if (Lst_Chain.SelectedIndex > -1 && Lst_Chain.SelectedIndex < chainList.Count-1)
            {
                int i = Lst_Chain.SelectedIndex;

                DSPBase d = np.DSPs[i];
                LvDspItem it = chainList[i];

                np.DSPs[i] = np.DSPs[i + 1];
                chainList[i] = chainList[i + 1];
                np.DSPs[i + 1] = d;
                chainList[i + 1] = it;
                np.UpdateDSP();
                Lst_Chain.Items.Refresh();

                Lst_Chain.SelectedIndex = i + 1;
            }
        }

        private void Cb_DspUse_Checked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                np.UseDspProcessing = true;
            }
        }

        private void Cb_DspUse_Unchecked(object sender, RoutedEventArgs e)
        {
            if (inited)
            {
                np.UseDspProcessing = false;
            }
        }
    }
}
