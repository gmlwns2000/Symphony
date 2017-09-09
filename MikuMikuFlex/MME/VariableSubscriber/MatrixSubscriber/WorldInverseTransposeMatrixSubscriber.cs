using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldInverseTransposeMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldInverseTransposeMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal WorldInverseTransposeMatrixSubscriber()
        {
        }


        public override string Semantics
        {
            get { return "WORLDINVERSETRANSPOSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Transpose(
                        Matrix.Invert(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model))),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldInverseTransposeMatrixSubscriber(Object);
        }
    }
}