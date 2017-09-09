using System;
using System.Collections.Generic;
using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering.StreamBuffers
{
    class GeometryMesh : IDisposable
    {
        private readonly List<InputElement> m_inputElements = new List<InputElement>();
        private readonly List<IStreamBuffer> m_streamBuffers = new List<IStreamBuffer>();
        private readonly PrimitiveTopology m_primitiveTopology;
        private readonly Device m_device;

        public GeometryMesh(Device device, PrimitiveTopology primitiveTopology = PrimitiveTopology.TriangleList)
        {
            m_primitiveTopology = primitiveTopology;
            m_device = device;
        }

        public Device InternalDevice
        {
            get { return m_device; }
        }

        public void AddVetexStreamBuffer<TDataType>(VertexStreamBuffer<TDataType> streamBuffer, InputElement inputElement) where TDataType : struct
        {
            m_streamBuffers.Add(streamBuffer);
            m_inputElements.Add(inputElement);
        }

        public void AddVetexStreamBuffer<TDataType>(VertexStreamBuffer<TDataType> streamBuffer, InputElement[] inputElements) where TDataType : struct
        {
            m_streamBuffers.Add(streamBuffer);
            m_inputElements.AddRange(inputElements);
        }

        public void AddVetexStreamBuffer<TDataType>(VertexStreamBuffer<TDataType> streamBuffer) where TDataType : struct
        {
            m_streamBuffers.Add(streamBuffer);
        }

        public void AddIndexStreamBuffer(ushort[] bufferData, 
                                         ResourceUsage usage = ResourceUsage.Default, 
                                         CpuAccessFlags accessFlags = CpuAccessFlags.None, 
                                         bool canRead = true, 
                                         bool canWrite = true)
        {
            var streamBuffer = new IndexStreamBuffer(InternalDevice, 
                                                     bufferData, 
                                                     usage, 
                                                     accessFlags, 
                                                     canRead, 
                                                     canWrite);
            m_streamBuffers.Add(streamBuffer);
        }

        public virtual void SetRenderState()
        {
            InternalDevice.InputAssembler.SetPrimitiveTopology(m_primitiveTopology);

            for (int i = 0; i < m_streamBuffers.Count; i++)
            {
                m_streamBuffers[i].SetRenderState();
            }
        }

        public InputElement[] GetInputElements()
        {
            return m_inputElements.ToArray();
        }

        public virtual void Dispose()
        {
            foreach (var streamBuffer in m_streamBuffers)
            {
                streamBuffer.Dispose();
            }

            m_inputElements.Clear();
            m_streamBuffers.Clear();
        }
    }
}
