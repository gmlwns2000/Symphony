using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewProjectionTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private ViewProjectionTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal ViewProjectionTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "VIEWPROJECTIONTRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix*
                                     variable.Context.MatrixManager.ProjectionMatrixManager.ProjectionMatrix),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewProjectionTransposeMatrixSubscriber(Object);
        }
    }
}