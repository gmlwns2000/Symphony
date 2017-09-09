using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.PeculiarValueSubscriber
{
    internal class ParthfSubscriber : PeculiarValueSubscriberBase
    {
        public override string Name
        {
            get { return "parthf"; }
        }

        public override VariableType Type
        {
            get { return VariableType.Bool; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument argument)
        {
            subscribeTo.AsScalar().Set(argument.Context.CurrentTargetContext.IsSelfShadowMode1);
        }
    }
}