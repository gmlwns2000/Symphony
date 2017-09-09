using SlimDX;

namespace MMF.Bone
{
    public interface IBone
    {
        Vector3 Translation
        {
            get;
            set;
        }

        Quaternion Rotation
        {
            get;
            set;
        }

        Matrix GlobalPose
        {
            get;
        }

        void UpdateGrobalPose();
    }
}
