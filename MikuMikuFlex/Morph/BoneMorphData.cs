using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;

namespace MMF.Morph
{
    internal class BoneMorphData
    {
        public System.Collections.Generic.List<BoneMorphOffset> BoneMorphs = new System.Collections.Generic.List<BoneMorphOffset>();

        public BoneMorphData(MorphData morphData)
        {
            foreach (MorphOffsetBase current in morphData.MorphOffsetes)
            {
                BoneMorphs.Add((BoneMorphOffset)current);
            }
        }
    }
}
