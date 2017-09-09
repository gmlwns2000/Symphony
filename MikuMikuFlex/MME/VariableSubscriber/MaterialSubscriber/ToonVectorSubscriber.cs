using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class ToonVectorSubscriber : MaterialSubscriberBase
    {
        public override string Semantics
        {
            get
            {
                return "TOONCOLOR";
            }
        }

        private ToonVectorSubscriber(TargetObject target, bool isVector3) : base(target, isVector3)
        {
        }

        public ToonVectorSubscriber()
        {
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            base.SetAsVector(variable.Material.ToonColor, subscribeTo, IsVector3);
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new ToonVectorSubscriber(target, isVector3);
        }
    }
}
