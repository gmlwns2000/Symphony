using System;
using System.IO;
using System.Runtime.InteropServices;
using DirectCanvas.Imaging.WIC;
using DirectCanvas.Misc;
using SlimDX;
using SlimDX.Direct3D10;
using FileAccess = DirectCanvas.Imaging.WIC.FileAccess;
using MapFlags = SlimDX.Direct3D10.MapFlags;

namespace DirectCanvas.Imaging
{
    public class Image : IDisposable
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, int length);

        private readonly DirectCanvasFactory m_directCanvasFactory;
        private IWICBitmap m_internalBitmap;
        private readonly string m_filename;

        internal Image(IWICBitmap internalWicBitmap)
        {
            m_internalBitmap = internalWicBitmap;
            Initialize();
        }

        internal Image(string filename, DirectCanvasFactory directCanvasFactory)
        {
            m_filename = filename;
            m_directCanvasFactory = directCanvasFactory;

            IWICBitmapDecoder decoder = null;

            var factory = m_directCanvasFactory.WicImagingFactory;

            var vendorGuid = Guid.Empty;
            int hr = factory.CreateDecoderFromFilename(m_filename, 
                                                       ref vendorGuid, 
                                                       FileAccess.GenericRead, 
                                                       WICDecodeOptions.WICDecodeMetadataCacheOnLoad, 
                                                       out decoder);

            if (hr != 0)
                throw new Exception(string.Format("Could not create the decoder from filename {0}", m_filename));

            Initialize(decoder);
        }

        internal Image(int width, int height, DirectCanvasFactory directCanvasFactory)
        {
            m_directCanvasFactory = directCanvasFactory;
            var factory = m_directCanvasFactory.WicImagingFactory;

            int hr = factory.CreateBitmap(width, 
                                          height, 
                                          ref WICFormats.WICPixelFormat32bppBGRA,
                                          WICBitmapCreateCacheOption.WICBitmapCacheOnLoad, 
                                          out m_internalBitmap);

            if (hr != 0)
                throw new Exception(string.Format("Could not create bitmap"));

            Initialize();
        }

        internal Image(Stream stream, DirectCanvasFactory directCanvasFactory)
        {
            m_directCanvasFactory = directCanvasFactory;
          
            var factory = m_directCanvasFactory.WicImagingFactory;

            var nativeStream = new NativeStream(stream);

            var vendorGuid = Guid.Empty;
            IWICBitmapDecoder decoder;

            int hr = factory.CreateDecoderFromStream(nativeStream,
                                                     ref vendorGuid,
                                                     WICDecodeOptions.WICDecodeMetadataCacheOnLoad,
                                                     out decoder);

            if (hr != 0)
               throw new Exception(string.Format("Could not create the decoder from stream {0}", hr));

            Initialize(decoder);
        }

        public ImageData Lock(ImageLock lockType)
        {
            return Lock(new Rectangle(0, 0, Width, Height), lockType);
        }

        public ImageData Lock(Rectangle lockRectangle, ImageLock lockType)
        {
            IWICBitmapLock imageLock;
            WICBitmapLockFlags flags;

            var rect = new WICRect
            {
                X = lockRectangle.X,
                Y = lockRectangle.Y,
                Width = lockRectangle.Width,
                Height = lockRectangle.Height
            };
           
            switch(lockType)
            {
                case ImageLock.Read:
                    flags = WICBitmapLockFlags.WICBitmapLockRead;
                    break;
                case ImageLock.Write:
                    flags = WICBitmapLockFlags.WICBitmapLockWrite;
                    break;
                case ImageLock.ReadWrite:
                    flags = WICBitmapLockFlags.WICBitmapLockRead | WICBitmapLockFlags.WICBitmapLockWrite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("lockType");
            }

            int hr = m_internalBitmap.Lock(ref rect,
                                          flags,
                                          out imageLock);

            if (hr != 0)
                throw new Exception("Could not lock the image");

            return new ImageData(imageLock);
        }

        private void Initialize(IWICBitmapDecoder decoder)
        {
            Exception exception = null;

            IWICBitmapFrameDecode source = null;
            IWICFormatConverter converter = null;
            var factory = m_directCanvasFactory.WicImagingFactory;
            int hr = 0;

            hr = decoder.GetFrame(0, out source);

            if (hr != 0)
            {
                exception = new Exception(string.Format("Could not get the frame from the decoder."));
                goto cleanup;
            }

            hr = factory.CreateFormatConverter(out converter);

            if (hr != 0)
            {
                exception = new Exception(string.Format("Could not create the format converter"));
                goto cleanup;
            }

            hr = converter.Initialize(source,
                                      ref WICFormats.WICPixelFormat32bppPBGRA,
                                      WICBitmapDitherType.WICBitmapDitherTypeNone,
                                      null,
                                      0,
                                      WICBitmapPaletteType.WICBitmapPaletteTypeMedianCut);

            if (hr != 0)
            {
                exception = new Exception(string.Format("Could not initialize the converter"));
                goto cleanup;
            }

            bool canConvert;
            hr = converter.CanConvert(ref WICFormats.WICPixelFormat32bppPBGRA,
                                      ref WICFormats.WICPixelFormat32bppBGRA,
                                      out canConvert);

            if (hr != 0)
            {
                exception = new Exception(string.Format("Could not convert color formats"));
                goto cleanup;
            }

            hr = factory.CreateBitmapFromSource(converter,
                                                WICBitmapCreateCacheOption.WICBitmapCacheOnLoad,
                                                out m_internalBitmap);

            if (hr != 0)
            {
                exception = new Exception(string.Format("Could not create bitmap"));
                goto cleanup;
            }

        cleanup:

            if (converter != null)
                Marshal.ReleaseComObject(converter);

            if (source != null)
                Marshal.ReleaseComObject(source);

            if (decoder != null)
                Marshal.ReleaseComObject(decoder);

            if (exception != null)
                throw exception;
            else
                Initialize();
        }

        private void Initialize()
        {
            uint width, height;

            m_internalBitmap.GetSize(out width, out height);

            Width = (int)width;

            Height = (int)height;
        }

        public void Unlock(ImageData imageData)
        {
            imageData.Unlock();
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        private static Guid ImageFormatToWicContainerFormat(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.Bitmap:
                    return WICContainerFormats.ContainerFormatBmp;
                case ImageFormat.Png:
                    return WICContainerFormats.ContainerFormatPng;
                case ImageFormat.Jpeg:
                    return WICContainerFormats.ContainerFormatJpeg;
                case ImageFormat.Gif:
                    return WICContainerFormats.ContainerFormatGif;
                case ImageFormat.Tiff:
                    return WICContainerFormats.ContainerFormatTiff;
                case ImageFormat.Wmp:
                    return WICContainerFormats.ContainerFormatWmp;
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }

        /// <summary>
        /// Saves an image to a stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="format">The image format to use</param>
        public void Save(Stream stream, ImageFormat format)
        {
            /* Get the wic factory from our direct canvas factory */
            var factory = m_directCanvasFactory.WicImagingFactory;

            /* The WIC encoder interface */
            IWICBitmapEncoder encoder = null;
            IWICBitmapFrameEncode frame = null;
            Exception exception = null;

            /* The area of the image we are going to encode */
            var rect = new WICRect() { X = 0, Y = 0, Width = Width, Height = Height };

            /* We don't prefer any vendor */
            Guid vendor = Guid.Empty;

            /* Get the WIC Guid for the format we want to use */
            Guid wicContainerFormat = ImageFormatToWicContainerFormat(format);

            /* Create our encoder */
            int hr = factory.CreateEncoder(ref wicContainerFormat, 
                                           ref vendor, 
                                           out encoder);

            if (hr != 0)
            {
                exception = new Exception("Could not create the encoder");
                goto cleanup;
            }

            /* Here we wrap our .NET stream with a COM IStream */
            var nativeStream = new NativeStream(stream);

            /* This intializes the encoder */
            hr = encoder.Initialize(nativeStream, WICBitmapEncoderCacheOption.WICBitmapEncoderNoCache);
            if(hr != 0)
            {
                exception = new Exception("Could not initialize the encoder");
                goto cleanup;
            }

            IntPtr options = IntPtr.Zero;

            /* Creates a new frame to be encoded */
            hr = encoder.CreateNewFrame(out frame, ref options);

            if (hr != 0)
            {
                exception = new Exception("Could not create a new frame for the encoder");
                goto cleanup;
            }

            /* This intializes with the defaults */
            hr = frame.Initialize(IntPtr.Zero);

            if (hr != 0)
            {
                exception = new Exception("Could not initialize the encoded frame");
                goto cleanup;
            }

            /* Sets the size of the frame */
            hr = frame.SetSize(Width, Height);

            if (hr != 0)
            {
                exception = new Exception("Could not set the size of the encode frame");
                goto cleanup;
            }

            hr = frame.SetPixelFormat(ref WICFormats.WICPixelFormatDontCare);

            /* Write our WIC bitmap source */
            hr = frame.WriteSource(m_internalBitmap, ref rect);

            if (hr != 0)
            {
                exception = new Exception("Could not write the bitmap source to the frame");
                goto cleanup;
            }

            /* Commits all our changes to the frame  */
            hr = frame.Commit();
            if (hr != 0)
            {
                exception = new Exception("Could not commit the frame");
                goto cleanup;
            }

            /* Actually does all the encoding */
            hr = encoder.Commit();

            if (hr != 0)
            {
                exception = new Exception("Could not commit the encoder");
                goto cleanup;
            }

        cleanup:

            if(frame != null)
                Marshal.ReleaseComObject(frame);    
            
            if(encoder != null)
                Marshal.ReleaseComObject(encoder);    

            if (exception != null)
                throw exception;
        }

        /// <summary>
        /// Saves an image to a file
        /// </summary>
        /// <param name="file">The file to write to</param>
        /// <param name="format">The image format to use</param>
        public void Save(string file, ImageFormat format)
        {
            using(var stream = File.Open(file, 
                                         FileMode.OpenOrCreate, 
                                         System.IO.FileAccess.Write, 
                                         FileShare.ReadWrite))
            {
                Save(stream, format);
                stream.Close();
            }
        }

        public void CopyFromDrawingLayer(DrawingLayer drawingLayer)
        {
            if(!drawingLayer.EnableImageCopy)
                throw new Exception("EnableImageCopy must be set to true in order to copy from the DrawingLayer");

            var device = m_directCanvasFactory.DeviceContext.Device;

            var stagingTexture = drawingLayer.SystemMemoryTexture;

            /* Copies our GPU texture to our system memory texture.  This does happen
             * asyncronously UNLESS you call map, which we do afterwards :) */
            device.CopyResource(drawingLayer.RenderTargetTexture.InternalTexture2D,
                                stagingTexture.InternalTexture2D);

            /* Map the texture, so we get an exclusive lock on the pixels */
            var dataRect = stagingTexture.InternalTexture2D.Map(0, MapMode.Read, MapFlags.None);

            /* We lock our image class so we have access to the pixel buffer */
            var imgData = Lock(ImageLock.Write);
            
            CopyTextureDataToImage(imgData, dataRect);

            Unlock(imgData);

            stagingTexture.InternalTexture2D.Unmap(0);
        }

        private void CopyTextureDataToImage(ImageData destination, DataRectangle source)
        {
            int pitch = source.Pitch;

            /* Todo: be portable to 64bit :) */
            int destDataPointer = destination.Scan0.ToInt32();
            int sourceDataPointer = source.Data.DataPointer.ToInt32();

            int rowByteSize = destination.Stride;

            int sourceByteOffset = 0;
            for (int y = 0; y < Height; ++y)
            {
                var destPointer = new IntPtr(destDataPointer + (y * rowByteSize));
                var sourcePointer = new IntPtr(sourceDataPointer + sourceByteOffset);

                CopyMemory(destPointer, sourcePointer, rowByteSize);

                sourceByteOffset += rowByteSize + (pitch - rowByteSize);
            }
        }

        internal void CopyToDrawingLayer(DrawingLayer drawingLayer, Rectangle destRect)
        {
            IntPtr pBuffer;
            int bufferSize;
            int stride = 0;
            int hr = 0;

            var rect = new WICRect
            {
                X = 0,
                Y = 0,
                Width = Width,
                Height = Height
            };

            IWICBitmapLock imageLock;

            hr = m_internalBitmap.Lock(ref rect,
                                       WICBitmapLockFlags.WICBitmapLockRead,
                                       out imageLock);

            if(hr != 0)
                throw new Exception("Could not lock the image");

            hr = imageLock.GetDataPointer(out bufferSize, out pBuffer);

            imageLock.GetStride(out stride);

            if(destRect.IsEmpty)
                drawingLayer.D2DRenderTarget.InternalBitmap.FromMemory(pBuffer, stride);
            else
                drawingLayer.D2DRenderTarget.InternalBitmap.FromMemory(pBuffer, 
                                                                       stride, 
                                                                       destRect.GetInternalRect());

            Marshal.ReleaseComObject(imageLock);
        }

        internal void CopyToDrawingLayer(DrawingLayer drawingLayer)
        {
            CopyToDrawingLayer(drawingLayer, Rectangle.Empty);
        }

        public Image Clone()
        {
            int hr = 0;
            var factory = m_directCanvasFactory.WicImagingFactory;

            IWICBitmap cloneWicBitmap;
            hr = factory.CreateBitmapFromSource(m_internalBitmap,
                                               WICBitmapCreateCacheOption.WICBitmapCacheOnLoad,
                                               out cloneWicBitmap);

            return new Image(cloneWicBitmap);
        }

        public void Dispose()
        {
            if (m_internalBitmap != null)
            {
                Marshal.ReleaseComObject(m_internalBitmap);
                m_internalBitmap = null;
            }
        }
    }
}