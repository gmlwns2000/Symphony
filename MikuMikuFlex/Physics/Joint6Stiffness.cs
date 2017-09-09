using SlimDX;

namespace MMF.Physics
{
    internal class Joint6Stiffness
    {
        public Vector3 translation
        {
            get;
            private set;
        }

        public Vector3 rotation
        {
            get;
            private set;
        }

        public Joint6Stiffness(Vector3 translation, Vector3 rotation)
        {
            this.translation = translation;
            this.rotation = rotation;
        }
    }
}
