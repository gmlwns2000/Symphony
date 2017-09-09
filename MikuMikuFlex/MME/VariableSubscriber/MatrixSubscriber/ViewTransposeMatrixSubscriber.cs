using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private ViewTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal ViewTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "VIEWTRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(Matrix.Transpose(variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix), subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewTransposeMatrixSubscriber(Object);
        }
    }
}