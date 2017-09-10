using System;
using System.Collections.Generic;
using CSCore;
using CSCore.SoundOut;
using CSCore.Streams;
using System.Threading;
using System.IO;
using CSCore.Codecs;
using Symphony.Player.DSP;
using Symphony.Player.Youtube;

namespace Symphony.Player
{
    public class PlayerCore : IDisposable
    {
        public PlayerLogger log;

        private bool userStopped = false;
        private bool debug = true;
        private bool indexing = false;

        private Thread waveformThread;
        private bool waveformCancelPadding = false;
        private int HotkeyPlaypause = 0;

        public delegate void PlaybackChanged();
        public event PlaybackChanged PlayStopped;
        public event PlaybackChanged PlayStarted;
        public event PlaybackChanged PlayPaused;
        public event PlaybackChanged PlayResumed;
        public event PlaybackChanged PlaySeeked;
        public event PlaybackChanged PlayStreamStopped;

        public event EventHandler<WaveformReady> WaveformReady;
        public int WaveformWidth = 990;
        public WaveformMode WaveformMode = WaveformMode.Mono;
        public float[] WaveformData;

        /// <summary>
        /// This is DspMaster, Work really like DSP Chain, Renew OnStartPlay, SpectrumAnalysis is Here
        /// </summary>
        public DSPMaster DSPMaster;
        private List<Playlist> _playlists = new List<Playlist>();
        public List<Playlist> Playlists
        {
            get { return _playlists; }
            set { _playlists = value; }
        }

        public Playlist CurrentPlaylist
        {
            get
            {
                if (Playlists != null && CurrentPlaylistIndex > -1 && CurrentPlaylistIndex < Playlists.Count) 
                {
                    return Playlists[CurrentPlaylistIndex];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (CurrentPlaylist != null)
                    CurrentPlaylist = value;
            }
        }

        public PlaylistItem CurrentMusic
        {
            get
            {
                if (CurrentPlaylist != null)
                    return CurrentPlaylist.GetCurrent();
                else
                    return null;
            }
            set
            {
                if(CurrentPlaylist != null)
                    CurrentPlaylist.Replace(CurrentPlaylist.Index, value);
            }
        }
        public List<DSPBase> DSPs;
        public bool _UseDspProcessing = true;
        public bool UseDspProcessing
        {
            get
            {
                return _UseDspProcessing;
            }
            set
            {
                _UseDspProcessing = value;
                if (DSPMaster != null)
                {
                    DSPMaster.UseDspProcessing = value;
                }
            }
        }
        public int _DspSampleRateLimit = 96000;
        public int DspSampleRateLimit
        {
            get
            {
                return _DspSampleRateLimit;
            }
            set
            {
                _DspSampleRateLimit = value;

                if (DSPMaster != null)
                {
                    DSPMaster.SamplerateLimit = value;
                }
            }
        }
        public bool _DspUseSampleRateLimit = true;
        public bool DspUseSampleRateLimit
        {
            get
            {
                return _DspUseSampleRateLimit;
            }
            set
            {
                _DspUseSampleRateLimit = value;

                if(DSPMaster!= null)
                {
                    DSPMaster.UseSamplerateLimit = value;
                }
            }
        }

        public ISoundOut SoundOut;
        public ISampleSource readerSource;
        public IWaveSource waveSource;
        public Stream fileStream;

        public string FilePath { get; private set; }
        public string FileName { get; private set; }
        public readonly static string[] SupportFormat = new string[]
        {
            "aif",
            "aiff",
            "aifc",
            "aac",
            "ac3",
            "mp3",
            "mpeg3",
            "m4a",
            "flac",
            "fla",
            "wav",
            "wma",
            "wmv",
            "mp4",
            "avi",
            "mov",
            "m4v",
            "mp4"
        };

        public static string FileFilter
        {
            get
            {
                string filter = "";
                filter += Lang_SupportFiles + "|*.aif;*.aiff;*.aifc;*.aac;*.ac3;*.mp3;*.mpeg3;*.m4a;*.flac;*.fla;*.wav;*.wma;*.wmv;*.mp4;*.avi;*.mov;*.m4v;*.mp4|";
                filter += Lang_MP3Files + "|*.mp3;*.mpeg3|";
                filter += Lang_FLACFiles + "|*.flac;*.fla|";
                filter += Lang_M4AFiles + "|*.m4a|";
                filter += Lang_WAVFiles + "|*.wav;*.wave|";
                filter += Lang_AIFFFiles + "|*.aiff;*.aif;*.aifc|";
                filter += Lang_AACFiles + "|*.aac;*.adt;*.adts;*.m2ts;*.mp2;*.3g2;*.3gp;*.3gpp;*.m4a;*.m4v;*.mp4v;*.mp4;*.mov|";
                filter += Lang_AC3Files + "|*.ac3|";
                filter += Lang_DDPFiles + "|*.mp2;*.m2ts;*.mp4;*.wmv;*.wma;*.avi|";
                filter += Lang_AllFiles + "|*.*";
                return filter;
            }
        }
        public static string Lang_SupportFiles = "지원되는 파일";
        public static string Lang_MP3Files = "MP3 파일";
        public static string Lang_FLACFiles = "FLAC 파일";
        public static string Lang_M4AFiles = "M4A 파일";
        public static string Lang_WAVFiles = "WAV 파일";
        public static string Lang_AIFFFiles = "AIFF 파일";
        public static string Lang_AACFiles = "AAC 파일";
        public static string Lang_AC3Files = "AC3 파일";
        public static string Lang_DDPFiles = "DDP 파일";
        public static string Lang_AllFiles = "모든 파일";

        public bool RenderWaveform = true;
        public bool isPlay { get; private set; }
        public bool isPaused { get; private set; }
        public float Volume { get; private set; }
        public int BitsPerSecond;
        public int BitsPerSample;
        public int SampleRate;
        private int _currentPlaylistIndex = 0;
        public int CurrentPlaylistIndex
        {
            get
            {
                return _currentPlaylistIndex;
            }
            set
            {
                _currentPlaylistIndex = value;
            }
        }

        public const int DefaultDesiredLatency = 100;
        public int DesiredLatency = 100;

        public delegate void RuntimeError(string msg);

        public event RuntimeError FileOpenFaild;
        public event RuntimeError FormatUnknown;
        public event RuntimeError WrongTimeFormat;

        public static string Lang_FileOpenError = "파일 열기중 오류가 발생했습니다";
        public static string Lang_UnSupportedFormat = "지원되지 않는 포맷입니다";

        public PlayerCore()
        {
            nPlayer_Init();
        }

        public PlayerCore(bool debug)
        {
            this.debug = debug;
            nPlayer_Init();
        }

        private void nPlayer_Init()
        {
            log = new PlayerLogger(debug);

            log.dlog("Start Init nPlayer");

            isPlay = false;
            isPaused = false;

            Volume = 0.75f;

            Playlists.Add(new Playlist("Default Playlist"));

            DSPs = new List<DSPBase>();

            HotkeyPlaypause = Hotkey.RegisterHotKey(System.Windows.Forms.Keys.MediaPlayPause, 0);
            Hotkey.HotKeyPressed += Hotkey_HotKeyPressed;
        }

        public static bool IsSupport(string FileName)
        {
            bool support = false;
            string Extention = Path.GetExtension(FileName).ToLower();

            foreach (string format in CodecFactory.Instance.GetSupportedFileExtensions())
            {
                if (Extention.EndsWith(format))
                {
                    support = true;
                    break;
                }
            }

            return support;
        }

        public static List<string> FolderSearch(DirectoryInfo dir)
        {
            return InternalFolderSearch(dir, 0);
        }

        private static List<string> InternalFolderSearch(DirectoryInfo dir, int depth)
        {
            depth++;

            if (depth > 100)
                return null;

            if (dir.Exists)
            {
                List<string> fileList = new List<string>();

                FileInfo[] fis = dir.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    if (IsSupport(fi.FullName))
                    {
                        fileList.Add(fi.FullName);
                    }
                }

                DirectoryInfo[] dis = dir.GetDirectories();
                if (dis != null && dis.Length > 0)
                {
                    foreach (DirectoryInfo di in dis)
                    {
                        List<string> ret = InternalFolderSearch(di, depth);

                        if (ret != null && ret.Count > 0)
                        {
                            fileList.AddRange(ret);
                        }
                    }
                }

                return fileList;
            }
            else
            {
                return null;
            }
        }

        private void Hotkey_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            if (e.Key == System.Windows.Forms.Keys.MediaPlayPause)
            {
                if (isPlay)
                {
                    PlayPause();
                }
            }
        }

        #region Waveform

        public void Waveform_Start()
        {
            waveformThread = new Thread(new ThreadStart(Waveform_Work2));
            waveformThread.IsBackground = true;
            waveformThread.Name = "Player waveform generator";

            waveformCancelPadding = false;

            waveformThread.Start();
        }

        public void Waveform_Stop()
        {
            if (waveformThread!=null && waveformThread.ThreadState == System.Threading.ThreadState.Running)
            {
                waveformCancelPadding = true;

                waveformThread.Join(30);

                waveformThread.Abort();

                Waveform_Dispose();

                Waveform_Ended();
            }
        }

        private void Waveform_Prograss()
        {
            if (WaveformReady != null && WaveformData != null)
            {
                WaveformReady(this, new WaveformReady(WaveformData, WaveformWidth*2));
            }
        }

        private void Waveform_Ended()
        {
            if (WaveformReady != null && WaveformData!=null)
            {
                WaveformReady(this, new WaveformReady(WaveformData, WaveformData.Length));
            }
        }

        private float MaxLeft = 0;
        private float MaxRight = 0;

        private void waveCheckSample(object sender, SampleEventArgs e)
        {
            switch (WaveformMode)
            {
                case WaveformMode.Mono:
                    if (MaxLeft < Math.Abs(e.Left))
                    {
                        MaxLeft = Math.Max((Math.Abs(e.Left) + Math.Abs(e.Right)) * 0.5f, MaxLeft);
                        MaxRight = MaxLeft * (float)-1;
                    }
                    break;
                case WaveformMode.Left:
                    if(MaxLeft < e.Left)
                    {
                        MaxLeft = (MaxLeft + e.Left) * 0.5f;
                        MaxRight = MaxLeft;
                    }
                    break;
                case WaveformMode.Right:
                    if(MaxRight > e.Right)
                    {
                        MaxRight = (MaxRight + e.Right) * 0.5f;
                        MaxLeft = MaxRight;
                    }
                    break;
                case WaveformMode.Stereao:
                    if(MaxLeft < e.Left)
                    {
                        MaxLeft = (MaxLeft + e.Left) * 0.5f;
                    }
                    if(MaxRight> e.Right)
                    {
                        MaxRight = (MaxRight + e.Right) * 0.5f;
                    }
                    break;
            }
        }

        private float fixFloat(float f)
        {
            if (float.IsInfinity(f))
            {
                return 0;
            }
            else if (float.IsNaN(f))
            {
                return 0;
            }
            else
            {
                return f;
            }
        }

        private ISampleSource waveSampleReader;
        private Stream waveFileStream;

        private void Waveform_Dispose()
        {
            if (waveFileStream != null)
            {
                if(!(CurrentMusic is YoutubeItem))
                {
                    waveFileStream.Close();
                    waveFileStream.Dispose();
                }
                waveFileStream = null;
            }

            if (waveSampleReader != null)
            {
                waveSampleReader.Dispose();
                waveSampleReader = null;
            }
        }

        private void Waveform_Work2()
        {
            log.dlog("Wavefrom Generation is Started!");

            if(waveFileStream == null)
                waveFileStream = CurrentPlaylist.GetCurrent().GetStream();

            waveSampleReader = CodecFactory.Instance.GetCodec(Path.GetExtension(FilePath), waveFileStream).ToSampleSource();
            if (waveSampleReader == null)
                return;

            long sampleLength = waveSampleReader.Length;
            long samplePart = sampleLength / WaveformWidth;
            long samplePartLength = (samplePart / 10) * 1;
            List<float> samples = new List<float>();

            for (int i = 0; i < WaveformWidth; i++)
            {
                if (waveSampleReader == null)
                {
                    waveformCancelPadding = false;
                    return;
                }

                if (waveformCancelPadding)
                {
                    Waveform_Dispose();

                    waveformCancelPadding = false;

                    return;
                }

                if (!waveformCancelPadding)
                {
                    try
                    {
                        float[] buf = new float[samplePartLength];
                        int readed = -1;

                        readed = waveSampleReader.Read(buf, 0, buf.Length);

                        waveSampleReader.Position = Math.Min(sampleLength - 1, waveSampleReader.Position + samplePart - readed);

                        for (int ii = 0; ii < buf.Length / 2 - 2; ii++)
                        {
                            waveCheckSample(null, new SampleEventArgs(buf[ii * 2], buf[ii * 2 + 1]));
                        }
                    }
                    catch
                    {
                        return;
                    }

                    samples.Add(MaxLeft);
                    samples.Add(MaxRight);

                    if ((samples.Count % 50) < 2 && samples.Count > 0)
                    {
                        if (WaveformReady != null)
                        {
                            WaveformData = samples.ToArray();
                            Waveform_Prograss();
                        }
                    }

                    MaxLeft = 0;
                    MaxRight = 0;
                }
            }

            WaveformData = samples.ToArray();

            Waveform_Ended();

            log.dlog("Wavefrom Generation: Data complete");

            Waveform_Dispose();

            waveformCancelPadding = false;
        }

        #endregion Waveform

        #region Play

        public void Play(int index)
        {
            if (CurrentPlaylistIndex >= 0 && Playlists.Count > 0)
            {
                if (Playlists[CurrentPlaylistIndex].Items != null)
                {
                    if ((Playlists[CurrentPlaylistIndex].Items.Count > index) && (index >= 0))
                    {
                        indexing = true;
                        Playlists[CurrentPlaylistIndex].Index = index;
                        Play(Playlists[CurrentPlaylistIndex].Items[index]);
                    }
                }
            }
        }
        
        private void Play(PlaylistItem playitem)
        {
            string filePath = playitem.FilePath;

            log.dlog("Start Play music: " + filePath);
            if (isPlay)
            {
                log.derr("it was already playing");
                Stop();
            }

            if (playitem.IsAvailable)
            {
                FilePath = filePath;
                FileName = Path.GetFileName(FilePath);
                try
                {
                    if (IsSupport(filePath))
                    {
                        log.dlog("detected");

                        fileStream = playitem.GetStream();
                        waveSource = CodecFactory.Instance.GetCodec(Path.GetExtension(FilePath), fileStream);
                        readerSource = waveSource.ToSampleSource();

                        BitsPerSecond = readerSource.WaveFormat.BytesPerSecond * 8;
                        BitsPerSample = readerSource.WaveFormat.BitsPerSample;
                        SampleRate = readerSource.WaveFormat.SampleRate;

                        if (RenderWaveform)
                        {
                            if (playitem is YoutubeItem)
                            {
                                //waveFileStream = fileStream;
                            }
                            else
                            {
                                waveFileStream = playitem.GetStream();
                                Waveform_Start();
                            }
                        }
                    }
                    else
                    {
                        log.derr("ERROR UNKNOWN");
                        FormatUnknown?.Invoke(Lang_UnSupportedFormat + " " + Path.GetExtension(filePath).Trim('.').ToUpper());
                        return;
                    }
                }
                catch (Exception e)
                {
                    FileOpenFaild?.Invoke(Lang_FileOpenError);
                    log.derr("ERROR: " + e.ToString());
                    return;
                }

                if (!indexing)
                {
                    Playlists[CurrentPlaylistIndex].Add(FilePath);
                    Playlists[CurrentPlaylistIndex].Index = Playlists[CurrentPlaylistIndex].Items.Count - 1;
                }
                else
                {
                    indexing = false;
                }

                DSPMaster = new DSPMaster(debug, this, readerSource);
                DSPMaster.UseDspProcessing = UseDspProcessing;
                DSPMaster.UseSamplerateLimit = DspUseSampleRateLimit;
                DSPMaster.SamplerateLimit = DspSampleRateLimit;

                UpdateDSP();

                if (SoundOut == null)
                {
                    SoundOut = GetSoundOut();
                    SoundOut.Stopped += SoundOut_PlaybackStopped;
                }

                SoundOut.Initialize(DSPMaster.ToWaveSource());
                SoundOut.Volume = Volume;

                SoundOut.Play();

                log.dlog("play started");

                isPlay = true;
                isPaused = false;

                PlayStarted?.Invoke();
            }
            else
            {
                FileOpenFaild?.Invoke("Item is not available");
            }
        }

        private ISoundOut GetSoundOut()
        {
            if (WasapiOut.IsSupportedOnCurrentPlatform)
            {
                log.dlog("Use WASAPI Driver. Over Window Vista");
                return new WasapiOut(true, CSCore.CoreAudioAPI.AudioClientShareMode.Shared, DesiredLatency, ThreadPriority.Highest) { StreamRoutingOptions = StreamRoutingOptions.All };
            }
            else
            {
                log.dlog("Use DirectSound Driver. Under Window Vista");
                return new DirectSoundOut(DesiredLatency, ThreadPriority.Highest, SynchronizationContext.Current);
            }
        }

        #endregion Play

        #region NextPrevious

        public void Next()
        {
            if (CurrentPlaylistIndex >= 0 && Playlists.Count > 0)
            {
                if (Playlists[CurrentPlaylistIndex].Items != null)
                {
                    Playlists[CurrentPlaylistIndex].GoNext();
                    if (Playlists[CurrentPlaylistIndex].Index != -1)
                    {
                        Stop();
                        Play(Playlists[CurrentPlaylistIndex].Index);
                    }
                    else
                    {
                        Stop();
                    }
                }
            }
            else
            {
                Stop();
            }
        }

        public void Previous()
        {
            if (CurrentPlaylistIndex >= 0 && Playlists.Count>0)
            {
                if (Playlists[CurrentPlaylistIndex].Items != null)
                {
                    Playlists[CurrentPlaylistIndex].GoPrevious();
                    if (Playlists[CurrentPlaylistIndex].Index != -1)
                    {
                        Stop();
                        Play(Playlists[CurrentPlaylistIndex].Index);
                    }
                    else
                    {
                        Stop();
                    }
                }
            }
            else
            {
                Stop();
            }
        }

        #endregion NextPrevious

        #region Stop

        private int PlayRetry = 0;

        void SoundOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            log.dlog("PlaybackStoped ");

            if(e.HasError && e.Exception != null)
            {
                Symphony.Logger.Error(this, e.Exception);
            }

            if (userStopped)
            {
                userStopped = false;
                return;
            }

            log.dlog("system normal end");

            FileName = "";
            FilePath = "";

            try
            {
                Waveform_Stop();
                
                SoundOut.Dispose();
                SoundOut = null;
                
                readerSource.Dispose();
                readerSource = null;

                DSPMaster.Dispose();
                DSPMaster = null;

                Dispose_FileStream();

                log.dlog("stopped Music");
            }
            catch (Exception ee)
            {
                log.derr("Errored on stop reader Stream: " + ee.Message + " || " + ee.Source + " || " + ee.StackTrace);
            }

            isPlay = false;
            PlayStopped?.Invoke();
            PlayStreamStopped?.Invoke();

            if (e.HasError)
            {
                PlayRetry++;

                if (PlayRetry < 5)
                {
                    Thread.Sleep(1000);

                    if(CurrentPlaylist != null)
                        Play(CurrentPlaylist.Index);

                    return;
                }
                else
                {
                    PlayRetry = 0;
                }
            }

            if (CurrentPlaylistIndex > -1)
            {
                Playlists[CurrentPlaylistIndex].GoNext();
                if (Playlists[CurrentPlaylistIndex].Index != -1)
                {
                    Play(Playlists[CurrentPlaylistIndex].Index);
                }
            }
        }

        public void Stop()
        {
            log.dlog("stop music");
            if (isPlay)
            {
                userStopped = true;

                FileName = "";
                FilePath = "";

                try
                {
                    Waveform_Stop();
                    
                    SoundOut.Stop();
                    SoundOut.WaitForStopped(30);
                    SoundOut.Dispose();
                    SoundOut = null;

                    readerSource.Dispose();
                    readerSource = null;

                    DSPMaster.Dispose();
                    DSPMaster = null;

                    Dispose_FileStream();

                    log.dlog("soundout Stoped");
                }
                catch (Exception ee)
                {
                    log.derr("Errored on stop soundout: " + ee.Message + " || " + ee.Source + " || " + ee.StackTrace);
                }

                if(waveformThread != null)
                {
                    Waveform_Stop();
                }

                isPlay = false;

                PlayStopped?.Invoke();
            }
            isPlay = false;
        }

        private void Dispose_FileStream()
        {
            if(fileStream is YoutubeStream)
            {
                YoutubeStream stream = (YoutubeStream)fileStream;
                stream.Close();
                stream.Dispose();
                stream = null;
                fileStream = null;
            }
            else
            {
                fileStream.Close();
                fileStream.Dispose();
                fileStream = null;
            }
        }

        #endregion Stop

        #region PlayPauseResume

        public void PlayPause()
        {
            if (isPlay)
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Pause()
        {
            log.dlog("pause music");
            isPaused = true;
            if (isPlay)
            {
                SoundOut.Pause();
                PlayPaused();
            }
        }

        public void Pause(bool on)
        {
            if (on)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        public void Resume()
        {
            log.dlog("resume music");
            isPaused = false;
            if (isPlay)
            {
                try
                {
                    SoundOut.Resume();

                    PlayResumed();
                }
                catch
                {
                    log.derr("No driver to play.");

                    Stop();
                }
            }
        }

        #endregion PlayPauseResume

        #region GetValue

        public TimeSpan GetPosition()
        {
            if (isPlay)
            {
                try
                {
                    return readerSource.GetPosition();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());

                    return new TimeSpan(0);
                }
            }
            else
            {
                return new TimeSpan(0);
            }
        }

        public string GetPosition(string format)
        {
            if (isPlay)
            {
                try
                {
                    return GetPosition().ToString(format);
                }
                catch
                {
                    WrongTimeFormat?.Invoke("TimeSpan's Format is wrong.");

                    return "-1";
                }
            }
            else
            {
                return "not playing...";
            }
        }

        public double GetPosition(TimeUnit tu)
        {
            double ret = -1;

            if (isPlay)
            {
                if (tu == TimeUnit.Hour)
                {
                    ret = GetPosition().TotalHours;
                }
                else if (tu == TimeUnit.MilliSecond)
                {
                    ret = GetPosition().TotalMilliseconds;
                }
                else if (tu == TimeUnit.Sec)
                {
                    ret = GetPosition().TotalSeconds;
                }
                else
                {
                    FormatUnknown("Format is not matched in MP3, AIFF, and WAVE.");
                }
            }

            return ret;
        }

        public TimeSpan GetLength()
        {
            if (isPlay)
            {
                try
                {
                    return readerSource.GetLength();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());

                    return new TimeSpan(0);
                }
            }
            else
            {
                return new TimeSpan(0);
            }
        }

        public string GetLength(string format)
        {
            if (isPlay)
            {
                try
                {
                    return readerSource.GetLength().ToString(format);
                }
                catch
                {
                    if (FormatUnknown != null)
                    {
                        FormatUnknown("time format wrong");
                    }
                    return "";
                }
            }
            else
            {
                return null;
            }
        }

        public double GetLength(TimeUnit tu)
        {
            double ret = -1;

            if (isPlay)
            {
                if (tu == TimeUnit.Hour)
                {
                    ret = readerSource.GetLength().TotalHours;
                }
                else if (tu == TimeUnit.MilliSecond)
                {
                    ret = readerSource.GetLength().TotalMilliseconds;
                }
                else if (tu == TimeUnit.Sec)
                {
                    ret = readerSource.GetLength().TotalSeconds;
                }
                else
                {
                    FormatUnknown("Time Unit couldn't known");
                }
            }

            return ret;
        }

        #endregion GetValue

        #region SetValue

        public void SetPosition(TimeUnit tu, int val)
        {
            if (isPlay)
            {
                bool isPause = isPaused;

                if (!isPause)
                {
                    Pause(true);
                }

                if (tu == TimeUnit.Hour)
                {
                    readerSource.SetPosition(TimeSpan.FromHours(val));
                }
                else if (tu == TimeUnit.MilliSecond)
                {
                    readerSource.SetPosition(TimeSpan.FromMilliseconds(val));
                }
                else if (tu == TimeUnit.Sec)
                {
                    readerSource.SetPosition(TimeSpan.FromSeconds(val));
                }
                else
                {
                    FormatUnknown?.Invoke("Time Unit couldn't known");
                }

                if(isPause != isPaused)
                {
                    Pause(isPause);
                }

                PlaySeeked?.Invoke();
            }
        }

        public void SetPosition(int position)
        {
            if (isPlay)
            {
                readerSource.SetPosition(TimeSpan.FromMilliseconds(Math.Max(0, position)));
            }
            PlaySeeked?.Invoke();
        }

        public void SetPosition(TimeSpan ts)
        {
            if (isPlay)
            {
                readerSource.SetPosition(ts);
            }
            PlaySeeked?.Invoke();
        }

        public void SetVolume(float volume)
        {
            Volume = volume;
            if (isPlay)
            {
                SoundOut.Volume = Volume;
            }
        }

        public void SetPlaylistOrder(PlaylistOrder order)
        {
            Playlists[CurrentPlaylistIndex].Order = order;
        }

        #endregion SetValue

        public void SetDSP(DSPBase dsp)
        {
            if (DSPs != null && DSPs.Count > 0) 
            {
                for(int i = 0; i<DSPs.Count; i++)
                {
                    if(DSPs[i].SID == dsp.SID)
                    {
                        DSPs[i] = dsp;
                        log.dlog("DSP set completed");
                        break;
                    }
                }
            }
            else
            {
                log.derr("ERRORED in setDSP. DSP does not exists.");
            }
        }

        public void UpdateDSP()
        {
            if (DSPMaster != null)
            {
                if (DSPMaster.DSPs != null)
                {
                    log.dlog("start update dsp");

                    lock (DSPMaster.DSPLocker)
                    {
                        for (int i = 0; i < DSPMaster.DSPs.Length; i++)
                        {
                            DSPMaster.DSPs[i] = null;
                        }
                        DSPMaster.DSPs = null;

                        for (int i = 0; i < DSPs.Count; i++)
                        {
                            DSPs[i].Init(DSPMaster);
                        }

                        DSPMaster.DSPs = DSPs.ToArray();
                    }

                    log.dlog("end update dsp");
                }
                else
                {
                    log.derr("DSP is not exists. Did you add it? or Is it correct index?");
                }
            }
        }

        public void Dispose()
        {
            Hotkey.UnregisterHotKey(HotkeyPlaypause);
            if(SoundOut != null)
            {
                SoundOut.Dispose();
            }
            if(DSPMaster != null)
            {
                DSPMaster.Dispose();
            }
        }
    }
}