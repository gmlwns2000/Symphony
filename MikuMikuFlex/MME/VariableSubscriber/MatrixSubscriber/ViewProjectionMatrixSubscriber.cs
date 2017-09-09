using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewProjectionMatrixSubscriber : MatrixSubscriberBase
    {
        private ViewProjectionMatrixSubscriber(ObjectAnnotationType Object) : base(Object)
        {
        }

        internal ViewProjectionMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "VIEWPROJECTION"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix*
                    variable.Context.MatrixManager.ProjectionMatrixManager.ProjectionMatrix,
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewProjectionMatrixSubscriber(Object);
        }
    }
}