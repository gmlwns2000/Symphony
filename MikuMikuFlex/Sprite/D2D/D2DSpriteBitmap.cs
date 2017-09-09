using SlimDX;
using SlimDX.Direct2D;
using SlimDX.DXGI;
using System.Diagnostics;

namespace MMF.Sprite.D2D
{
    public class D2DSpriteBitmap : System.IDisposable
    {
        private System.Drawing.Bitmap orgBitmap;

        private D2DSpriteBatch batch;

        public Bitmap SpriteBitmap
        {
            get;
            private set;
        }

        ~D2DSpriteBitmap()
        {
            Debug.WriteLine("D2DSpriteBitmapはIDisposableですが、Disposeされませんでした。");
            if (SpriteBitmap != null && !SpriteBitmap.Disposed)
            {
                SpriteBitmap.Dispose();
            }
        }

        internal D2DSpriteBitmap(D2DSpriteBatch batch, System.IO.Stream fs)
        {
            this.batch = batch;
            batch.BatchDisposing += new System.EventHandler<System.EventArgs>(batch_BatchDisposing);
            orgBitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromStream(fs);
            CreateBitmap();
        }

        private void batch_BatchDisposing(object sender, System.EventArgs e)
        {
            Dispose();
        }

        private void CreateBitmap()
        {
            System.Drawing.Imaging.BitmapData bitmapData = orgBitmap.LockBits(new System.Drawing.Rectangle(0, 0, orgBitmap.Width, orgBitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            using (DataStream dataStream = new DataStream(bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, true, false))
            {
                SlimDX.Direct2D.PixelFormat pixelFormat = new SlimDX.Direct2D.PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied);
                BitmapProperties properties = default(BitmapProperties);
                properties.HorizontalDpi = (properties.VerticalDpi = 96f);
                properties.PixelFormat = pixelFormat;
                if (SpriteBitmap != null && !SpriteBitmap.Disposed)
                {
                    SpriteBitmap.Dispose();
                }
                SpriteBitmap = new Bitmap(batch.DWRenderTarget, new System.Drawing.Size(orgBitmap.Width, orgBitmap.Height), dataStream, bitmapData.Stride, properties);
                orgBitmap.UnlockBits(bitmapData);
            }
        }

        public void Dispose()
        {
            if (SpriteBitmap != null && !SpriteBitmap.Disposed)
            {
                SpriteBitmap.Dispose();
            }
            System.GC.SuppressFinalize(this);
        }

        public static implicit operator Bitmap(D2DSpriteBitmap bitmap)
        {
            return bitmap.SpriteBitmap;
        }
    }
}