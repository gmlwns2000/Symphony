using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;

namespace MMF.Morph
{
    public class GroupMorphData
    {
        public System.Collections.Generic.List<GroupMorphOffset> MorphOffsets = new System.Collections.Generic.List<GroupMorphOffset>();

        public GroupMorphData(MorphData data)
        {
            foreach (MorphOffsetBase current in data.MorphOffsetes)
            {
                MorphOffsets.Add((GroupMorphOffset)current);
            }
        }
    }
}
