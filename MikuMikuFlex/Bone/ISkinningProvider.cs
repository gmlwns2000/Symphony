using MMF.Morph;
using SlimDX.Direct3D11;

namespace MMF.Bone
{
    public interface ISkinningProvider : System.IDisposable
    {
        event System.EventHandler SkeletonUpdated;

        PMXBone[] Bone
        {
            get;
        }

        System.Collections.Generic.Dictionary<string, PMXBone> BoneDictionary
        {
            get;
        }

        System.Collections.Generic.List<PMXBone> IkBone
        {
            get;
        }

        System.Collections.Generic.List<ITransformUpdater> KinematicsProviders
        {
            get;
        }

        void ApplyEffect(Effect effect);

        void UpdateSkinning(IMorphManager morphManager);

        void ResetAllBoneTransform();
    }
}
