using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPlayer
{
    public static class Logger
    {
        public static nPlayerLog Inner;
        private static bool _isDebug = true;
        public static bool IsDebug
        {
            get
            {
                return _isDebug;
            }
            set
            {
                _isDebug = value;
                Inner.debug = value;
            }
        }

        static Logger()
        {
            Inner = new nPlayerLog(true);
        }

        public static void Log(string msg)
        {
            Inner.dlog(msg);
        }

        public static void Error(string msg)
        {
            Inner.derr(msg);
        }

        public static void Info(string msg)
        {
            Inner.info(msg);
        }
    }
}
