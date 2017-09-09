using NiTEWrapper;

namespace MMF.Kinect
{
    public class KinectUserDetectEventArgs : System.EventArgs
    {
        public UserData data;

        public KinectUserDetectEventArgs(UserData user)
        {
            data = user;
        }
    }
}
