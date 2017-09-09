using MMDFileParser.PMXModelParser;
using MMF.Bone;
using MMF.Model;
using MMF.Morph;

namespace MMF.Motion
{
    public interface IMotionManager : ITransformUpdater
    {
        event System.EventHandler<ActionAfterMotion> MotionFinished;

        event System.EventHandler MotionListUpdated;

        IMotionProvider CurrentMotionProvider
        {
            get;
        }

        float CurrentFrame
        {
            get;
        }

        float ElapsedTime
        {
            get;
        }

        System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, IMotionProvider>> SubscribedMotionMap
        {
            get;
        }

        void Initialize(ModelData model, IMorphManager morph, ISkinningProvider skinning, IBufferManager bufferManager);

        IMotionProvider AddMotionFromFile(string filePath, bool ignoreParent);

        void ApplyMotion(IMotionProvider provider, int startFrame = 0, ActionAfterMotion setting = ActionAfterMotion.Nothing);

        void StopMotion(bool toIdentity = false);

        IMotionProvider AddMotionFromStream(string fileName, System.IO.Stream stream, bool ignoreParent);
    }
}
