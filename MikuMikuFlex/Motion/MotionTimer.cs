using System.Diagnostics;

namespace MMF.Motion
{
    public class MotionTimer
    {
        private readonly RenderContext _context;

        public int MotionFramePerSecond = 30;

        public int TimerPerSecond = 300;

        public static Stopwatch stopWatch;

        private long lastMillisecound = 0L;

        public float ElapesedTime
        {
            get;
            private set;
        }

        static MotionTimer()
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        public MotionTimer(RenderContext context)
        {
            _context = context;
        }

        public void TickUpdater()
        {
            if (lastMillisecound == 0L)
            {
                lastMillisecound = stopWatch.ElapsedMilliseconds;
            }
            else
            {
                long elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                if (elapsedMilliseconds - lastMillisecound > 1000 / TimerPerSecond)
                {
                    ElapesedTime = elapsedMilliseconds - lastMillisecound;
                    lastMillisecound = stopWatch.ElapsedMilliseconds;
                    _context.UpdateWorlds();
                }
            }
        }
    }
}
