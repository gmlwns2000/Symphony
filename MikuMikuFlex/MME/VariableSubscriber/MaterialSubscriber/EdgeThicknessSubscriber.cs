using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal class EdgeThicknessSubscriber:MaterialSubscriberBase
    {
        public override string Semantics
        {
            get { return "EDGETHICKNESS"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsScalar().Set(variable.Material.EdgeSize);
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new EdgeThicknessSubscriber();
        }
    }
}
