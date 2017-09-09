using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SlimDX.Direct2D;
using SlimDX.DXGI;

namespace DirectCanvas.Imaging.WIC
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("2cd90691-12e2-11dc-9fed-001143a055f9")]
    interface ID2D1Resource
    {
        [PreserveSig]
        int GetFactory(out IntPtr factory);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("2cd90694-12e2-11dc-9fed-001143a055f9")]
    interface ID2D1RenderTarget : ID2D1Resource
    {
        [PreserveSig]
        new int GetFactory(out IntPtr factory);
        [PreserveSig]
        int CreateBitmap( /*stub*/);
        [PreserveSig]
        int CreateBitmapFromWicBitmap(IWICBitmapSource source, ref BitmapProperties pProps, out IntPtr bitmap);
    }

    static class D2DRenderTargetEx
    {
        public static SlimDX.Direct2D.Bitmap CreateBitmapFromWicBitmap(IWICBitmapSource source, BitmapProperties bitmapProperties, RenderTarget renderTarget)
        {
            var pBitmap = IntPtr.Zero;
            SlimDX.Direct2D.Bitmap bmp = null;

            var pRenderTarget = Marshal.GetObjectForIUnknown(renderTarget.ComPointer) as ID2D1RenderTarget;

            int hr = pRenderTarget.CreateBitmapFromWicBitmap(source, ref bitmapProperties, out pBitmap);

            if (hr != 0)
                goto cleanup;
          
            bmp = SlimDX.Direct2D.Bitmap.FromPointer(pBitmap);
           
cleanup:
            Marshal.Release(pBitmap);
            Marshal.ReleaseComObject(pRenderTarget);

            return bmp;
        }
    }
}
