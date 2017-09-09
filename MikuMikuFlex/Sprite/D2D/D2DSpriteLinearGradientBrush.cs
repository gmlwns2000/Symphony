using SlimDX.Direct2D;
using System.Diagnostics;

namespace MMF.Sprite.D2D
{
    public class D2DSpriteLinearGradientBrush : System.IDisposable
    {
        public LinearGradientBrush Brush;

        private D2DSpriteBatch _batch;

        private D2DSpriteGradientStopCollection _gradientStops;

        private LinearGradientBrushProperties _linearGradientBrushProperties;

        public D2DSpriteLinearGradientBrush(D2DSpriteBatch batch, D2DSpriteGradientStopCollection gradientStops, LinearGradientBrushProperties lgbp)
        {
            _linearGradientBrushProperties = lgbp;
            _gradientStops = gradientStops;
            _batch = batch;
            batch.BatchDisposing += new System.EventHandler<System.EventArgs>(batch_BatchDisposing);
            Brush = new LinearGradientBrush(_batch.DWRenderTarget, _gradientStops.GradientStopCollection, _linearGradientBrushProperties);
        }

        ~D2DSpriteLinearGradientBrush()
        {
            Debug.WriteLine("D2DSpriteLinearGradientBrushはIDisposableですが、Disposeされませんでした。");
            Dispose();
        }

        private void batch_BatchDisposing(object sender, System.EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Brush != null && !Brush.Disposed)
            {
                Brush.Dispose();
            }
            System.GC.SuppressFinalize(this);
        }
    }
}
