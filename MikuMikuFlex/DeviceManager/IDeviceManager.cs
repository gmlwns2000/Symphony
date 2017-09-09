using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace MMF.DeviceManager
{
    public interface IDeviceManager : System.IDisposable
    {
        SlimDX.Direct3D11.Device Device
        {
            get;
        }

        SlimDX.Direct3D10.Device Device10
        {
            get;
        }

        FeatureLevel DeviceFeatureLevel
        {
            get;
        }

        DeviceContext Context
        {
            get;
        }

        Adapter CurrentAdapter
        {
            get;
        }

        Factory1 Factory
        {
            get;
            set;
        }

        void Load();
    }
}
