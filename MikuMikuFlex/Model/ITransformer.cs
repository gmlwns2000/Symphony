using SlimDX;

namespace MMF.Model
{
    public interface ITransformer
    {
        Vector3 Position
        {
            get;
            set;
        }

        Vector3 Foward
        {
            get;
            set;
        }

        Vector3 Top
        {
            get;
            set;
        }

        Quaternion Rotation
        {
            get;
            set;
        }

        Vector3 Scale
        {
            get;
            set;
        }

        Vector3 InitialTop
        {
            get;
        }

        Vector3 InitialFoward
        {
            get;
        }

        Matrix LocalTransform
        {
            get;
        }

        void Reset();
    }
}
