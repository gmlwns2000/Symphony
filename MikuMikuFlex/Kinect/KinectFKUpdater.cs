using MMF.Bone;
using NiTEWrapper;
using SlimDX;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Media3D;

namespace MMF.Kinect
{
    public class KinectFKUpdater : ITransformUpdater
    {
        private static System.Collections.Generic.Dictionary<SkeletonJoint.JointType, SkeletonJoint.JointType> ParentPairDictionary;

        private PMXBone[] bones;

        private KinectDeviceManager device;

        public Skeleton trackTarget;

        public short CurrentTrackUserId;

        public event System.EventHandler<UserData> TrackingUser;

        public System.Collections.Generic.Dictionary<SkeletonJoint.JointType, string> BindDictionary
        {
            get;
            set;
        }

        static KinectFKUpdater()
        {
            KinectFKUpdater.ParentPairDictionary = new System.Collections.Generic.Dictionary<SkeletonJoint.JointType, SkeletonJoint.JointType>();
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.HEAD, SkeletonJoint.JointType.NECK);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.LEFT_SHOULDER, SkeletonJoint.JointType.NECK);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.RIGHT_SHOULDER, SkeletonJoint.JointType.NECK);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.LEFT_ELBOW, SkeletonJoint.JointType.LEFT_SHOULDER);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.LEFT_HAND, SkeletonJoint.JointType.LEFT_ELBOW);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.RIGHT_ELBOW, SkeletonJoint.JointType.RIGHT_SHOULDER);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.RIGHT_HAND, SkeletonJoint.JointType.RIGHT_ELBOW);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.TORSO, SkeletonJoint.JointType.NECK);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.RIGHT_HIP, SkeletonJoint.JointType.TORSO);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.LEFT_HIP, SkeletonJoint.JointType.TORSO);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.LEFT_KNEE, SkeletonJoint.JointType.LEFT_HIP);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.RIGHT_KNEE, SkeletonJoint.JointType.RIGHT_HIP);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.RIGHT_FOOT, SkeletonJoint.JointType.RIGHT_KNEE);
            KinectFKUpdater.ParentPairDictionary.Add(SkeletonJoint.JointType.LEFT_FOOT, SkeletonJoint.JointType.LEFT_KNEE);
        }

        public void StartTracking(short userId)
        {
            NiTE.Status status = device.NiteUserTracker.StartSkeletonTracking(userId);
            CurrentTrackUserId = userId;
            Debug.WriteLine(status.ToString());
        }

        public KinectFKUpdater(KinectDeviceManager device, PMXBone[] bones)
        {
            this.device = device;
            this.bones = bones;
            BindDictionary = new System.Collections.Generic.Dictionary<SkeletonJoint.JointType, string>();
            BindDictionary.Add(SkeletonJoint.JointType.RIGHT_SHOULDER, "右腕");
            BindDictionary.Add(SkeletonJoint.JointType.RIGHT_KNEE, "右ひじ");
        }

        public bool UpdateTransform()
        {
            UserTrackerFrameRef currentUserTrackerFrameRef = device.CurrentUserTrackerFrameRef;
            bool result;
            if (CurrentTrackUserId >= 0)
            {
                UserData[] array = (from sk in currentUserTrackerFrameRef.Users
                                    where sk.UserId == CurrentTrackUserId
                                    select sk).ToArray<UserData>();
                if (array.Length != 1)
                {
                    result = true;
                    return result;
                }
                Skeleton skeleton = array[0].Skeleton;
                if (skeleton.State == Skeleton.SkeletonState.TRACKED)
                {
                    trackTarget = skeleton;
                    if (TrackingUser != null)
                    {
                        TrackingUser(this, array[0]);
                    }
                    PMXBone bone = getBone("頭");
                    PMXBone bone2 = getBone("首");
                    PMXBone bone3 = getBone("上半身");
                    PMXBone bone4 = getBone("右腕");
                    PMXBone bone5 = getBone("右ひじ");
                    PMXBone bone6 = getBone("右手首");
                    PMXBone bone7 = getBone("右足");
                    PMXBone bone8 = getBone("右ひざ");
                    PMXBone bone9 = getBone("右足首");
                    PMXBone bone10 = getBone("左腕");
                    PMXBone bone11 = getBone("左ひじ");
                    PMXBone bone12 = getBone("左手首");
                    PMXBone bone13 = getBone("左足");
                    PMXBone bone14 = getBone("左ひざ");
                    PMXBone bone15 = getBone("左足首");
                    SkeletonJoint joint = skeleton.getJoint(SkeletonJoint.JointType.HEAD);
                    SkeletonJoint joint2 = skeleton.getJoint(SkeletonJoint.JointType.NECK);
                    SkeletonJoint joint3 = skeleton.getJoint(SkeletonJoint.JointType.RIGHT_SHOULDER);
                    SkeletonJoint joint4 = skeleton.getJoint(SkeletonJoint.JointType.LEFT_SHOULDER);
                    SkeletonJoint joint5 = skeleton.getJoint(SkeletonJoint.JointType.RIGHT_ELBOW);
                    SkeletonJoint joint6 = skeleton.getJoint(SkeletonJoint.JointType.LEFT_ELBOW);
                    SkeletonJoint joint7 = skeleton.getJoint(SkeletonJoint.JointType.RIGHT_HAND);
                    SkeletonJoint joint8 = skeleton.getJoint(SkeletonJoint.JointType.LEFT_HAND);
                    SkeletonJoint joint9 = skeleton.getJoint(SkeletonJoint.JointType.TORSO);
                    SkeletonJoint joint10 = skeleton.getJoint(SkeletonJoint.JointType.RIGHT_HIP);
                    SkeletonJoint joint11 = skeleton.getJoint(SkeletonJoint.JointType.LEFT_HIP);
                    SkeletonJoint joint12 = skeleton.getJoint(SkeletonJoint.JointType.RIGHT_KNEE);
                    SkeletonJoint joint13 = skeleton.getJoint(SkeletonJoint.JointType.LEFT_KNEE);
                    SkeletonJoint joint14 = skeleton.getJoint(SkeletonJoint.JointType.RIGHT_FOOT);
                    SkeletonJoint joint15 = skeleton.getJoint(SkeletonJoint.JointType.LEFT_FOOT);
                    Vector3 left = Vector3.Normalize(ToVector3(joint3.Position) - ToVector3(joint4.Position));
                    Vector3 right = Vector3.Normalize(ToVector3(joint10.Position) - ToVector3(joint11.Position));
                    float angle = (float)System.Math.Acos(System.Math.Min(Vector3.Dot(left, right), 1f));
                    bone3.Rotation *= SlimDX.Quaternion.RotationAxis(new Vector3(0f, 1f, 0f), angle);
                    bone3.UpdateGrobalPose();
                    getRotation(bone2, bone, skeleton, joint2, joint);
                    getRotation(bone4, bone5, skeleton, joint3, joint5);
                    getRotation(bone5, bone6, skeleton, joint5, joint7);
                    getRotation(bone10, bone11, skeleton, joint4, joint6);
                    getRotation(bone11, bone12, skeleton, joint6, joint8);
                    getRotation(bone3, bone2, skeleton, joint9, joint2);
                    getRotation(bone7, bone8, skeleton, joint10, joint12);
                    getRotation(bone8, bone9, skeleton, joint12, joint14);
                    getRotation(bone13, bone14, skeleton, joint11, joint13);
                    getRotation(bone14, bone15, skeleton, joint13, joint15);
                }
            }
            result = true;
            return result;
        }

        private PMXBone getBone(string name)
        {
            PMXBone[] array = bones;
            PMXBone result;
            for (int i = 0; i < array.Length; i++)
            {
                PMXBone pMXBone = array[i];
                if (pMXBone.BoneName.Equals(name))
                {
                    result = pMXBone;
                    return result;
                }
            }
            result = null;
            return result;
        }

        private void getRotation(PMXBone b1, PMXBone b2, Skeleton skeleton, SkeletonJoint skel1, SkeletonJoint skel2)
        {
            if (skel1.PositionConfidence >= 0.5 && skel2.PositionConfidence >= 0.5)
            {
                Vector3 left = Vector3.Normalize(ToVector3(skel1.Position) - ToVector3(skel2.Position));
                Vector3 right = Vector3.Normalize(Vector3.TransformCoordinate(b2.Position, b2.GlobalPose) - Vector3.TransformCoordinate(b1.Position, b1.GlobalPose));
                Vector3 axis = Vector3.Cross(left, right);
                float angle = -(float)System.Math.Acos(Vector3.Dot(left, right));
                b1.Rotation *= SlimDX.Quaternion.RotationAxis(axis, angle);
                b1.UpdateGrobalPose();
            }
        }

        private Vector3 ToVector3(Point3D pos)
        {
            return new Vector3((float)pos.X, -(float)pos.Y, -(float)pos.Z);
        }
    }
}
