using System;
using DirectCanvas.Misc;
using DirectCanvas.Multimedia;

namespace DirectCanvas.Brushes
{
    public class VideoBrush : Brush
    {
        private readonly MediaPlayer m_player;
        private SlimDX.Direct2D.BitmapBrush m_bitmapBrush;
        private SlimDX.Direct2D.SolidColorBrush m_solidColorBrush;
        private ExtendMode m_horizontalExtendMode;
        private ExtendMode m_verticalExtendMode;

        internal VideoBrush(MediaPlayer player)
        {
            m_player = player;
            m_player.IsVideoReadyChanged += IsVideoReadyChangedCallback;
            
            SetBrush();
        }

        internal override SizeF BrushSize
        {
            get
            {
                if (m_player.IsVideoReady && m_bitmapBrush != null)
                {
                    var size = m_bitmapBrush.Bitmap.PixelSize;
                    return new SizeF(size.Width, size.Height);
                }
                else
                    return new SizeF(1,1);
            }
        }

        public ExtendMode HorizontalExtendMode
        {
            get { return m_horizontalExtendMode; }
            set
            {
                m_horizontalExtendMode = value;
                if (m_bitmapBrush != null)
                {
                    m_bitmapBrush.VerticalExtendMode = (SlimDX.Direct2D.ExtendMode)m_horizontalExtendMode;
                }
            }
        }

        public ExtendMode VerticalExtendMode
        {
            get { return m_verticalExtendMode; }
            set
            {
                m_verticalExtendMode = value;
                if(m_bitmapBrush != null)
                {
                    m_bitmapBrush.VerticalExtendMode = (SlimDX.Direct2D.ExtendMode)m_verticalExtendMode;
                }
            }
        }

        protected override SlimDX.Direct2D.Brush GetInternalBrush()
        {
            if (m_player.IsVideoReady && m_bitmapBrush != null)
                return m_bitmapBrush;
            else
                return m_solidColorBrush;
        }

        private void SetBrush()
        {
            if(InternalBrush != null)
                InternalBrush.Dispose();

            if (m_player.IsVideoReady)
            {
                m_bitmapBrush = new SlimDX.Direct2D.BitmapBrush(m_player.ResourceOwner.InternalRenderTarget,
                                                                m_player.InternalBitmap);

                m_bitmapBrush.HorizontalExtendMode = (SlimDX.Direct2D.ExtendMode)HorizontalExtendMode;
                m_bitmapBrush.VerticalExtendMode = (SlimDX.Direct2D.ExtendMode)VerticalExtendMode;
            }
            else
                m_solidColorBrush = new SlimDX.Direct2D.SolidColorBrush(m_player.ResourceOwner.InternalRenderTarget, 
                                                                        new Color4(1, 0, 0, 0).InternalColor4);
        }

        private void IsVideoReadyChangedCallback(object sender, EventArgs e)
        {
            SetBrush();
        }

        public override void Dispose()
        {
            if(m_bitmapBrush != null)
            {
                m_bitmapBrush.Dispose();
                m_bitmapBrush = null;
            }

            if(m_solidColorBrush != null)
            {
                m_solidColorBrush.Dispose();
                m_solidColorBrush = null;
            }

            if(m_player != null)
            {
                m_player.IsVideoReadyChanged -= IsVideoReadyChangedCallback;
            }
        }

        ~VideoBrush()
        {
            Dispose();
        }
    }
}