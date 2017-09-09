using System;
using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D10;
using Buffer = SlimDX.Direct3D10.Buffer;

namespace DirectCanvas.Rendering.StreamBuffers
{
    abstract class StreamBuffer<TDataType> : IStreamBuffer where TDataType : struct
    {
        private readonly Device m_device;
        private int m_byteSize;
        private readonly BindFlags m_bindFlags = BindFlags.None;
        private readonly ResourceUsage m_usage = ResourceUsage.Default;
        private readonly CpuAccessFlags m_accessFlags = CpuAccessFlags.None;
        private readonly bool m_canRead = true;
        private readonly bool m_canWrite = true;
        private Buffer m_internalDeviceBuffer;
        private DataStream m_currentDataStream;

        protected StreamBuffer(Device device, 
                               TDataType[] data, 
                               BindFlags bindFlags, 
                               ResourceUsage usage = ResourceUsage.Default, 
                               CpuAccessFlags accessFlags = CpuAccessFlags.None, 
                               bool canRead = true, 
                               bool canWrite = true)
        {
            m_device = device;
            m_bindFlags = bindFlags;
            m_usage = usage;
            m_accessFlags = accessFlags;
            m_canRead = canRead;
            m_canWrite = canWrite;

            CreateAndFillBuffer(data);
        }

        protected StreamBuffer(Device device,
                               int byteSize,
                               BindFlags bindFlags,
                               ResourceUsage usage = ResourceUsage.Default,
                               CpuAccessFlags accessFlags = CpuAccessFlags.None,
                               bool canRead = true,
                               bool canWrite = true)
        {
            m_device = device;
            m_byteSize = byteSize;
            m_bindFlags = bindFlags;
            m_usage = usage;
            m_accessFlags = accessFlags;
            m_canRead = canRead;
            m_canWrite = canWrite;
          
            CreateEmptyBuffer();
        }

        public abstract void SetRenderState();

        public int DataItemSize { get; private set; }

        public Device InternalDevice
        {
            get { return m_device; }
        }

        public Buffer InternalDeviceBuffer
        {
            get { return m_internalDeviceBuffer; }
        }

        public int BufferSize
        {
            get { return m_byteSize; }
            private set { m_byteSize = value; }
        }

        private BufferDescription CreateBufferDescription()
        {
            var bufferDesc = new BufferDescription
            {
                BindFlags = m_bindFlags,
                CpuAccessFlags = m_accessFlags,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = BufferSize,
                Usage = m_usage
            };

            return bufferDesc;
        }

        private void CreateEmptyBuffer()
        {
            DataItemSize = Marshal.SizeOf(typeof(TDataType));

            var desc = CreateBufferDescription();

            var dataStream = new DataStream(m_byteSize, m_canRead, m_canWrite);

            dataStream.Position = 0;

            m_internalDeviceBuffer = new Buffer(InternalDevice, dataStream, desc);
        }

        private void CreateAndFillBuffer(TDataType[] data)
        {
            DataItemSize = Marshal.SizeOf(typeof (TDataType));
            BufferSize = data.Length * DataItemSize;

            var desc = CreateBufferDescription();

            var dataStream = new DataStream(BufferSize, m_canRead, m_canWrite);

            for (int i = 0; i < data.Length; i++)
            {
                dataStream.Write(data[i]);
            }

            dataStream.Position = 0;
            m_internalDeviceBuffer = new Buffer(InternalDevice, dataStream, desc);
        }

        public void BeginBatchWrite()
        {
           m_currentDataStream = m_internalDeviceBuffer.Map(MapMode.WriteDiscard, MapFlags.None);
        }

        public void WriteBatch(ref TDataType data)
        {
            m_currentDataStream.Write(data);
        }

        public void EndBatchWrite()
        {
            m_internalDeviceBuffer.Unmap();

            m_currentDataStream = null;
        }

        public void Write(TDataType[] data)
        {
            var dataStream = m_internalDeviceBuffer.Map(MapMode.WriteDiscard, MapFlags.None);

            dataStream.WriteRange(data);

            m_internalDeviceBuffer.Unmap();
        }

        public void Write(IntPtr pData, long count)
        {
            var dataStream = m_internalDeviceBuffer.Map(MapMode.WriteDiscard, MapFlags.None);

            dataStream.WriteRange(pData, count);

            m_internalDeviceBuffer.Unmap();
        }

        public void WriteRange(ref TDataType[] data, int offset, int count)
        {
            var dataStream = m_internalDeviceBuffer.Map(MapMode.WriteDiscard, MapFlags.None);

            dataStream.WriteRange(data, offset, count);

            m_internalDeviceBuffer.Unmap();
        }

        public void Write(TDataType data)
        {
            var dataStream = m_internalDeviceBuffer.Map(MapMode.WriteDiscard, MapFlags.None);
          
            dataStream.Write(data);

            m_internalDeviceBuffer.Unmap();
        }

        public virtual void Dispose()
        {
            m_internalDeviceBuffer.Dispose();
        }
    }
}
