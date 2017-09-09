using SlimDX.Direct2D;
using System.Diagnostics;

namespace MMF.Sprite.D2D
{
    public class D2DSpriteBitmapBrush : System.IDisposable
    {
        public BitmapBrush Brush;

        private BitmapBrushProperties _bitmapBrushProperties;

        private D2DSpriteBatch _batch;

        private D2DSpriteBitmap _bitmap;

        public D2DSpriteBitmapBrush(D2DSpriteBatch batch, D2DSpriteBitmap bitmap, BitmapBrushProperties bbp)
        {
            _bitmap = bitmap;
            _batch = batch;
            _bitmapBrushProperties = bbp;
            Brush = new BitmapBrush(batch.DWRenderTarget, _bitmap, bbp);
            _batch.BatchDisposing += new System.EventHandler<System.EventArgs>(_batch_BatchDisposing);
        }

        private void _batch_BatchDisposing(object sender, System.EventArgs e)
        {
            Dispose();
        }

        ~D2DSpriteBitmapBrush()
        {
            Debug.WriteLine("D2DSpriteBitmapBrushはIDisposableですが、Disposeされませんでした。");
            if (Brush != null && !Brush.Disposed)
            {
                Brush.Dispose();
            }
        }

        public void Dispose()
        {
            if (Brush != null && !Brush.Disposed)
            {
                Brush.Dispose();
            }
            System.GC.SuppressFinalize(this);
        }

        public static implicit operator BitmapBrush(D2DSpriteBitmapBrush brush)
        {
            return brush.Brush;
        }
    }
}
