using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.StreamBuffers
{
    class VertexStreamBuffer<TDataType> : StreamBuffer<TDataType> where TDataType : struct
    {
        private VertexBufferBinding m_vertexBufferBinding;
        private readonly int m_slot;

        public VertexStreamBuffer(Device device, 
                                  TDataType[] data, 
                                  int slot, 
                                  ResourceUsage usage, 
                                  CpuAccessFlags accessFlags = CpuAccessFlags.None, 
                                  bool canRead = true, 
                                  bool canWrite = true) :
            base(device, data, BindFlags.VertexBuffer, usage, accessFlags, canRead, canWrite)
        {
            m_slot = slot;
            CreateVertexBufferBinding();
        }

        public VertexStreamBuffer(Device device,
                          int byteSize,
                          int slot,
                          ResourceUsage usage,
                          CpuAccessFlags accessFlags = CpuAccessFlags.None,
                          bool canRead = true,
                          bool canWrite = true) :
            base(device, byteSize, BindFlags.VertexBuffer, usage, accessFlags, canRead, canWrite)
        {
            m_slot = slot;
            CreateVertexBufferBinding();
        }

        private void CreateVertexBufferBinding()
        {
            m_vertexBufferBinding = new VertexBufferBinding(InternalDeviceBuffer, DataItemSize, 0);
        }

        public override void SetRenderState()
        {
            InternalDevice.InputAssembler.SetVertexBuffers(m_slot, m_vertexBufferBinding);
        }
    }
}
