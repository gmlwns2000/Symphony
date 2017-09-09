using MMF.Utility;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Sprite
{
    public class GDISpriteBatch : System.IDisposable
    {
        private BlendState state;

        private Matrix ViewMatrix;

        private Matrix SpriteProjectionMatrix;

        private Viewport spriteViewport;

        private Buffer VertexBuffer
        {
            get;
            set;
        }

        private InputLayout VertexInputLayout
        {
            get;
            set;
        }

        public Effect SpriteEffect
        {
            get;
            private set;
        }

        private EffectPass renderPass
        {
            get;
            set;
        }

        public ShaderResourceView SpriteTexture
        {
            get;
            private set;
        }

        public Vector2 TextureSize
        {
            get;
            private set;
        }

        public Vector3 TransparentColor
        {
            get;
            private set;
        }

        private System.Drawing.Bitmap mapedBitmap
        {
            get;
            set;
        }

        public System.Drawing.Graphics mapedGraphic
        {
            get;
            private set;
        }

        public bool NeedRedraw
        {
            get;
            set;
        }

        private SamplerState sampler
        {
            get;
            set;
        }

        private RenderContext Context
        {
            get;
            set;
        }

        public GDISpriteBatch(RenderContext context, int width, int height)
        {
            Context = context;
            Resize(width, height);
            SpriteEffect = CGHelper.CreateEffectFx5("Shader\\sprite.fx", context.DeviceManager.Device);
            renderPass = SpriteEffect.GetTechniqueByIndex(0).GetPassByIndex(0);
            VertexInputLayout = new InputLayout(context.DeviceManager.Device, SpriteEffect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature, SpriteVertexLayout.InputElements);
            SamplerDescription description = default(SamplerDescription);
            description.Filter = Filter.MinMagMipLinear;
            description.AddressU = TextureAddressMode.Wrap;
            description.AddressV = TextureAddressMode.Wrap;
            description.AddressW = TextureAddressMode.Wrap;
            sampler = SamplerState.FromDescription(context.DeviceManager.Device, description);
            mapedGraphic = System.Drawing.Graphics.FromImage(mapedBitmap);
            BlendStateDescription description2 = default(BlendStateDescription);
            description2.AlphaToCoverageEnable = false;
            description2.IndependentBlendEnable = false;
            for (int i = 0; i < description2.RenderTargets.Length; i++)
            {
                description2.RenderTargets[i].BlendEnable = true;
                description2.RenderTargets[i].SourceBlend = BlendOption.SourceAlpha;
                description2.RenderTargets[i].DestinationBlend = BlendOption.InverseSourceAlpha;
                description2.RenderTargets[i].BlendOperation = BlendOperation.Add;
                description2.RenderTargets[i].SourceBlendAlpha = BlendOption.One;
                description2.RenderTargets[i].DestinationBlendAlpha = BlendOption.Zero;
                description2.RenderTargets[i].BlendOperationAlpha = BlendOperation.Add;
                description2.RenderTargets[i].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            }
            ViewMatrix = Matrix.LookAtLH(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
            state = BlendState.FromDescription(context.DeviceManager.Device, description2);
            NeedRedraw = true;
            Update();
        }

        public void Resize(int width, int height)
        {
            TextureSize = new Vector2(width, height);
            float num = width / 2f;
            float num2 = height / 2f;
            System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
            TransparentColor = new Vector3(0f, 0f, 0f);
            CGHelper.AddListBuffer(new Vector3(-num, num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(0f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(num, num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(1f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(-num, -num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(0f, 1f), list);
            CGHelper.AddListBuffer(new Vector3(num, num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(1f, 0f), list);
            CGHelper.AddListBuffer(new Vector3(num, -num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(1f, 1f), list);
            CGHelper.AddListBuffer(new Vector3(-num, -num2, 0f), list);
            CGHelper.AddListBuffer(new Vector2(0f, 1f), list);
            using (DataStream dataStream = new DataStream(list.ToArray(), true, true))
            {
                BufferDescription description = new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)dataStream.Length
                };
                VertexBuffer = new Buffer(Context.DeviceManager.Device, dataStream, description);
            }
            SpriteProjectionMatrix = Matrix.OrthoLH(width, height, 0f, 100f);
            spriteViewport = new Viewport
            {
                Width = width,
                Height = height,
                MaxZ = 1f
            };
            mapedBitmap = new System.Drawing.Bitmap(width, height);
            if (mapedGraphic != null)
            {
                mapedGraphic.Dispose();
            }
            mapedGraphic = System.Drawing.Graphics.FromImage(mapedBitmap);
        }

        public void Update()
        {
            if (NeedRedraw)
            {
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    lock (mapedBitmap)
                    {
                        mapedBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                    memoryStream.Seek(0L, System.IO.SeekOrigin.Begin);
                    try
                    {
                        if (SpriteTexture != null)
                        {
                            lock (SpriteTexture)
                            {
                                SpriteTexture.Dispose();
                                SpriteTexture = ShaderResourceView.FromStream(Context.DeviceManager.Device, memoryStream, (int)memoryStream.Length);
                            }
                        }
                        else
                        {
                            SpriteTexture = ShaderResourceView.FromStream(Context.DeviceManager.Device, memoryStream, (int)memoryStream.Length);
                        }
                    }
                    catch (Direct3D11Exception)
                    {
                    }
                }
                NeedRedraw = false;
            }
        }

        public void Draw()
        {
            BlendState blendState = Context.DeviceManager.Context.OutputMerger.BlendState;
            Viewport[] viewports = Context.DeviceManager.Context.Rasterizer.GetViewports();
            SpriteEffect.GetVariableBySemantic("WORLDVIEWPROJECTION").AsMatrix().SetMatrix(ViewMatrix * SpriteProjectionMatrix);
            SpriteEffect.GetVariableBySemantic("TRANSCOLOR").AsVector().Set(TransparentColor);
            SpriteEffect.GetVariableBySemantic("SPRITETEXTURE").AsResource().SetResource(SpriteTexture);
            Context.DeviceManager.Context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, SpriteVertexLayout.SizeInBytes, 0));
            Context.DeviceManager.Context.InputAssembler.InputLayout = VertexInputLayout;
            Context.DeviceManager.Context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            Context.DeviceManager.Context.OutputMerger.BlendState = state;
            Context.DeviceManager.Context.Rasterizer.SetViewports(spriteViewport);
            renderPass.Apply(Context.DeviceManager.Context);
            Context.DeviceManager.Context.Draw(6, 0);
            Context.DeviceManager.Context.Rasterizer.SetViewports(viewports);
            Context.DeviceManager.Context.OutputMerger.BlendState = blendState;
        }

        public void Dispose()
        {
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
            if (SpriteTexture != null && !SpriteTexture.Disposed)
            {
                SpriteTexture.Dispose();
            }
            if (mapedBitmap != null)
            {
                mapedBitmap.Dispose();
            }
            if (mapedGraphic != null)
            {
                mapedGraphic.Dispose();
            }
        }
    }
}
