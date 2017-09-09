using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class MaterialSphereMapSubscriber : MaterialSubscriberBase
    {
        public override string Semantics
        {
            get { return "MATERIALSPHEREMAP"; }
        }

        public override VariableType[] Types
        {
            get { return new[] {VariableType.Texture2D}; }
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new MaterialSphereMapSubscriber();
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsResource().SetResource(variable.Material.MaterialSphereMap);
        }
    }
}