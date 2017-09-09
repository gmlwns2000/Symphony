using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.WorldInfoSubscriber
{
    public sealed class PositionSubscriber : WorldInfoSubscriberBase
    {
        private PositionSubscriber(ObjectAnnotationType obj) : base(obj)
        {
        }

        internal PositionSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "POSITION"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (Object == ObjectAnnotationType.Camera)
            {
                subscribeTo.AsVector().Set(variable.Context.MatrixManager.ViewMatrixManager.CameraPosition);
            }
            else
            {
                subscribeTo.AsVector().Set(variable.Context.LightManager.Position);
            }
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType type)
        {
            return new PositionSubscriber(type);
        }
    }
}