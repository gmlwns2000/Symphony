using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class SpecularVectorSubscriber : MaterialSubscriberBase
    {
        private SpecularVectorSubscriber(TargetObject target, bool isVector3) : base(target, isVector3)
        {
        }

        internal SpecularVectorSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "SPECULAR"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (Target == TargetObject.Geometry) SetAsVector(variable.Material.SpecularColor, subscribeTo, IsVector3);
        }


        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new SpecularVectorSubscriber(target, isVector3);
        }
    }
}