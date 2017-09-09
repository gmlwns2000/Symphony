using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ProjectionInverseTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private ProjectionInverseTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal ProjectionInverseTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "PROJECTIONINVERSETRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(
                        Matrix.Invert(variable.Context.MatrixManager.ProjectionMatrixManager.ProjectionMatrix)),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ProjectionInverseTransposeMatrixSubscriber(Object);
        }
    }
}