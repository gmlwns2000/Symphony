using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    public sealed class MaterialTextureSubscriber : MaterialSubscriberBase
    {
        internal MaterialTextureSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "MATERIALTEXTURE"; }
        }

        public override VariableType[] Types
        {
            get { return new[] {VariableType.Texture2D}; }
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new MaterialTextureSubscriber();
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsResource().SetResource(variable.Material.MaterialTexture);
        }
    }
}