using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Matricies.Camera
{
    public abstract class CameraProvider
    {
        public abstract event System.EventHandler<CameraMatrixChangedEventArgs> CameraMatrixChanged;

        public abstract Vector3 CameraPosition
        {
            get;
            set;
        }

        public abstract Vector3 CameraLookAt
        {
            get;
            set;
        }

        public abstract Vector3 CameraUpVec
        {
            get;
            set;
        }

        public abstract Matrix ViewMatrix
        {
            get;
        }

        public CameraProvider(Vector3 cameraPos, Vector3 lookAtPos, Vector3 upVec)
        {
            CameraPosition = cameraPos;
            CameraLookAt = lookAtPos;
            CameraUpVec = upVec;
        }

        public virtual void SubscribeToEffect(Effect effect)
        {
            effect.GetVariableBySemantic("CAMERAPOSITION").AsVector().Set(CameraPosition);
        }
    }
}
