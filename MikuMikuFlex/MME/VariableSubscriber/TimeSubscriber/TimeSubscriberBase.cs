using MMF.Model.PMX;
using MMF.Motion;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.TimeSubscriber
{
    public abstract class TimeSubscriberBase : SubscriberBase
    {
        protected bool SyncInEditMode;

        private PMXModel modelCache;

        private bool isCached;

        public override VariableType[] Types
        {
            get
            {
                return new VariableType[]
                {
                    VariableType.Float
                };
            }
        }

        protected TimeSubscriberBase(bool syncInEditMode)
        {
            SyncInEditMode = syncInEditMode;
        }

        protected TimeSubscriberBase()
        {
        }

        public override SubscriberBase GetSubscriberInstance(EffectVariable variable, RenderContext context, MMEEffectManager effectManager, int semanticIndex)
        {
            EffectVariable annotationByName = variable.GetAnnotationByName("SyncInEditMode");
            bool syncInEditMode = false;
            if (annotationByName != null)
            {
                syncInEditMode = (annotationByName.AsScalar().GetInt() == 1);
            }
            return GetSubscriberInstance(syncInEditMode);
        }

        protected abstract SubscriberBase GetSubscriberInstance(bool syncInEditMode);

        public override void Subscribe(EffectVariable subscribeTo, SubscribeArgument variable)
        {
            if (!isCached)
            {
                modelCache = (variable.Model as PMXModel);
                isCached = true;
            }
            if (modelCache != null)
            {
                Subscribe(subscribeTo, modelCache.MotionManager, variable.Context);
            }
        }

        protected abstract void Subscribe(EffectVariable variable, IMotionManager motion, RenderContext context);
    }
}
