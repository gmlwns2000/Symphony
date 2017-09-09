using MMF.Motion;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.TimeSubscriber
{
    internal sealed class ElapsedTimeSubScriber : TimeSubscriberBase
    {
        private ElapsedTimeSubScriber(bool syncInEditMode) : base(syncInEditMode)
        {
        }

        internal ElapsedTimeSubScriber()
        {
        }

        public override string Semantics
        {
            get { return "ELAPSEDTIME"; }
        }

        protected override void Subscribe(EffectVariable variable, IMotionManager motion, RenderContext context)
        {
            if (SyncInEditMode)
            {
                variable.AsScalar().Set(motion.ElapsedTime);
            }
            else
            {
                variable.AsScalar().Set(motion.ElapsedTime);
            }
        }

        protected override SubscriberBase GetSubscriberInstance(bool syncInEditMode)
        {
            return new ElapsedTimeSubScriber(syncInEditMode);
        }
    }
}