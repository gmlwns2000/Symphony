using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using SlimDX.Direct3D9;
using SlimDX.Direct3D10;
using SlimDX.DXGI;

namespace DirectCanvas
{
    class D3DImageSlimDX : D3DImage, IDisposable
    {
        private static int ActiveImageCount;

        private static Direct3DEx D3DContext;

        private static DeviceEx D3DDevice;

        private Texture SharedTexture;

        private Texture2D ResolvedTexture;

        private SlimDX.Direct3D9.Surface surface;

        public D3DImageSlimDX()
        {
            InitD3D9();
            ActiveImageCount++;
        }

        public void Dispose()
        {
            SetBackBufferSlimDX(null, null);
            if (SharedTexture != null)
            {
                SharedTexture.Dispose();
                SharedTexture = null;
            }
            ActiveImageCount--;
            ShutdownD3D9();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern System.IntPtr GetDesktopWindow();

        public void SetBackBufferSlimDX(Texture2D texture, SlimDX.Direct3D10.Device context)
        {
            if (SharedTexture != null)
            {
                SharedTexture.Dispose();
                SharedTexture = null;
            }
            if (ResolvedTexture != null)
            {
                ResolvedTexture.Dispose();
                ResolvedTexture = null;
            }
            if (IsShareable(texture))
            {
                ResolvedTexture = new Texture2D(context, new Texture2DDescription
                {
                    BindFlags = texture.Description.BindFlags,
                    Format = texture.Description.Format,
                    Width = texture.Description.Width,
                    Height = texture.Description.Height,
                    MipLevels = texture.Description.MipLevels,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = texture.Description.Usage,
                    OptionFlags = texture.Description.OptionFlags,
                    CpuAccessFlags = texture.Description.CpuAccessFlags,
                    ArraySize = texture.Description.ArraySize
                });
                context.ResolveSubresource(texture, 0, ResolvedTexture, 0, texture.Description.Format);
                SlimDX.Direct3D9.Format format = TranslateFormat(ResolvedTexture);
                if (format == SlimDX.Direct3D9.Format.Unknown)
                {
                    throw new NotImplementedException("対応していないテクスチャフォーマットが指定されました。");
                }
                System.IntPtr sharedHandle = GetSharedHandle(ResolvedTexture);
                if (sharedHandle == System.IntPtr.Zero)
                {
                    throw new NotImplementedException("共有ハンドルの生成に失敗");
                }
                SharedTexture = new Texture(D3DImageSlimDX.D3DDevice, ResolvedTexture.Description.Width, ResolvedTexture.Description.Height, 1, SlimDX.Direct3D9.Usage.RenderTarget, format, Pool.Default, ref sharedHandle);
                surface = SharedTexture.GetSurfaceLevel(0);
                base.Lock();
                base.SetBackBuffer(D3DResourceType.IDirect3DSurface9, surface.ComPointer);
                base.Unlock();
            }
            else if (texture != null)
            {
                throw new NotImplementedException("テクスチャはResourceOptioFlags.Sharedをつけて作成されなければなりません。");
            }
        }

        private System.IntPtr GetSharedHandle(Texture2D texture)
        {
            SlimDX.DXGI.Resource resource = new SlimDX.DXGI.Resource(texture);
            System.IntPtr sharedHandle = resource.SharedHandle;
            resource.Dispose();
            return sharedHandle;
        }

        private SlimDX.Direct3D9.Format TranslateFormat(Texture2D texture)
        {
            SlimDX.DXGI.Format format = texture.Description.Format;
            SlimDX.Direct3D9.Format result;
            if (format <= SlimDX.DXGI.Format.R10G10B10A2_UNorm)
            {
                if (format == SlimDX.DXGI.Format.R16G16B16A16_Float)
                {
                    result = SlimDX.Direct3D9.Format.A16B16G16R16F;
                    return result;
                }
                if (format == SlimDX.DXGI.Format.R10G10B10A2_UNorm)
                {
                    result = SlimDX.Direct3D9.Format.A2B10G10R10;
                    return result;
                }
            }
            else if (format == SlimDX.DXGI.Format.B8G8R8A8_UNorm || format == SlimDX.DXGI.Format.B8G8R8A8_UNorm_SRGB)
            {
                result = SlimDX.Direct3D9.Format.A8R8G8B8;
                return result;
            }
            result = SlimDX.Direct3D9.Format.Unknown;
            return result;
        }

        private bool IsShareable(Texture2D texture)
        {
            return texture != null && (texture.Description.OptionFlags & ResourceOptionFlags.Shared) != ResourceOptionFlags.None;
        }

        private void InitD3D9()
        {
            if (D3DImageSlimDX.ActiveImageCount == 0)
            {
                D3DImageSlimDX.D3DContext = new Direct3DEx();
                PresentParameters presentParameters = new PresentParameters();
                presentParameters.Windowed = true;
                presentParameters.SwapEffect = SlimDX.Direct3D9.SwapEffect.Discard;
                presentParameters.DeviceWindowHandle = D3DImageSlimDX.GetDesktopWindow();
                presentParameters.PresentationInterval = PresentInterval.Immediate;
                D3DImageSlimDX.D3DDevice = new DeviceEx(D3DImageSlimDX.D3DContext, 0, DeviceType.Hardware, System.IntPtr.Zero, CreateFlags.Multithreaded | CreateFlags.HardwareVertexProcessing | CreateFlags.FpuPreserve, presentParameters);
                D3DImageSlimDX.D3DDevice.SetRenderState(RenderState.AlphaBlendEnable, false);
            }
        }

        private void ShutdownD3D9()
        {
            if (D3DImageSlimDX.ActiveImageCount == 0)
            {
                if (SharedTexture != null)
                {
                    SharedTexture.Dispose();
                    SharedTexture = null;
                }
                if (D3DImageSlimDX.D3DDevice != null)
                {
                    D3DImageSlimDX.D3DDevice.Dispose();
                    D3DImageSlimDX.D3DDevice = null;
                }
                if (D3DImageSlimDX.D3DContext != null)
                {
                    D3DImageSlimDX.D3DContext.Dispose();
                    D3DImageSlimDX.D3DContext = null;
                }
            }
        }

        public void InvalidateD3DImage()
        {
            if (SharedTexture != null)
            {
                base.Lock();
                base.AddDirtyRect(new Int32Rect(0, 0, ResolvedTexture.Description.Width, ResolvedTexture.Description.Height));
                base.Unlock();
            }
        }

        public void PushFrame(Texture2D texture, SlimDX.Direct3D10.Device device)
        {
            if (ResolvedTexture != null)
            {
                device.ResolveSubresource(texture, 0, ResolvedTexture, 0, texture.Description.Format);
            }
        }
    }
}
