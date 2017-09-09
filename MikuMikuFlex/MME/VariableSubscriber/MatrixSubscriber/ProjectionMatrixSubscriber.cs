using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ProjectionMatrixSubscriber : MatrixSubscriberBase
    {
        private ProjectionMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal ProjectionMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "PROJECTION"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(variable.Context.MatrixManager.ProjectionMatrixManager.ProjectionMatrix, subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ProjectionMatrixSubscriber(Object);
        }
    }
}