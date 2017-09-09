using SlimDX;

namespace MMF.Matricies.World
{
    public interface IBasicWorldMatrixProvider
    {
        Matrix getWorldMatrix(Matrix localMatrix);
    }
}
