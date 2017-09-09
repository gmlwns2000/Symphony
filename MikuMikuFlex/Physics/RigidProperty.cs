namespace MMF.Physics
{
    internal class RigidProperty
    {
        public float mass
        {
            get;
            private set;
        }

        public float restitution
        {
            get;
            private set;
        }

        public float friction
        {
            get;
            private set;
        }

        public float linear_damp
        {
            get;
            private set;
        }

        public float angular_damp
        {
            get;
            private set;
        }

        public RigidProperty(float mass = 0f, float restitution = 0f, float friction = 0.5f, float linear_damp = 0f, float angular_damp = 0f)
        {
            this.mass = mass;
            this.restitution = restitution;
            this.friction = friction;
            this.linear_damp = linear_damp;
            this.angular_damp = angular_damp;
        }
    }
}
