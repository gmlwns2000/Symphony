using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.WorldInfoSubscriber
{
    public sealed class DirectionSubscriber : WorldInfoSubscriberBase
    {
        private DirectionSubscriber(ObjectAnnotationType type) : base(type)
        {
        }

        internal DirectionSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "DIRECTION"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (Object == ObjectAnnotationType.Camera)
            {
                Vector3 direction =
                    Vector3.Normalize(variable.Context.MatrixManager.ViewMatrixManager.CameraLookAt -
                                      variable.Context.MatrixManager.ViewMatrixManager.CameraPosition);
                subscribeTo.AsVector().Set(direction);
            }
            else
            {
                subscribeTo.AsVector().Set(variable.Context.LightManager.Direction);
            }
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType type)
        {
            return new DirectionSubscriber(type);
        }
    }
}