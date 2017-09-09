using MMDFileParser.PMXModelParser;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.PeculiarValueSubscriber
{
    public class SpAddSubscriber : PeculiarValueSubscriberBase
    {
        public override string Name
        {
            get { return "spadd"; }
        }

        public override VariableType Type
        {
            get { return VariableType.Bool; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument argument)
        {
            subscribeTo.AsScalar().Set(argument.Material.SphereMode == SphereMode.Add);
        }
    }
}