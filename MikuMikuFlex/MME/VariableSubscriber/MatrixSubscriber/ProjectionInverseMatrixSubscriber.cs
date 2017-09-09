using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ProjectionInverseMatrixSubscriber : MatrixSubscriberBase
    {
        private ProjectionInverseMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal ProjectionInverseMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "PROJECTIONINVERSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(Matrix.Invert(variable.Context.MatrixManager.ProjectionMatrixManager.ProjectionMatrix),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ProjectionInverseMatrixSubscriber(Object);
        }
    }
}