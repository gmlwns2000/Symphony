using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewInverseTransposeMatrixSubsriber : MatrixSubscriberBase
    {
        private ViewInverseTransposeMatrixSubsriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal ViewInverseTransposeMatrixSubsriber()
        {
        }

        public override string Semantics
        {
            get { return "VIEWINVERSETRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(Matrix.Invert(variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix)),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewInverseTransposeMatrixSubsriber(Object);
        }
    }
}