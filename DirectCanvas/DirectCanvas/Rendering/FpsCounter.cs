/* Modified from SlimDx sample */

using System;
using System.Diagnostics;

namespace DirectCanvas.Rendering
{
    internal class FpsCounter
    {
        private float m_frameDelta;
        private float m_frameCount;
        private float m_frameAccumulator;
        private float m_framesPerSecond;
        private Clock m_clock;

        public FpsCounter()
        {
            m_clock = new Clock();
        }

        public float FramesPerSecond
        {
            get { return m_framesPerSecond; }
        }

        public TimeSpan FrameDelta
        {
            get { return TimeSpan.FromSeconds(m_frameDelta); }
        }

        public void Start()
        {
            m_clock.Start();    
        }

        public void Stop()
        {
            m_frameDelta = 0;
            m_frameCount = 0;
            m_frameAccumulator = 0;
            m_framesPerSecond = 0;
            m_clock.Stop();
        }

        public void Update()
        {
            m_frameDelta = m_clock.Update();
            m_frameAccumulator += m_frameDelta;

            ++m_frameCount;
            if (m_frameAccumulator >= 1.0f)
            {
                m_framesPerSecond = m_frameCount / m_frameAccumulator;

                m_frameAccumulator = 0.0f;
                m_frameCount = 0;
            }
        }

        private class Clock
        {
            private bool m_isRunning;
            private readonly long m_clockFrequency;
            private long m_tickCount;

            public Clock()
            {
                m_clockFrequency = Stopwatch.Frequency;
            }

            public void Start()
            {
                m_tickCount = Stopwatch.GetTimestamp();
                m_isRunning = true;
            }

            public void Stop()
            {
                m_tickCount = 0;
                m_isRunning = false;
                Stopwatch.StartNew();
            }

            public float Update()
            {
                float result = 0.0f;
                if (m_isRunning)
                {
                    long last = m_tickCount;
                    m_tickCount = Stopwatch.GetTimestamp();
                    result = (float)(m_tickCount - last) / m_clockFrequency;
                }

                return result;
            }
        }
    }
}
