using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.PeculiarValueSubscriber
{
    public class Use_ToonSubscriber : PeculiarValueSubscriberBase
    {
        public override string Name
        {
            get { return "use_toon"; }
        }

        public override VariableType Type
        {
            get { return VariableType.Bool; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument argument)
        {
            subscribeTo.AsScalar().Set(argument.Material.IsToonUsed);
        }
    }
}