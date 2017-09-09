using System;
using System.Diagnostics;
using System.Text;

namespace NPlayer
{
    public class nPlayerLog
    {
        private bool stopped = false;

        private bool _debug;
        public bool debug
        {
            get
            {
                return _debug && !stopped;
            }
            set
            {
                _debug = value;

                if (value)
                {
                    if (!clock.IsRunning)
                    {
                        clock.Start();
                    }
                }
                else
                {
                    if (clock.IsRunning)
                    {
                        clock.Stop();
                    }
                }
            }
        }
        public string name = "nAudio";

        public Stopwatch clock = new Stopwatch();

        private StringBuilder sb = new StringBuilder();

        public nPlayerLog(string Name = "nAudio")
        {
            debug = true;
            name = Name;
        }

        public nPlayerLog(bool debug)
        {
            this.debug = debug;
        }

        public void dlog(string msg)
        {
            if (debug)
            {
                string text = string.Format("[{0} ({1})] ({2}) Log: {3}", DateTime.Now.ToString("%h:%m:%s"), clock.ElapsedMilliseconds.ToString("0,0"), name, msg);

                Debug.WriteLine(text);

                sb.AppendLine(text);
            }
        }

        public void info(string msg)
        {
            if (debug)
            {
                string text = string.Format("[{0} ({1})] ({2}) Info: {3}", DateTime.Now.ToString("%h:%m:%s"), clock.ElapsedMilliseconds.ToString("0,0"), name, msg);

                Debug.WriteLine(text);

                sb.AppendLine(text);
            }
        }

        public void derr(string msg)
        {
            if (debug)
            {
                string text = string.Format("[{0} ({1})] ({2}) Error: {3}", DateTime.Now.ToString("%h:%m:%s"), clock.ElapsedMilliseconds.ToString("0,0"), name, msg);

                Debug.WriteLine(text);

                sb.AppendLine(text);
            }
        }

        public string GetString()
        {
            return sb.ToString();
        }

        public void Stop()
        {
            clock.Stop();
            stopped = true;
        }
    }
}
