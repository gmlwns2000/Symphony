using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class ModelData
    {
        public Header Header
        {
            get;
            private set;
        }

        public ModelInfo ModelInfo
        {
            get;
            private set;
        }

        public VertexList VertexList
        {
            get;
            private set;
        }

        public SurfaceList SurfaceList
        {
            get;
            private set;
        }

        public TextureList TextureList
        {
            get;
            private set;
        }

        public MaterialList MaterialList
        {
            get;
            private set;
        }

        public BoneList BoneList
        {
            get;
            private set;
        }

        public MorphList MorphList
        {
            get;
            private set;
        }

        public ShowFrameList ShowFrameList
        {
            get;
            private set;
        }

        public RigidBodyList RigidBodyList
        {
            get;
            private set;
        }

        public JointList JointList
        {
            get;
            private set;
        }

        public static ModelData GetModel(FileStream fs)
        {
            ModelData modelData = new ModelData();
            modelData.Header = Header.getHeader(fs);
            modelData.ModelInfo = ModelInfo.getModelInfo(fs, modelData.Header);
            modelData.VertexList = VertexList.getVertexList(fs, modelData.Header);
            modelData.SurfaceList = SurfaceList.getSurfaceList(fs, modelData.Header);
            modelData.TextureList = TextureList.getTextureList(fs, modelData.Header);
            modelData.MaterialList = MaterialList.getMaterialList(fs, modelData.Header);
            modelData.BoneList = BoneList.getBoneList(fs, modelData.Header);
            modelData.MorphList = MorphList.getMorphList(fs, modelData.Header);
            modelData.ShowFrameList = ShowFrameList.getShowFrameList(fs, modelData.Header);
            modelData.RigidBodyList = RigidBodyList.GetRigidBodyList(fs, modelData.Header);
            modelData.JointList = JointList.getJointList(fs, modelData.Header);
            return modelData;
        }
    }
}
