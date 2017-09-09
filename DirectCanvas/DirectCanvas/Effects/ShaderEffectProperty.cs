using System;

namespace DirectCanvas.Effects
{
    abstract class ShaderEffectProperty : IDisposable
    {
        private readonly ShaderEffect m_effect;
        private object m_value;

        internal ShaderEffectProperty(ShaderEffect effect, int register)
        {
            m_effect = effect;
            Register = register;
        }

        public object Value
        {
            get { return m_value; }
            set
            {
                object oldValue = m_value;
                m_value = value;
                if (m_value != oldValue)
                {
                    ValueChanged(oldValue, m_value);
                }
            }
        }

        protected abstract void ValueChanged(object oldValue, object newValue);

        public abstract void SetRenderState();

        public int Register { get; private set; }

        protected ShaderEffect Effect
        {
            get { return m_effect; }
        }

        public virtual void Dispose()
        {
            
        }
    }
}