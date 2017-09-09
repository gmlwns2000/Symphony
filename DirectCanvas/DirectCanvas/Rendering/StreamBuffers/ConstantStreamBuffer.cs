using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.StreamBuffers
{
    enum ConstantStreamBufferType
    {
        VertexShader,
        PixelShader,
    }

    class ConstantStreamBuffer<TDataType> : StreamBuffer<TDataType> where TDataType : struct
    {
        private int m_slot;
        private ConstantStreamBufferType m_dataType;

        public ConstantStreamBuffer(Device device, 
                                  TDataType[] data, 
                                  int slot, 
                                  ConstantStreamBufferType dataType,
                                  ResourceUsage usage, 
                                  CpuAccessFlags accessFlags = CpuAccessFlags.None, 
                                  bool canRead = true, 
                                  bool canWrite = true) :
            base(device, data, BindFlags.ConstantBuffer, usage, accessFlags, canRead, canWrite)
        {
            m_dataType = dataType;
            m_slot = slot;
        }

        public ConstantStreamBuffer(Device device,
                                    int byteSize,
                                    int slot,
                                    ConstantStreamBufferType dataType,
                                    ResourceUsage usage,
                                    CpuAccessFlags accessFlags = CpuAccessFlags.None,
                                    bool canRead = true,
                                    bool canWrite = true) :
            base(device, byteSize, BindFlags.ConstantBuffer, usage, accessFlags, canRead, canWrite)
        {
            m_dataType = dataType;
            m_slot = slot;
        }

        public int Slot
        {
            get { return m_slot; }
        }

        public override void SetRenderState()
        {
            switch (m_dataType)
            {
                case ConstantStreamBufferType.VertexShader:
                    InternalDevice.VertexShader.SetConstantBuffer(InternalDeviceBuffer, Slot);
                    break;
                case ConstantStreamBufferType.PixelShader:
                    InternalDevice.PixelShader.SetConstantBuffer(InternalDeviceBuffer, Slot);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
