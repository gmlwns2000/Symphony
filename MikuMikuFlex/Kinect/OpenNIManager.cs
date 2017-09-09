using NiTEWrapper;
using OpenNIWrapper;

namespace MMF.Kinect
{
    public class OpenNIManager : System.IDisposable
    {
        private static OpenNIManager Instance;

        private static System.Collections.Generic.List<Device> createdDevices = new System.Collections.Generic.List<Device>();

        public static DeviceInfo[] ConnectedDevices;

        public static void ShutDown()
        {
            if (OpenNIManager.Instance != null)
            {
                OpenNIManager.Instance.Dispose();
            }
        }

        internal OpenNIManager()
        {
        }

        ~OpenNIManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (createdDevices != null)
            {
                foreach (Device current in OpenNIManager.createdDevices)
                {
                    current.Close();
                    current.Dispose();
                }
                createdDevices.Clear();
                createdDevices = null;
            }
            OpenNI.Shutdown();
            NiTE.Shutdown();
            System.GC.SuppressFinalize(this);
        }

        public static void Initialize()
        {
            OpenNI.Shutdown();
            OpenNI.Initialize();
            NiTE.Initialize();
            OpenNIManager.ConnectedDevices = OpenNI.EnumerateDevices();
            OpenNI.onDeviceConnected += new OpenNI.DeviceConnectionStateChanged(OpenNIManager.OpenNI_onDeviceConnected);
            OpenNI.onDeviceDisconnected += new OpenNI.DeviceConnectionStateChanged(OpenNIManager.OpenNI_onDeviceConnected);
            OpenNI.onDeviceStateChanged += new OpenNI.DeviceStateChanged(OpenNIManager.OpenNI_onDeviceStateChanged);
            OpenNIManager.Instance = new OpenNIManager();
        }

        private static void OpenNI_onDeviceStateChanged(DeviceInfo Device, OpenNI.DeviceState state)
        {
            OpenNIManager.ConnectedDevices = OpenNI.EnumerateDevices();
        }

        private static void OpenNI_onDeviceConnected(DeviceInfo Device)
        {
            OpenNIManager.ConnectedDevices = OpenNI.EnumerateDevices();
        }

        public static KinectDeviceManager getDevice(string uri = null)
        {
            Device device = Device.Open(uri, "");
            OpenNIManager.createdDevices.Add(device);
            return new KinectDeviceManager(device);
        }
    }
}
