namespace MMF.Bone
{
    internal class BoneComparer : System.Collections.Generic.IComparer<PMXBone>
    {
        public int BoneCount
        {
            get;
            private set;
        }

        public BoneComparer(int boneCount)
        {
            BoneCount = boneCount;
        }

        public int Compare(PMXBone x, PMXBone y)
        {
            int num = 0;
            int num2 = 0;
            if (x.PhysicsOrder == PhysicsOrder.After)
            {
                num += BoneCount * BoneCount;
            }
            if (y.PhysicsOrder == PhysicsOrder.After)
            {
                num2 += BoneCount * BoneCount;
            }
            num += BoneCount * x.Layer;
            num2 += BoneCount * y.Layer;
            num += x.BoneIndex;
            num2 += y.BoneIndex;
            return num - num2;
        }
    }
}
