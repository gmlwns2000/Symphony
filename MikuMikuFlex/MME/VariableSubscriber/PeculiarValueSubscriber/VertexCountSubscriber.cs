using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.PeculiarValueSubscriber
{
    internal class VertexCountSubscriber : PeculiarValueSubscriberBase
    {
        public override string Name
        {
            get { return "VertexCount"; }
        }

        public override VariableType Type
        {
            get { return VariableType.Int; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument argument)
        {
            subscribeTo.AsScalar().Set(argument.Model.VertexCount);
        }
    }
}