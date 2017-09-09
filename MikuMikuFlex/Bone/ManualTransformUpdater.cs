using MMF.Model.PMX;
using MMF.Morph;
using SlimDX;

namespace MMF.Bone
{
    public class ManualTransformUpdater : ITransformUpdater
    {
        private System.Collections.Generic.Dictionary<string, BoneTransformer> updaters = new System.Collections.Generic.Dictionary<string, BoneTransformer>();

        private System.Collections.Generic.Dictionary<string, MorphTransformer> morphUpdaters = new System.Collections.Generic.Dictionary<string, MorphTransformer>();

        private PMXModel model;

        public ManualTransformUpdater(PMXModel model)
        {
            this.model = model;
        }

        public bool UpdateTransform()
        {
            System.Collections.Generic.Dictionary<string, PMXBone> boneDictionary = model.Skinning.BoneDictionary;
            IMorphManager morphmanager = model.Morphmanager;
            foreach (System.Collections.Generic.KeyValuePair<string, BoneTransformer> current in updaters)
            {
                PMXBone pMXBone = boneDictionary[current.Key];
                pMXBone.Rotation *= current.Value.Rotation;
                pMXBone.Translation += current.Value.Translation;
            }
            foreach (System.Collections.Generic.KeyValuePair<string, MorphTransformer> current2 in morphUpdaters)
            {
                morphmanager.ApplyMorphProgress(current2.Value.MorphValue, current2.Key);
            }
            return true;
        }

        public BoneTransformer getBoneTransformer(string boneName)
        {
            if (!model.Skinning.BoneDictionary.ContainsKey(boneName))
            {
                throw new System.InvalidOperationException("そのような名前のボーンは存在しません。");
            }
            BoneTransformer result;
            if (updaters.ContainsKey(boneName))
            {
                result = updaters[boneName];
            }
            else
            {
                BoneTransformer boneTransformer = new BoneTransformer(boneName, Quaternion.Identity, Vector3.Zero);
                updaters.Add(boneTransformer.BoneName, boneTransformer);
                result = boneTransformer;
            }
            return result;
        }

        public MorphTransformer getMorphTransformer(string morphName)
        {
            MorphTransformer result;
            if (morphUpdaters.ContainsKey(morphName))
            {
                result = morphUpdaters[morphName];
            }
            else
            {
                MorphTransformer morphTransformer = new MorphTransformer(morphName);
                morphUpdaters.Add(morphTransformer.MorphName, morphTransformer);
                result = morphTransformer;
            }
            return result;
        }
    }
}
