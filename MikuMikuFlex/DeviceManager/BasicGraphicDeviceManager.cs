using MMF.MME;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System.Windows.Forms;

namespace MMF.DeviceManager
{
    public class BasicGraphicDeviceManager : IDeviceManager, System.IDisposable
    {
        public FeatureLevel DeviceFeatureLevel
        {
            get;
            private set;
        }

        public SlimDX.Direct3D11.Device Device
        {
            get;
            private set;
        }

        public DeviceContext Context
        {
            get;
            private set;
        }

        public SlimDX.Direct3D10.Device Device10
        {
            get;
            private set;
        }

        public Adapter CurrentAdapter
        {
            get;
            private set;
        }

        public Factory1 Factory
        {
            get;
            set;
        }

        public void Load()
        {
            Load(false, SlimDX.Direct3D11.DeviceCreationFlags.None, SlimDX.Direct3D10.DeviceCreationFlags.BgraSupport);
        }

        public void Load(bool needDX11 = false, DeviceCreationFlags dx11flag = SlimDX.Direct3D11.DeviceCreationFlags.None, SlimDX.Direct3D10.DeviceCreationFlags dx10flag_for2DDraw = SlimDX.Direct3D10.DeviceCreationFlags.BgraSupport)
        {
            ApplyDebugFlags(ref dx11flag, ref dx10flag_for2DDraw);
            Factory = new Factory1();
            CurrentAdapter = Factory.GetAdapter1(0);
            try
            {
                Device = new SlimDX.Direct3D11.Device(CurrentAdapter, dx11flag, new FeatureLevel[]
                {
                    SlimDX.Direct3D11.FeatureLevel.Level_11_0
                });
            }
            catch (Direct3D11Exception)
            {
                if (needDX11)
                {
                    throw new System.NotSupportedException("DX11がサポートされていません。DX10.1で初期化するにはLoadの第一引数needDraw=falseとして下さい。");
                }
                try
                {
                    Device = new SlimDX.Direct3D11.Device(CurrentAdapter, dx11flag, new FeatureLevel[]
                    {
                        SlimDX.Direct3D11.FeatureLevel.Level_10_0
                    });
                }
                catch (Direct3D11Exception)
                {
                    throw new System.NotSupportedException("DX11,DX10.1での初期化を試みましたが、両方ともサポートされていません。");
                }
            }
            DeviceFeatureLevel = Device.FeatureLevel;
            Context = Device.ImmediateContext;
            SampleDescription sampleDescription = new SampleDescription(1, 0);
            Device10 = new SlimDX.Direct3D10_1.Device1(CurrentAdapter, SlimDX.Direct3D10.DriverType.Hardware, dx10flag_for2DDraw, SlimDX.Direct3D10_1.FeatureLevel.Level_9_3);
            SlimDX.Direct3D11.BlendStateDescription description = default(SlimDX.Direct3D11.BlendStateDescription);
            description.AlphaToCoverageEnable = false;
            description.IndependentBlendEnable = false;
            for (int i = 0; i < description.RenderTargets.Length; i++)
            {
                description.RenderTargets[i].BlendEnable = true;
                description.RenderTargets[i].SourceBlend = SlimDX.Direct3D11.BlendOption.SourceAlpha;
                description.RenderTargets[i].DestinationBlend = SlimDX.Direct3D11.BlendOption.InverseSourceAlpha;
                description.RenderTargets[i].BlendOperation = SlimDX.Direct3D11.BlendOperation.Add;
                description.RenderTargets[i].SourceBlendAlpha = SlimDX.Direct3D11.BlendOption.One;
                description.RenderTargets[i].DestinationBlendAlpha = SlimDX.Direct3D11.BlendOption.Zero;
                description.RenderTargets[i].BlendOperationAlpha = SlimDX.Direct3D11.BlendOperation.Maximum;
                description.RenderTargets[i].RenderTargetWriteMask = SlimDX.Direct3D11.ColorWriteMaskFlags.All;
            }
            Device.ImmediateContext.OutputMerger.BlendState = BlendState.FromDescription(Device, description);
            MMEEffectManager.IniatializeMMEEffectManager(this);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void ApplyDebugFlags(ref DeviceCreationFlags dx11flag, ref SlimDX.Direct3D10.DeviceCreationFlags dx10flag_for2DDraw)
        {
            System.Diagnostics.Debug.Print("デバイスはデバッグモードで作成されました。");
            dx10flag_for2DDraw |= SlimDX.Direct3D10.DeviceCreationFlags.BgraSupport;
        }

        public virtual void Dispose()
        {
            if (!Context.Disposed && Context.Rasterizer.State != null && !Context.Rasterizer.State.Disposed)
            {
                Context.Rasterizer.State.Dispose();
            }
            if (Device != null && !Device.Disposed)
            {
                Device.Dispose();
            }
            if (Device10 != null && !Device10.Disposed)
            {
                Device10.Dispose();
            }
            if (CurrentAdapter != null && !CurrentAdapter.Disposed)
            {
                CurrentAdapter.Dispose();
            }
            if (Factory != null && !Factory.Disposed)
            {
                Factory.Dispose();
            }
        }

        protected virtual void PostLoad(Control control)
        {
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

        protected virtual SlimDX.Direct3D11.Texture2DDescription getDepthBufferTexture2DDescription(Control control, SampleDescription desc)
        {
            return new SlimDX.Direct3D11.Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = SlimDX.Direct3D11.BindFlags.DepthStencil,
                Format = Format.D32_Float,
                Width = control.Width,
                Height = control.Height,
                MipLevels = 1,
                SampleDescription = desc
            };
        }

        protected virtual SlimDX.Direct3D11.Viewport getViewport(Control control)
        {
            return new SlimDX.Direct3D11.Viewport
            {
                Width = control.Width,
                Height = control.Height,
                MaxZ = 1f
            };
        }

        protected virtual SlimDX.Direct3D11.RasterizerStateDescription getRasterizerStateDescription(Control control)
        {
            return new SlimDX.Direct3D11.RasterizerStateDescription
            {
                CullMode = SlimDX.Direct3D11.CullMode.Back,
                FillMode = SlimDX.Direct3D11.FillMode.Solid,
                DepthBias = 0,
                DepthBiasClamp = 0f,
                IsAntialiasedLineEnabled = true,
                IsDepthClipEnabled = true,
                IsFrontCounterclockwise = false,
                IsMultisampleEnabled = true,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0f
            };
        }
    }
}
