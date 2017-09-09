using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class MaterialData
    {
        public string MatrialName;

        public string MaterialName_En;

        public Vector4 Diffuse;

        public Vector3 Specular;

        public float SpecularCoefficient;

        public Vector3 Ambient;

        public RenderFlag bitFlag;

        public Vector4 EdgeColor;

        public float EdgeSize;

        public int TextureTableReferenceIndex;

        public int SphereTextureTableReferenceIndex;

        public SphereMode SphereMode;

        public byte ShareToonFlag;

        public int textureIndex;

        public string Memo;

        public int VertexNumber;

        internal static MaterialData getMaterial(FileStream fs, Header header)
        {
            MaterialData materialData = new MaterialData();
            materialData.MatrialName = ParserHelper.getTextBuf(fs, header.Encode);
            materialData.MaterialName_En = ParserHelper.getTextBuf(fs, header.Encode);
            materialData.Diffuse = ParserHelper.getFloat4(fs);
            materialData.Specular = ParserHelper.getFloat3(fs);
            materialData.SpecularCoefficient = ParserHelper.getFloat(fs);
            materialData.Ambient = ParserHelper.getFloat3(fs);
            materialData.bitFlag = (RenderFlag)ParserHelper.getByte(fs);
            materialData.EdgeColor = ParserHelper.getFloat4(fs);
            materialData.EdgeSize = ParserHelper.getFloat(fs);
            materialData.TextureTableReferenceIndex = ParserHelper.getIndex(fs, header.TextureIndexSize);
            materialData.SphereTextureTableReferenceIndex = ParserHelper.getIndex(fs, header.TextureIndexSize);
            switch (ParserHelper.getByte(fs))
            {
                case 0:
                    materialData.SphereMode = SphereMode.Disable;
                    break;
                case 1:
                    materialData.SphereMode = SphereMode.Multiply;
                    break;
                case 2:
                    materialData.SphereMode = SphereMode.Add;
                    break;
                case 3:
                    materialData.SphereMode = SphereMode.SubTexture;
                    break;
                default:
                    throw new InvalidDataException("スフィアモード値が以上です");
            }
            materialData.ShareToonFlag = ParserHelper.getByte(fs);
            materialData.textureIndex = ((materialData.ShareToonFlag == 0) ? ParserHelper.getIndex(fs, header.TextureIndexSize) : ParserHelper.getByte(fs));
            materialData.Memo = ParserHelper.getTextBuf(fs, header.Encode);
            materialData.VertexNumber = ParserHelper.getInt(fs);
            if (materialData.VertexNumber % 3 != 0)
            {
                throw new InvalidDataException();
            }
            return materialData;
        }
    }
}