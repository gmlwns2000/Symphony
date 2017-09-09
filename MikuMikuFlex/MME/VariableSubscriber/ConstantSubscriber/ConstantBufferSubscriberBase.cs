namespace MMF.MME.VariableSubscriber.ConstantSubscriber
{
    public abstract class ConstantBufferSubscriberBase : SubscriberBase
    {
        public override VariableType[] Types
        {
            get { return new[] {VariableType.Cbuffer}; }
        }
    }
}