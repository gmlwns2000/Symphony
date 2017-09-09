using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldInverseMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldInverseMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal WorldInverseMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDINVERSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Invert(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model)),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldInverseMatrixSubscriber(Object);
        }
    }
}