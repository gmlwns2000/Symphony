using BulletSharp;
using SlimDX;

namespace MMF.Physics
{
    internal class DynamicsWorldFactory : System.IDisposable
    {
        private readonly DefaultCollisionConfiguration collisionConfiguration;

        private readonly CollisionDispatcher dispatcher;

        private readonly BroadphaseInterface overlappingPairCache;

        private readonly SequentialImpulseConstraintSolver solver;

        public DynamicsWorldFactory()
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            overlappingPairCache = new DbvtBroadphase();
            solver = new SequentialImpulseConstraintSolver();
        }

        public DiscreteDynamicsWorld CreateDynamicsWorld(Vector3 gravity)
        {
            return new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, solver, collisionConfiguration)
            {
                Gravity = gravity
            };
        }

        public void Dispose()
        {
            solver.Dispose();
            overlappingPairCache.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
        }
    }
}
