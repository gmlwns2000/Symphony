using BulletSharp;
using SlimDX;

namespace MMF.Physics
{
    internal class Joint6ConnectedBody
    {
        public RigidBody rigidBody
        {
            get;
            private set;
        }

        public Matrix world
        {
            get;
            private set;
        }

        public Joint6ConnectedBody(RigidBody rigidBody, Matrix world)
        {
            this.rigidBody = rigidBody;
            this.world = world;
        }
    }
}
