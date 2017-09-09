using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal WorldTransposeMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDTRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model)),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldTransposeMatrixSubscriber(Object);
        }
    }
}