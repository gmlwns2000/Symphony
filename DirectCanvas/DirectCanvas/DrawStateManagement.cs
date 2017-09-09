using System;

namespace DirectCanvas
{
    class DrawStateManagement
    {
        private bool m_beganDraw;

        public bool BeganDraw
        {
            get { return m_beganDraw; }
        }

        protected virtual void ValidateBeginDrawState()
        {
            if(BeganDraw)
                //throw new Exception("Begin draw already called");
                Console.WriteLine("Begin draw already called");
        }

        protected virtual void ValidateEndDrawState()
        {
            if (!BeganDraw)
                throw new Exception("End draw already called");
        }

        public virtual void DrawPreamble()
        {
            if (!BeganDraw)
                //throw new Exception("BeginDraw must be called first");
                Console.WriteLine("BeginDraw must be called first");
        }

        public virtual void BeginDrawState()
        {
            ValidateBeginDrawState();
            m_beganDraw = true;
        }

        public virtual void EndDrawState()
        {
            ValidateEndDrawState();
            m_beganDraw = false;
        }
    }
}
