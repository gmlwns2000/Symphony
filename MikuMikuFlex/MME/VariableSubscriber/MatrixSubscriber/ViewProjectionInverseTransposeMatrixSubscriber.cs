using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewProjectionInverseTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private ViewProjectionInverseTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal ViewProjectionInverseTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "VIEWPROJECTIONINVERSETRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(
                        Matrix.Invert(variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix*
                                      variable.Context.MatrixManager.ProjectionMatrixManager.ProjectionMatrix)),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewProjectionInverseTransposeMatrixSubscriber(Object);
        }
    }
}