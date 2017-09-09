using SlimDX;

namespace MMF.Bone
{
    public class BoneTransformer
    {
        public string BoneName
        {
            get;
            private set;
        }

        public Quaternion Rotation
        {
            get;
            set;
        }

        public Vector3 Translation
        {
            get;
            set;
        }

        public BoneTransformer(string boneName, Quaternion rotation, Vector3 translation)
        {
            BoneName = boneName;
            Rotation = rotation;
            Translation = translation;
        }
    }
}
