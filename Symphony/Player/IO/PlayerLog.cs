using System;
using System.Diagnostics;
using System.Text;

namespace Symphony.Player
{
    public class PlayerLogger
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
            }
        }
        public string name = "nAudio";

        public static Stopwatch Stopwatch = new Stopwatch();

        private StringBuilder sb = new StringBuilder();

        public PlayerLogger(string Name = "nAudio")
        {
            debug = true;
            name = Name;
        }

        public PlayerLogger(bool debug)
        {
            this.debug = debug;
        }

        public void dlog(string msg)
        {
            if (debug)
            {
                string text = string.Format("[{0} ({1})] ({2}) Log: {3}", DateTime.Now.ToString("%h:%m:%s"), Stopwatch.ElapsedMilliseconds.ToString("0,0"), name, msg);

                Debug.WriteLine(text);

                sb.AppendLine(text);
            }
        }

        public void info(string msg)
        {
            if (debug)
            {
                string text = string.Format("[{0} ({1})] ({2}) Info: {3}", DateTime.Now.ToString("%h:%m:%s"), Stopwatch.ElapsedMilliseconds.ToString("0,0"), name, msg);

                Debug.WriteLine(text);

                sb.AppendLine(text);
            }
        }

        public void derr(string msg)
        {
            if (debug)
            {
                string text = string.Format("[{0} ({1})] ({2}) Error: {3}", DateTime.Now.ToString("%h:%m:%s"), Stopwatch.ElapsedMilliseconds.ToString("0,0"), name, msg);

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
            Stopwatch.Stop();
            stopped = true;
        }
    }
}
