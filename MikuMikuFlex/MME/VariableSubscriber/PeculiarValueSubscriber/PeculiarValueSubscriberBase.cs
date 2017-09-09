using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.PeculiarValueSubscriber
{
    public abstract class PeculiarValueSubscriberBase
    {
        public abstract string Name
        {
            get;
        }

        public abstract VariableType Type
        {
            get;
        }

        public abstract void Subscribe(EffectVariable subscribeTo, SubscribeArgument argument);
    }
}
