using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MaterialSubscriber
{
    internal sealed class AmbientVectorSubscriber : MaterialSubscriberBase
    {
        private AmbientVectorSubscriber(TargetObject target, bool isVector3) : base(target, isVector3)
        {
        }

        internal AmbientVectorSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "AMBIENT"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (Target == TargetObject.Geometry) SetAsVector(new Vector4(variable.Material.DiffuseColor.X, variable.Material.DiffuseColor.Y, variable.Material.DiffuseColor.Z,0), subscribeTo, IsVector3);
        }


        protected override SubscriberBase GetSubscriberInstance(TargetObject target, bool isVector3)
        {
            return new AmbientVectorSubscriber(target, isVector3);
        }
    }
}