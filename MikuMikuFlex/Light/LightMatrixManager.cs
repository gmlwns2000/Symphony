using MMF.Matricies;
using MMF.Matricies.Camera;
using MMF.Matricies.Projection;
using SlimDX;

namespace MMF.Light
{
    public class LightMatrixManager
    {
        private MatrixManager manager;

        private Vector3 direction;

        public CameraProvider Camera
        {
            get;
            private set;
        }

        public BasicProjectionMatrixProvider Projection
        {
            get;
            private set;
        }

        public Vector3 Position
        {
            get
            {
                return Camera.CameraPosition;
            }
            set
            {
                Camera.CameraPosition = value;
                UpdateDirection();
            }
        }

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            private set
            {
                direction = value;
            }
        }

        public LightMatrixManager(MatrixManager manager)
        {
            this.manager = manager;
            Camera = new BasicCamera(new Vector3(0f, 0f, -20f), new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
            Projection = new BasicProjectionMatrixProvider();
        }

        private void UpdateDirection()
        {
            direction = Vector3.Normalize(-Camera.CameraPosition);
        }
    }
}
