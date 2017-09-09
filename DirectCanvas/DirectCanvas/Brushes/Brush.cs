using System;
using DirectCanvas.Misc;
using DirectCanvas.Transforms;

namespace DirectCanvas.Brushes
{
    public abstract class Brush : IDisposable
    {
        private float m_opacity;

        public BrushAlignment Alignment { get; set; }

        internal abstract SizeF BrushSize { get; }

        public GeneralTransform Transform { get; set; }

        internal Brush()
        {
            Alignment = BrushAlignment.DrawingLayerAbsolute;
        }

        internal SlimDX.Direct2D.Brush InternalBrush
        {
            get
            {
                return GetInternalBrush();
            }
        }

        protected abstract SlimDX.Direct2D.Brush GetInternalBrush();

        public virtual float Opacity
        {
            get
            {
                return m_opacity;
            }
            set
            {
                m_opacity = value;
                InternalBrush.Opacity = m_opacity;
            }
        }

        public abstract void Dispose();
    }
}
