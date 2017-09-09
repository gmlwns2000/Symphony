using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;

namespace MMF.Morph
{
    public class UVMorphData
    {
        public System.Collections.Generic.List<UVMorphOffset> MorphOffsets = new System.Collections.Generic.List<UVMorphOffset>();

        public UVMorphData(MorphData morphData)
        {
            foreach (MorphOffsetBase current in morphData.MorphOffsetes)
            {
                MorphOffsets.Add((UVMorphOffset)current);
            }
        }
    }
}
