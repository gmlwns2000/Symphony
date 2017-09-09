using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class ViewInverseMatrixSubscriber : MatrixSubscriberBase
    {
        private ViewInverseMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal ViewInverseMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "VIEWINVERSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(Matrix.Invert(variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix), subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new ViewInverseMatrixSubscriber(Object);
        }
    }
}