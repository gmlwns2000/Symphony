namespace MMF.MME.VariableSubscriber.ConstantSubscriber.ConstantBufferManager
{
    class FullMaterialConstantBufferManager:ConstantBufferManagerBase<FullMaterialConstantBufferInputLayout>
    {
        protected override void OnInitialize()
        {
            
        }

        public override void ApplyToEffect(FullMaterialConstantBufferInputLayout obj)
        {
            WriteToBuffer(obj);
            SetConstantBuffer();
        }
    }
}
