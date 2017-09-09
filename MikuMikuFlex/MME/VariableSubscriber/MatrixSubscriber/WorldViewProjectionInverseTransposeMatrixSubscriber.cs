using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewProjectionInverseTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewProjectionInverseTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal WorldViewProjectionInverseTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEWPROJECTIONINVERSETRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(
                        Matrix.Invert(variable.Context.MatrixManager.makeWorldViewProjectionMatrix(variable.Model))),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewProjectionInverseTransposeMatrixSubscriber(Object);
        }
    }
}