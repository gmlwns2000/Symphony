using MMDFileParser.PMXModelParser.BoneWeight;
using SlimDX;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class VertexData
    {
        public Vector3 Position;

        public Vector3 Normal;

        public Vector2 UV;

        public Vector4[] AdditionalUV;

        public WeightTranslationType TranslationType;

        public BoneWeightBase BoneWeight;

        public float EdgeMagnification;

        internal static VertexData getVertex(FileStream fs, Header header)
        {
            VertexData vertexData = new VertexData();
            vertexData.Position = ParserHelper.getFloat3(fs);
            vertexData.Normal = ParserHelper.getFloat3(fs);
            vertexData.UV = ParserHelper.getFloat2(fs);
            vertexData.AdditionalUV = new Vector4[header.AdditionalUVCount];
            for (int i = 0; i < header.AdditionalUVCount; i++)
            {
                vertexData.AdditionalUV[i] = ParserHelper.getFloat4(fs);
            }
            switch (ParserHelper.getByte(fs))
            {
                case 0:
                    vertexData.TranslationType = WeightTranslationType.BDEF1;
                    vertexData.BoneWeight = new BDEF1
                    {
                        boneIndex = ParserHelper.getIndex(fs, header.BoneIndexSize)
                    };
                    break;
                case 1:
                    vertexData.TranslationType = WeightTranslationType.BDEF2;
                    vertexData.BoneWeight = new BDEF2
                    {
                        Bone1ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone2ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Weight = ParserHelper.getFloat(fs)
                    };
                    break;
                case 2:
                    vertexData.TranslationType = WeightTranslationType.BDEF4;
                    vertexData.BoneWeight = new BDEF4
                    {
                        Bone1ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone2ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone3ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone4ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Weights = ParserHelper.getFloat4(fs)
                    };
                    break;
                case 3:
                    vertexData.TranslationType = WeightTranslationType.SDEF;
                    vertexData.BoneWeight = new SDEF
                    {
                        Bone1ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone2ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone1Weight = ParserHelper.getFloat(fs),
                        SDEF_C = ParserHelper.getFloat3(fs),
                        SDEF_R0 = ParserHelper.getFloat3(fs),
                        SDEF_R1 = ParserHelper.getFloat3(fs)
                    };
                    break;
                case 4:
                    if (header.Version != 2.1f)
                    {
                        throw new InvalidDataException("QDEFはPMX2.1でのみサポートされます。");
                    }
                    vertexData.TranslationType = WeightTranslationType.QDEF;
                    vertexData.BoneWeight = new QDEF
                    {
                        Bone1ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone2ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone3ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Bone4ReferenceIndex = ParserHelper.getIndex(fs, header.BoneIndexSize),
                        Weights = ParserHelper.getFloat4(fs)
                    };
                    break;
                default:
                    throw new InvalidDataException();
            }
            vertexData.EdgeMagnification = ParserHelper.getFloat(fs);
            return vertexData;
        }
    }
}
