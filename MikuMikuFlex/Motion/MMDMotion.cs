using MMDFileParser.MotionParser;
using MMF.Bone;
using MMF.Morph;
using System.Linq;
using System;

namespace MMF.Motion
{
    public class MMDMotion : IMotionProvider, IDisposable
    {
        private PMXBone[] bones;

        private readonly System.Collections.Generic.List<BoneMotion> boneMotions = new System.Collections.Generic.List<BoneMotion>();

        private readonly System.Collections.Generic.List<MorphMotion> morphMotions = new System.Collections.Generic.List<MorphMotion>();

        private MotionData motionData;

        private ActionAfterMotion actionAfterMotion = ActionAfterMotion.Nothing;

        private bool isPlaying;

        private bool isAttached;

        private bool ignoreParent;

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

        private void _MMDMotion(System.IO.Stream fs, bool ignoreParent)
        {
            this.ignoreParent = ignoreParent;
            motionData = MotionData.getMotion(fs);
        }

        public MMDMotion(string filePath, bool ignoreParent)
        {
            using (System.IO.FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {
                _MMDMotion(fileStream, ignoreParent);
            }
        }

        public MMDMotion(System.IO.Stream fs, bool ignoreParent)
        {
            _MMDMotion(fs, ignoreParent);
        }

        public void Stop()
        {
            isPlaying = false;
        }

        public void AttachMotion(PMXBone[] bones)
        {
            this.bones = bones;
            AttachBoneFrameDataToBoneMotion();
            AttachMorphFrameDataToMorphMotion();
            foreach (BoneMotion current in boneMotions)
            {
                current.SortBoneFrameDatas();
                FinalFrame = System.Math.Max((int)current.GetFinalFrameNumber(), FinalFrame);
            }
            foreach (MorphMotion current2 in morphMotions)
            {
                current2.SortMorphFrameDatas();
            }
            isAttached = true;
        }

        public void Tick(int fps, float elapsedTime, IMorphManager morphManager)
        {
            foreach (BoneMotion current in boneMotions)
            {
                current.ReviseBone(CurrentFrame);
            }
            foreach (MorphMotion current2 in morphMotions)
            {
                morphManager.ApplyMorphProgress(current2.GetMorphValue((ulong)CurrentFrame), current2.MorphName);
            }
            if (isPlaying)
            {
                CurrentFrame += elapsedTime * fps;
                if (CurrentFrame >= FinalFrame)
                {
                    CurrentFrame = FinalFrame;
                }
                if (FrameTicked != null)
                {
                    FrameTicked(this, new System.EventArgs());
                }
                if (CurrentFrame >= FinalFrame)
                {
                    if (MotionFinished != null)
                    {
                        MotionFinished(this, actionAfterMotion);
                    }
                    if (actionAfterMotion == ActionAfterMotion.Replay)
                    {
                        CurrentFrame = 0.001f;
                    }
                }
            }
        }

        public void Start(float frame, ActionAfterMotion action)
        {
            if (frame > FinalFrame)
            {
                frame = FinalFrame - 1;
                //throw new System.InvalidOperationException("最終フレームを超えた場所から再生を求められました。");
            }
            CurrentFrame = frame;
            isPlaying = true;
            actionAfterMotion = action;
        }

        private void AttachBoneFrameDataToBoneMotion()
        {
            using (System.Collections.Generic.List<BoneFrameData>.Enumerator enumerator = motionData.boneFrameList.boneFrameDatas.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    BoneFrameData boneFrameData = enumerator.Current;
                    if (!ignoreParent || !boneFrameData.BoneName.Equals("全ての親"))
                    {
                        if (bones.Any((PMXBone b) => b.BoneName.Equals(boneFrameData.BoneName)))
                        {
                            PMXBone bone = bones.Single((PMXBone b) => b.BoneName.Equals(boneFrameData.BoneName));
                            if (!boneMotions.Any((BoneMotion bm) => bm.GetBoneName().Equals(boneFrameData.BoneName)))
                            {
                                BoneMotion boneMotion = new BoneMotion(bone);
                                boneMotion.AddBoneFrameData(boneFrameData);
                                boneMotions.Add(boneMotion);
                            }
                            else
                            {
                                boneMotions.Single((BoneMotion bm) => bm.GetBoneName().Equals(boneFrameData.BoneName)).AddBoneFrameData(boneFrameData);
                            }
                        }
                    }
                }
            }
        }

        private void AttachMorphFrameDataToMorphMotion()
        {
            using (System.Collections.Generic.List<MorphFrameData>.Enumerator enumerator = motionData.morphFrameList.morphFrameDatas.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    MorphFrameData morphFrameData = enumerator.Current;
                    if (!morphMotions.Any((MorphMotion mm) => mm.MorphName.Equals(morphFrameData.Name)))
                    {
                        MorphMotion morphMotion = new MorphMotion(morphFrameData.Name);
                        morphMotion.AddMorphFrameData(morphFrameData);
                        morphMotions.Add(morphMotion);
                    }
                    morphMotions.Single((MorphMotion mm) => mm.MorphName.Equals(morphFrameData.Name)).AddMorphFrameData(morphFrameData);
                }
            }
        }

        public void Dispose()
        {
            bones = null;

            if (motionData != null)
            {
                motionData.Dispose();
                motionData = null;
            }
        }
    }
}
