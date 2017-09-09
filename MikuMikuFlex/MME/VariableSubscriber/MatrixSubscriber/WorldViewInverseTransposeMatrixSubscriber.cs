using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewInverseTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewInverseTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal WorldViewInverseTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEWINVERSETRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(
                        Matrix.Invert(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model)*
                                      variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix)), subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewInverseTransposeMatrixSubscriber(Object);
        }
    }
}