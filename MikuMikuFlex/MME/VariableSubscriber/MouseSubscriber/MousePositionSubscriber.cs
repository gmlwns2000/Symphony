using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MouseSubscriber
{
    internal sealed class MousePositionSubscriber : MouseSubscriberBase
    {
        public override string Semantics
        {
            get { return "MOUSEPOSITION"; }
        }

        public override VariableType[] Types
        {
            get { return new[] {VariableType.Float2}; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsVector().Set(variable.Context.CurrentPanelObserver.MousePosition);
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            return new MousePositionSubscriber();
        }
    }
}