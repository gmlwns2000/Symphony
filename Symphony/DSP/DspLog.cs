using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symphony.DSP
{
    public class LogUpdated : EventArgs
    {
        public string NewLine;

        public LogUpdated(string line)
        {
            NewLine = line;
        }
    }

    public static class DspLog
    {
        public static Queue<string> LogQueue = new Queue<string>(512);
        public static int MaxLogs = 512;
        public static event EventHandler<LogUpdated> Updated;

        public static void Log(DspWrapper sender, string msg)
        {
            string line = string.Format("[{0}] {1} - {2}", DateTime.Now.ToString(), sender.LuaFileName, msg );

            if (LogQueue.Count + 1 > MaxLogs)
            {
                LogQueue.Dequeue();
            }
            LogQueue.Enqueue(line);

            Updated?.Invoke(null, new LogUpdated(line));
        }

        public static void Error(DspWrapper sender, string msg)
        {
            string line = string.Format("[{0}] (ERRORED!) {1} - {2}", DateTime.Now.ToString(), sender.LuaFileName, msg);

            if (LogQueue.Count + 1 > MaxLogs)
            {
                LogQueue.Dequeue();
            }
            LogQueue.Enqueue(line);

            Updated?.Invoke(null, new LogUpdated(line));
        }
    }
}
