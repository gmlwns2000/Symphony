using BulletSharp;
using SlimDX;
using System.Collections.Generic;

namespace MMF.Physics
{
    internal class RigidBodyFactory : System.IDisposable
    {
        List<CollisionShape> cache = new List<CollisionShape>();

        private DiscreteDynamicsWorld dynamicsWorld;

        public RigidBodyFactory(DiscreteDynamicsWorld dynamicsWorld)
        {
            this.dynamicsWorld = dynamicsWorld;
        }

        public RigidBody CreateRigidBody(CollisionShape collisionShape, Matrix world, RigidProperty rigidProperty, SuperProperty superProperty)
        {
            float num = superProperty.kinematic ? 0f : rigidProperty.mass;
            cache.Add(collisionShape);
            Vector3 localInertia = new Vector3(0f, 0f, 0f);
            if (num != 0f)
            {
                collisionShape.CalculateLocalInertia(num, out localInertia);
            }
            DefaultMotionState motionState = new DefaultMotionState(world);
            RigidBodyConstructionInfo info = new RigidBodyConstructionInfo(num, motionState, collisionShape, localInertia);
            RigidBody rigidBody = new RigidBody(info);
            rigidBody.Restitution = rigidProperty.restitution;
            rigidBody.Friction = rigidProperty.friction;
            rigidBody.SetDamping(rigidProperty.linear_damp, rigidProperty.angular_damp);
            float linearDamping = rigidBody.LinearDamping;
            float angularDamping = rigidBody.AngularDamping;
            if (superProperty.kinematic)
            {
                rigidBody.CollisionFlags |= CollisionFlags.KinematicObject;
            }
            rigidBody.ActivationState = ActivationState.DisableDeactivation;
            dynamicsWorld.AddRigidBody(rigidBody, superProperty.group, superProperty.mask);
            return rigidBody;
        }

        public void Dispose()
        {
            for (int i = dynamicsWorld.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject collisionObject = dynamicsWorld.CollisionObjectArray[i];
                RigidBody rigidBody = RigidBody.Upcast(collisionObject);
                if (rigidBody != null && rigidBody.MotionState != null)
                {
                    rigidBody.MotionState.Dispose();
                }
                dynamicsWorld.RemoveCollisionObject(collisionObject);
                collisionObject.Dispose();
            }
            for (int i = 0; i < cache.Count; i++)
            {
                CollisionShape collisionShape = cache[i];
                cache[i] = null;
                collisionShape.Dispose();
            }
            cache.Clear();
        }
    }
}
