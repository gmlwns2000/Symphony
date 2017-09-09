using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal WorldViewMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEW"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(
                    variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model)*
                    variable.Context.MatrixManager.ViewMatrixManager.ViewMatrix, subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewMatrixSubscriber(Object);
        }
    }
}