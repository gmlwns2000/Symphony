using SlimDX.Direct3D10;

namespace DirectCanvas.Rendering
{
    /// <summary>
    /// Internal initialization settings for a Direct3D 10.1 device
    /// </summary>
    internal class DeviceSettings10_1
    {
        public int AdapterOrdinal { get; set; }

        public DeviceCreationFlags CreationFlags { get; set; }
    }
}
