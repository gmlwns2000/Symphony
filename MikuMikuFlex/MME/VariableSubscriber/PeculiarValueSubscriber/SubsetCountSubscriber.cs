using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.PeculiarValueSubscriber
{
    public class SubsetCountSubscriber : PeculiarValueSubscriberBase
    {
        public override string Name
        {
            get { return "SubsetCount"; }
        }

        public override VariableType Type
        {
            get { return VariableType.Int; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument argument)
        {
            subscribeTo.AsScalar().Set(argument.Model.SubsetCount);
        }
    }
}