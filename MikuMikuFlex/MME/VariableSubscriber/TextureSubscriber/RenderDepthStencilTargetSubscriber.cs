using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.MME.VariableSubscriber.TextureSubscriber
{
    public class RenderDepthStencilTargetSubscriber : SubscriberBase, System.IDisposable
    {
        private DepthStencilView depthStencilView;

        private ShaderResourceView shaderResource;

        private Texture2D depthStencilTexture;

        public override string Semantics
        {
            get
            {
                return "RENDERDEPTHSTENCILTARGET";
            }
        }

        public override VariableType[] Types
        {
            get
            {
                return new VariableType[]
                {
                    VariableType.Texture2D,
                    VariableType.Texture
                };
            }
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            RenderDepthStencilTargetSubscriber renderDepthStencilTargetSubscriber = new RenderDepthStencilTargetSubscriber();
            int width;
            int height;
            int num;
            int mipLevels;
            Format format;
            TextureAnnotationParser.GetBasicTextureAnnotations(variable, context, Format.D24_UNorm_S8_UInt, new Vector2(1f, 1f), false, out width, out height, out num, out mipLevels, out format);
            Texture2DDescription description = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = (BindFlags.DepthStencil | BindFlags.ShaderResource),
                CpuAccessFlags = CpuAccessFlags.None,
                Format = format,
                Height = height,
                Width = width,
                MipLevels = mipLevels,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };
            renderDepthStencilTargetSubscriber.depthStencilTexture = new Texture2D(context.DeviceManager.Device, description);
            renderDepthStencilTargetSubscriber.depthStencilView = new DepthStencilView(context.DeviceManager.Device, renderDepthStencilTargetSubscriber.depthStencilTexture);
            renderDepthStencilTargetSubscriber.shaderResource = new ShaderResourceView(context.DeviceManager.Device, renderDepthStencilTargetSubscriber.depthStencilTexture);
            effectManager.RenderDepthStencilTargets.Add(variable.Description.Name, renderDepthStencilTargetSubscriber.depthStencilView);
            return renderDepthStencilTargetSubscriber;
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsResource().SetResource(shaderResource);
        }

        public void Dispose()
        {
            if (depthStencilTexture != null && depthStencilTexture.Disposed)
            {
                depthStencilTexture.Dispose();
                depthStencilTexture = null;
            }
            if (depthStencilView != null && depthStencilView.Disposed)
            {
                depthStencilView.Dispose();
                depthStencilTexture = null;
            }
            if (shaderResource != null && shaderResource.Disposed)
            {
                shaderResource.Dispose();
                shaderResource = null;
            }
        }
    }
}