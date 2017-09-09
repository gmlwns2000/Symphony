using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D10.Device;

namespace DirectCanvas.Rendering.Materials
{
    class StagingTexture : Texture
    {
        public StagingTexture(Device device,
                                   int width,
                                   int height,
                                   Format format = Format.B8G8R8A8_UNorm,
                                   ResourceOptionFlags optionFlags = ResourceOptionFlags.None)
            : base(new Texture2D(device, CreateTextureDescription(width, height, format, optionFlags)))
        {
        }

        public StagingTexture(Texture2D texture2D)
            : base(texture2D)
        {
        }

        public StagingTexture(Device device, Texture2DDescription desc)
            : base(new Texture2D(device, desc))
        {
        }

        private static Texture2DDescription CreateTextureDescription(int width, int height, Format format, ResourceOptionFlags optionFlags)
        {
            return new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
                Format = format,
                Height = height,
                Width = width,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Staging,
                OptionFlags = optionFlags
            };
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
