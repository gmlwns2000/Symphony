using MMF.Motion;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.TimeSubscriber
{
    internal sealed class TimeSubScriber : TimeSubscriberBase
    {
        private TimeSubScriber(bool syncInEditMode) : base(syncInEditMode)
        {
        }

        internal TimeSubScriber()
        {
        }

        public override string Semantics
        {
            get { return "TIME"; }
        }

        protected override SubscriberBase GetSubscriberInstance(bool syncInEditMode)
        {
            return new TimeSubScriber(syncInEditMode);
        }

        protected override void Subscribe(EffectVariable variable, IMotionManager motion, RenderContext context)
        {
            if (SyncInEditMode)
            {
                variable.AsScalar().Set(motion.CurrentFrame);
            }
            else
            {
                variable.AsScalar().Set(MotionTimer.stopWatch.ElapsedMilliseconds/1000f);
            }
        }
    }
}