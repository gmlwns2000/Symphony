using System;
using DirectCanvas.Misc;

namespace DirectCanvas.Brushes
{
    public sealed class DrawingLayerBrush : Brush
    {
        private readonly DrawingLayer m_drawingLayer;
        private SlimDX.Direct2D.BitmapBrush m_internalBitmapBrush;

        internal DrawingLayerBrush(DrawingLayer drawingLayer)
        {
            m_drawingLayer = drawingLayer;
            m_internalBitmapBrush = new SlimDX.Direct2D.BitmapBrush(drawingLayer.D2DRenderTarget.InternalRenderTarget, 
                                                                    drawingLayer.D2DRenderTarget.InternalBitmap);
        }
        internal override SizeF BrushSize
        {
            get
            {
                var size = m_internalBitmapBrush.Bitmap.PixelSize;

                return new SizeF(size.Width, size.Height);
            }
        }

        protected override SlimDX.Direct2D.Brush GetInternalBrush()
        {
            return m_internalBitmapBrush;
        }

        public ExtendMode HorizontalExtendMode
        {
            get { return (ExtendMode)m_internalBitmapBrush.HorizontalExtendMode; }
            set { m_internalBitmapBrush.HorizontalExtendMode = (SlimDX.Direct2D.ExtendMode)value; }
        }

        public ExtendMode VerticalExtendMode
        {
            get { return (ExtendMode)m_internalBitmapBrush.VerticalExtendMode; }
            set { m_internalBitmapBrush.VerticalExtendMode = (SlimDX.Direct2D.ExtendMode)value; }
        }

        public override void Dispose()
        {
            if(m_internalBitmapBrush != null)
            {
                m_internalBitmapBrush.Dispose();
                m_internalBitmapBrush = null;
            }
        }

        ~DrawingLayerBrush()
        {
            Dispose();
        }
    }
}
