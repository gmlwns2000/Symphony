using MMDFileParser.PMXModelParser;
using MMF.Utility;
using SlimDX;

namespace MMF.Bone
{
    public class IkLink
    {
        private readonly int index;

        private readonly ISkinningProvider skinning;

        public bool isLimited = false;

        public Vector3 maxRot;

        public Vector3 minRot;

        public int loopCount;

        public PMXBone ikLinkBone
        {
            get
            {
                return skinning.Bone[index];
            }
        }

        public IkLink(ISkinningProvider skinning, IkLinkData linkData)
        {
            this.skinning = skinning;
            index = linkData.LinkBoneIndex;
            Vector3 minimumRadian = linkData.MinimumRadian;
            Vector3 maximumRadian = linkData.MaximumRadian;
            minRot = new Vector3(System.Math.Min(minimumRadian.X, maximumRadian.X), System.Math.Min(minimumRadian.Y, maximumRadian.Y), System.Math.Min(minimumRadian.Z, maximumRadian.Z));
            maxRot = new Vector3(System.Math.Max(minimumRadian.X, maximumRadian.X), System.Math.Max(minimumRadian.Y, maximumRadian.Y), System.Math.Max(minimumRadian.Z, maximumRadian.Z));
            maxRot = Vector3.Clamp(maxRot, CGHelper.EularMinimum, CGHelper.EularMaximum);
            minRot = Vector3.Clamp(minRot, CGHelper.EularMinimum, CGHelper.EularMaximum);
            isLimited = linkData.isRotateLimited;
        }
    }
}
