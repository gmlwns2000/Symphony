using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.ScreenInfoSubscriber
{
    public class ViewPortPixelSizeScriber : SubscriberBase
    {
        public override string Semantics
        {
            get { return "VIEWPORTPIXELSIZE"; }
        }

        public override VariableType[] Types
        {
            get { return new[] {VariableType.Float2}; }
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            return new ViewPortPixelSizeScriber();
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            Viewport vp = variable.Context.DeviceManager.Context.Rasterizer.GetViewports()[0];
            Vector2 argument = new Vector2(vp.Width, vp.Height);
            subscribeTo.AsVector().Set(argument);
        }
    }
}