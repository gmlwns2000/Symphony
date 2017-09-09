using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DirectCanvas.Imaging.WIC
{
    [Flags]
    enum WICDecodeOptions 
    {
        WICDecodeMetadataCacheOnDemand   = 0x00000000,
        WICDecodeMetadataCacheOnLoad     = 0x00000001 
    }

    [Flags]
    public enum FileAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000
    }

    [Flags]
    enum WICBitmapDitherType 
    {
        WICBitmapDitherTypeNone = 0x00000000,
        WICBitmapDitherTypeSolid = 0x00000000,
        WICBitmapDitherTypeOrdered4x4 = 0x00000001,
        WICBitmapDitherTypeOrdered8x8 = 0x00000002,
        WICBitmapDitherTypeOrdered16x16 = 0x00000003,
        WICBitmapDitherTypeSpiral4x4 = 0x00000004,
        WICBitmapDitherTypeSpiral8x8 = 0x00000005,
        WICBitmapDitherTypeDualSpiral4x4 = 0x00000006,
        WICBitmapDitherTypeDualSpiral8x8 = 0x00000007,
        WICBitmapDitherTypeErrorDiffusion = 0x00000008,
    }

    [Flags]
    enum WICBitmapPaletteType 
    {
        WICBitmapPaletteTypeCustom = 0x00000000,
        WICBitmapPaletteTypeMedianCut = 0x00000001,
        WICBitmapPaletteTypeFixedBW = 0x00000002,
        WICBitmapPaletteTypeFixedHalftone8 = 0x00000003,
        WICBitmapPaletteTypeFixedHalftone27 = 0x00000004,
        WICBitmapPaletteTypeFixedHalftone64 = 0x00000005,
        WICBitmapPaletteTypeFixedHalftone125 = 0x00000006,
        WICBitmapPaletteTypeFixedHalftone216 = 0x00000007,
        WICBitmapPaletteTypeFixedWebPalette = WICBitmapPaletteTypeFixedHalftone216,
        WICBitmapPaletteTypeFixedHalftone252 = 0x00000008,
        WICBitmapPaletteTypeFixedHalftone256 = 0x00000009,
        WICBitmapPaletteTypeFixedGray4 = 0x0000000A,
        WICBitmapPaletteTypeFixedGray16 = 0x0000000B,
        WICBitmapPaletteTypeFixedGray256 = 0x0000000C,
    }

    [ComImport(), Guid("cacaf262-9370-4615-a13b-9f5539da4c0a")]
    class WICImagingFactory
    {
        
    }

    [ComImport, Guid("0000000c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IStream
    {
        void Read([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbRead);
        void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, int cb, IntPtr pcbWritten);
        void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition);
        void SetSize(long libNewSize);
        void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten);
        void Commit(int grfCommitFlags);
        void Revert();
        void LockRegion(long libOffset, long cb, int dwLockType);
        void UnlockRegion(long libOffset, long cb, int dwLockType);
        void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
        void Clone(out IStream ppstm);
    }

    enum WICBitmapEncoderCacheOption
    {
       WICBitmapEncoderCacheInMemory           = 0x00000000,
       WICBitmapEncoderCacheTempFile           = 0x00000001,
       WICBitmapEncoderNoCache                 = 0x00000002,
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000105-a8f2-4877-ba0a-fd2b6645fb94")]
    interface IWICBitmapFrameEncode
    {
        [PreserveSig]
        int Initialize(IntPtr IPropertyBag2);
        [PreserveSig]
        int SetSize(int width, int height);
        [PreserveSig]
        int SetResolution(double dpiX, double dpiY);
        [PreserveSig]
        int SetPixelFormat(ref Guid pixelFormat);
        [PreserveSig]
        int SetColorContexts( /* stub */);
        [PreserveSig]
        int SetPalette( /* stub */);
        [PreserveSig]
        int SetThumbnail(IWICBitmapSource thumbanil);
        [PreserveSig]
        int WritePixels(int lineCount, int cbStride, int cbBufferSize, IntPtr pbPixels);
        [PreserveSig]
        int WriteSource(IWICBitmapSource bitmapSource, ref WICRect rect);
        [PreserveSig]
        int Commit();
        [PreserveSig]
        int GetMetadataQueryWriter(out IntPtr ppIMetadataQueryWriter);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
    Guid("00000103-a8f2-4877-ba0a-fd2b6645fb94")]
    interface IWICBitmapEncoder
    {
        [PreserveSig]
        int Initialize(IStream stream, WICBitmapEncoderCacheOption cacheOption);
        [PreserveSig]
        int GetContainerFormat(ref Guid pguidContainerFormat);
        [PreserveSig]
        int GetEncoderInfo(/* stub */);
        [PreserveSig]
        int SetColorContexts(/* stub */);
        [PreserveSig]
        int SetPalette(/* stub */);
        [PreserveSig]
        int SetThumbnail(IWICBitmapSource thumbnail);
        [PreserveSig]
        int SetPreview(IWICBitmapSource thumbnail);
        [PreserveSig]
        int CreateNewFrame(out IWICBitmapFrameEncode ppIFrameEncode, ref IntPtr encoderOptions);
        [PreserveSig]
        int Commit();
        [PreserveSig]
        int GetMetadataQueryWriter(out IntPtr ppIMetadataQueryWriter);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
    Guid("ec5ec8a9-c395-4314-9c77-54d7a935ff70")]
    interface IWICImagingFactory
    {
        [PreserveSig]
        int CreateDecoderFromFilename(string filename, ref Guid guidVendor, FileAccess desiredAccess, WICDecodeOptions options, out IWICBitmapDecoder decoder);
        [PreserveSig]
        int CreateDecoderFromStream(IStream pstream, ref Guid pguidVendor, WICDecodeOptions metadataOptions, out IWICBitmapDecoder ppIDecoder);
        [PreserveSig]
        int CreateDecoderFromFileHandle(IntPtr hFile, ref Guid pguidVendor, WICDecodeOptions metadataOptions, out IWICBitmapDecoder ppIDecoder);
        [PreserveSig]
        int CreateComponentInfo(Guid clsidComponent, out IntPtr ppIInfo);
        [PreserveSig]
        int CreateDecoder(Guid guidDContainerFormat, ref Guid pguidVendor, out IWICBitmapDecoder ppIDecoder);
        [PreserveSig]
        int CreateEncoder(ref Guid guidContainerFormat, ref Guid pguidVendor, out IWICBitmapEncoder ppIEncoder);
        [PreserveSig]
        int CreatePalette(out IntPtr ppIPalette);
        [PreserveSig]
        int CreateFormatConverter(out IWICFormatConverter ppIFormatConverter);
        [PreserveSig]
        int CreateBitmapScaler();
        [PreserveSig]
        int CreateBitmapClipper();
        [PreserveSig]
        int CreateBitmapFlipRotator();
        [PreserveSig]
        int CreateStream();
        [PreserveSig]
        int CreateColorContext();
        [PreserveSig]
        int CreateColorTransformer();
        [PreserveSig]
        int CreateBitmap(int uiWidth, 
                         int uiHeight, 
                         ref Guid pixelFormat, 
                         WICBitmapCreateCacheOption option,
                         out IWICBitmap ppIBitmap);
        [PreserveSig]
        int CreateBitmapFromSource(IWICBitmapSource pIBitmapSource, 
                                   WICBitmapCreateCacheOption option,
                                   out IWICBitmap ppIBitmap);
    }
    
    enum WICBitmapCreateCacheOption
    {
        WICBitmapNoCache                       = 0x00000000,
        WICBitmapCacheOnDemand                 = 0x00000001,
        WICBitmapCacheOnLoad                   = 0x00000002,
    }

    class WICContainerFormats
    {
        public static Guid ContainerFormatBmp =  new Guid("0af1d87e-fcfe-4188-bdeb-a7906471cbe3");
        public static Guid ContainerFormatPng =  new Guid("1b7cfaf4-713f-473c-bbcd-6137425faeaf");
        public static Guid ContainerFormatJpeg = new Guid("19e4a5aa-5662-4fc5-a0c0-1758028e1057");
        public static Guid ContainerFormatTiff = new Guid("163bcc30-e2e9-4f0b-961d-a3e9fdb788a3");
        public static Guid ContainerFormatGif =  new Guid("1f8a5601-7d4d-4cbd-9c82-1bc8d4eeb9a5");
        public static Guid ContainerFormatWmp =  new Guid("57a37caa-367a-4540-916b-f183c5093a4b");
    }

    class WICFormats
    {
        public static Guid WICPixelFormatDontCare = new Guid("6fddc324-4e03-4bfe-b185-3d77768dc900");
        public static Guid WICPixelFormat32bppBGRA =  new Guid("6fddc324-4e03-4bfe-b185-3d77768dc90f");
        public static Guid WICPixelFormat32bppPBGRA = new Guid("6fddc324-4e03-4bfe-b185-3d77768dc910");
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000301-a8f2-4877-ba0a-fd2b6645fb94")]
    interface IWICFormatConverter : IWICBitmapSource
    {
        [PreserveSig]
        new int GetSize(out uint width, out uint height);
        [PreserveSig]
        new int GetPixelFormat(out Guid pPixelFormat);
        [PreserveSig]
        new int GetResolution(out double pDpiX, out double pDpiY);
        [PreserveSig]
        new int CopyPalette(ref IntPtr pIPalette);
        [PreserveSig]
        new int CopyPixels(IntPtr rect, uint stride, uint cbBufferSize, out IntPtr pbuffer);

        [PreserveSig]
        int Initialize(IWICBitmapSource pSource, ref Guid dstFormat, WICBitmapDitherType dither, IWICPalette palette, double alphaThresholdPercent, WICBitmapPaletteType paletteTranslate);
        [PreserveSig]
        int CanConvert(ref Guid srcPixelFormat, ref Guid dstPixelFormat, out bool canConvert);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000040-a8f2-4877-ba0a-fd2b6645fb94")]
    interface IWICPalette
    {
        [PreserveSig]
        int InitializePredefined(WICBitmapPaletteType ePaletteType, bool fAddTransparentColor);
        
        
        //    HRESULT InitializeCustom(
        //        [in, size_is(colorCount)] WICColor *pColors,
        //        [in] UINT colorCount);

        //    HRESULT InitializeFromBitmap(
        //        [in] IWICBitmapSource *pISurface,
        //        [in] UINT colorCount,
        //        [in] BOOL fAddTransparentColor);

        //    HRESULT InitializeFromPalette(
        //        [in] IWICPalette *pIPalette);

        //    HRESULT GetType(
        //        [out] WICBitmapPaletteType *pePaletteType);

        //    HRESULT GetColorCount(
        //        [out] UINT *pcCount);

        //    HRESULT GetColors(
        //        [in] UINT colorCount,
        //        [out, size_is(colorCount)] WICColor *pColors,
        //        [out] UINT *pcActualColors);

        //    HRESULT IsBlackWhite(
        //        [out] BOOL *pfIsBlackWhite);

        //    HRESULT IsGrayscale(
        //        [out] BOOL *pfIsGrayscale);

        //    HRESULT HasAlpha(
        //        [out] BOOL *pfHasAlpha);

    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("9edde9e7-8dee-47ea-99df-e6faf2ed44bf")]
    interface IWICBitmapDecoder
    {
        [PreserveSig]
        int QueryCapability(IntPtr pIStream, ref int dwCapability);
        [PreserveSig]
        int Initialize(IntPtr pIStream, int cacheOptions);
        [PreserveSig]
        int GetContainerFormat(ref Guid guidContainerFormat);
        [PreserveSig]
        int GetDecoderInfo(out IntPtr ppIDecoderInfo);
        [PreserveSig]
        int CopyPalette(IntPtr pIPalette);
        [PreserveSig]
        int GetMetadataQueryReader(out IntPtr ppIMetadataQueryReader);
        [PreserveSig]
        int GetPreview(out IntPtr ppIBitmapSource);
        [PreserveSig]
        int GetColorContext(uint count, out IntPtr ppIColorContexts, out uint pcActualCount);
        [PreserveSig]
        int GetThumbnail(out IntPtr ppIThumbnail);
        [PreserveSig]
        int GetFrameCount(out uint pCount);
        [PreserveSig]
        int GetFrame(uint index, out IWICBitmapFrameDecode ppIBitmapFrame);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000121-a8f2-4877-ba0a-fd2b6645fb94")]
    interface IWICBitmap : IWICBitmapSource
    {
        [PreserveSig]
        new int GetSize(out uint width, out uint height);
        [PreserveSig]
        new int GetPixelFormat(out Guid pPixelFormat);
        [PreserveSig]
        new int GetResolution(out double pDpiX, out double pDpiY);
        [PreserveSig]
        new int CopyPalette(ref IntPtr pIPalette);
        [PreserveSig]
        new int CopyPixels(IntPtr rect, uint stride, uint cbBufferSize, out IntPtr pbuffer);
        [PreserveSig]
        int Lock(ref WICRect prcLock, WICBitmapLockFlags flags, out IWICBitmapLock ppLock);
        [PreserveSig]
        int SetPalette(IWICPalette pIPalette);
        [PreserveSig]
        int SetResolution(double dpiX, double dpiY);
    }

    [Flags]
    enum WICBitmapLockFlags
    {
        WICBitmapLockRead = 0x00000001,
        WICBitmapLockWrite = 0x00000002,
    }

    [StructLayout(LayoutKind.Sequential)]
    struct WICRect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
    } 
    
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
    Guid("00000123-a8f2-4877-ba0a-fd2b6645fb94")]
    interface IWICBitmapLock
    {
        [PreserveSig]
        int GetSize(ref int puiWidth, ref int puiHeight);
        [PreserveSig]
        int GetStride(out int pcbStride);
        [PreserveSig]
        int GetDataPointer(out int pcbBufferSize, out IntPtr ppbData);
        [PreserveSig]
        int GetPixelFormat(ref Guid pPixelFormat);
    }

     [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000120-a8f2-4877-ba0a-fd2b6645fb94")]
    interface IWICBitmapSource
    {
        [PreserveSig]
        int GetSize(out uint width, out uint height);
        [PreserveSig]
        int GetPixelFormat(out Guid pPixelFormat);
        [PreserveSig]
        int GetResolution(out double pDpiX, out double pDpiY);
        [PreserveSig]
        int CopyPalette(ref IntPtr pIPalette);
        [PreserveSig]
        int CopyPixels(IntPtr rect, uint stride, uint cbBufferSize, out IntPtr pbuffer);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3b16811b-6a43-4ec9-a813-3d930c13b940")]
    interface IWICBitmapFrameDecode : IWICBitmapSource
    {
        [PreserveSig]
        new int GetSize(out uint width, out uint height);
        [PreserveSig]
        new int GetPixelFormat(out Guid pPixelFormat);
        [PreserveSig]
        new int GetResolution(out double pDpiX, out double pDpiY);
        [PreserveSig]
        new int CopyPalette(ref IntPtr pIPalette);
        [PreserveSig]
        new int CopyPixels(IntPtr rect, uint stride, uint cbBufferSize, out IntPtr pbuffer);

        [PreserveSig]
        int GetMetadataQueryReader(out IntPtr ppIMetadataQueryReader);
        [PreserveSig]
        int GetColorContexts(uint count, out IntPtr ppIColorContexts, out uint pcActualCount);
        [PreserveSig]
        int GetThumbnail(out IWICBitmapSource ppIThumbnail);
    }
}
