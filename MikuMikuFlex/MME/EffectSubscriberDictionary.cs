using MMF.MME.VariableSubscriber;
using System.Collections.Generic;

namespace MMF.MME
{
    public class EffectSubscriberDictionary : Dictionary<string, SubscriberBase>
    {
        public void Add(SubscriberBase subs)
        {
            Add(subs.Semantics, subs);
        }
    }
}
