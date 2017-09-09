using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.MME.VariableSubscriber.ConstantSubscriber.ConstantBufferManager
{
    public abstract class ConstantBufferManagerBase<T> : System.IDisposable where T : struct
    {
        protected DataBox BufferDataBox;

        public Buffer ConstantBuffer;

        protected Device device;

        protected EffectConstantBuffer target;

        public void Dispose()
        {
            ConstantBuffer.Dispose();
        }

        public void Initialize(Device device, EffectConstantBuffer effectVariable, int size, T obj)
        {
            this.device = device;
            target = effectVariable;
            BufferDataBox = new DataBox(0, 0, new DataStream(new T[]
            {
                obj
            }, true, true));
            ConstantBuffer = new Buffer(device, new BufferDescription
            {
                SizeInBytes = size,
                BindFlags = BindFlags.ConstantBuffer
            });
            OnInitialize();
        }

        protected abstract void OnInitialize();

        public abstract void ApplyToEffect(T obj);

        protected void WriteToBuffer(T obj)
        {
            BufferDataBox.Data.WriteRange<T>(new T[]
            {
                obj
            });
            BufferDataBox.Data.Position = 0L;
            device.ImmediateContext.UpdateSubresource(BufferDataBox, ConstantBuffer, 0);
        }

        protected void SetConstantBuffer()
        {
            target.ConstantBuffer = ConstantBuffer;
        }
    }
}
