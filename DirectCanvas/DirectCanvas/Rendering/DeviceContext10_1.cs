using System;
using SlimDX.Direct2D;
using SlimDX.Direct3D10;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D10.Device;
using FeatureLevel = SlimDX.Direct3D10_1.FeatureLevel;
using FillMode = SlimDX.Direct3D10.FillMode;

namespace DirectCanvas.Rendering
{
    /// <summary>
    /// The DeviceContext10_1 is an internal class to keep track of 
    /// Direct3D objects relating to the Device and factories
    /// </summary>
    internal class DeviceContext10_1 : IDisposable
    {
        /// <summary>
        /// Our creation settings
        /// </summary>
        private readonly DeviceSettings10_1 m_settings;

        public DeviceContext10_1(DeviceSettings10_1 settings)
        {
            m_settings = settings;

            Direct3DFactory = new Factory1();
         
            /* Create a Direct3D device using our passed settings */
            Device = new SlimDX.Direct3D10_1.Device1(Direct3DFactory.GetAdapter(m_settings.AdapterOrdinal),
                                                       DriverType.Hardware,
                                                       settings.CreationFlags,
                                                       FeatureLevel.Level_10_0);

            /* Create a Direct2D factory while we are at it...*/
            Direct2DFactory = new SlimDX.Direct2D.Factory(FactoryType.Multithreaded);

            MakeBothSidesRendered();
        }

        public void MakeBothSidesRendered()
        {
            var rsDesc = new RasterizerStateDescription();
            rsDesc.IsAntialiasedLineEnabled = false;
            rsDesc.CullMode = CullMode.None;
            rsDesc.DepthBias = 0;
            rsDesc.DepthBiasClamp = 0;
            rsDesc.IsDepthClipEnabled = true;
            rsDesc.FillMode = FillMode.Solid;
            rsDesc.IsFrontCounterclockwise = false;
            rsDesc.IsMultisampleEnabled = false;
            rsDesc.IsScissorEnabled = false;
            rsDesc.SlopeScaledDepthBias = 0;
            Device.Rasterizer.State = RasterizerState.FromDescription(Device, rsDesc);
        }

        /// <summary>
        /// The internal Direct3D factory
        /// </summary>
        public Factory1 Direct3DFactory { get; private set; }

        public SlimDX.Direct2D.Factory Direct2DFactory { get; private set; }

        /// <summary>
        /// Gets the underlying Direct3D10 device.
        /// </summary>
        public Device Device { get; private set; }

        public void Dispose()
        {
            Direct3DFactory.Dispose();
            Device.Dispose();
        }
    }
}
