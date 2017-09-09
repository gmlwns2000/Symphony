using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;

namespace MMF.Morph
{
    internal class MaterialMorphData
    {
        public System.Collections.Generic.List<MaterialMorphOffset> Morphoffsets = new System.Collections.Generic.List<MaterialMorphOffset>();

        public MaterialMorphData(MorphData morph)
        {
            foreach (MorphOffsetBase current in morph.MorphOffsetes)
            {
                Morphoffsets.Add((MaterialMorphOffset)current);
            }
        }
    }
}
