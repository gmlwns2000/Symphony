using SlimDX;

namespace MMF.Physics
{
    internal class Joint6MovementRestriction
    {
        public Vector3 c_p1
        {
            get;
            private set;
        }

        public Vector3 c_p2
        {
            get;
            private set;
        }

        public Joint6MovementRestriction(Vector3 c_p1, Vector3 c_p2)
        {
            this.c_p1 = c_p1;
            this.c_p2 = c_p2;
        }
    }
}
