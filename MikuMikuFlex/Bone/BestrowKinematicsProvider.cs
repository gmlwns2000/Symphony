using MMF.Utility;
using SlimDX;

namespace MMF.Bone
{
    internal class BestrowKinematicsProvider : ITransformUpdater
    {
        private class BestrowKinematicsOrderSolver : HierarchicalOrderSolver<PMXBone>
        {
            public int getParentIndex(PMXBone child)
            {
                int result;
                if (!child.isMoveProvided && child.isRotateProvided)
                {
                    result = -1;
                }
                else
                {
                    result = child.ProvideParentBone;
                }
                return result;
            }

            public int getIndex(PMXBone target)
            {
                return target.BoneIndex;
            }
        }

        private HierarchicalOrderCollection<PMXBone> bones;

        public BestrowKinematicsProvider(PMXBone[] bones)
        {
            this.bones = new HierarchicalOrderCollection<PMXBone>(bones, new BestrowKinematicsProvider.BestrowKinematicsOrderSolver());
        }

        public bool UpdateTransform()
        {
            foreach (PMXBone current in bones)
            {
                if (current.isMoveProvided)
                {
                    PMXBone pMXBone = bones[current.ProvideParentBone];
                    current.Translation += Vector3.Lerp(Vector3.Zero, pMXBone.Translation, current.ProvidedRatio);
                }
                if (current.isRotateProvided)
                {
                    PMXBone pMXBone = bones[current.ProvideParentBone];
                    current.Rotation *= Quaternion.Slerp(Quaternion.Identity, pMXBone.Rotation, current.ProvidedRatio);
                }
            }
            return true;
        }
    }
}
