using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal WorldViewTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEWTRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model)*
                                     variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix), subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewTransposeMatrixSubscriber(Object);
        }
    }
}