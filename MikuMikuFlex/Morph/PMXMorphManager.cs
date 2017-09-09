using MMDFileParser.PMXModelParser;
using MMF.Model.PMX;
using MMF.Motion;

namespace MMF.Morph
{
    public class PMXMorphManager : IMorphManager
    {
        public System.Collections.Generic.List<IMorphProvider> MMDMorphs = new System.Collections.Generic.List<IMorphProvider>();

        private System.Collections.Generic.Dictionary<string, float> morphProgresses = new System.Collections.Generic.Dictionary<string, float>();

        public PMXMorphManager(PMXModel model)
        {
            MMDMorphs.Add(new VertexMorphProvider(model.Model, model.BufferManager));
            MMDMorphs.Add(new BoneMorphProvider(model));
            MMDMorphs.Add(new MaterialMorphProvider(model));
            MMDMorphs.Add(new GroupMorphProvider(model, this));
            MMDMorphs.Add(new UVMorphProvider(model, MorphType.UV));
            MMDMorphs.Add(new UVMorphProvider(model, MorphType.UV_Additional1));
            MMDMorphs.Add(new UVMorphProvider(model, MorphType.UV_Additional2));
            MMDMorphs.Add(new UVMorphProvider(model, MorphType.UV_Additional3));
            MMDMorphs.Add(new UVMorphProvider(model, MorphType.UV_Additional4));
        }

        public float getMorphProgress(string morphName)
        {
            return morphProgresses[morphName];
        }

        public void ApplyMorphProgress(float frameNumber, System.Collections.Generic.IEnumerable<MorphMotion> morphMotions)
        {
            foreach (MorphMotion current in morphMotions)
            {
                if (morphProgresses.ContainsKey(current.MorphName))
                {
                    morphProgresses[current.MorphName] = current.GetMorphValue(frameNumber);
                }
                else
                {
                    morphProgresses.Add(current.MorphName, current.GetMorphValue(frameNumber));
                }
            }
            foreach (IMorphProvider current2 in MMDMorphs)
            {
                current2.ApplyMorphProgress(frameNumber, morphMotions);
            }
        }

        public void ApplyMorphProgress(float frame, string morphName)
        {
            if (morphProgresses.ContainsKey(morphName))
            {
                morphProgresses[morphName] = frame;
            }
            else
            {
                morphProgresses.Add(morphName, frame);
            }
            foreach (IMorphProvider current in MMDMorphs)
            {
                if (current.ApplyMorphProgress(frame, morphName))
                {
                    break;
                }
            }
        }

        public void UpdateFrame()
        {
            foreach (IMorphProvider current in MMDMorphs)
            {
                current.UpdateFrame();
            }
        }
    }
}
