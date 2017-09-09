using BulletSharp;
using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.JointParam;
using MMF.Bone;
using SlimDX;
using System.Diagnostics;

namespace MMF.Physics
{
    public class PMXPhysicsTransformManager : ITransformUpdater, System.IDisposable
    {
        private class TempRigidBodyData
        {
            public readonly Vector3 position;

            public readonly Matrix init_matrix;

            public readonly Matrix offset_matrix;

            public readonly int boneIndex;

            public readonly PhysicsCalcType physicsCalcType;

            public readonly RigidBodyShape shape;

            public TempRigidBodyData(RigidBodyData rigidBodyData)
            {
                position = rigidBodyData.Position;
                Vector3 rotation = rigidBodyData.Rotation;
                init_matrix = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(position);
                offset_matrix = Matrix.Invert(init_matrix);
                boneIndex = rigidBodyData.BoneIndex;
                physicsCalcType = rigidBodyData.PhysicsCalcType;
                shape = rigidBodyData.Shape;
            }
        }

        private PMXBone[] bones;

        private System.Collections.Generic.List<PMXPhysicsTransformManager.TempRigidBodyData> tempRigidBodyData_s = new System.Collections.Generic.List<PMXPhysicsTransformManager.TempRigidBodyData>();

        private BulletManager bulletManager;

        private System.Collections.Generic.List<RigidBody> rigidBodies = new System.Collections.Generic.List<RigidBody>();

        private static bool physicsAsserted;

        private void CreateRigid(System.Collections.Generic.List<RigidBodyData> rigidBodyData_s)
        {
            foreach (RigidBodyData current in rigidBodyData_s)
            {
                PMXPhysicsTransformManager.TempRigidBodyData tempRigidBodyData = new PMXPhysicsTransformManager.TempRigidBodyData(current);
                Matrix init_matrix = tempRigidBodyData.init_matrix;
                tempRigidBodyData_s.Add(tempRigidBodyData);
                CollisionShape collisionShape;
                switch (current.Shape)
                {
                    case RigidBodyShape.Sphere:
                        collisionShape = new SphereShape(current.Size.X);
                        break;
                    case RigidBodyShape.Box:
                        collisionShape = new BoxShape(current.Size.X, current.Size.Y, current.Size.Z);
                        break;
                    case RigidBodyShape.Capsule:
                        collisionShape = new CapsuleShape(current.Size.X, current.Size.Y);
                        break;
                    default:
                        throw new System.Exception("Invalid rigid body data");
                }
                RigidProperty rigidProperty = new RigidProperty(current.Mass, current.Repulsion, current.Friction, current.MoveAttenuation, current.RotationAttenuation);
                SuperProperty superProperty = new SuperProperty(current.PhysicsCalcType == PhysicsCalcType.Static, (CollisionFilterGroups)(1 << current.RigidBodyGroup), (CollisionFilterGroups)current.UnCollisionGroupFlag);
                RigidBody item = bulletManager.CreateRigidBody(collisionShape, init_matrix, rigidProperty, superProperty);
                rigidBodies.Add(item);
            }
        }

        private void CreateJoint(System.Collections.Generic.List<JointData> jointData_s)
        {
            foreach (JointData current in jointData_s)
            {
                Spring6DofJointParam jointParam = (Spring6DofJointParam)current.JointParam;
                Joint6ConnectedBodyPair connectedBodyPair = CreateConnectedBodyPair(jointParam);
                Joint6Restriction restriction = CreateRestriction(jointParam);
                Joint6Stiffness stiffness = CreateStiffness(jointParam);
                bulletManager.Add6DofSpringConstraint(connectedBodyPair, restriction, stiffness);
            }
        }

        private Joint6ConnectedBodyPair CreateConnectedBodyPair(Spring6DofJointParam jointParam)
        {
            RigidBody rigidBody = rigidBodies[jointParam.RigidBodyAIndex];
            Matrix right = Matrix.Invert(bulletManager.GetWorld(rigidBody));
            RigidBody rigidBody2 = rigidBodies[jointParam.RigidBodyBIndex];
            Matrix right2 = Matrix.Invert(bulletManager.GetWorld(rigidBody2));
            Vector3 rotation = jointParam.Rotation;
            Vector3 position = jointParam.Position;
            Matrix left = Matrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * Matrix.Translation(position.X, position.Y, position.Z);
            Joint6ConnectedBody connectedBodyA = new Joint6ConnectedBody(rigidBody, left * right);
            Joint6ConnectedBody connectedBodyB = new Joint6ConnectedBody(rigidBody2, left * right2);
            return new Joint6ConnectedBodyPair(connectedBodyA, connectedBodyB);
        }

        private Joint6Restriction CreateRestriction(Spring6DofJointParam jointParam)
        {
            Joint6MovementRestriction movementRestriction = new Joint6MovementRestriction(jointParam.MoveLimitationMin, jointParam.MoveLimitationMax);
            Joint6RotationRestriction rotationRestriction = new Joint6RotationRestriction(jointParam.RotationLimitationMin, jointParam.RotationLimitationMax);
            return new Joint6Restriction(movementRestriction, rotationRestriction);
        }

        private Joint6Stiffness CreateStiffness(Spring6DofJointParam jointParam)
        {
            return new Joint6Stiffness(jointParam.SpringMoveCoefficient, jointParam.SpringRotationCoefficient);
        }

        public PMXPhysicsTransformManager(PMXBone[] bones, System.Collections.Generic.List<RigidBodyData> rigidBodyData_s, System.Collections.Generic.List<JointData> jointData_s)
        {
            this.bones = bones;
            Vector3 gravity = new Vector3(0f, -24.5f, 0f);
            bulletManager = new BulletManager(gravity);
            CreateRigid(rigidBodyData_s);
            CreateJoint(jointData_s);
        }

        public void Dispose()
        {
            if (tempRigidBodyData_s != null)
            {
                tempRigidBodyData_s.Clear();
                tempRigidBodyData_s = null;
            }
            if (bulletManager != null)
            {
                bulletManager.Dispose();
                bulletManager = null;
            }
            if (rigidBodies != null)
            {
                rigidBodies.Clear();
                rigidBodies = null;
            }
        }

        public bool UpdateTransform()
        {
            int i;
            for (i = 0; i < rigidBodies.Count; i++)
            {
                PMXPhysicsTransformManager.TempRigidBodyData tempRigidBodyData = tempRigidBodyData_s[i];
                if (tempRigidBodyData.boneIndex != -1 && tempRigidBodyData.physicsCalcType == PhysicsCalcType.Static)
                {
                    PMXBone pMXBone = bones[tempRigidBodyData.boneIndex];
                    Matrix world = tempRigidBodyData.init_matrix * pMXBone.GlobalPose;
                    bulletManager.MoveRigidBody(rigidBodies[i], world);
                }
            }
            bulletManager.StepSimulation();
            i = 0;
            while (i < rigidBodies.Count)
            {
                PMXPhysicsTransformManager.TempRigidBodyData tempRigidBodyData = tempRigidBodyData_s[i];
                if (tempRigidBodyData.boneIndex != -1)
                {
                    PMXBone pMXBone = bones[tempRigidBodyData.boneIndex];
                    Matrix left = tempRigidBodyData.offset_matrix * bulletManager.GetWorld(rigidBodies[i]);
                    if (float.IsNaN(left.M11))
                    {
                        if (!PMXPhysicsTransformManager.physicsAsserted)
                        {
                            Debug.WriteLine("物理演算の結果が不正な結果を出力しました。\nPMXの設定を見直してください。うまくモーション動作しない可能性があります。");
                        }
                        PMXPhysicsTransformManager.physicsAsserted = true;
                    }
                    else
                    {
                        Matrix pm = new Matrix();
                        if(pMXBone.Parent != null)
                        {
                            pm = pMXBone.Parent.GlobalPose;
                        }

                        Matrix right = left * Matrix.Invert(pm);
                        Matrix matrix = Matrix.Translation(pMXBone.Position) * right * Matrix.Translation(-pMXBone.Position);
                        pMXBone.Translation = new Vector3(matrix.M41, matrix.M42, matrix.M43);
                        pMXBone.Rotation = Quaternion.RotationMatrix(matrix);
                        pMXBone.UpdateGrobalPose();
                    }
                }
                KOIMANSEI:
                i++;
                continue;
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
                goto KOIMANSEI;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
            }
            for (i = 0; i < rigidBodies.Count; i++)
            {
                PMXPhysicsTransformManager.TempRigidBodyData tempRigidBodyData = tempRigidBodyData_s[i];
                if (tempRigidBodyData.boneIndex != -1 && tempRigidBodyData.physicsCalcType == PhysicsCalcType.BoneAlignment)
                {
                    PMXBone pMXBone = bones[tempRigidBodyData.boneIndex];
                    Vector3 right2 = new Vector3(pMXBone.GlobalPose.M41, pMXBone.GlobalPose.M42, pMXBone.GlobalPose.M43);
                    Vector3 vector = new Vector3(tempRigidBodyData.init_matrix.M41, tempRigidBodyData.init_matrix.M42, tempRigidBodyData.init_matrix.M43) + right2;
                    Matrix world2 = bulletManager.GetWorld(rigidBodies[i]);
                    world2.M41 = vector.X;
                    world2.M42 = vector.Y;
                    world2.M43 = vector.Z;
                    bulletManager.MoveRigidBody(rigidBodies[i], world2);
                }
            }
            return false;
        }
    }
}
