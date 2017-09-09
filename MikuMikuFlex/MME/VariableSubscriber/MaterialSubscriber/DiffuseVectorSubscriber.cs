using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class DiffuseVectorSubscriber : MaterialSubscriberBase
    {
        private DiffuseVectorSubscriber(TargetObject target, bool isVector3) : base(target, isVector3)
        {
        }

        internal DiffuseVectorSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "DIFFUSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (Target == TargetObject.Geometry) SetAsVector(variable.Material.DiffuseColor, subscribeTo, IsVector3);
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new DiffuseVectorSubscriber(target, isVector3);
        }
    }
}