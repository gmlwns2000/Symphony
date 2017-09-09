using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class EmissiveVectorSubscriber : MaterialSubscriberBase
    {
        internal EmissiveVectorSubscriber()
        {
        }

        internal EmissiveVectorSubscriber(TargetObject target, bool isVector3) : base(target, isVector3)
        {
        }

        public override string Semantics
        {
            get { return "EMISSIVE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            SetAsVector(variable.Material.AmbientColor, subscribeTo, IsVector3);
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new EmissiveVectorSubscriber(target, isVector3);
        }
    }
}