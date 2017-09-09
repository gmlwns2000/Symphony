using System;
using System.Runtime.InteropServices;
using DirectCanvas.Imaging.WIC;

namespace DirectCanvas.Imaging
{
    public class ImageData
    {
        private IWICBitmapLock m_imageLock;

        internal ImageData(IWICBitmapLock imageLock)
        {
            m_imageLock = imageLock;
            Initialize();
        }

        internal void Unlock()
        {
            if(m_imageLock != null)
            {
                Marshal.ReleaseComObject(m_imageLock);
                m_imageLock = null;
            }

            Stride = 0;
            BufferSize = 0;
            Scan0 = IntPtr.Zero;
        }

        private void Initialize()
        {
            IntPtr pBuffer;
            int bufferSize;
            int stride = 0;

            int hr = m_imageLock.GetDataPointer(out bufferSize, out pBuffer);

            m_imageLock.GetStride(out stride);

            Scan0 = pBuffer;
            BufferSize = bufferSize;
            Stride = stride;
        }

        public IntPtr Scan0 { get; private set; }

        public int Stride { get; private set; }

        public int BufferSize { get; private set; }
    }
}