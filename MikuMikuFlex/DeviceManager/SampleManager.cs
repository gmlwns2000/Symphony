using SlimDX.DXGI;

namespace MMF.DeviceManager
{
    public class SampleManager
    {
        public static SampleDescription getSampleDesc(int Count, int Quality, RenderContext Context, Format TextrueFormat)
        {
            SlimDX.Direct3D11.Device device = Context.DeviceManager.Device;
            int num = Count;
            int num2;
            while (true)
            {
                num2 = device.CheckMultisampleQualityLevels(TextrueFormat, num);
                if (num2 > 0)
                {
                    break;
                }
                num--;
                if (num <= 0)
                {
                    goto NOAA;
                }
            }
            int quality = System.Math.Min(num2 - 1, Quality);
            SampleDescription result = new SampleDescription(num, quality);
            return result;
            NOAA:
            result = new SampleDescription(1, 0);
            return result;
        }
    }
}