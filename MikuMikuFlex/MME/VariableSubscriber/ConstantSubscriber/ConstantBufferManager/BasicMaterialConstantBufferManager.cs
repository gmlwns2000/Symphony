namespace MMF.MME.VariableSubscriber.ConstantSubscriber.ConstantBufferManager
{
    public class BasicMaterialConstantBufferManager : ConstantBufferManagerBase<BasicMaterialConstantBufferInputLayout>
    {
        public override void ApplyToEffect(BasicMaterialConstantBufferInputLayout obj)
        {
            WriteToBuffer(obj);
            SetConstantBuffer();
        }

        protected override void OnInitialize()
        {
        }
    }
}