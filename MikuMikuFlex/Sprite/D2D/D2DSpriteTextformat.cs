using SlimDX.DirectWrite;
using System.Diagnostics;

namespace MMF.Sprite.D2D
{
    public class D2DSpriteTextformat : System.IDisposable
    {
        public TextFormat Format
        {
            get;
            private set;
        }

        internal D2DSpriteTextformat(D2DSpriteBatch batch, string fontFamiry, int size, FontWeight weight, FontStyle style, FontStretch stretch, string locale)
        {
            batch.BatchDisposing += new System.EventHandler<System.EventArgs>(batch_BatchDisposing);
            Format = new TextFormat(batch.context.DWFactory, fontFamiry, weight, style, stretch, size, locale);
        }

        private void batch_BatchDisposing(object sender, System.EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (Format != null && !Format.Disposed)
            {
                Format.Dispose();
            }
            System.GC.SuppressFinalize(this);
        }

        ~D2DSpriteTextformat()
        {
            Debug.WriteLine("D2DSpriteTextformatはDisposableですがDisposeされませんでした。");
            if (Format != null && !Format.Disposed)
            {
                Format.Dispose();
            }
        }

        public static implicit operator TextFormat(D2DSpriteTextformat format)
        {
            return format.Format;
        }
    }
}
