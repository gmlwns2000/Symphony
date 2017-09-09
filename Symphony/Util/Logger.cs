using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony.Player;
using Symphony.Util;
using Symphony.UI;
using System.Diagnostics;

namespace Symphony
{
    public static class Logger
    {
        public static Stopwatch Stopwatch
        {
            get
            {
                return PlayerLogger.Stopwatch;
            }
        }

        private static PlayerLogger logger = new PlayerLogger("Symphony Global");
#if DEBUG
        private const bool IsDebug = true;
#else
        private const bool IsDebug = false;
#endif

        private const string LineSpliterThick = "======================================================";
        private const string LineSpliterThin =  "------------------------------------------------------";

        static Logger()
        {
            Console.WriteLine("Logger Inited. DebugMode: " + XmlHelper.Bool2String(IsDebug));

            logger.debug = IsDebug;
        }

        public static string GetText()
        {
            return logger.GetString();
        }

#pragma warning disable CS0162 // Unreachable code detected

        public static void Error(string msg)
        {
            if (!IsDebug)
                return;

            logger.derr(msg);
        }

        public static void Error(object sender, string msg)
        {
            if (!IsDebug)
                return;

            logger.derr(string.Format("From '{0}' : {1}", sender.ToString(), msg));
        }

        public static void Error(Exception e)
        {
            if (!IsDebug)
                return;

            logger.derr(string.Format("{0}\n\n{1}\n\n{0}", LineSpliterThick, e.ToString()));
        }

        public static void Error(object sender, Exception e)
        {
            if (!IsDebug)
                return;

            logger.derr(string.Format("{0}\n\n{3}\n\n{1}\n\n{2}\n\n{0}", LineSpliterThick, LineSpliterThin, sender.ToString(), e.ToString()));
        }

        public static void Info(string msg)
        {
            if (!IsDebug)
                return;

            logger.info(msg);
        }

        public static void Info(object sender, string msg)
        {
            if (!IsDebug)
                return;

            logger.info(string.Format("From '{0}' : {1}", sender.ToString(), msg));
        }

        public static void Info(string sender, string msg)
        {
            if (!IsDebug)
                return;

            logger.info(string.Format("From '{0}' : {1}", sender, msg));
        }

        public static void Log(string msg)
        {
            if (!IsDebug)
                return;

            logger.dlog(msg);
        }

        public static void Log(object sender, string msg)
        {
            if (!IsDebug)
                return;

            logger.dlog(string.Format("From '{0}' : {1}", sender.ToString(), msg));
        }

        public static void Log(string sender, string msg)
        {
            if (!IsDebug)
                return;

            logger.dlog(string.Format("From '{0}' : {1}", sender, msg));
        }

        public static void Show(string msg)
        {
            if (!IsDebug)
                return;

            Log(string.Format("DebugMessage Showed: {0}", msg));

            if (App.Current.MainWindow != null)
            {
                App.Current.MainWindow.Dispatcher.Invoke(new Action(() => 
                {
                    DialogMessage.Show(App.Current.MainWindow, msg);
                }));
            }
        }

#pragma warning restore CS0162 // Unreachable code detected

    }
}
