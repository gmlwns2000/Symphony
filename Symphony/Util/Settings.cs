using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.Util
{
    public abstract class BaseSettingListener
    {
        protected Settings _settings = Settings.Current;
        public virtual Settings Settings
        {
            get => _settings;
            set
            {
                if(_settings != null)
                {
                    _settings.PropertyChanged -= Settings_PropertyChanged;
                }

                _settings = value;
                _settings.PropertyChanged += Settings_PropertyChanged;

                OnSettingChanged(_settings);
            }
        }

        public BaseSettingListener()
        {
            _settings.PropertyChanged += Settings_PropertyChanged;

            OnSettingChanged(_settings);
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnSettingPropertyChanged(e);
        }

        public abstract void OnSettingChanged(Settings e);
        public abstract void OnSettingPropertyChanged(PropertyChangedEventArgs e);
    }

    public class SettingListener : BaseSettingListener
    {
        public event EventHandler<Settings> SettingChanged;
        public event EventHandler<PropertyChangedEventArgs> SettingPropertyChanged;

        public override void OnSettingChanged(Settings e)
        {
            SettingChanged?.Invoke(this, e);
        }

        public override void OnSettingPropertyChanged(PropertyChangedEventArgs e)
        {
            SettingPropertyChanged?.Invoke(this, e);
        }
    }

    public class Settings : INotifyPropertyChanged
    {
        public static string SettingsLibrary => Path.Combine(Environment.CurrentDirectory, "NewSettings");
        public static string SettingsSavePath => Path.Combine(SettingsLibrary, "Settings.xml");

        public static Settings Current { get; private set; }

        public static void Load()
        {
            Current = new Settings();
        }

        public static void Save()
        {

        }

        #region Properties

        //General Settings
        private bool _useImageAnimation = true;
        public bool UseImageAnimation
        {
            get
            {
                return _useImageAnimation;
            }
            set
            {
                _useImageAnimation = value;
            }
        }

        private int _guiUpdate = 66;
        public int GUIUpdate
        {
            get
            {
                return _guiUpdate;
            }
            set
            {
                _guiUpdate = value;
            }
        }

        private bool _useFooterInfoText = true;
        public bool UseFooterInfoText
        {
            get
            {
                return _useFooterInfoText;
            }
            set
            {
                _useFooterInfoText = value;
                if (_useFooterInfoText)
                {
                    Lb_Footer_Info.Visibility = Visibility.Visible;
                }
                else
                {
                    Lb_Footer_Info.Visibility = Visibility.Hidden;
                }
            }
        }

        private bool _saveWindowMode = true;
        public bool SaveWindowMode
        {
            get { return _saveWindowMode; }
            set { _saveWindowMode = value; }
        }

        //Player Settings
        public string[] PlayerAlbumArtSearchPathes
        {
            get
            {
                return Tags.AlbumArtFolders;
            }
            set
            {
                Tags.AlbumArtFolders = value;
            }
        }

        public bool PlayerUseSearchLocalAlbumArt
        {
            get
            {
                return Tags.UseLocalAlbumArts;
            }
            set
            {
                Tags.UseLocalAlbumArts = value;
            }
        }

        private bool _playerMiniControlShow = false;
        public bool PlayerMiniControlShow
        {
            get
            {
                return _playerMiniControlShow;
            }
            set
            {
                if (_playerMiniControlShow != value)
                {
                    _playerMiniControlShow = value;

                    if (value)
                    {
                        OpenMiniControl();
                    }
                    else
                    {
                        CloseMiniControl();
                    }
                }
            }
        }

        private bool _playerMiniControlTopmost = true;
        public bool PlayerMiniControlTopmost
        {
            get
            {
                return _playerMiniControlTopmost;
            }
            set
            {
                _playerMiniControlTopmost = value;

                if (miniControlWindow != null)
                {
                    miniControlWindow.Topmost = value;

                    miniControlWindow.Update();
                }
            }
        }

        private bool _playerMiniControlSavePosition = true;
        public bool PlayerMiniControlSavePosition
        {
            get
            {
                return _playerMiniControlSavePosition;
            }
            set
            {
                _playerMiniControlSavePosition = value;
            }
        }

        private bool _playerMiniControlSetPosition = false;

        private double _playerMiniControlTop = -1;
        public double PlayerMiniControlTop
        {
            get
            {
                return _playerMiniControlTop;
            }
            set
            {
                _playerMiniControlTop = value;

                if (_playerMiniControlSetPosition && miniControlWindow != null)
                {
                    miniControlWindow.Top = value;
                }
            }
        }

        private double _playerMiniControlLeft = -1;
        public double PlayerMiniControlLeft
        {
            get
            {
                return _playerMiniControlLeft;
            }
            set
            {
                _playerMiniControlLeft = value;

                if (_playerMiniControlSetPosition && miniControlWindow != null)
                {
                    miniControlWindow.Left = value;
                }
            }
        }

        //Audio Settings
        public int AudioDesiredLantency
        {
            get
            {
                return np.DesiredLatency;
            }
            set
            {
                np.DesiredLatency = value;
            }
        }

        /// <summary>
        /// Volume: 0.0 ~ 100.0
        /// </summary>
        public double AudioVolume
        {
            get
            {
                return Sld_Footer_Volume.Value;
            }
            set
            {
                Sld_Footer_Volume_ValueChanged(this, new RoutedPropertyChangedEventArgs<double>(AudioVolume, value));

                Sld_Footer_Volume.Value = value;
            }
        }

        public bool AudioUseDspProcessing
        {
            get
            {
                return np.UseDspProcessing;
            }
            set
            {
                np.UseDspProcessing = value;
            }
        }

        public bool AudioUseDspLimit
        {
            get
            {
                return np.DspUseSampleRateLimit;
            }
            set
            {
                np.DspUseSampleRateLimit = value;
            }
        }

        public int AudioDspLimitSampleRate
        {
            get
            {
                return np.DspSampleRateLimit;
            }
            set
            {
                np.DspSampleRateLimit = value;
            }
        }

        //Theme Color Settings
        private Color _waveformForeground = Color.FromRgb(76, 215, 255);
        public Color WaveformForeground
        {
            get
            {
                return _waveformForeground;
            }
            set
            {
                _waveformForeground = value;
                Sld_Big_Position.ForegroundColor = _waveformForeground;
            }
        }

        private Color _waveformHighlight = Color.FromRgb(255, 255, 255);
        public Color WaveformHighlight
        {
            get
            {
                return _waveformHighlight;
            }
            set
            {
                _waveformHighlight = value;
                Sld_Big_Position.HighlightColor = _waveformHighlight;
            }
        }

        private string _currentTheme = "Default Theme";
        public string CurrentTheme
        {
            get { return _currentTheme; }
            set { _currentTheme = value; }
        }

        //Singer Settings
        private HorizontalAlignment _singerHorizontalAlignment = HorizontalAlignment.Center;
        public HorizontalAlignment SingerHorizontalAlignment
        {
            get
            {
                return _singerHorizontalAlignment;
            }
            set
            {
                _singerHorizontalAlignment = value;

                if (singer != null)
                {
                    singer.HorizontalAlignment = _singerHorizontalAlignment;
                }
            }
        }

        private VerticalAlignment _singerVerticalAlignment = VerticalAlignment.Bottom;
        public VerticalAlignment SingerVerticalAlignment
        {
            get
            {
                return _singerVerticalAlignment;
            }
            set
            {
                _singerVerticalAlignment = value;

                if (singer != null)
                {
                    singer.VerticalAlignment = _singerVerticalAlignment;
                }
            }
        }

        private FadeInMode _singerDefaultFadeInMode = FadeInMode.FadeIn;
        public FadeInMode SingerDefaultFadeInMode
        {
            get
            {
                return _singerDefaultFadeInMode;
            }
            set
            {

                LyricLineRenderer.DefaultFadeIn = value;
                _singerDefaultFadeInMode = value;
            }
        }

        private FadeOutMode _singerDefaultFadeOutMode = FadeOutMode.FadeOut;
        public FadeOutMode SingerDefaultFadeOutMode
        {
            get
            {
                return _singerDefaultFadeOutMode;
            }
            set
            {
                LyricLineRenderer.DefaultFadeOut = value;
                _singerDefaultFadeOutMode = value;
            }
        }

        private double SingerTop = double.NegativeInfinity;
        private double SingerLeft = double.NegativeInfinity;

        private bool _singerResetPosition = true;
        public bool SingerResetPosition
        {
            get
            {
                return _singerResetPosition;
            }
            set
            {
                _singerResetPosition = value;

                if (singer != null)
                {
                    if (SingerCanDragmove || !SingerResetPosition)
                    {
                        singer.ResetPosition = false;
                    }
                    else
                    {
                        singer.ResetPosition = true;
                    }
                }
            }
        }

        private bool _singerCanDragmove = false;
        public bool SingerCanDragmove
        {
            get
            {
                return _singerCanDragmove;
            }
            set
            {
                _singerCanDragmove = value;

                if (singer != null && Lyric != null)
                {
                    SingerTop = singer.Top;
                    SingerLeft = singer.Left;

                    singer.Dispose();
                    singer = null;

                    bool resetpos = _singerResetPosition;

                    _singerResetPosition = false;
                    CreateSinger(Lyric);
                    _singerResetPosition = resetpos;

                    singer.Refresh();
                }
            }
        }

        private double _singerZoom = 1;
        public double SingerZoom
        {
            get { return _singerZoom; }
            set
            {
                _singerZoom = value;
                if (singer != null)
                {
                    singer.Zoom = _singerZoom;
                    singer.Refresh();
                }
            }
        }

        private bool _singerShow = true;
        public bool SingerShow
        {
            get
            {
                return _singerShow;
            }
            set
            {
                if (!value && singer != null)
                {
                    singer.Dispose();
                    singer = null;
                }

                _singerShow = value;
            }
        }

        private double _singerOpacity = 1;
        public double SingerOpacity
        {
            get
            {
                return _singerOpacity;
            }
            set
            {
                if (singer != null)
                {
                    singer.Opacity = value;
                }
                _singerOpacity = value;
            }
        }

        private bool _singerWindowMode = true;
        public bool SingerWindowMode
        {
            get
            {
                return _singerWindowMode;
            }
            set
            {
                if (_singerWindowMode != value)
                {
                    _singerWindowMode = value;

                    if (singer != null && Lyric != null)
                    {
                        singer.Dispose();
                        singer = null;

                        CreateSinger(Lyric);

                        singer.Refresh();
                    }
                }
            }
        }

        //Composer Settings
        private bool _composerTopmost = false;
        public bool ComposerTopmost
        {
            get
            {
                return _composerTopmost;
            }
            set
            {
                if (dlrenderer != null)
                {
                    dlrenderer.Topmost = value;
                }
                _composerTopmost = value;
            }
        }

        private bool _composerWindowMode = false;
        public bool ComposerWindowMode
        {
            get
            {
                return _composerWindowMode;
            }
            set
            {
                _composerWindowMode = value;

                if (dlrenderer != null)
                {
                    dlrenderer.WindowMode = value;
                }
            }
        }

        private bool _composerUse = true;
        public bool ComposerUse
        {
            get
            {
                return _composerUse;
            }
            set
            {
                _composerUse = value;
                if (!value)
                {
                    if (dlrenderer != null)
                    {
                        dlrenderer.Close();
                        dlrenderer = null;
                        Util.MemoryManagement.FlushMemory();
                    }
                }
            }
        }

        private double _composerOpacity = 1;
        public double ComposerOpacity
        {
            get { return _composerOpacity; }
            set
            {
                _composerOpacity = value;
                if (dlrenderer != null)
                {
                    dlrenderer.Opacity = value;
                }
            }
        }

        //Osiloscope

        private Brush _osilo_fill;
        public Brush OsiloFill
        {
            get
            {
                return _osilo_fill;
            }
            set
            {
                _osilo_fill = value;

                if (osVisualizer != null)
                {
                    osVisualizer.FillBrush = _osilo_fill;
                }
            }
        }

        private double _osiloHeight = 0.317;
        public double OsiloHeight
        {
            get
            {
                return _osiloHeight;
            }
            set
            {
                _osiloHeight = value;
                if (osVisualizer != null)
                {
                    osVisualizer.Height = value;
                }
            }
        }

        private float _osiloView = 50;
        public float OsiloView
        {
            get
            {
                return _osiloView;
            }
            set
            {
                _osiloView = value;
                if (osVisualizer != null)
                {
                    osVisualizer.View = value;
                }
            }
        }

        private float _osiloStrength = 0.735f;
        public float OsiloStrength
        {
            get
            {
                return _osiloStrength;
            }
            set
            {
                _osiloStrength = value;
                if (osVisualizer != null)
                {
                    osVisualizer.Strength = value;
                }
            }
        }

        private double _osiloWidth = 15;
        public double OsiloWidth
        {
            get
            {
                return _osiloWidth;
            }
            set
            {
                _osiloWidth = value;
                if (osVisualizer != null)
                {
                    osVisualizer.Width = value;
                }
            }
        }

        private double _osiloDash = 1;
        public double OsiloDash
        {
            get
            {
                return _osiloDash;
            }
            set
            {
                _osiloDash = value;
                if (osVisualizer != null)
                {
                    osVisualizer.Dash = value;
                }
            }
        }

        private double _osiloTop = 0.52;
        public double OsiloTop
        {
            get
            {
                return _osiloTop;
            }
            set
            {
                _osiloTop = value;
                if (osVisualizer != null)
                {
                    osVisualizer.Top = value;
                }
            }
        }

        private double _osiloOpacity = 1;
        public double OsiloOpacity
        {
            get { return _osiloOpacity; }
            set
            {
                _osiloOpacity = value;

                if (osVisualizer != null)
                {
                    osVisualizer.Opacity = value;
                }
            }
        }

        private bool _osiloUseInvert = false;
        public bool OsiloUseInvert
        {
            get
            {
                return _osiloUseInvert;
            }
            set
            {
                _osiloUseInvert = value;
                if (osVisualizer != null)
                {
                    osVisualizer.UseInvert = value;
                }
            }
        }

        private BarRenderTypes _osiloRenderType = BarRenderTypes.Rectangle;
        public BarRenderTypes OsiloRenderType
        {
            get
            {
                return _osiloRenderType;
            }
            set
            {
                _osiloRenderType = value;

                if (osVisualizer != null)
                {
                    osVisualizer.RenderType = value;
                }
            }
        }

        private bool _osiloGridShow = false;
        public bool OsiloGridShow
        {
            get
            {
                return _osiloGridShow;
            }
            set
            {
                _osiloGridShow = value;

                if (osVisualizer != null)
                {
                    osVisualizer.RenderGrid = value;
                }
            }
        }

        private HorizontalAlignment _osiloGridTextHoritontalAlignment = HorizontalAlignment.Left;
        public HorizontalAlignment OsiloGridTextHorizontalAlignment
        {
            get
            {
                return _osiloGridTextHoritontalAlignment;
            }
            set
            {
                _osiloGridTextHoritontalAlignment = value;

                if (osVisualizer != null)
                {
                    osVisualizer.GridTextHorizontalAlignment = value;
                }
            }
        }

        //VU meter
        private Brush _brushVUDark;
        public Brush Brush_VU_Dark
        {
            get
            {
                return _brushVUDark;
            }
            set
            {
                _brushVUDark = value;
                if (vuVisualizer != null)
                {
                    vuVisualizer.DarkBrush = _brushVUDark;
                }
            }
        }

        private Brush _brushVUWhite;
        public Brush Brush_VU_White
        {
            get
            {
                return _brushVUWhite;
            }
            set
            {
                _brushVUWhite = value;
                if (vuVisualizer != null)
                {
                    vuVisualizer.WhiteBrush = _brushVUWhite;
                }
            }
        }

        private double _vuOpacity = 1;
        public double VU_Opacity
        {
            get { return _vuOpacity; }
            set
            {
                _vuOpacity = value;

                if (vuVisualizer != null)
                {
                    vuVisualizer.Opacity = value * Grid_Mid.Opacity;
                }
            }
        }

        private double _vuSenstive = 150;
        public double VU_Senstive
        {
            get
            {
                return _vuSenstive;
            }
            set
            {
                _vuSenstive = value;
                if (vuVisualizer != null)
                {
                    vuVisualizer.Senstive = value;
                }
            }
        }

        //Spectrum Analyser
        private Brush _spec_fill;
        public Brush SpecFill
        {
            get
            {
                return _spec_fill;
            }
            set
            {
                _spec_fill = value;

                if (specVisualizer != null)
                {
                    specVisualizer.Brush = _spec_fill;
                }
            }
        }

        private double _spec_opacity = 0.528;
        public double SpecOpacity
        {
            get { return _spec_opacity; }
            set
            {
                _spec_opacity = value;

                if (specVisualizer != null)
                {
                    specVisualizer.Opacity = value;
                }
            }
        }

        private int _spec_MinFreq = 32;
        public int SpecMinFreq
        {
            get
            {
                return _spec_MinFreq;
            }
            set
            {
                _spec_MinFreq = value;

                if (np != null && np.DSPMaster != null && specVisualizer.SpectrumAnalysis != null)
                {
                    specVisualizer.SpectrumAnalysis.MinimumFrequency = value;
                }
            }
        }

        private int _spec_MaxFreq = 22050;
        public int SpecMaxFreq
        {
            get
            {
                return _spec_MaxFreq;
            }
            set
            {
                _spec_MaxFreq = value;

                if (np != null && np.DSPMaster != null && specVisualizer.SpectrumAnalysis != null)
                {
                    specVisualizer.SpectrumAnalysis.MaximumFrequency = value;
                }
            }
        }

        private ScalingStrategy _spec_ScalingMode = ScalingStrategy.Decibel;
        public ScalingStrategy Spec_ScalingMode
        {
            get
            {
                return _spec_ScalingMode;
            }
            set
            {
                _spec_ScalingMode = value;

                if (np != null && np.DSPMaster != null && specVisualizer.SpectrumAnalysis != null)
                {
                    specVisualizer.SpectrumAnalysis.ScalingStrategy = value;
                }
            }
        }

        private double _spec_dash = 1;
        public double Spec_Dash
        {
            get
            {
                return _spec_dash;
            }
            set
            {
                _spec_dash = value;

                if (specVisualizer != null)
                {
                    specVisualizer.Dash = value;
                }
            }
        }

        private double _spec_width = 15;
        public double Spec_Width
        {
            get
            {
                return _spec_width;
            }
            set
            {
                _spec_width = value;

                if (specVisualizer != null)
                    specVisualizer.Width = value;
            }
        }

        private double _spec_height = -0.375;
        public double SpecHeight
        {
            get
            {
                return _spec_height;
            }
            set
            {
                _spec_height = value;

                if (specVisualizer != null)
                    specVisualizer.Height = value;
            }
        }

        private double _spec_top = 0.52;
        public double SpecTop
        {
            get
            {
                return _spec_top;
            }
            set
            {
                _spec_top = value;

                if (specVisualizer != null)
                {
                    specVisualizer.Top = value;
                }
            }
        }

        private bool _spec_useResampler = false;
        public bool SpecUseResampler
        {
            get
            {
                return _spec_useResampler;
            }
            set
            {
                _spec_useResampler = value;

                if (np != null && np.DSPMaster != null && specVisualizer.SpectrumAnalysis != null)
                {
                    specVisualizer.SpectrumAnalysis.UseResampling = value;
                }
            }
        }

        private ResamplingMode _specResamplingMode = ResamplingMode.Linear;
        public ResamplingMode SpecResampleMode
        {
            get
            {
                return _specResamplingMode;
            }
            set
            {
                _specResamplingMode = value;

                if (specVisualizer.SpectrumAnalysis != null)
                {
                    specVisualizer.SpectrumAnalysis.ResamplingMode = value;
                }
            }
        }

        private bool _spec_invert = false;
        public bool SpecInvert
        {
            get
            {
                return _spec_invert;
            }
            set
            {
                _spec_invert = value;

                if (specVisualizer != null)
                {
                    specVisualizer.UseInvert = value;
                }
            }
        }

        private bool _spec_UseLogScale = true;
        public bool SpecUseLogScale
        {
            get
            {
                return _spec_UseLogScale;
            }
            set
            {
                _spec_UseLogScale = value;

                if (np != null && np.DSPMaster != null && specVisualizer.SpectrumAnalysis != null)
                {
                    specVisualizer.SpectrumAnalysis.IsXLogScale = value;
                }
            }
        }

        private BarRenderTypes _specRenderType = BarRenderTypes.Rectangle;
        public BarRenderTypes SpecRenderType
        {
            get
            {
                return _specRenderType;
            }
            set
            {
                _specRenderType = value;
                if (specVisualizer != null)
                {
                    specVisualizer.RenderType = value;
                }
            }
        }

        private double _specStrength = 0.75;
        public double SpecStrength
        {
            get
            {
                return _specStrength;
            }
            set
            {
                _specStrength = value;
                if (specVisualizer != null)
                {
                    specVisualizer.Strength = value;
                }
            }
        }

        private bool _specGridShow = false;
        public bool SpecGridShow
        {
            get
            {
                return _specGridShow;
            }
            set
            {
                _specGridShow = value;

                if (specVisualizer != null)
                {
                    specVisualizer.RenderGrid = value;
                }
            }
        }

        private HorizontalAlignment _specGridTextHorizontalAlignment = HorizontalAlignment.Right;
        public HorizontalAlignment SpecGridTextHorizontalAlignment
        {
            get
            {
                return _specGridTextHorizontalAlignment;
            }
            set
            {
                _specGridTextHorizontalAlignment = value;

                if (specVisualizer != null)
                {
                    specVisualizer.GridTextHorizontalAlignment = value;
                }
            }
        }

        #endregion Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
