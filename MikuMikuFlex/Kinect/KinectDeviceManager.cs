using NiTEWrapper;
using OpenNIWrapper;
using System.Linq;

namespace MMF.Kinect
{
    public class KinectDeviceManager : System.IDisposable
    {
        private UserTracker _niteUserTracker;

        private bool hasNewValue = true;

        private UserTrackerFrameRef _currentUserTrackerFrameRef;

        public System.Collections.Generic.Dictionary<short, UserData> TrackedUsers;

        private short _userCursor;

        public UserTrackerFrameRef CurrentUserTrackerFrameRef
        {
            get
            {
                if (hasNewValue)
                {
                    _currentUserTrackerFrameRef = _niteUserTracker.readFrame();
                    UpdateUserData();
                    hasNewValue = false;
                }
                return _currentUserTrackerFrameRef;
            }
        }

        public Device KinnectDevice
        {
            get;
            private set;
        }

        public UserTracker NiteUserTracker
        {
            get
            {
                return _niteUserTracker;
            }
        }

        public short UserCursor
        {
            get
            {
                return _userCursor;
            }
            set
            {
                _userCursor = value;
                if (!_currentUserTrackerFrameRef.getUserById(value).isValid)
                {
                    _userCursor = 0;
                }
            }
        }

        private void UpdateUserData()
        {
            TrackedUsers = (from user in _currentUserTrackerFrameRef.Users
                                 where user.Skeleton.State == Skeleton.SkeletonState.TRACKED
                                 select user).ToDictionary((UserData user) => user.UserId, (UserData user) => user);
        }

        public void StartTracking()
        {
            NiteUserTracker.StartSkeletonTracking(_userCursor);
        }

        private void _niteUserTracker_onNewData(UserTracker uTracker)
        {
            hasNewValue = true;
        }

        internal KinectDeviceManager(Device device)
        {
            KinnectDevice = device;
            _niteUserTracker = UserTracker.Create(KinnectDevice);
            _niteUserTracker.onNewData += new UserTracker.UserTrackerListener(_niteUserTracker_onNewData);
        }

        public void Dispose()
        {
            KinnectDevice.Close();
            if (NiteUserTracker != null)
            {
                NiteUserTracker.Destroy();
            }
        }
    }
}