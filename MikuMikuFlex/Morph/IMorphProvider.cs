using MMF.Motion;

namespace MMF.Morph
{
    public interface IMorphProvider
    {
        void ApplyMorphProgress(float frameNumber, System.Collections.Generic.IEnumerable<MorphMotion> morphMotions);

        bool ApplyMorphProgress(float progress, string morphName);

        void UpdateFrame();
    }
}
