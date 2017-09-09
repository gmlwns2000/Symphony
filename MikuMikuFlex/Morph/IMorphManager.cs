using MMF.Motion;

namespace MMF.Morph
{
    public interface IMorphManager
    {
        void ApplyMorphProgress(float frame, System.Collections.Generic.IEnumerable<MorphMotion> morphs);

        void ApplyMorphProgress(float frame, string morphName);

        void UpdateFrame();

        float getMorphProgress(string morphName);
    }
}
