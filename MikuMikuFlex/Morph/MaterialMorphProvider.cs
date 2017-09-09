using MMDFileParser.PMXModelParser;
using MMDFileParser.PMXModelParser.MorphOffset;
using MMF.MME.VariableSubscriber.MaterialSubscriber;
using MMF.Model;
using MMF.Model.PMX;
using MMF.Motion;
using SlimDX;

namespace MMF.Morph
{
    internal class MaterialMorphProvider : IMorphProvider
    {
        public System.Collections.Generic.Dictionary<string, MaterialMorphData> Morphs = new System.Collections.Generic.Dictionary<string, MaterialMorphData>();

        private PMXModel model;

        public MaterialMorphProvider(PMXModel model)
        {
            this.model = model;
            foreach (MorphData current in model.Model.MorphList.Morphes)
            {
                if (current.type == MorphType.Matrial)
                {
                    Morphs.Add(current.MorphName, new MaterialMorphData(current));
                }
            }
        }

        public void ApplyMorphProgress(float frameNumber, System.Collections.Generic.IEnumerable<MorphMotion> morphMotions)
        {
            foreach (MorphMotion current in morphMotions)
            {
                SetMorphProgress(current.GetMorphValue(frameNumber), current.MorphName);
            }
        }

        public bool ApplyMorphProgress(float progress, string morphName)
        {
            return SetMorphProgress(progress, morphName);
        }

        public void UpdateFrame()
        {
        }

        private bool SetMorphProgress(float progress, string morphName)
        {
            bool result;
            if (!Morphs.ContainsKey(morphName))
            {
                result = false;
            }
            else
            {
                MaterialMorphData materialMorphData = Morphs[morphName];
                foreach (MaterialMorphOffset current in materialMorphData.Morphoffsets)
                {
                    if (current.MaterialIndex == -1)
                    {
                        foreach (ISubset current2 in model.SubsetManager.Subsets)
                        {
                            MaterialInfo materialInfo = current2.MaterialInfo;
                            materialInfo = ((current.OffsetCalclationType == 0) ? materialInfo.MulMaterialInfo : materialInfo.AddMaterialInfo);
                            materialInfo.DiffuseColor += current.Diffuse * progress;
                            materialInfo.AmbientColor += new Vector4(current.Ambient, 1f) * progress;
                            materialInfo.SpecularColor += new Vector4(current.Specular, 1f) * progress;
                            materialInfo.SpecularPower += current.SpecularCoefficient * progress;
                            materialInfo.EdgeColor += current.EdgeColor * progress;
                        }
                    }
                    else
                    {
                        MaterialInfo materialInfo = model.SubsetManager.Subsets[current.MaterialIndex].MaterialInfo;
                        materialInfo = ((current.OffsetCalclationType == 0) ? materialInfo.MulMaterialInfo : materialInfo.AddMaterialInfo);
                        materialInfo.DiffuseColor += current.Diffuse * progress;
                        materialInfo.AmbientColor += new Vector4(current.Ambient, 1f) * progress;
                        materialInfo.SpecularColor += new Vector4(current.Specular, 1f) * progress;
                        materialInfo.SpecularPower += current.SpecularCoefficient * progress;
                        materialInfo.EdgeColor += current.EdgeColor * progress;
                    }
                }
                result = true;
            }
            return result;
        }
    }
}
