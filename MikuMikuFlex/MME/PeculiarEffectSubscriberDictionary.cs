using MMF.MME.VariableSubscriber.PeculiarValueSubscriber;

namespace MMF.MME
{
    public class PeculiarEffectSubscriberDictionary : System.Collections.Generic.Dictionary<string, PeculiarValueSubscriberBase>
    {
        public void Add(PeculiarValueSubscriberBase subscriber)
        {
            Add(subscriber.Name, subscriber);
        }
    }
}
