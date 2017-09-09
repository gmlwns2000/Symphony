using MMF.Matricies.Projection;

namespace MMF.Matricies.Camera
{
    public interface ICameraMotionProvider
    {
        void UpdateCamera(CameraProvider cp, IProjectionMatrixProvider proj);
    }
}
