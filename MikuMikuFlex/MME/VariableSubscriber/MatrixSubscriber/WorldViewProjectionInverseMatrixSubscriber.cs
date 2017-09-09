using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewProjectionInverseMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewProjectionInverseMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal WorldViewProjectionInverseMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEWPROJECTIONINVERSE"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    Matrix.Invert(variable.Context.MatrixManager.makeWorldViewProjectionMatrix(variable.Model)),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewProjectionInverseMatrixSubscriber(Object);
        }
    }
}