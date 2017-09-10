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
        public static string SettingsLibrary => Path.Combine(Environment.CurrentDirectory, "Settings");
        public static string SettingsSaveFilePath => Path.Combine(SettingsLibrary, "Settings.xml");
        public static string DspChainSaveFilePath => Path.Combine(SettingsLibrary, "Effects.DSPs");

        public static Settings Current { get; private set; }

        public static void Load()
        {
            //TODO : Impl load
            Current = new Settings();
        }

        public static void Save()
        {
            //TODO: Impl save
        }

        #region Properties

        //General Settings
        private bool _useImageAnimation = true;
        public bool UseImageAnimation
        {
            get => _useImageAnimation;
            set { _useImageAnimation = value; OnPropertyChanged(); }
        }

        private int _guiUpdate = 66;
        public int GUIUpdate
        {
            get => _guiUpdate;
            set { _guiUpdate = value; OnPropertyChanged(); }
        }

        private bool _useFooterInfoText = true;
        public bool UseFooterInfoText
        {
            get =>_useFooterInfoText;
            set { _useFooterInfoText = value; OnPropertyChanged(); }
        }

        private bool _saveWindowMode = true;
        public bool SaveWindowMode
        {
            get => _saveWindowMode;
            set { _saveWindowMode = value; OnPropertyChanged(); }
        }

        //Player Settings
        private string[] _playerAlbumArtSearchPathes = Tags.AlbumArtFolders;
        public string[] PlayerAlbumArtSearchPathes
        {
            get => _playerAlbumArtSearchPathes;
            set { _playerAlbumArtSearchPathes = value; OnPropertyChanged(); }
        }

        private bool _playerUseSearchLocalAlbumArt = Player.Tags.UseLocalAlbumArts;
        public bool PlayerUseSearchLocalAlbumArt
        {
            get => _playerUseSearchLocalAlbumArt;
            set { _playerUseSearchLocalAlbumArt = value; OnPropertyChanged(); }
        }

        private bool _playerMiniControlShow = false;
        public bool PlayerMiniControlShow
        {
            get => _playerMiniControlShow;
            set
            {
                if (_playerMiniControlShow != value)
                {
                    _playerMiniControlShow = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _playerMiniControlTopmost = true;
        public bool PlayerMiniControlTopmost
        {
            get => _playerMiniControlTopmost;
            set { _playerMiniControlTopmost = value; OnPropertyChanged(); }
        }

        private bool _playerMiniControlSavePosition = true;
        public bool PlayerMiniControlSavePosition
        {
            get => _playerMiniControlSavePosition;
            set { _playerMiniControlSavePosition = value; OnPropertyChanged(); }
        }

        private double _playerMiniControlTop = -1;
        public double PlayerMiniControlTop
        {
            get => _playerMiniControlTop;
            set { _playerMiniControlTop = value; OnPropertyChanged(); }
        }

        private double _playerMiniControlLeft = -1;
        public double PlayerMiniControlLeft
        {
            get => _playerMiniControlLeft;
            set { _playerMiniControlLeft = value; OnPropertyChanged(); }
        }

        //Audio Settings
        private int _audioDesiredLantency = PlayerCore.DefaultDesiredLatency;
        public int AudioDesiredLantency
        {
            get => _audioDesiredLantency;
            set { _audioDesiredLantency = value; OnPropertyChanged(); }
        }

        private double _audioVolume = 100;
        /// <summary>
        /// Volume: 0.0 ~ 100.0
        /// </summary>
        public double AudioVolume
        {
            get => _audioVolume;
            set { _audioVolume = value; OnPropertyChanged(); }
        }

        private bool _audioUseDspProcessing = true;
        public bool AudioUseDspProcessing
        {
            get => _audioUseDspProcessing;
            set { _audioUseDspProcessing = value; OnPropertyChanged(); }
        }

        private bool _audioUseDspLimit = true;
        public bool AudioUseDspLimit
        {
            get => _audioUseDspLimit;
            set { _audioUseDspLimit = value; OnPropertyChanged(); }
        }

        private int _audioDspLimitSampleRate = 96000;
        public int AudioDspLimitSampleRate
        {
            get => _audioDspLimitSampleRate;
            set { _audioDspLimitSampleRate = value; OnPropertyChanged(); }
        }

        //Theme Color Settings
        private string _currentTheme = "Default Theme";
        public string CurrentTheme
        {
            get { return _currentTheme; }
            set { _currentTheme = value; OnPropertyChanged(); }
        }

        //Singer Settings
        private HorizontalAlignment _singerHorizontalAlignment = HorizontalAlignment.Center;
        public HorizontalAlignment SingerHorizontalAlignment
        {
            get => _singerHorizontalAlignment;
            set
            {
                _singerHorizontalAlignment = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private double _singerZoom = 1;
        public double SingerZoom
        {
            get { return _singerZoom; }
            set
            {
                _singerZoom = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                    OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private double _composerOpacity = 1;
        public double ComposerOpacity
        {
            get { return _composerOpacity; }
            set
            {
                _composerOpacity = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }

        private double _osiloOpacity = 1;
        public double OsiloOpacity
        {
            get { return _osiloOpacity; }
            set
            {
                _osiloOpacity = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
