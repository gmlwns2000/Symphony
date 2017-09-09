using SlimDX.Direct2D;
using System.Diagnostics;

namespace MMF.Sprite.D2D
{
    public class D2DSpriteGradientStopCollection : System.IDisposable
    {
        public GradientStopCollection GradientStopCollection;

        private D2DSpriteBatch _batch;

        private GradientStop[] _stops;

        private Gamma _gamma;

        private ExtendMode _extendMode;

        public D2DSpriteGradientStopCollection(D2DSpriteBatch batch, GradientStop[] stops, Gamma gamma, ExtendMode mode)
        {
            batch.BatchDisposing += new System.EventHandler<System.EventArgs>(batch_BatchDisposing);
            _extendMode = mode;
            _gamma = gamma;
            _stops = stops;
            _batch = batch;
        }

        private void batch_BatchDisposing(object sender, System.EventArgs e)
        {
            Dispose();
        }

        ~D2DSpriteGradientStopCollection()
        {
            Debug.WriteLine("D2DSpriteGradientStopCollectionはIDisposableですが、Disposeされませんでした。");
            Dispose();
        }

        public void Dispose()
        {
            if (GradientStopCollection != null && !GradientStopCollection.Disposed)
            {
                GradientStopCollection.Dispose();
            }
            System.GC.SuppressFinalize(this);
        }
    }
}
