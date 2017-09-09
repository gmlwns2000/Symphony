using MMF.DeviceManager;
using MMF.Sprite.D2D;
using MMF.Utility;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.DirectWrite;
using SlimDX.DXGI;

namespace MMF.Sprite
{
    public class D2DSpriteBatch : System.IDisposable
    {
        private SlimDX.Direct3D11.BlendState state;

        private Matrix ViewMatrix;

        private Matrix SpriteProjectionMatrix;

        private SlimDX.Direct3D11.Viewport spriteViewport;

        internal RenderContext context;

        public event System.EventHandler<System.EventArgs> RenderTargetRecreated;

        public event System.EventHandler<System.EventArgs> BatchDisposing;

        private IDeviceManager DeviceManager
        {
            get;
            set;
        }

        private SlimDX.Direct3D11.Texture2D TextureD3D11
        {
            get;
            set;
        }

        private SlimDX.Direct3D10.Texture2D TextureD3D10
        {
            get;
            set;
        }

        private KeyedMutex MutexD3D10
        {
            get;
            set;
        }

        private KeyedMutex MutexD3D11
        {
            get;
            set;
        }

        public RenderTarget DWRenderTarget
        {
            get;
            private set;
        }

        private SlimDX.Direct3D11.BlendState TransParentBlendState
        {
            get;
            set;
        }

        public System.Drawing.Rectangle FillRectangle
        {
            get
            {
                return new System.Drawing.Rectangle(0, 0, (int)TextureSize.X, (int)TextureSize.Y);
            }
        }

        private SlimDX.Direct3D11.Buffer VertexBuffer
        {
            get;
            set;
        }

        private SlimDX.Direct3D11.InputLayout VertexInputLayout
        {
            get;
            set;
        }

        private SlimDX.Direct3D11.Effect SpriteEffect
        {
            get;
            set;
        }

        private SlimDX.Direct3D11.EffectPass renderPass
        {
            get;
            set;
        }

        private SlimDX.Direct3D11.SamplerState sampler
        {
            get;
            set;
        }

        public Vector2 TextureSize
        {
            get;
            private set;
        }

        public D2DSpriteBatch(RenderContext context)
        {
            this.context = context;
            DeviceManager = context.DeviceManager;
            SpriteEffect = CGHelper.CreateEffectFx5FromResource("MMF.Resource.Shader.SpriteShader.fx", DeviceManager.Device);
            renderPass = SpriteEffect.GetTechniqueByIndex(0).GetPassByIndex(1);
            VertexInputLayout = new SlimDX.Direct3D11.InputLayout(DeviceManager.Device, SpriteEffect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, SpriteVertexLayout.InputElements);
            SlimDX.Direct3D11.SamplerDescription description = default(SlimDX.Direct3D11.SamplerDescription);
            description.Filter = SlimDX.Direct3D11.Filter.MinMagMipLinear;
            description.AddressU = SlimDX.Direct3D11.TextureAddressMode.Wrap;
            description.AddressV = SlimDX.Direct3D11.TextureAddressMode.Wrap;
            description.AddressW = SlimDX.Direct3D11.TextureAddressMode.Wrap;
            sampler = SlimDX.Direct3D11.SamplerState.FromDescription(DeviceManager.Device, description);
            SlimDX.Direct3D11.BlendStateDescription description2 = default(SlimDX.Direct3D11.BlendStateDescription);
            description2.AlphaToCoverageEnable = false;
            description2.IndependentBlendEnable = false;
            for (int i = 0; i < description2.RenderTargets.Length; i++)
            {
                description2.RenderTargets[i].BlendEnable = true;
                description2.RenderTargets[i].SourceBlend = SlimDX.Direct3D11.BlendOption.SourceAlpha;
                description2.RenderTargets[i].DestinationBlend = SlimDX.Direct3D11.BlendOption.InverseSourceAlpha;
                description2.RenderTargets[i].BlendOperation = SlimDX.Direct3D11.BlendOperation.Add;
                description2.RenderTargets[i].SourceBlendAlpha = SlimDX.Direct3D11.BlendOption.One;
                description2.RenderTargets[i].DestinationBlendAlpha = SlimDX.Direct3D11.BlendOption.Zero;
                description2.RenderTargets[i].BlendOperationAlpha = SlimDX.Direct3D11.BlendOperation.Add;
                description2.RenderTargets[i].RenderTargetWriteMask = SlimDX.Direct3D11.ColorWriteMaskFlags.All;
            }
            state = SlimDX.Direct3D11.BlendState.FromDescription(DeviceManager.Device, description2);
            SlimDX.Direct3D11.BlendStateDescription description3 = default(SlimDX.Direct3D11.BlendStateDescription);
            description3.RenderTargets[0].BlendEnable = true;
            description3.RenderTargets[0].SourceBlend = SlimDX.Direct3D11.BlendOption.SourceAlpha;
            description3.RenderTargets[0].DestinationBlend = SlimDX.Direct3D11.BlendOption.InverseSourceAlpha;
            description3.RenderTargets[0].BlendOperation = SlimDX.Direct3D11.BlendOperation.Add;
            description3.RenderTargets[0].SourceBlendAlpha = SlimDX.Direct3D11.BlendOption.One;
            description3.RenderTargets[0].DestinationBlendAlpha = SlimDX.Direct3D11.BlendOption.Zero;
            description3.RenderTargets[0].BlendOperationAlpha = SlimDX.Direct3D11.BlendOperation.Add;
            description3.RenderTargets[0].RenderTargetWriteMask = SlimDX.Direct3D11.ColorWriteMaskFlags.All;
            TransParentBlendState = SlimDX.Direct3D11.BlendState.FromDescription(DeviceManager.Device, description3);
            Resize();
        }

        public void Resize()
        {
            SlimDX.Direct3D11.Viewport viewport = DeviceManager.Context.Rasterizer.GetViewports()[0];
            int num = (int)viewport.Width;
            int num2 = (int)viewport.Height;
            if (num2 != 0 && num != 0)
            {
                TextureSize = new Vector2(num, num2);
                float num3 = num / 2f;
                float num4 = num2 / 2f;
                System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
                CGHelper.AddListBuffer(new Vector3(-num3, num4, 0f), list);
                CGHelper.AddListBuffer(new Vector2(0f, 0f), list);
                CGHelper.AddListBuffer(new Vector3(num3, num4, 0f), list);
                CGHelper.AddListBuffer(new Vector2(1f, 0f), list);
                CGHelper.AddListBuffer(new Vector3(-num3, -num4, 0f), list);
                CGHelper.AddListBuffer(new Vector2(0f, 1f), list);
                CGHelper.AddListBuffer(new Vector3(num3, num4, 0f), list);
                CGHelper.AddListBuffer(new Vector2(1f, 0f), list);
                CGHelper.AddListBuffer(new Vector3(num3, -num4, 0f), list);
                CGHelper.AddListBuffer(new Vector2(1f, 1f), list);
                CGHelper.AddListBuffer(new Vector3(-num3, -num4, 0f), list);
                CGHelper.AddListBuffer(new Vector2(0f, 1f), list);
                using (DataStream dataStream = new DataStream(list.ToArray(), true, true))
                {
                    SlimDX.Direct3D11.BufferDescription description = new SlimDX.Direct3D11.BufferDescription
                    {
                        BindFlags = SlimDX.Direct3D11.BindFlags.VertexBuffer,
                        SizeInBytes = (int)dataStream.Length
                    };
                    if (VertexBuffer != null && !VertexBuffer.Disposed)
                    {
                        VertexBuffer.Dispose();
                    }
                    VertexBuffer = new SlimDX.Direct3D11.Buffer(DeviceManager.Device, dataStream, description);
                }
                SpriteProjectionMatrix = Matrix.OrthoLH(num, num2, 0f, 100f);
                spriteViewport = new SlimDX.Direct3D11.Viewport
                {
                    Width = num,
                    Height = num2,
                    MaxZ = 1f
                };
                ViewMatrix = Matrix.LookAtLH(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
                if (TextureD3D11 != null && !TextureD3D11.Disposed)
                {
                    TextureD3D11.Dispose();
                }
                TextureD3D11 = new SlimDX.Direct3D11.Texture2D(DeviceManager.Device, new SlimDX.Direct3D11.Texture2DDescription
                {
                    Width = num,
                    Height = num2,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.B8G8R8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = SlimDX.Direct3D11.ResourceUsage.Default,
                    BindFlags = (SlimDX.Direct3D11.BindFlags.ShaderResource | SlimDX.Direct3D11.BindFlags.RenderTarget),
                    CpuAccessFlags = SlimDX.Direct3D11.CpuAccessFlags.None,
                    OptionFlags = SlimDX.Direct3D11.ResourceOptionFlags.KeyedMutex
                });
                SlimDX.DXGI.Resource resource = new SlimDX.DXGI.Resource(TextureD3D11);
                if (TextureD3D10 != null && !TextureD3D10.Disposed)
                {
                    TextureD3D10.Dispose();
                }
                TextureD3D10 = DeviceManager.Device10.OpenSharedResource<SlimDX.Direct3D10.Texture2D>(resource.SharedHandle);
                if (MutexD3D10 != null && !MutexD3D10.Disposed)
                {
                    MutexD3D10.Dispose();
                }
                if (MutexD3D11 != null && !MutexD3D11.Disposed)
                {
                    MutexD3D11.Dispose();
                }
                MutexD3D10 = new KeyedMutex(TextureD3D10);
                MutexD3D11 = new KeyedMutex(TextureD3D11);
                resource.Dispose();
                Surface surface = TextureD3D10.AsSurface();
                RenderTargetProperties properties = default(RenderTargetProperties);
                properties.MinimumFeatureLevel = SlimDX.Direct2D.FeatureLevel.Direct3D10;
                properties.Type = RenderTargetType.Hardware;
                properties.Usage = RenderTargetUsage.None;
                properties.PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Premultiplied);
                if (DWRenderTarget != null && !DWRenderTarget.Disposed)
                {
                    DWRenderTarget.Dispose();
                }
                DWRenderTarget = RenderTarget.FromDXGI(context.D2DFactory, surface, properties);
                surface.Dispose();
                if (RenderTargetRecreated != null)
                {
                    RenderTargetRecreated(this, new System.EventArgs());
                }
            }
        }

        public void Begin()
        {
            if (!(TextureSize == Vector2.Zero))
            {
                MutexD3D10.Acquire(0L, 100);
                DWRenderTarget.BeginDraw();
                DWRenderTarget.Clear(new Color4(0f, 0f, 0f, 0f));
            }
        }

        public void End()
        {
            if (!(TextureSize == Vector2.Zero))
            {
                DWRenderTarget.EndDraw();
                MutexD3D10.Release(0L);
                MutexD3D11.Acquire(0L, 100);
                SlimDX.Direct3D11.ShaderResourceView shaderResourceView = new SlimDX.Direct3D11.ShaderResourceView(DeviceManager.Device, TextureD3D11);
                SlimDX.Direct3D11.BlendState blendState = DeviceManager.Context.OutputMerger.BlendState;
                SlimDX.Direct3D11.Viewport[] viewports = DeviceManager.Context.Rasterizer.GetViewports();
                SpriteEffect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(ViewMatrix * SpriteProjectionMatrix);
                SpriteEffect.GetVariableBySemantic("SPRITETEXTURE").AsResource().SetResource(shaderResourceView);
                DeviceManager.Context.InputAssembler.SetVertexBuffers(0, new SlimDX.Direct3D11.VertexBufferBinding(VertexBuffer, SpriteVertexLayout.SizeInBytes, 0));
                DeviceManager.Context.InputAssembler.InputLayout = VertexInputLayout;
                DeviceManager.Context.InputAssembler.PrimitiveTopology = SlimDX.Direct3D11.PrimitiveTopology.TriangleList;
                DeviceManager.Context.OutputMerger.BlendState = state;
                DeviceManager.Context.Rasterizer.SetViewports(spriteViewport);
                renderPass.Apply(DeviceManager.Context);
                DeviceManager.Context.Draw(6, 0);
                DeviceManager.Context.Rasterizer.SetViewports(viewports);
                DeviceManager.Context.OutputMerger.BlendState = blendState;
                shaderResourceView.Dispose();
                MutexD3D11.Release(0L);
            }
        }

        public void Dispose()
        {
            if (BatchDisposing != null)
            {
                BatchDisposing(this, new System.EventArgs());
            }
            if (TextureD3D10 != null && !TextureD3D10.Disposed)
            {
                TextureD3D10.Dispose();
            }
            if (TextureD3D11 != null && !TextureD3D11.Disposed)
            {
                TextureD3D11.Dispose();
            }
            if (MutexD3D10 != null && !MutexD3D10.Disposed)
            {
                MutexD3D10.Dispose();
            }
            if (MutexD3D11 != null && !MutexD3D11.Disposed)
            {
                MutexD3D11.Dispose();
            }
            if (DWRenderTarget != null && !DWRenderTarget.Disposed)
            {
                DWRenderTarget.Dispose();
            }
            if (VertexBuffer != null && !VertexBuffer.Disposed)
            {
                VertexBuffer.Dispose();
            }
            if (VertexInputLayout != null && !VertexInputLayout.Disposed)
            {
                VertexInputLayout.Dispose();
            }
            if (SpriteEffect != null && !SpriteEffect.Disposed)
            {
                SpriteEffect.Dispose();
            }
            if (sampler != null && !sampler.Disposed)
            {
                sampler.Dispose();
            }
            context.Disposables.Add(TransParentBlendState);
            context.Disposables.Add(state);
        }

        public D2DSpriteSolidColorBrush CreateSolidColorBrush(System.Drawing.Color color)
        {
            return new D2DSpriteSolidColorBrush(this, color);
        }

        public D2DSpriteTextformat CreateTextformat(string fontFamiry, int size = 15, SlimDX.DirectWrite.FontWeight weight = SlimDX.DirectWrite.FontWeight.Normal, SlimDX.DirectWrite.FontStyle style = SlimDX.DirectWrite.FontStyle.Normal, FontStretch stretch = FontStretch.Normal, string locale = "ja-jp")
        {
            return new D2DSpriteTextformat(this, fontFamiry, size, weight, style, stretch, locale);
        }

        public D2DSpriteBitmap CreateBitmap(string fileName)
        {
            return new D2DSpriteBitmap(this, System.IO.File.OpenRead(fileName));
        }

        public D2DSpriteBitmap CreateBitmap(System.IO.Stream fs)
        {
            return new D2DSpriteBitmap(this, fs);
        }

        public D2DSpriteBitmapBrush CreateBitmapBrush(string fileName, BitmapBrushProperties bbp = default(BitmapBrushProperties))
        {
            return new D2DSpriteBitmapBrush(this, CreateBitmap(fileName), bbp);
        }

        public D2DSpriteBitmapBrush CreateBitmapBrush(System.IO.Stream fileStream, BitmapBrushProperties bbp = default(BitmapBrushProperties))
        {
            return new D2DSpriteBitmapBrush(this, CreateBitmap(fileStream), bbp);
        }

        public D2DSpriteLinearGradientBrush CreateLinearGradientBrush(D2DSpriteGradientStopCollection collection, LinearGradientBrushProperties gradient)
        {
            return new D2DSpriteLinearGradientBrush(this, collection, gradient);
        }

        public D2DSpriteGradientStopCollection CreateGradientStopCollection(GradientStop[] stops)
        {
            return new D2DSpriteGradientStopCollection(this, stops, Gamma.Linear, ExtendMode.Mirror);
        }

        public D2DSpriteRadialGradientBrush CreateRadialGradientBrush(D2DSpriteGradientStopCollection collection, RadialGradientBrushProperties r)
        {
            return new D2DSpriteRadialGradientBrush(this, collection.GradientStopCollection, r);
        }
    }
}
