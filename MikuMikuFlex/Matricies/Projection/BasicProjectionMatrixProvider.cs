using SlimDX;

namespace MMF.Matricies.Projection
{
    public class BasicProjectionMatrixProvider : IProjectionMatrixProvider
    {
        private float aspectRatio = 1.618f;

        private float fovy;

        private Matrix projectionMatrix = Matrix.Identity;

        private float zFar;

        private float zNear;

        public event System.EventHandler<ProjectionMatrixChangedEventArgs> ProjectionMatrixChanged;

        public Matrix ProjectionMatrix
        {
            get
            {
                return projectionMatrix;
            }
        }

        public float Fovy
        {
            get
            {
                return fovy;
            }
            set
            {
                fovy = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.Fovy);
            }
        }

        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }
            set
            {
                aspectRatio = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.AspectRatio);
            }
        }

        public float ZNear
        {
            get
            {
                return zNear;
            }
            set
            {
                zNear = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.ZNear);
            }
        }

        public float ZFar
        {
            get
            {
                return zFar;
            }
            set
            {
                zFar = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.ZFar);
            }
        }

        public void InitializeProjection(float fovyAngle, float aspect, float znear, float zfar)
        {
            fovy = fovyAngle;
            aspectRatio = aspect;
            zNear = znear;
            zFar = zfar;
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            projectionMatrix = Matrix.PerspectiveFovLH(fovy, aspectRatio, zNear, zFar);
        }

        private void NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType type)
        {
            if (ProjectionMatrixChanged != null)
            {
                ProjectionMatrixChanged(this, new ProjectionMatrixChangedEventArgs(type));
            }
        }
    }
}
