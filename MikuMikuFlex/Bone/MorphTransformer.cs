namespace MMF.Bone
{
    public class MorphTransformer
    {
        public string MorphName
        {
            get;
            private set;
        }

        public float MorphValue
        {
            get;
            set;
        }

        public MorphTransformer(string morphName)
        {
            MorphName = morphName;
        }
    }
}
