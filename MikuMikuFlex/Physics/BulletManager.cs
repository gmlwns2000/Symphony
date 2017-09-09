using BulletSharp;
using SlimDX;

namespace MMF.Physics
{
    internal class BulletManager : System.IDisposable
    {
        private DiscreteDynamicsWorld dynamicsWorld;

        private DynamicsWorldFactory dynamicsWorldFactory = new DynamicsWorldFactory();

        private RigidBodyFactory rigidBodyFactory;

        private ConstraintFactory constraintFactory;

        private BulletTimer bulletTimer = new BulletTimer();

        private bool isDisposed = false;

        public BulletManager(Vector3 gravity)
        {
            dynamicsWorld = dynamicsWorldFactory.CreateDynamicsWorld(gravity);
            rigidBodyFactory = new RigidBodyFactory(dynamicsWorld);
            constraintFactory = new ConstraintFactory(dynamicsWorld);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                constraintFactory.Dispose();
                rigidBodyFactory.Dispose();
                dynamicsWorld.Dispose();
                dynamicsWorldFactory.Dispose();
                isDisposed = true;
            }
        }

        public RigidBody CreateRigidBody(CollisionShape collisionShape, Matrix world, RigidProperty rigidProperty, SuperProperty superProperty)
        {
            return rigidBodyFactory.CreateRigidBody(collisionShape, world, rigidProperty, superProperty);
        }

        public void AddPointToPointConstraint(RigidBody body, ref Vector3 pivot)
        {
            constraintFactory.AddPointToPointConstraint(body, ref pivot);
        }

        public void AddPointToPointConstraint(RigidBody bodyA, RigidBody bodyB, ref Vector3 pivotInA, ref Vector3 pivotInB)
        {
            constraintFactory.AddPointToPointConstraint(bodyA, bodyB, ref pivotInA, ref pivotInB);
        }

        public void Add6DofSpringConstraint(Joint6ConnectedBodyPair connectedBodyPair, Joint6Restriction restriction, Joint6Stiffness stiffness)
        {
            constraintFactory.Add6DofSpringConstraint(connectedBodyPair, restriction, stiffness);
        }

        public void MoveRigidBody(RigidBody body, Matrix world)
        {
            body.MotionState.WorldTransform = world;
        }

        public void StepSimulation()
        {
            long elapsedTime = bulletTimer.GetElapsedTime();
            dynamicsWorld.StepSimulation(elapsedTime / 1000f, 10);
        }

        public Matrix GetWorld(RigidBody body)
        {
            return body.MotionState.WorldTransform;
        }
    }
}
