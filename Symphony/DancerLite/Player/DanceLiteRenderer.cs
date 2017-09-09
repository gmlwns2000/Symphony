using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;
using System.Windows.Media;

namespace Symphony.DancerLite
{
    public class DanceLiteRenderer : IDisposable
    {
        MainWindow mw;
        PlayerCore np;
        PlotLite pl;

        bool showed = false;

        bool _windowMode = false;
        public bool WindowMode
        {
            get
            {
                return _windowMode;
            }
            set
            {
                if (_windowMode != value)
                {
                    _windowMode = value;

                    if (showed)
                    {
                        if (value)
                        {
                            control.Background = null;

                            mw.Grid_Dance.Children.Clear();

                            window = new DanceLiteWindow(control, mw, np);
                        }
                        else
                        {
                            if (window != null)
                            {
                                window.Close();
                                window = null;
                            }

                            control.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                            control.Background.Freeze();

                            mw.Grid_Dance.Children.Clear();
                            mw.Grid_Dance.Children.Add(control);
                        }
                    }
                }
            }
        }

        private bool _topmost = false;
        public bool Topmost
        {
            get
            {
                return _topmost;
            }
            set
            {
                _topmost = value;

                if (window != null)
                    window.Topmost = value;
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

                if (control != null)
                    control.Opacity = _opacity;
            }
        }

        public DanceLiteControl control;
        DanceLiteWindow window;

        public DanceLiteRenderer(MainWindow mw, PlayerCore np, PlotLite pl)
        {
            this.mw = mw;
            this.np = np;
            this.pl = pl;

            control = new DanceLiteControl(mw, np, pl);
        }

        public void Show()
        {
            showed = true;

            if (WindowMode)
            {
                control.Background = null;

                window = new DanceLiteWindow(control, mw, np);
            }
            else
            {
                control.Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
                control.Background.Freeze();

                mw.Grid_Dance.Children.Add(control);
            }
        }

        public void Close()
        {
            if (WindowMode)
            {
                window.Close();
                window = null;
            }
            else
            {
                mw.Grid_Dance.Children.Clear();
            }

            Dispose();
        }

        public void Dispose()
        {
            if(window != null)
            {
                window = null;
            }

            if(control != null)
            {
                if (!WindowMode)
                {
                    mw.Grid_Dance.Children.Clear();
                }

                control.Dispose();
                control = null;
            }
        }
    }
}
