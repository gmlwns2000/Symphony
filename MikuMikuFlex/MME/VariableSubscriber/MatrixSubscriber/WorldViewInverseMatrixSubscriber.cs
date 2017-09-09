using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewInverseMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewInverseMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal WorldViewInverseMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEWINVERSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Invert(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model)*
                                  variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix), subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewInverseMatrixSubscriber(Object);
        }
    }
}