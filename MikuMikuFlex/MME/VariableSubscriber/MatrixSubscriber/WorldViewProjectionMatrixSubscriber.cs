using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.MatrixSubscriber
{
    internal sealed class WorldViewProjectionMatrixSubscriber : MatrixSubscriberBase
    {
        private WorldViewProjectionMatrixSubscriber(ObjectAnnotationType objectAnnotationType)
            : base(objectAnnotationType)
        {
        }

        internal WorldViewProjectionMatrixSubscriber()
        {
        }

        public override string Semantics
        {
            get { return "WORLDVIEWPROJECTION"; }
        }

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (TargetObject == ObjectAnnotationType.Camera)
                SetAsMatrix(variable.Context.MatrixManager.makeWorldViewProjectionMatrix(variable.Model), subscribeTo);
        }

        protected override SubscriberBase GetSubscriberInstance(ObjectAnnotationType Object)
        {
            return new WorldViewProjectionMatrixSubscriber(Object);
        }
    }
}