using MMF.Bone;

namespace MMF.Motion
{
    public interface IMovable
    {
        ISkinningProvider Skinning
        {
            get;
        }

        IMotionManager MotionManager
        {
            get;
        }

        void ApplyMove();
    }
}
