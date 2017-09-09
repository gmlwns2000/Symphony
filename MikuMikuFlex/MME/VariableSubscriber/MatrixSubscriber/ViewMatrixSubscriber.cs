using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewMatrixSubscriber : MatrixSubscriberBase
    {
        private ViewMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal ViewMatrixSubscriber()
        {
        }


        public override string Semantics
        {
            get { return "VIEW"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix, subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewMatrixSubscriber(Object);
        }
    }
}