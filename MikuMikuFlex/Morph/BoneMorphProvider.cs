using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;
using MMF.Bone;
using MMF.Model.PMX;
using MMF.Motion;
using SlimDX;

namespace MMF.Morph
{
    internal class BoneMorphProvider : IMorphProvider
    {
        public System.Collections.Generic.Dictionary<string, BoneMorphData> MorphList = new System.Collections.Generic.Dictionary<string, BoneMorphData>();

        private ISkinningProvider skinningProvider;

        public BoneMorphProvider(PMXModel model)
        {
            skinningProvider = model.Skinning;
            foreach (MorphData current in model.Model.MorphList.Morphes)
            {
                if (current.type == MorphType.Bone)
                {
                    MorphList.Add(current.MorphName, new BoneMorphData(current));
                }
            }
        }

        public void ApplyMorphProgress(float frameNumber, System.Collections.Generic.IEnumerable<MorphMotion> morphMotions)
        {
            foreach (MorphMotion current in morphMotions)
            {
                SetMorphProgress(current.GetMorphValue(frameNumber), current.MorphName);
            }
        }

        public bool ApplyMorphProgress(float progress, string morphName)
        {
            return SetMorphProgress(progress, morphName);
        }

        public void UpdateFrame()
        {
        }

        private bool SetMorphProgress(float progress, string morphName)
        {
            bool result;
            if (!MorphList.ContainsKey(morphName))
            {
                result = false;
            }
            else
            {
                BoneMorphData boneMorphData = MorphList[morphName];
                foreach (BoneMorphOffset current in boneMorphData.BoneMorphs)
                {
                    Quaternion right = new Quaternion(current.QuantityOfRotating.X, current.QuantityOfRotating.Y, current.QuantityOfRotating.Z, current.QuantityOfRotating.W);
                    skinningProvider.Bone[current.BoneIndex].Rotation *= right;
                    skinningProvider.Bone[current.BoneIndex].Translation += current.QuantityOfMoving;
                }
                result = true;
            }
            return result;
        }
    }
}