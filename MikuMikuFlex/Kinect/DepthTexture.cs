using MMF.Sprite;
using OpenNIWrapper;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.Kinect
{
    public class DepthTexture : IDynamicTexture, System.IDisposable
    {
        private RenderContext context;

        private VideoStream videoStream;

        public Texture2D TextureResource
        {
            get;
            private set;
        }

        public ShaderResourceView TextureResourceView
        {
            get;
            private set;
        }

        public int MaxDistance
        {
            get;
            set;
        }

        public bool NeedUpdate
        {
            get;
            private set;
        }

        public DepthTexture(RenderContext context, int maxDistance, KinectDeviceManager device)
        {
            this.context = context;
            MaxDistance = maxDistance;
            Texture2DDescription description = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.Write,
                Format = Format.R8G8B8A8_UNorm,
                Height = 480,
                Width = 640,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Dynamic
            };
            TextureResource = new Texture2D(context.DeviceManager.Device, description);
            videoStream = device.KinnectDevice.CreateVideoStream(OpenNIWrapper.Device.SensorType.DEPTH);
            videoStream.Start();
            TextureResourceView = new ShaderResourceView(context.DeviceManager.Device, TextureResource);
            NeedUpdate = true;
        }

        public void UpdateTexture()
        {
            int num = 640;
            int num2 = 480;
            DataBox dataBox = context.DeviceManager.Context.MapSubresource(TextureResource, 0, MapMode.WriteDiscard, SlimDX.Direct3D11.MapFlags.None);
            VideoFrameRef videoFrameRef = videoStream.readFrame();
            byte[] array = new byte[num * num2 * 2];
            System.Collections.Generic.List<byte> list = new System.Collections.Generic.List<byte>();
            System.Runtime.InteropServices.Marshal.Copy(videoFrameRef.Data, array, 0, num * num2 * 2);
            dataBox.Data.Seek(0L, System.IO.SeekOrigin.Begin);
            for (int i = 0; i < num * num2; i++)
            {
                ushort num3 = System.BitConverter.ToUInt16(array, i * 2);
                byte item = (byte)System.Math.Min(num3 * 255 / MaxDistance, 255);
                list.Add(item);
                list.Add(item);
                list.Add(item);
                list.Add(255);
            }
            dataBox.Data.WriteRange<byte>(list.ToArray());
            context.DeviceManager.Context.UnmapSubresource(TextureResource, 0);
        }

        public void Dispose()
        {
            TextureResource.Dispose();
        }
    }
}
