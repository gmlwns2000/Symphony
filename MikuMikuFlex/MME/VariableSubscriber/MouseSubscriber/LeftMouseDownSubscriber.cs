using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MouseSubscriber
{
    internal sealed class LeftMouseDownSubscriber : MouseSubscriberBase
    {
        public override string Semantics
        {
            get { return "LEFTMOUSEDOWN"; }
        }

        public override VariableType[] Types
        {
            get { return new[] {VariableType.Float4}; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsVector().Set(variable.Context.CurrentPanelObserver.LeftMouseDown);
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            return new LeftMouseDownSubscriber();
        }
    }
}