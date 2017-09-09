using MMF.Bone;
using MMF.Morph;

namespace MMF.Motion
{
    public interface IMotionProvider
    {
        event System.EventHandler<ActionAfterMotion> MotionFinished;

        event System.EventHandler<System.EventArgs> FrameTicked;

        bool IsAttached
        {
            get;
        }

        float CurrentFrame
        {
            get;
            set;
        }

        int FinalFrame
        {
            get;
        }

        void AttachMotion(PMXBone[] bones);

        void Start(float frame, ActionAfterMotion action);

        void Stop();

        void Tick(int fps, float elapsedTime, IMorphManager morphManager);
    }
}
