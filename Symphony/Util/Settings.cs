using Symphony.Lyrics;
using Symphony.Player;
using Symphony.Player.DSP.CSCore;
using Symphony.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
        /* TODO: Add more settings
        case "Waveform.Zoom":
        case "Visualizer.Use":
        case "Osilo.Use"
        case "VU.Use": */

        public static string SettingsLibrary => Path.Combine(Environment.CurrentDirectory, "NewSettings");
        public static string SettingsSaveFilePath => Path.Combine(SettingsLibrary, "Settings.xml");
        public static string DspChainSaveFilePath => Path.Combine(SettingsLibrary, "Effects.DSPs");

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
            }
        }

        private bool _saveWindowMode = true;
        public bool SaveWindowMode
        {
            get { return _saveWindowMode; }
            set { _saveWindowMode = value; }
        }

        //Player Settings
        private string[] _playerAlbumArtSearchPathes = Player.Tags.AlbumArtFolders;
        public string[] PlayerAlbumArtSearchPathes
        {
            get
            {
                return _playerAlbumArtSearchPathes;
            }
            set
            {
                _playerAlbumArtSearchPathes = value;
            }
        }

        private bool _playerUseSearchLocalAlbumArt = Player.Tags.UseLocalAlbumArts;
        public bool PlayerUseSearchLocalAlbumArt
        {
            get
            {
                return _playerUseSearchLocalAlbumArt;
            }
            set
            {
                _playerUseSearchLocalAlbumArt = value;
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
            }
        }

        //Audio Settings
        private int _audioDesiredLantency = Player.PlayerCore.DefaultDesiredLatency;
        public int AudioDesiredLantency
        {
            get
            {
                return _audioDesiredLantency;
            }
            set
            {
                _audioDesiredLantency = value;
            }
        }

        private double _audioVolume = 100;
        /// <summary>
        /// Volume: 0.0 ~ 100.0
        /// </summary>
        public double AudioVolume
        {
            get
            {
                return _audioVolume;
            }
            set
            {
                _audioVolume = value;
            }
        }

        private bool _audioUseDspProcessing = true;
        public bool AudioUseDspProcessing
        {
            get
            {
                return _audioUseDspProcessing;
            }
            set
            {
                _audioUseDspProcessing = value;
            }
        }

        private bool _audioUseDspLimit = true;
        public bool AudioUseDspLimit
        {
            get
            {
                return _audioUseDspLimit;
            }
            set
            {
                _audioUseDspLimit = value;
            }
        }

        private int _audioDspLimitSampleRate = 96000;
        public int AudioDspLimitSampleRate
        {
            get
            {
                return _audioDspLimitSampleRate;
            }
            set
            {
                _audioDspLimitSampleRate = value;
            }
        }

        //Theme Color Settings
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
                _singerDefaultFadeOutMode = value;
            }
        }

        public double SingerTop { get; set; } = double.NegativeInfinity;
        public double SingerLeft { get; set; } = double.NegativeInfinity;

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
            }
        }

        private double _singerZoom = 1;
        public double SingerZoom
        {
            get { return _singerZoom; }
            set
            {
                _singerZoom = value;
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
            }
        }

        private double _composerOpacity = 1;
        public double ComposerOpacity
        {
            get { return _composerOpacity; }
            set
            {
                _composerOpacity = value;
            }
        }

        //Osiloscope

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
            }
        }

        private double _osiloOpacity = 1;
        public double OsiloOpacity
        {
            get { return _osiloOpacity; }
            set
            {
                _osiloOpacity = value;
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
            }
        }

        //VU meter

        private double _vuOpacity = 1;
        public double VUOpacity
        {
            get { return _vuOpacity; }
            set
            {
                _vuOpacity = value;
            }
        }

        private double _vuSenstive = 150;
        public double VUSenstive
        {
            get
            {
                return _vuSenstive;
            }
            set
            {
                _vuSenstive = value;
            }
        }

        //Spectrum Analyser

        private double _specOpacity = 0.528;
        public double SpecOpacity
        {
            get { return _specOpacity; }
            set
            {
                _specOpacity = value;
            }
        }

        private int _specMinFreq = 32;
        public int SpecMinFreq
        {
            get
            {
                return _specMinFreq;
            }
            set
            {
                _specMinFreq = value;
            }
        }

        private int _specMaxFreq = 22050;
        public int SpecMaxFreq
        {
            get
            {
                return _specMaxFreq;
            }
            set
            {
                _specMaxFreq = value;
            }
        }

        private ScalingStrategy _specScalingMode = ScalingStrategy.Decibel;
        public ScalingStrategy SpecScalingMode
        {
            get
            {
                return _specScalingMode;
            }
            set
            {
                _specScalingMode = value;
            }
        }

        private double _specDash = 1;
        public double SpecDash
        {
            get
            {
                return _specDash;
            }
            set
            {
                _specDash = value;
            }
        }

        private double _specWidth = 15;
        public double SpecWidth
        {
            get
            {
                return _specWidth;
            }
            set
            {
                _specWidth = value;
            }
        }

        private double _specHeight = -0.375;
        public double SpecHeight
        {
            get
            {
                return _specHeight;
            }
            set
            {
                _specHeight = value;
            }
        }

        private double _specTop = 0.52;
        public double SpecTop
        {
            get
            {
                return _specTop;
            }
            set
            {
                _specTop = value;
            }
        }

        private bool _specUseResampler = false;
        public bool SpecUseResampler
        {
            get
            {
                return _specUseResampler;
            }
            set
            {
                _specUseResampler = value;
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
            }
        }

        private bool _specInvert = false;
        public bool SpecInvert
        {
            get
            {
                return _specInvert;
            }
            set
            {
                _specInvert = value;
            }
        }

        private bool _specUseLogScale = true;
        public bool SpecUseLogScale
        {
            get
            {
                return _specUseLogScale;
            }
            set
            {
                _specUseLogScale = value;
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
