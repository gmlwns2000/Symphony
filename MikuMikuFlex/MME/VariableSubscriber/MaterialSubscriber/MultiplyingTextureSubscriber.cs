using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class MultiplyingTextureSubscriber : MaterialSubscriberBase
    {
        public override string Semantics
        {
            get { return "MULTIPLYINGTEXTURE"; }
        }


        public override VariableType[] Types
        {
            get { return new[] {VariableType.Float4}; }
        }

        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new MultiplyingTextureSubscriber();
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            subscribeTo.AsVector().Set(variable.Material.TextureMulValue);
        }
    }
}