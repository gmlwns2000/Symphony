using SlimDX;

namespace MMF.Matricies.Projection
{
    public interface IProjectionMatrixProvider
    {
        event System.EventHandler<ProjectionMatrixChangedEventArgs> ProjectionMatrixChanged;

        Matrix ProjectionMatrix
        {
            get;
        }

        float Fovy
        {
            get;
            set;
        }

        float AspectRatio
        {
            get;
            set;
        }

        float ZNear
        {
            get;
            set;
        }

        float ZFar
        {
            get;
            set;
        }

        void InitializeProjection(float fovyAngle, float aspect, float znear, float zfar);
    }
}
