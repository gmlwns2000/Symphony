using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class GroundShadowColorVectorSubscriber : MaterialSubscriberBase
    {
        private GroundShadowColorVectorSubscriber(TargetObject target, bool isVector3) : base(target, isVector3)
        {
        }

        internal GroundShadowColorVectorSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "GROUNDSHADOWCOLOR"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (Target == TargetObject.Geometry)
                SetAsVector(variable.Material.GroundShadowColor, subscribeTo, IsVector3);
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new GroundShadowColorVectorSubscriber(target, isVector3);
        }
    }
}