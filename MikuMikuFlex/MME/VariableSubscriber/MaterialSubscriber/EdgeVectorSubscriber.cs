using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class EdgeVectorSubscriber : MaterialSubscriberBase
    {
        internal EdgeVectorSubscriber(TargetObject target, bool isVector3) : base(target, isVector3)
        {
        }

        internal EdgeVectorSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "EDGECOLOR"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (Target == TargetObject.Geometry) SetAsVector(variable.Material.EdgeColor, subscribeTo, IsVector3);
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new EdgeVectorSubscriber(target, isVector3);
        }
    }
}