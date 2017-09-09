using SlimDX;

namespace MMF.Physics
{
    internal class Joint6RotationRestriction
    {
        public Vector3 c_r1
        {
            get;
            private set;
        }

        public Vector3 c_r2
        {
            get;
            private set;
        }

        public Joint6RotationRestriction(Vector3 c_r1, Vector3 c_r2)
        {
            this.c_r1 = c_r1;
            this.c_r2 = c_r2;
        }
    }
}
