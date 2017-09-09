using System;
using SlimDX.Direct3D10;
using Buffer = SlimDX.Direct3D10.Buffer;

namespace DirectCanvas.Rendering.StreamBuffers
{
    internal interface IStreamBuffer : IDisposable
    {
        int BufferSize { get; }
        int DataItemSize { get; }
        Device InternalDevice { get; }
        Buffer InternalDeviceBuffer { get; }
        void SetRenderState();
    }
}