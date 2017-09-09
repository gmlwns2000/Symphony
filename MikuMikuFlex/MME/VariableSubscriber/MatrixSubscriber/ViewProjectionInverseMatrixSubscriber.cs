using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewProjectionInverseMatrixSubscriber : MatrixSubscriberBase
    {
        private ViewProjectionInverseMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal ViewProjectionInverseMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "VIEWPROJECTIONINVERSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Invert(variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix*
                                  variable.Context.MatrixManager.ProjectionMatrixManager.ProjectionMatrix), subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewProjectionInverseMatrixSubscriber(Object);
        }
    }
}