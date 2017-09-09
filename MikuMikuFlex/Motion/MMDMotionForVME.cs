using MMF.Bone;
using MMF.Morph;
using OpenMMDFormat;
using ProtoBuf;
using System.Linq;

namespace MMF.Motion
{
    public class MMDMotionForVME : IMotionProvider
    {
        private VocaloidMotionEvolved vocaloidMotionEvolved;

        private PMXBone[] bones;

        private bool ignoreParent;

        private readonly System.Collections.Generic.List<BoneMotionForVME> boneMotions = new System.Collections.Generic.List<BoneMotionForVME>();

        private readonly System.Collections.Generic.List<MorphMotionForVME> morphMotions = new System.Collections.Generic.List<MorphMotionForVME>();

        private bool isAttached = false;

        private bool isPlaying = false;

        private ActionAfterMotion actionAfterMotion = ActionAfterMotion.Nothing;

        public event System.EventHandler<System.EventArgs> FrameTicked;

        public event System.EventHandler<ActionAfterMotion> MotionFinished;

        public float CurrentFrame
        {
            get;
            set;
        }

        public int FinalFrame
        {
            get;
            private set;
        }

        public bool IsAttached
        {
            get
            {
                return isAttached;
            }
        }

        private void _MMDMotionFromVME(System.IO.Stream fs, bool ignoreParent)
        {
            this.ignoreParent = ignoreParent;
            vocaloidMotionEvolved = Serializer.Deserialize<VocaloidMotionEvolved>(fs);
        }

        public MMDMotionForVME(string filePath, bool ignoreParent)
        {
            using (System.IO.FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {
                _MMDMotionFromVME(fileStream, ignoreParent);
            }
        }

        public MMDMotionForVME(System.IO.Stream fs, bool ignoreParent)
        {
            _MMDMotionFromVME(fs, ignoreParent);
        }

        public void AttachMotion(PMXBone[] bones)
        {
            this.bones = bones;
            System.Collections.Generic.Dictionary<ulong, string> dictionary = new System.Collections.Generic.Dictionary<ulong, string>();
            foreach (IDTag current in vocaloidMotionEvolved.boneIDTable)
            {
                dictionary[current.id] = current.name;
            }
            foreach (BoneFrameTable current2 in vocaloidMotionEvolved.boneFrameTables)
            {
                string boneName = dictionary[current2.id];
                if ((!ignoreParent || !boneName.Equals("全ての親")) && bones.Any((PMXBone b) => b.BoneName.Equals(boneName)))
                {
                    boneMotions.Add(new BoneMotionForVME(bones.Single((PMXBone b) => b.BoneName.Equals(boneName)), current2.frames));
                }
            }
            System.Collections.Generic.Dictionary<ulong, string> dictionary2 = new System.Collections.Generic.Dictionary<ulong, string>();
            foreach (IDTag current in vocaloidMotionEvolved.morphIDTable)
            {
                dictionary2[current.id] = current.name;
            }
            foreach (MorphFrameTable current3 in vocaloidMotionEvolved.morphFrameTables)
            {
                string morphName = dictionary2[current3.id];
                morphMotions.Add(new MorphMotionForVME(morphName, current3.frames));
            }
            foreach (BoneMotionForVME current4 in boneMotions)
            {
                FinalFrame = System.Math.Max((int)current4.GetFinalFrame(), FinalFrame);
            }
            isAttached = true;
        }

        public void Tick(int fps, float elapsedTime, IMorphManager morphManager)
        {
            foreach (BoneMotionForVME current in boneMotions)
            {
                current.ReviseBone((ulong)CurrentFrame);
            }
            foreach (MorphMotionForVME current2 in morphMotions)
            {
                morphManager.ApplyMorphProgress(current2.GetMorphValue((ulong)CurrentFrame), current2.MorphName);
            }
            if (isPlaying)
            {
                CurrentFrame += elapsedTime * fps;
                if (FrameTicked != null)
                {
                    FrameTicked(this, new System.EventArgs());
                }
                if (CurrentFrame >= FinalFrame)
                {
                    CurrentFrame = ((actionAfterMotion == ActionAfterMotion.Replay) ? 0.001f : FinalFrame);
                    if (MotionFinished != null)
                    {
                        MotionFinished(this, actionAfterMotion);
                    }
                }
            }
        }

        public void Start(float frame, ActionAfterMotion actionAfterMotion)
        {
            if (frame > FinalFrame)
            {
                frame = FinalFrame - 1;
                //throw new System.InvalidOperationException("最終フレームを超えた場所から再生を求められました。");
            }
            CurrentFrame = frame;
            this.actionAfterMotion = actionAfterMotion;
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
        }
    }
}