using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;

namespace MMF.Morph
{
    public class VertexMorphData
    {
        public System.Collections.Generic.List<VertexMorphOffset> MorphOffsets = new System.Collections.Generic.List<VertexMorphOffset>();

        public VertexMorphData(MorphData morphData)
        {
            foreach (MorphOffsetBase current in morphData.MorphOffsetes)
            {
                MorphOffsets.Add((VertexMorphOffset)current);
            }
        }
    }
}
