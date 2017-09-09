using MMDFileParser.PMXModelParser;
using MMF.Physics;

namespace MMF.Bone
{
    internal class PMXSkeletonWithPhysics : PMXSkeleton
    {
        private readonly PMXPhysicsTransformManager physicsTransformManager;

        public PMXSkeletonWithPhysics(ModelData model) : base(model)
        {
            physicsTransformManager = new PMXPhysicsTransformManager(base.Bone, model.RigidBodyList.RigidBodies, model.JointList.Joints);
            KinematicsProviders.Add(physicsTransformManager);
        }

        public override void Dispose()
        {
            physicsTransformManager.Dispose();
        }
    }
}
