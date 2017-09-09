using MMDFileParser.PMXModelParser.MorphOffset;
using System.Collections.Generic;
using System.IO;

namespace MMDFileParser.PMXModelParser
{
    public class MorphData
    {
        public string MorphName
        {
            get;
            private set;
        }

        public string MorphName_En
        {
            get;
            private set;
        }

        public byte OperationPanel
        {
            get;
            private set;
        }

        public MorphType type
        {
            get;
            private set;
        }

        public int MorphOffsetCount
        {
            get;
            private set;
        }

        public List<MorphOffsetBase> MorphOffsetes
        {
            get;
            private set;
        }

        internal static MorphData getMorph(FileStream fs, Header header)
        {
            MorphData morphData = new MorphData();
            morphData.MorphOffsetes = new List<MorphOffsetBase>();
            morphData.MorphName = ParserHelper.getTextBuf(fs, header.Encode);
            morphData.MorphName_En = ParserHelper.getTextBuf(fs, header.Encode);
            morphData.OperationPanel = ParserHelper.getByte(fs);
            byte @byte = ParserHelper.getByte(fs);
            morphData.MorphOffsetCount = ParserHelper.getInt(fs);
            for (int i = 0; i < morphData.MorphOffsetCount; i++)
            {
                switch (@byte)
                {
                    case 0:
                        morphData.type = MorphType.Group;
                        morphData.MorphOffsetes.Add(GroupMorphOffset.getGroupMorph(fs, header));
                        break;
                    case 1:
                        morphData.type = MorphType.Vertex;
                        morphData.MorphOffsetes.Add(VertexMorphOffset.getVertexMorph(fs, header));
                        break;
                    case 2:
                        morphData.type = MorphType.Bone;
                        morphData.MorphOffsetes.Add(BoneMorphOffset.getBoneMorph(fs, header));
                        break;
                    case 3:
                        morphData.type = MorphType.UV;
                        morphData.MorphOffsetes.Add(UVMorphOffset.getUVMorph(fs, header, MorphType.UV));
                        break;
                    case 4:
                        morphData.type = MorphType.UV_Additional1;
                        morphData.MorphOffsetes.Add(UVMorphOffset.getUVMorph(fs, header, MorphType.UV_Additional1));
                        break;
                    case 5:
                        morphData.type = MorphType.UV_Additional2;
                        morphData.MorphOffsetes.Add(UVMorphOffset.getUVMorph(fs, header, MorphType.UV_Additional2));
                        break;
                    case 6:
                        morphData.type = MorphType.UV_Additional3;
                        morphData.MorphOffsetes.Add(UVMorphOffset.getUVMorph(fs, header, MorphType.UV_Additional3));
                        break;
                    case 7:
                        morphData.type = MorphType.UV_Additional4;
                        morphData.MorphOffsetes.Add(UVMorphOffset.getUVMorph(fs, header, MorphType.UV_Additional4));
                        break;
                    case 8:
                        morphData.type = MorphType.Matrial;
                        morphData.MorphOffsetes.Add(MaterialMorphOffset.getMaterialMorph(fs, header));
                        break;
                    case 9:
                        if (header.Version < 2.1)
                        {
                            throw new InvalidDataException("FlipモーフはPMX2.1以降でサポートされています。");
                        }
                        morphData.type = MorphType.Flip;
                        morphData.MorphOffsetes.Add(FlipMorphOffset.getFlipMorph(fs, header));
                        break;
                    case 10:
                        if (header.Version < 2.1)
                        {
                            throw new InvalidDataException("ImpulseモーフはPMX2.1以降でサポートされています。");
                        }
                        morphData.type = MorphType.Impulse;
                        morphData.MorphOffsetes.Add(ImpulseMorphOffset.getImpulseMorph(fs, header));
                        break;
                }
            }
            return morphData;
        }
    }
}