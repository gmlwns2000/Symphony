using DirectCanvas.Misc;

namespace DirectCanvas.Brushes
{
    public class SolidColorBrush : Brush
    {
        private readonly Direct2DRenderTarget m_renderTargetOwner;
        private SlimDX.Direct2D.SolidColorBrush m_internalSolidColorBrush;
        private Color4 m_color;
        
        internal SolidColorBrush(Direct2DRenderTarget renderTargetOwner, Color4 color)
        {
            m_renderTargetOwner = renderTargetOwner;
            m_internalSolidColorBrush = new SlimDX.Direct2D.SolidColorBrush(m_renderTargetOwner.InternalRenderTarget, color.InternalColor4);
            Color = color;
        }

        internal override SizeF BrushSize
        {
            get
            {
                return new SizeF(10,10);
            }
        }

        protected override SlimDX.Direct2D.Brush GetInternalBrush()
        {
            return m_internalSolidColorBrush;
        }

        public override float Opacity
        {
            get { return m_color.A; }
            set
            {
                m_color.A = value;
                m_internalSolidColorBrush.Color = m_color.InternalColor4;
            }
        }

        public Color4 Color
        {
            get { return m_color; }
            set
            {
                m_color = value;
                m_internalSolidColorBrush.Color = m_color.InternalColor4;
            }
        }

        public override void Dispose()
        {
            if(m_internalSolidColorBrush != null)
            {
                m_internalSolidColorBrush.Dispose();
                m_internalSolidColorBrush = null;
            }
        }

        ~SolidColorBrush()
        {
            Dispose();
        }
    }
}
