using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewProjectionTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewProjectionTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal WorldViewProjectionTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEWPROJECTIONTRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(variable.Context.MatrixManager.makeWorldViewProjectionMatrix(variable.Model)),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewProjectionTransposeMatrixSubscriber(Object);
        }
    }
}