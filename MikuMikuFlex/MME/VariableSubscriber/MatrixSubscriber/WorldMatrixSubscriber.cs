using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldMatrixSubscriber(ObjectAnnotationType objectAnnotationType) : base(objectAnnotationType)
        {
        }

        internal WorldMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLD"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)

        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(variable.Context.MatrixManager.WorldMatrixManager.getWorldMatrix(variable.Model),
                    subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldMatrixSubscriber(Object);
        }
    }
}