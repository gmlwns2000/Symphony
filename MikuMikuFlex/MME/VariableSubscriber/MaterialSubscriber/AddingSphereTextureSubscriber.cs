using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class AddingSphereTextureSubscriber : MaterialSubscriberBase
    {
        public override string Semantics
        {
            get { return "ADDINGSPHERETEXTURE"; }
        }

        public override VariableType[] Types
        {
            get { return new[] {VariableType.Float4}; }
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new AddingSphereTextureSubscriber();
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsVector().Set(variable.Material.SphereAddValue);
        }
    }
}