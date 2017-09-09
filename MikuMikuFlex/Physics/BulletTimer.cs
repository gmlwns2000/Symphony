using System.Diagnostics;

namespace MMF.Physics
{
    internal class BulletTimer
    {
        private Stopwatch stopWatch = new Stopwatch();

        private long lastTime = 0L;

        public BulletTimer()
        {
            stopWatch.Start();
        }

        public long GetElapsedTime()
        {
            long elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            long result = elapsedMilliseconds - lastTime;
            lastTime = elapsedMilliseconds;
            return result;
        }
    }
}
