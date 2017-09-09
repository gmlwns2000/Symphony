using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Matricies.Camera
{
    public class BasicCamera : CameraProvider
    {
        private Vector3 cameraLookAt = Vector3.Zero;

        private Vector3 cameraPosition = Vector3.Zero;

        private Vector3 cameraUpVec = Vector3.Zero;

        private Matrix viewMatrix = Matrix.Identity;

        public override event System.EventHandler<CameraMatrixChangedEventArgs> CameraMatrixChanged;

        public override Matrix ViewMatrix
        {
            get
            {
                return viewMatrix;
            }
        }

        public override Vector3 CameraPosition
        {
            get
            {
                return cameraPosition;
            }
            set
            {
                cameraPosition = value;
                UpdateCamera();
                NotifyCameraMatrixChanged(CameraMatrixChangedVariableType.Position);
            }
        }

        public override Vector3 CameraLookAt
        {
            get
            {
                return cameraLookAt;
            }
            set
            {
                cameraLookAt = value;
                UpdateCamera();
                NotifyCameraMatrixChanged(CameraMatrixChangedVariableType.LookAt);
            }
        }

        public override Vector3 CameraUpVec
        {
            get
            {
                return cameraUpVec;
            }
            set
            {
                cameraUpVec = value;
                UpdateCamera();
                NotifyCameraMatrixChanged(CameraMatrixChangedVariableType.Up);
            }
        }

        public BasicCamera(Vector3 cameraPos, Vector3 lookAtPos, Vector3 upVec) : base(cameraPos, lookAtPos, upVec)
        {
        }

        private void UpdateCamera()
        {
            viewMatrix = Matrix.LookAtLH(cameraPosition, cameraLookAt, cameraUpVec);
        }

        public override void SubscribeToEffect(Effect effect)
        {
            effect.GetVariableBySemantic("CAMERAPOSITION").AsVector().Set(cameraPosition);
        }

        private void NotifyCameraMatrixChanged(CameraMatrixChangedVariableType type)
        {
            if (CameraMatrixChanged != null)
            {
                CameraMatrixChanged(this, new CameraMatrixChangedEventArgs(type));
            }
        }
    }
}
