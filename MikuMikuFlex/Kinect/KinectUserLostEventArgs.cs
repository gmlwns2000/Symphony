using NiTEWrapper;

namespace MMF.Kinect
{
    public class KinectUserLostEventArgs : System.EventArgs
    {
        public UserData data;

        public KinectUserLostEventArgs(UserData user)
        {
            data = user;
        }
    }
}
