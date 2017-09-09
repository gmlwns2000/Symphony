using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.MME.VariableSubscriber.TextureSubscriber
{
    internal class RenderColorTargetSubscriber : SubscriberBase, System.IDisposable
    {
        private RenderTargetView renderTarget;

        private ShaderResourceView shaderResource;

        private Texture2D renderTexture;

        private string variableName;

        public override string Semantics
        {
            get
            {
                return "RENDERCOLORTARGET";
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
            RenderColorTargetSubscriber renderColorTargetSubscriber = new RenderColorTargetSubscriber();
            variableName = variable.Description.Name;
            int width;
            int height;
            int num;
            int mipLevels;
            Format format;
            TextureAnnotationParser.GetBasicTextureAnnotations(variable, context, Format.R8G8B8A8_UNorm, new Vector2(1f, 1f), false, out width, out height, out num, out mipLevels, out format);
            if (num != -1)
            {
                throw new InvalidMMEEffectShaderException(string.Format("RENDERCOLORTARGETの型はTexture2Dである必要があるためアノテーション「int depth」は指定できません。", new object[0]));
            }
            Texture2DDescription description = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = (BindFlags.ShaderResource | BindFlags.RenderTarget),
                CpuAccessFlags = CpuAccessFlags.None,
                Format = format,
                Height = height,
                Width = width,
                MipLevels = mipLevels,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };
            renderColorTargetSubscriber.renderTexture = new Texture2D(context.DeviceManager.Device, description);
            renderColorTargetSubscriber.renderTarget = new RenderTargetView(context.DeviceManager.Device, renderColorTargetSubscriber.renderTexture);
            renderColorTargetSubscriber.shaderResource = new ShaderResourceView(context.DeviceManager.Device, renderColorTargetSubscriber.renderTexture);
            effectManager.RenderColorTargetViewes.Add(variableName, renderColorTargetSubscriber.renderTarget);
            return renderColorTargetSubscriber;
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsResource().SetResource(shaderResource);
        }

        public void Dispose()
        {
            if (shaderResource != null && !shaderResource.Disposed)
            {
                shaderResource.Dispose();
                shaderResource = null;
            }
            if (renderTarget != null && !renderTarget.Disposed)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }
            if (renderTexture != null && !renderTexture.Disposed)
            {
                renderTexture.Dispose();
                renderTexture = null;
            }
        }
    }
}
