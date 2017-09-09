using MMDFileParser.PMXModelParser;
using MMF.Bone;
using MMF.Model;
using MMF.Morph;
using System.Diagnostics;
using System;

namespace MMF.Motion
{
    public class BasicMotionManager : IMotionManager, ITransformUpdater, IDisposable
    {
        private readonly RenderContext context;

        private Stopwatch motionTimer;

        private ISkinningProvider skinningProvider;

        private IMorphManager morphManager;

        public event System.EventHandler<ActionAfterMotion> MotionFinished;

        public event System.EventHandler MotionListUpdated;

        private long lastTime
        {
            get;
            set;
        }

        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, IMotionProvider>> SubscribedMotionMap
        {
            get;
            private set;
        }

        public IMotionProvider CurrentMotionProvider
        {
            get;
            set;
        }

        public float CurrentFrame
        {
            get
            {
                float result;
                if (CurrentMotionProvider == null)
                {
                    result = float.NaN;
                }
                else
                {
                    result = CurrentMotionProvider.CurrentFrame / context.Timer.MotionFramePerSecond;
                }
                return result;
            }
        }

        public float ElapsedTime
        {
            get;
            private set;
        }

        public BasicMotionManager(RenderContext context)
        {
            this.context = context;
        }

        public void Initialize(ModelData model, IMorphManager morph, ISkinningProvider skinning, IBufferManager bufferManager)
        {
            skinningProvider = skinning;
            motionTimer = new Stopwatch();
            motionTimer.Start();
            morphManager = morph;
            SubscribedMotionMap = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, IMotionProvider>>();
        }

        public bool UpdateTransform()
        {
            if (lastTime == 0L)
            {
                lastTime = motionTimer.ElapsedMilliseconds;
            }
            else
            {
                long elapsedMilliseconds = motionTimer.ElapsedMilliseconds;
                ElapsedTime = elapsedMilliseconds - lastTime;
                if (CurrentMotionProvider != null)
                {
                    CurrentMotionProvider.Tick(context.Timer.MotionFramePerSecond, ElapsedTime / 1000f, morphManager);
                }
                lastTime = elapsedMilliseconds;
            }
            return true;
        }

        public IMotionProvider AddMotionFromFile(string filePath, bool ignoreParent)
        {
            string extension = System.IO.Path.GetExtension(filePath);
            IMotionProvider motionProvider;
            if (string.Compare(extension, ".vmd", true) == 0)
            {
                motionProvider = new MMDMotion(filePath, ignoreParent);
            }
            else
            {
                if (string.Compare(extension, ".vme", true) != 0)
                {
                    throw new System.Exception("ファイルが不適切です！");
                }
                motionProvider = new MMDMotionForVME(filePath, ignoreParent);
            }
            motionProvider.AttachMotion(skinningProvider.Bone);
            motionProvider.MotionFinished += new System.EventHandler<ActionAfterMotion>(motion_MotionFinished);
            SubscribedMotionMap.Add(new System.Collections.Generic.KeyValuePair<string, IMotionProvider>(filePath, motionProvider));
            if (MotionListUpdated != null)
            {
                MotionListUpdated(this, new System.EventArgs());
            }
            return motionProvider;
        }

        public void ApplyMotion(IMotionProvider motionProvider, int startFrame = 0, ActionAfterMotion setting = ActionAfterMotion.Nothing)
        {
            if (CurrentMotionProvider != null)
            {
                CurrentMotionProvider.Stop();
            }
            motionProvider.Start(startFrame, setting);
            CurrentMotionProvider = motionProvider;
        }

        public void StopMotion(bool toIdentity = false)
        {
            if (CurrentMotionProvider != null)
            {
                CurrentMotionProvider.Stop();
            }
            if (toIdentity)
            {
                CurrentMotionProvider = null;
            }
        }

        public IMotionProvider AddMotionFromStream(string fileName, System.IO.Stream stream, bool ignoreParent)
        {
            IMotionProvider motionProvider = new MMDMotion(stream, ignoreParent);
            motionProvider.MotionFinished += new System.EventHandler<ActionAfterMotion>(motion_MotionFinished);
            SubscribedMotionMap.Add(new System.Collections.Generic.KeyValuePair<string, IMotionProvider>(fileName, motionProvider));
            MotionListUpdated?.Invoke(this, new System.EventArgs());
            return motionProvider;
        }

        private void motion_MotionFinished(object owner, ActionAfterMotion obj)
        {
            MotionFinished?.Invoke(this, obj);
        }

        public void Dispose()
        {
            if(motionTimer != null)
            {
                if (motionTimer.IsRunning)
                {
                    motionTimer.Stop();
                }

                motionTimer = null;
            }
        }
    }
}
