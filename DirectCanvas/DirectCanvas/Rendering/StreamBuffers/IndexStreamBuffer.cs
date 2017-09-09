using System;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D10.Device;

namespace DirectCanvas.Rendering.StreamBuffers
{
    class IndexStreamBuffer : StreamBuffer<ushort>
    {
        public IndexStreamBuffer(Device device, 
                                 ushort[] data, 
                                 ResourceUsage usage = ResourceUsage.Default, 
                                 CpuAccessFlags accessFlags = CpuAccessFlags.None, 
                                 bool canRead = true, 
                                 bool canWrite = true) 
            : base(device, data, BindFlags.IndexBuffer, usage, accessFlags, canRead, canWrite )
        {
            
        }

        public override void SetRenderState()
        {
            InternalDevice.InputAssembler.SetIndexBuffer(InternalDeviceBuffer, Format.R16_UInt, 0);
        }
    }
}
