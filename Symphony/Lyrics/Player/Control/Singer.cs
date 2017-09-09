using Symphony.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Symphony.Lyrics
{
    public class Singer : IDisposable
    {
        Stopwatch sw;
        PlayerCore np;
        MainWindow mw;

        SingerControl control;
        SingerWindow window;

        bool stopped = false;
        public int scrIndex = -1;
        public bool ResetPosition = true;
        public bool Optimize = true;
        public Lyric Lyric;

        Grid grid
        {
            get
            {
                return control.grid;
            }
        }

        double _position = 0;
        public double Position
        {
            get
            {
                return _position;
            }
        }

        public double _syncOffset = 0;
        public double SyncOffset
        {
            get
            {
                return _syncOffset;
            }
            set
            {
                _syncOffset = value;

                if(grid != null)
                    Refresh();
            }
        }

        private HorizontalAlignment _HorizontalAlignment = HorizontalAlignment.Center;
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return _HorizontalAlignment;
            }
            set
            {
                _HorizontalAlignment = value;
                if (WindowMode)
                {
                    if (window != null)
                    {
                        window.HorizontalAlignment = value;
                    }
                }
                else
                {
                    control.HorizontalAlignment = value;
                }
                SetScreen(scrIndex);
            }
        }

        private VerticalAlignment _VerticalAlignment = VerticalAlignment.Center;
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return _VerticalAlignment;
            }
            set
            {
                _VerticalAlignment = value;
                if (WindowMode)
                {
                    if (window != null)
                    {
                        window.VerticalAlignment = value;
                    }
                }
                else
                {
                    control.VerticalAlignment = value;
                }
                SetScreen(scrIndex);
            }
        }

        public double _zoom = 1;
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (_zoom != value)
                {
                    _zoom = value;

                    double ct = control.ActualHeight;
                    double cl = control.ActualWidth;

                    control.MinHeight = Lyric.MinHeight * value;
                    control.MinWidth = Lyric.MinWidth * value;
                    
                    if (window != null)
                    {
                        if (ResetPosition)
                        {
                            window.SetScreen(scrIndex);
                        }
                        else
                        {
                            control.UpdateLayout();

                            double ct2 = control.ActualHeight;
                            double cl2 = control.ActualWidth;

                            Top -= (ct2 - ct)/2;
                            Left -= (cl2 - cl)/2;
                        }
                    }

                    Refresh();
                }
            }
        }
        
        public readonly bool WindowMode = false;

        private bool _topmost = false;
        public bool Topmost
        {
            get { return _topmost; }
            set
            {
                _topmost = value;

                if (window != null)
                {
                    window.Topmost = value;
                }
            }
        }

        private double _opacity = 1;
        public double Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
                control.Opacity = value;
            }
        }
        
        public double Top
        {
            get
            {
                if(window != null)
                {
                    return window.Top;
                }
                else
                {
                    return double.NegativeInfinity;
                }
            }
            set
            {
                if (window != null)
                {
                    window.Top = value;
                }
            }
        }

        public double Left
        {
            get
            {
                if (window != null)
                {
                    return window.Left;
                }
                else
                {
                    return double.NegativeInfinity;
                }
            }
            set
            {
                if (window != null)
                {
                    window.Left = value;
                }
            }
        }

        public double Width
        {
            get
            {
                if(window != null)
                {
                    return window.ActualWidth;
                }
                else
                {
                    return control.ActualWidth;
                }
            }
        }

        public double Height
        {
            get
            {
                if(window != null)
                {
                    return window.ActualHeight;
                }
                else
                {
                    return control.ActualHeight;
                }
            }
        }

        private Thickness _margin = new Thickness(24);
        public Thickness Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                _margin = value;

                if (WindowMode)
                {
                    if (window != null)
                    {
                        window.SingerMargin = value;
                    }
                }
                else
                {
                    control.Margin = _margin;
                }

                if (ResetPosition)
                {
                    SetScreen(scrIndex);
                }
            }
        }

        public event EventHandler Closed;
        public event EventHandler Rendering;

        public Singer(ref Lyric Lyric, PlayerCore np, MainWindow mw, bool dragMovable, bool windowmode)
        {
            WindowMode = windowmode;

            if (WindowMode)
            {
                window = new SingerWindow(dragMovable, mw);
                window.MinWidth = Lyric.MinWidth;
                window.MinHeight = Lyric.MinHeight;
                window.Closed += delegate
                {
                    Closed?.Invoke(this, null);
                };

                control = window.control;
            }
            else
            {
                control = new SingerControl();
                control.MinWidth = Lyric.MinWidth;
                control.MinHeight = Lyric.MinHeight;
            }

            control.Background = Lyric.Background;

            sw = new Stopwatch();
            sw.Start();

            this.Lyric = Lyric;
            this.np = np;
            this.mw = mw;

            this.np = np;
            this.np.PlayStarted += Np_PlaySeeked;
            this.np.PlaySeeked += Np_PlaySeeked;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        public void Init(ref Lyric Lyric)
        {
            grid.Children.Clear();

            this.Lyric = Lyric;

            stopped = false;
        }

        public void Show(Window Owner)
        {
            if (WindowMode)
            {
                window.Show(Owner);
            }
            else
            {
                control.Margin = Margin;
                control.UseLayoutRounding = true;
                mw.Grid_Lyric.Children.Add(control);
                control.SizeChanged += Control_SizeChanged;
                mw.SizeChanged += Mw_SizeChanged;
                Control_SizeChanged(this,null);
                control.IsHitTestVisible = false;
            }

            SetScreen(scrIndex);
        }

        private void Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            control.MinWidth = Math.Min(mw.ActualWidth - Margin.Left - Margin.Right, Lyric.MinWidth);
            control.MinHeight = Math.Min(mw.ActualHeight - Margin.Top - Margin.Bottom, Lyric.MinHeight);
        }

        private void Mw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            control.MinWidth = Math.Min(e.NewSize.Width - Margin.Left - Margin.Right, Lyric.MinWidth);
            control.MinHeight = Math.Min(e.NewSize.Height - Margin.Top - Margin.Bottom, Lyric.MinHeight);
        }

        public void SetScreen(int index)
        {
            scrIndex = index;

            if(window != null)
            {
                window.SetScreen(index);
            }
        }

        #region Stop Dispose

        public void Stop()
        {
            grid.Children.Clear();

            stopped = true;
        }

        public void Dispose()
        {
            np.PlayStarted -= Np_PlaySeeked;
            np.PlaySeeked -= Np_PlaySeeked;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            Stop();

            if(sw != null)
            {
                sw.Stop();
                sw = null;
            }

            if(window != null)
            {
                window.Close();
            }

            if(control != null && !WindowMode)
            {
                mw.Grid_Lyric.Children.Remove(control);
                mw.SizeChanged -= Mw_SizeChanged;
                control.SizeChanged -= Control_SizeChanged;
                control.IsHitTestVisible = true;
            }
        }

        #endregion Stop Dispose

        #region Update

        double preFrame = 0;

        public void Refresh()
        {
            grid.Children.Clear();

            Np_PlaySeeked();
        }

        private void Np_PlaySeeked()
        {
            _position = np.GetPosition(TimeUnit.MilliSecond);

            if (stopped)
                return;

            grid.Children.Clear();

            for (int i=0; i< Lyric.Lines.Count; i++)
            {
                LyricLine line = Lyric.Lines[i];

                line.View = false;

                if (line.Position < _position)
                {
                    line.View = true;

                    double duration = 0;

                    if (line.Duration >= 0)
                    {
                        duration = line.Duration;
                    }
                    else
                    {
                        if (i < Lyric.Lines.Count - 1)
                        {
                            duration = Lyric.Lines[i + 1].Position - line.Position;
                        }
                        else
                        {
                            duration = np.GetLength(TimeUnit.MilliSecond) - line.Position;
                        }
                    }

                    if (line.Position + duration >= _position)
                    {
                        grid.Children.Add(new LyricLineRenderer(ref np, this, line, duration));
                    }
                }

                Lyric.Lines[i] = line;
            }

            if (ResetPosition)
            {
                control.UpdateLayout();

                if (window != null)
                    window.SetScreen(scrIndex);
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (stopped)
                return;

            _position = np.GetPosition(TimeUnit.MilliSecond);

            if (np != null && np.isPlay && !np.isPaused && sw.ElapsedMilliseconds - preFrame > 33)
            {
                Rendering?.Invoke(this, null);

                int index = 0;
                while (true)
                {
                    if (index >= grid.Children.Count)
                    {
                        break;
                    }

                    if (((LyricLineRenderer)grid.Children[index]).isClosed)
                    {
                        ((LyricLineRenderer)grid.Children[index]).Stop();

                        grid.Children.RemoveAt(index);

                        if (ResetPosition && grid.Children.Count == 1)
                        {
                            control.UpdateLayout();

                            if (window != null)
                                window.SetScreen(scrIndex);
                        }
                    }
                    else
                    {
                        index++;
                    }
                }

                //Add
                for (int i = 0; i < Lyric.Lines.Count; i++)
                {
                    LyricLine line = Lyric.Lines[i];
                    if (!line.View && line.Position < _position)
                    {
                        double duration = 0;

                        if (line.Duration > 0)
                        {
                            duration = line.Duration;
                        }
                        else
                        {
                            if (i < Lyric.Lines.Count - 1)
                            {
                                duration = Lyric.Lines[i + 1].Position - line.Position;
                            }
                            else
                            {
                                duration = np.GetLength(TimeUnit.MilliSecond) - line.Position;
                            }
                        }
                        grid.Children.Add(new LyricLineRenderer(ref np, this, line, duration));

                        if (ResetPosition && grid.Children.Count >= 1)
                        {
                            control.UpdateLayout();

                            if (window != null)
                                window.SetScreen(scrIndex);
                        }

                        Lyric.Lines[i].View = true;
                    }
                }

                preFrame = sw.ElapsedMilliseconds;
            }
        }

        #endregion Update

    }
}
