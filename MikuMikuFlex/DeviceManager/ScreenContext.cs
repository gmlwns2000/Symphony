using MMF.Matricies;
using MMF.Matricies.Camera;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Windows.Forms;

namespace MMF.DeviceManager
{
    public class ScreenContext : ITargetContext, System.IDisposable
	{
		public RenderTargetView RenderTargetView
		{
			get;
			private set;
		}

		public DepthStencilView DepthTargetView
		{
			get;
			private set;
		}

		public MatrixManager MatrixManager
		{
			get;
			set;
		}

		public SwapChain SwapChain
		{
			get;
			private set;
		}

		public Control BindedControl
		{
			get;
			private set;
		}

		public ICameraMotionProvider CameraMotionProvider
		{
			get;
			set;
		}

		public WorldSpace WorldSpace
		{
			get;
			set;
		}

		public PanelObserver PanelObserver
		{
			get;
			private set;
		}

		public bool IsSelfShadowMode1
		{
			get;
			private set;
		}

		public bool IsEnabledTransparent
		{
			get;
			private set;
		}

		public RenderContext Context
		{
			get;
			set;
		}

		public ScreenContext(Control owner, RenderContext context, MatrixManager manager)
		{
            Context = context;
			SlimDX.Direct3D11.Device device = context.DeviceManager.Device;
			SampleDescription sampleDesc = SampleManager.getSampleDesc(8, 0, Context, Format.B8G8R8A8_UNorm);
            SwapChain = new SwapChain(context.DeviceManager.Factory, device, getSwapChainDescription(owner, sampleDesc));
			using (Texture2D texture2D = new Texture2D(device, getDepthBufferTexture2DDescription(owner, sampleDesc)))
			{
                DepthTargetView = new DepthStencilView(device, texture2D);
			}
			using (Texture2D texture2D2 = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(SwapChain, 0))
			{
                RenderTargetView = new RenderTargetView(device, texture2D2);
			}
            WorldSpace = new WorldSpace();
            BindedControl = owner;
            MatrixManager = manager;
            PanelObserver = new PanelObserver(owner);
            SetViewport();
		}

		public void SetViewport()
		{
            Context.DeviceManager.Context.Rasterizer.SetViewports(getViewport());
		}

		public void SetPanelObserver()
		{
            Context.CurrentPanelObserver = PanelObserver;
		}

		public void Resize()
		{
			if (BindedControl.Width != 0 && BindedControl.Height != 0)
			{
				if (RenderTargetView != null && !RenderTargetView.Disposed)
				{
                    RenderTargetView.Dispose();
				}
				if (DepthTargetView != null && !DepthTargetView.Disposed)
				{
                    DepthTargetView.Dispose();
				}
				SwapChainDescription description = SwapChain.Description;
                SwapChain.ResizeBuffers(description.BufferCount, BindedControl.Width, BindedControl.Height, description.ModeDescription.Format, description.Flags);
				using (Texture2D texture2D = new Texture2D(Context.DeviceManager.Device, getDepthBufferTexture2DDescription(BindedControl, description.SampleDescription)))
				{
                    DepthTargetView = new DepthStencilView(Context.DeviceManager.Device, texture2D);
				}
				using (Texture2D texture2D2 = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(SwapChain, 0))
				{
                    RenderTargetView = new RenderTargetView(Context.DeviceManager.Device, texture2D2);
				}
			}
		}

		public void MoveCameraByCameraMotionProvider()
		{
			if (CameraMotionProvider != null)
			{
                CameraMotionProvider.UpdateCamera(MatrixManager.ViewMatrixManager, MatrixManager.ProjectionMatrixManager);
			}
		}

		protected virtual SwapChainDescription getSwapChainDescription(Control control, SampleDescription sampDesc)
		{
			return new SwapChainDescription
			{
				BufferCount = 2,
				Flags = SwapChainFlags.AllowModeSwitch,
				IsWindowed = true,
				ModeDescription = new ModeDescription
				{
					Format = Format.R8G8B8A8_UNorm,
					Height = control.Height,
					Width = control.Width,
					RefreshRate = new Rational(60, 1)
				},
				OutputHandle = control.Handle,
				SampleDescription = sampDesc,
				SwapEffect = SwapEffect.Discard,
				Usage = Usage.RenderTargetOutput
			};
		}

		protected virtual Texture2DDescription getDepthBufferTexture2DDescription(Control control, SampleDescription desc)
		{
			return new Texture2DDescription
			{
				ArraySize = 1,
				BindFlags = BindFlags.DepthStencil,
				Format = Format.D32_Float,
				Width = control.Width,
				Height = control.Height,
				MipLevels = 1,
				SampleDescription = desc
			};
		}

		protected virtual Viewport getViewport()
		{
			return new Viewport
			{
				Width = BindedControl.Width,
				Height = BindedControl.Height,
				MaxZ = 1f
			};
		}

		public void Dispose()
		{
			if (WorldSpace != null)
			{
                WorldSpace.Dispose();
			}
			if (RenderTargetView != null && !RenderTargetView.Disposed)
			{
                RenderTargetView.Dispose();
			}
			if (DepthTargetView != null && !DepthTargetView.Disposed)
			{
                DepthTargetView.Dispose();
			}
			if (SwapChain != null && !SwapChain.Disposed)
			{
                SwapChain.Dispose();
			}
		}
	}
}