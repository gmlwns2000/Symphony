using System.Collections.Generic;
using System.Timers;

namespace MMF.Utility
{
    public class FPSCounter
    {
        private bool isCached;

        private float cachedFPS;

        private Queue<int> frameHistory
        {
            get;
            set;
        }

        private int counter
        {
            get;
            set;
        }

        public Timer FpsTimer
        {
            get;
            private set;
        }

        public int AvarageSpan
        {
            get;
            set;
        }

        public float FPS
        {
            get
            {
                float result;
                if (!isCached)
                {
                    int num = 0;
                    foreach (int current in frameHistory)
                    {
                        num += current;
                    }
                    cachedFPS = num / (float)frameHistory.Count;
                    isCached = true;
                    result = cachedFPS;
                }
                else
                {
                    result = cachedFPS;
                }
                return result;
            }
        }

        public FPSCounter()
        {
            frameHistory = new Queue<int>();
            AvarageSpan = 10;
            FpsTimer = new Timer(1000.0);
            FpsTimer.Elapsed += new ElapsedEventHandler(Tick);
        }

        public void Start()
        {
            counter = 0;
            FpsTimer.Start();
        }

        public void CountFrame()
        {
            counter++;
        }

        private void Tick(object sender, ElapsedEventArgs args)
        {
            if (frameHistory.Count > AvarageSpan)
            {
                frameHistory.Dequeue();
            }
            frameHistory.Enqueue(counter);
            counter = 0;
            isCached = false;
        }
    }
}