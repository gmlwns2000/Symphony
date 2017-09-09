using MMDFileParser.PMXModelParser;
using SlimDX;

namespace MMF.Bone
{
    public class PMXBone : IBone
    {
        private readonly ISkinningProvider skinning;

        public int BoneIndex;

        public string BoneName;

        public System.Collections.Generic.List<PMXBone> Children = new System.Collections.Generic.List<PMXBone>();

        public Vector3 DefaultLocalX;

        public Vector3 DefaultLocalY;

        public Vector3 DefaultLocalZ;

        public PMXBone Parent;

        public Vector3 Position;

        public bool isLocalAxis;

        private readonly int targetBoneIndex;

        public int Iterator;

        public float RotationLimited;

        public System.Collections.Generic.List<IkLink> ikLinks = new System.Collections.Generic.List<IkLink>();

        public bool isIK = false;

        private Quaternion rotation;

        public Vector3 Translation
        {
            get;
            set;
        }

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                rotation.Normalize();
            }
        }

        public int Layer
        {
            get;
            private set;
        }

        public PhysicsOrder PhysicsOrder
        {
            get;
            private set;
        }

        public Vector3 LocalX
        {
            get
            {
                return Vector3.TransformNormal(DefaultLocalX, Matrix.RotationQuaternion(Rotation));
            }
        }

        public Vector3 LocalY
        {
            get
            {
                return Vector3.TransformNormal(DefaultLocalY, Matrix.RotationQuaternion(Rotation));
            }
        }

        public Vector3 LocalZ
        {
            get
            {
                return Vector3.TransformNormal(DefaultLocalZ, Matrix.RotationQuaternion(Rotation));
            }
        }

        public Matrix LocalPose
        {
            get;
            set;
        }

        public Matrix GlobalPose
        {
            get;
            private set;
        }

        public PMXBone IkTargetBone
        {
            get
            {
                return skinning.Bone[targetBoneIndex];
            }
        }

        public bool isMoveProvided
        {
            get;
            private set;
        }

        public bool isRotateProvided
        {
            get;
            private set;
        }

        public int ProvideParentBone
        {
            get;
            private set;
        }

        public float ProvidedRatio
        {
            get;
            private set;
        }

        public PMXBone(System.Collections.Generic.List<BoneData> bones, int index, int layer, ISkinningProvider skinning)
        {
            this.skinning = skinning;
            BoneData boneData = bones[index];
            skinning.Bone[index] = this;
            BoneIndex = index;
            Position = boneData.Position;
            BoneName = boneData.BoneName;
            isLocalAxis = boneData.isLocalAxis;
            PhysicsOrder = (boneData.transformAfterPhysics ? PhysicsOrder.After : PhysicsOrder.Before);
            Layer = layer;
            if (isLocalAxis)
            {
                DefaultLocalX = boneData.DimentionXDirectionVector;
                DefaultLocalY = Vector3.Cross(boneData.DimentionZDirectionVector, DefaultLocalX);
                DefaultLocalZ = Vector3.Cross(DefaultLocalX, DefaultLocalY);
            }
            if (boneData.isIK)
            {
                skinning.IkBone.Add(this);
                isIK = true;
                RotationLimited = boneData.IKLimitedRadian;
                targetBoneIndex = boneData.IKTargetBoneIndex;
                Iterator = boneData.IKLoopNumber;
                foreach (IkLinkData current in boneData.ikLinks)
                {
                    ikLinks.Add(new IkLink(skinning, current));
                }
            }
            isRotateProvided = boneData.isRotateProvided;
            isMoveProvided = boneData.isMoveProvided;
            if (boneData.ProvidedParentBoneIndex == -1)
            {
                isRotateProvided = (isMoveProvided = false);
            }
            if (isMoveProvided || isRotateProvided)
            {
                ProvideParentBone = boneData.ProvidedParentBoneIndex;
                ProvidedRatio = boneData.ProvidedRatio;
            }
            else
            {
                ProvideParentBone = -1;
            }
            for (int i = 0; i < bones.Count; i++)
            {
                BoneData boneData2 = bones[i];
                if (boneData2.ParentBoneIndex == index)
                {
                    PMXBone child = new PMXBone(bones, i, layer + 1, skinning);
                    AddChild(child);
                }
            }
        }

        public void AddChild(PMXBone child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        public void UpdateGrobalPose()
        {
            LocalPose = Matrix.Translation(-Position) * Matrix.RotationQuaternion(Rotation) * Matrix.Translation(Translation) * Matrix.Translation(Position);
            GlobalPose = LocalPose;
            if (Parent != null)
            {
                GlobalPose *= Parent.GlobalPose;
            }
            foreach (PMXBone current in Children)
            {
                current.UpdateGrobalPose();
            }
        }
    }
}
