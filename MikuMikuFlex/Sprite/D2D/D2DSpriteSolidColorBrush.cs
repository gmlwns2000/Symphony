using SlimDX;
using SlimDX.Direct2D;
using System.Diagnostics;

namespace MMF.Sprite.D2D
{
    public class D2DSpriteSolidColorBrush : System.IDisposable
    {
        private readonly D2DSpriteBatch batch;

        private Color4 color;

        public Color4 Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                if (Brush != null)
                {
                    Brush.Color = color;
                }
            }
        }

        public SolidColorBrush Brush
        {
            get;
            private set;
        }

        internal D2DSpriteSolidColorBrush(D2DSpriteBatch batch, Color4 color)
        {
            this.batch = batch;
            this.color = color;
            batch.BatchDisposing += new System.EventHandler<System.EventArgs>(batch_BatchDisposing);
            Brush = new SolidColorBrush(batch.DWRenderTarget, color);
        }

        public void Dispose()
        {
            if (Brush != null && !Brush.Disposed)
            {
                Brush.Dispose();
            }
            batch.BatchDisposing -= new System.EventHandler<System.EventArgs>(batch_BatchDisposing);
            System.GC.SuppressFinalize(this);
        }

        private void batch_BatchDisposing(object sender, System.EventArgs e)
        {
            Dispose();
        }

        ~D2DSpriteSolidColorBrush()
        {
            if (Brush != null && !Brush.Disposed)
            {
                Brush.Dispose();
            }
            Debug.WriteLine("D2DSpriteSolidColorBrushはDisposableですがDisposeされませんでした。");
        }

        public static implicit operator SolidColorBrush(D2DSpriteSolidColorBrush brush)
        {
            return brush.Brush;
        }
    }
}
