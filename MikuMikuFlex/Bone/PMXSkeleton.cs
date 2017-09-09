using MMDFileParser.PMXModelParser;
using MMF.Morph;
using SlimDX;
using SlimDX.Direct3D11;

namespace MMF.Bone
{
    public class PMXSkeleton : ISkinningProvider, System.IDisposable
    {
        public Matrix[] GlobalBonePose;

        public ITransformUpdater IkProvider;

        public System.Collections.Generic.List<PMXBone> RootBone = new System.Collections.Generic.List<PMXBone>();

        public event System.EventHandler SkeletonUpdated = delegate (object param0, System.EventArgs param1)
        {
        };

        public System.Collections.Generic.List<ITransformUpdater> KinematicsProviders
        {
            get;
            private set;
        }

        public PMXBone[] Bone
        {
            get;
            set;
        }

        public System.Collections.Generic.Dictionary<string, PMXBone> BoneDictionary
        {
            get;
            private set;
        }

        public System.Collections.Generic.List<PMXBone> IkBone
        {
            get;
            set;
        }

        public PMXSkeleton(ModelData model)
        {
            GlobalBonePose = new Matrix[model.BoneList.BoneCount];
            Bone = new PMXBone[model.BoneList.BoneCount];
            IkBone = new System.Collections.Generic.List<PMXBone>();
            LoadBones(model);
            BoneDictionary = new System.Collections.Generic.Dictionary<string, PMXBone>();
            PMXBone[] bone = Bone;
            for (int i = 0; i < bone.Length; i++)
            {
                PMXBone pMXBone = bone[i];
                if (BoneDictionary.ContainsKey(pMXBone.BoneName))
                {
                    int num = 0;
                    do
                    {
                        num++;
                    }
                    while (BoneDictionary.ContainsKey(pMXBone.BoneName + num.ToString()));
                    BoneDictionary.Add(pMXBone.BoneName + num.ToString(), pMXBone);
                    System.Diagnostics.Debug.WriteLine("ボーン名{0}は重複しています。自動的にボーン名{1}と読み替えられました。", new object[]
                    {
                        pMXBone.BoneName,
                        pMXBone.BoneName + num
                    });
                }
                else
                {
                    BoneDictionary.Add(pMXBone.BoneName, pMXBone);
                }
            }
            KinematicsProviders = new System.Collections.Generic.List<ITransformUpdater>();
            IkProvider = new CCDIK(IkBone);
            KinematicsProviders.Add(IkProvider);
            KinematicsProviders.Add(new BestrowKinematicsProvider(Bone));
            if (Bone.Length > 512)
            {
                throw new System.InvalidOperationException("MMFでは現在512以上のボーンを持つモデルについてサポートしていません。\nただし、Resource\\Shader\\DefaultShader.fx内のボーン変形行列の配列float4x4 BoneTrans[512]:BONETRANS;の要素数を拡張しこの部分をコメントアウトすれば暫定的に利用することができるかもしれません。");
            }
        }

        public void ApplyEffect(Effect effect)
        {
            effect.GetVariableBySemantic("BONETRANS").AsMatrix().SetMatrixArray(GlobalBonePose);
        }

        public virtual void UpdateSkinning(IMorphManager morphManager)
        {
            ResetAllBoneTransform();
            UpdateGlobal();
            foreach (ITransformUpdater current in KinematicsProviders)
            {
                if (current.UpdateTransform())
                {
                    UpdateGlobal();
                }
            }
            foreach (PMXBone current2 in RootBone)
            {
                current2.UpdateGrobalPose();
            }
            SkeletonUpdated(this, new System.EventArgs());
            for (int i = 0; i < Bone.Length; i++)
            {
                GlobalBonePose[Bone[i].BoneIndex] = Bone[i].GlobalPose;
            }
        }

        public void ResetAllBoneTransform()
        {
            PMXBone[] bone = Bone;
            for (int i = 0; i < bone.Length; i++)
            {
                PMXBone pMXBone = bone[i];
                pMXBone.Rotation = Quaternion.Identity;
                pMXBone.Translation = Vector3.Zero;
            }
        }

        private void LoadBones(ModelData model)
        {
            for (int i = 0; i < model.BoneList.BoneCount; i++)
            {
                if (model.BoneList.Bones[i].ParentBoneIndex == -1)
                {
                    RootBone.Add(new PMXBone(model.BoneList.Bones, i, 0, this));
                }
            }
            BoneComparer comparer = new BoneComparer(model.BoneList.Bones.Count);
            IkBone.Sort(comparer);
            RootBone.Sort(comparer);
        }

        protected void UpdateGlobal()
        {
            foreach (PMXBone current in RootBone)
            {
                current.UpdateGrobalPose();
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
