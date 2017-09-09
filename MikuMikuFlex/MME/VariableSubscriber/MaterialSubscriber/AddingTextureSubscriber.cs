using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class AddingTextureSubscriber : MaterialSubscriberBase
    {
        public override string Semantics
        {
            get { return "ADDINGTEXTURE"; }
        }

        public override VariableType[] Types
        {
            get { return new[] {VariableType.Float4}; }
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new AddingTextureSubscriber();
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsVector().Set(variable.Material.TextureAddValue);
        }
    }
}