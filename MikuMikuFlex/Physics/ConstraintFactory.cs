using BulletSharp;
using SlimDX;

namespace MMF.Physics
{
    internal class ConstraintFactory : System.IDisposable
    {
        private DiscreteDynamicsWorld dynamicsWorld;

        public ConstraintFactory(DiscreteDynamicsWorld dynamicsWorld)
        {
            this.dynamicsWorld = dynamicsWorld;
        }

        public void AddPointToPointConstraint(RigidBody body, ref Vector3 pivot)
        {
            dynamicsWorld.AddConstraint(new Point2PointConstraint(body, pivot));
        }

        public void AddPointToPointConstraint(RigidBody bodyA, RigidBody bodyB, ref Vector3 pivotInA, ref Vector3 pivotInB)
        {
            dynamicsWorld.AddConstraint(new Point2PointConstraint(bodyA, bodyB, pivotInA, pivotInB));
        }

        public void Add6DofSpringConstraint(Joint6ConnectedBodyPair connectedBodyPair, Joint6Restriction restriction, Joint6Stiffness stiffness)
        {
            RigidBody rigidBody = connectedBodyPair.connectedBodyA.rigidBody;
            RigidBody rigidBody2 = connectedBodyPair.connectedBodyB.rigidBody;
            Matrix world = connectedBodyPair.connectedBodyA.world;
            Matrix world2 = connectedBodyPair.connectedBodyB.world;
            Generic6DofSpringConstraint generic6DofSpringConstraint = new Generic6DofSpringConstraint(rigidBody, rigidBody2, world, world2, true);
            Vector3 c_p = restriction.movementRestriction.c_p1;
            Vector3 c_p2 = restriction.movementRestriction.c_p2;
            Vector3 c_r = restriction.rotationRestriction.c_r1;
            Vector3 c_r2 = restriction.rotationRestriction.c_r2;
            generic6DofSpringConstraint.LinearLowerLimit = new Vector3(c_p.X, c_p.Y, c_p.Z);
            generic6DofSpringConstraint.LinearUpperLimit = new Vector3(c_p2.X, c_p2.Y, c_p2.Z);
            generic6DofSpringConstraint.AngularLowerLimit = new Vector3(c_r.X, c_r.Y, c_r.Z);
            generic6DofSpringConstraint.AngularUpperLimit = new Vector3(c_r2.X, c_r2.Y, c_r2.Z);
            SetStiffness(stiffness.translation.X, 0, generic6DofSpringConstraint);
            SetStiffness(stiffness.translation.Y, 1, generic6DofSpringConstraint);
            SetStiffness(stiffness.translation.Z, 2, generic6DofSpringConstraint);
            SetStiffness(stiffness.rotation.X, 3, generic6DofSpringConstraint);
            SetStiffness(stiffness.rotation.Y, 4, generic6DofSpringConstraint);
            SetStiffness(stiffness.rotation.Z, 5, generic6DofSpringConstraint);
            dynamicsWorld.AddConstraint(generic6DofSpringConstraint);
        }

        private void SetStiffness(float stiffness, int index, Generic6DofSpringConstraint constraint)
        {
            if (stiffness != 0f)
            {
                constraint.EnableSpring(index, true);
                constraint.SetStiffness(index, stiffness);
            }
        }

        public void Dispose()
        {
            for (int i = dynamicsWorld.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = dynamicsWorld.GetConstraint(i);
                dynamicsWorld.RemoveConstraint(constraint);
                constraint.Dispose();
            }
        }
    }
}
