using MMF.Model;
using SlimDX;

namespace MMF.Matricies.World
{
    public class BasicWorldMatrixProvider : IWorldMatrixProvider
    {
        private Quaternion rotation;

        private Vector3 scaling;

        private Vector3 translation;

        public event System.EventHandler<WorldMatrixChangedEventArgs> WorldMatrixChanged;

        public Vector3 Scaling
        {
            get
            {
                return scaling;
            }
            set
            {
                scaling = value;
                NotifyWorldMatrixChanged(new WorldMatrixChangedEventArgs(ChangedWorldMatrixValueType.Scaling));
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                NotifyWorldMatrixChanged(new WorldMatrixChangedEventArgs(ChangedWorldMatrixValueType.Rotation));
            }
        }

        public Vector3 Translation
        {
            get
            {
                return translation;
            }
            set
            {
                translation = value;
                NotifyWorldMatrixChanged(new WorldMatrixChangedEventArgs(ChangedWorldMatrixValueType.Translation));
            }
        }

        public BasicWorldMatrixProvider()
        {
            scaling = new Vector3(1f, 1f, 1f);
            rotation = Quaternion.Identity;
            translation = Vector3.Zero;
        }

        public Matrix getWorldMatrix(Vector3 scalingLocal, Quaternion rotationLocal, Vector3 translationLocal)
        {
            Vector3 scale = new Vector3(Scaling.X * scalingLocal.X, Scaling.Y * scalingLocal.Y, Scaling.Z * scalingLocal.Z);
            return Matrix.Scaling(scale) * Matrix.RotationQuaternion(rotationLocal * Rotation) * Matrix.Translation(translationLocal + Translation);
        }

        public Matrix getWorldMatrix(Matrix localMatrix)
        {
            return Matrix.Scaling(scaling) * Matrix.RotationQuaternion(rotation) * Matrix.Translation(translation) * localMatrix;
        }

        public Matrix getWorldMatrix(IDrawable drawable)
        {
            return getWorldMatrix(drawable.Transformer.LocalTransform);
        }

        private void NotifyWorldMatrixChanged(WorldMatrixChangedEventArgs arg)
        {
            if (WorldMatrixChanged != null)
            {
                WorldMatrixChanged(this, arg);
            }
        }
    }
}
