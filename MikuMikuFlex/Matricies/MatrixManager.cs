using MMF.Matricies.Camera;
using MMF.Matricies.Projection;
using MMF.Matricies.World;
using MMF.Model;
using SlimDX;

namespace MMF.Matricies
{
    public class MatrixManager
    {
        private readonly IProjectionMatrixProvider projectionMatrixManager;

        private readonly CameraProvider viewMatrixManager;

        private readonly IWorldMatrixProvider worldMatrixManager;

        public IWorldMatrixProvider WorldMatrixManager
        {
            get
            {
                return worldMatrixManager;
            }
        }

        public CameraProvider ViewMatrixManager
        {
            get
            {
                return viewMatrixManager;
            }
        }

        public IProjectionMatrixProvider ProjectionMatrixManager
        {
            get
            {
                return projectionMatrixManager;
            }
        }

        public MatrixManager(IWorldMatrixProvider world, CameraProvider cam, IProjectionMatrixProvider projection)
        {
            worldMatrixManager = world;
            viewMatrixManager = cam;
            projectionMatrixManager = projection;
        }

        public Matrix makeWorldViewProjectionMatrix(Vector3 localScaling, Quaternion localRotation, Vector3 localTranslation)
        {
            return worldMatrixManager.getWorldMatrix(localScaling, localRotation, localTranslation) * viewMatrixManager.ViewMatrix * projectionMatrixManager.ProjectionMatrix;
        }

        public Matrix makeWorldViewProjectionMatrix(IDrawable drawable)
        {
            return worldMatrixManager.getWorldMatrix(drawable) * viewMatrixManager.ViewMatrix * projectionMatrixManager.ProjectionMatrix;
        }
    }
}
