using MMF.Model;
using SlimDX;

namespace MMF.Matricies.World
{
    public interface IWorldMatrixProvider
    {
        event System.EventHandler<WorldMatrixChangedEventArgs> WorldMatrixChanged;

        Vector3 Scaling
        {
            get;
            set;
        }

        Quaternion Rotation
        {
            get;
            set;
        }

        Vector3 Translation
        {
            get;
            set;
        }

        Matrix getWorldMatrix(Vector3 scalingLocal, Quaternion rotationLocal, Vector3 translationLocal);

        Matrix getWorldMatrix(IDrawable drawable);

        Matrix getWorldMatrix(Matrix localMatrix);
    }
}
